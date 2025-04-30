using LMCMLCore.CORE.data;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.OS
{

    /// <summary>
    /// 表示Java虚拟机的详细信息
    /// </summary>
    public class JavaVMInfo
    {
        /// <summary>
        /// Java安装名称（例如：Java(TM) SE Development Kit 8）
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 存储Java版本号（例如java1.8存储为8，java17存储为17）
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Java完整版本号
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Java安装路径
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 系统架构（x86/x64/arm）
        /// </summary>
        public string Architecture { get; set; } = string.Empty;

        /// <summary>
        /// 供应商信息（Oracle/OpenJDK等）
        /// </summary>
        public string Vendor { get; set; } = string.Empty;

        /// <summary>
        /// 是否为JDK
        /// </summary>
        public bool IsJDK { get; set; }
    }

    /// <summary>
    /// 提供跨平台的Java安装查找功能
    /// </summary>
    public static class JavaFinder
    {
        private static readonly string[] LinuxCommonPaths = { "/usr/lib/jvm", "/usr/java" };
        private static readonly string[] MacCommonPaths = { "/Library/Java/JavaVirtualMachines" };
        private static readonly string[] WindowsCommonPaths = { @"C:\Program Files\Java" };

        /// <summary>
        /// 查找所有已安装的Java版本
        /// </summary>
        /// <param name="enableDeepScan">是否启用深度文件系统扫描</param>
        /// <returns>找到的Java安装信息集合</returns>
        public static IEnumerable<JavaVMInfo> FindJava(bool enableDeepScan = false)
        {
            var foundPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var installations = new List<JavaVMInfo>();

            // 常规查找
            CheckEnvironmentVariables(installations, foundPaths);
            CheckCommonInstallPaths(installations, foundPaths);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CheckRegistry(installations, foundPaths);
            }

            // 深度扫描
            if (enableDeepScan)
            {
                ScanFileSystem(installations, foundPaths);
            }

            return installations;
        }

        #region 检测方法
        private static void CheckEnvironmentVariables(List<JavaVMInfo> installations, HashSet<string> foundPaths)
        {
            // 检查JAVA_HOME
            var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(javaHome))
            {
                AddIfValid(javaHome, installations, foundPaths);
            }

            // 检查PATH中的Java
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var pathEntry in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrEmpty(pathEntry.Trim())) { continue; }
                var fullPath = Path.GetFullPath(pathEntry.Trim());
                if (File.Exists(Path.Combine(fullPath, GetJavaExeName())))
                {
                    var parentDir = Directory.GetParent(fullPath)?.Parent?.FullName;
                    if (!string.IsNullOrEmpty(parentDir))
                    {
                        AddIfValid(parentDir, installations, foundPaths);
                    }
                }
            }
        }

        private static void CheckCommonInstallPaths(List<JavaVMInfo> installations, HashSet<string> foundPaths)
        {
            IEnumerable<string> paths = Array.Empty<string>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) paths = WindowsCommonPaths;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) paths = LinuxCommonPaths;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) paths = MacCommonPaths;

            foreach (var basePath in paths)
            {
                if (!Directory.Exists(basePath)) continue;

                foreach (var dir in Directory.EnumerateDirectories(basePath))
                {
                    var normalizedPath = NormalizePath(dir);
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        var macHomePath = Path.Combine(dir, "Contents", "Home");
                        if (Directory.Exists(macHomePath))
                        {
                            AddIfValid(macHomePath, installations, foundPaths);
                        }
                    }
                    AddIfValid(dir, installations, foundPaths);
                }
            }
        }

        private static void CheckRegistry(List<JavaVMInfo> installations, HashSet<string> foundPaths)
        {
            CheckRegistryKey(@"SOFTWARE\JavaSoft\Java Development Kit", installations, foundPaths);
            CheckRegistryKey(@"SOFTWARE\JavaSoft\Java Runtime Environment", installations, foundPaths);
        }

        private static void CheckRegistryKey(string keyPath, List<JavaVMInfo> installations, HashSet<string> foundPaths)
        {
            using var baseKey = Registry.LocalMachine.OpenSubKey(keyPath);
            if (baseKey == null) return;

            foreach (var versionKeyName in baseKey.GetSubKeyNames())
            {
                using var versionKey = baseKey.OpenSubKey(versionKeyName);
                var javaHome = versionKey?.GetValue("JavaHome")?.ToString();
                var displayName = versionKey?.GetValue("DisplayName")?.ToString();

                if (!string.IsNullOrEmpty(javaHome))
                {
                    var info = CreateJavaInfo(javaHome);
                    info.Name = string.IsNullOrEmpty(displayName) ? info.Name : displayName;
                    if (foundPaths.Add(NormalizePath(javaHome)))
                    {
                        installations.Add(info);
                    }
                }
            }
        }

        private static void ScanFileSystem(List<JavaVMInfo> installations, HashSet<string> foundPaths)
        {
            var roots = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => d.RootDirectory.FullName)
                : new[] { "/" };

            foreach (var root in roots)
            {
                ScanDirectory(root, installations, foundPaths);
            }
        }

        private static void ScanDirectory(string path, List<JavaVMInfo> installations, HashSet<string> foundPaths, int depth = 0)
        {
            if (depth > 5) return;

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(path))
                {
                    if (IsJavaHome(dir))
                    {
                        AddIfValid(dir, installations, foundPaths);
                    }
                    ScanDirectory(dir, installations, foundPaths, depth + 1);
                }
            }
            catch { /* 忽略访问错误 */ }
        }
        #endregion

        #region 辅助方法
        private static bool IsJavaHome(string path)
        {
            try
            {
                var binPath = Path.Combine(path, "bin");
                return Directory.Exists(binPath) &&
                       File.Exists(Path.Combine(binPath, GetJavaExeName()));
            }
            catch { return false; }
        }

        private static string GetJavaExeName() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "java.exe" : "java";

        private static string GetJavacExeName() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "javac.exe" : "javac";

        private static void AddIfValid(string path, List<JavaVMInfo> installations, HashSet<string> foundPaths)
        {
            var normalizedPath = NormalizePath(path);
            if (!foundPaths.Add(normalizedPath)) return;

            if (IsJavaHome(normalizedPath))
            {
                installations.Add(CreateJavaInfo(normalizedPath));
            }
        }

        private static JavaVMInfo CreateJavaInfo(string javaHome)
        {
            var versionInfo = GetJavaVersionInfo(javaHome);
            return new JavaVMInfo
            {
                Path = NormalizePath(javaHome),
                Version = versionInfo.fullVersion,
                Id = versionInfo.majorVersion,
                Architecture = DetectArchitecture(javaHome),
                Vendor = DetectVendor(javaHome),
                IsJDK = DetectJDK(javaHome),
                Name = DetectName(javaHome)
            };
        }
        #endregion

        #region 详细检测逻辑
        private static (string fullVersion, string majorVersion) GetJavaVersionInfo(string javaHome)
        {
            try
            {
                var releaseFile = Path.Combine(javaHome, "release");
                if (File.Exists(releaseFile))
                {
                    var lines = File.ReadAllLines(releaseFile);
                    var versionLine = lines.FirstOrDefault(l => l.StartsWith("JAVA_VERSION="));
                    var fullVersion = versionLine?.Split('=')[1].Trim('"') ?? string.Empty;

                    var versionMatch = System.Text.RegularExpressions.Regex.Match(
                        fullVersion,
                        @"(?:(1\.)(?<major>\d+)|(?<major>\d+))"
                    );

                    return versionMatch.Success
                        ? (fullVersion, versionMatch.Groups["major"].Value)
                        : (fullVersion, ParseVersionFromPath(javaHome));
                }
                return (ParseVersionFromPath(javaHome), ParseVersionFromPath(javaHome));
            }
            catch
            {
                return (string.Empty, string.Empty);
            }
        }

        private static string ParseVersionFromPath(string path)
        {
            var dirName = new DirectoryInfo(path).Name;
            var match = System.Text.RegularExpressions.Regex.Match(dirName, @"(\d+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static string DetectArchitecture(string javaHome)
        {
            try
            {
                var exePath = Path.Combine(javaHome, "bin", GetJavaExeName());
                if (!File.Exists(exePath)) return string.Empty;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using var fs = new FileStream(exePath, FileMode.Open, FileAccess.Read);
                    using var br = new BinaryReader(fs);
                    fs.Seek(0x3C, SeekOrigin.Begin);
                    var peOffset = br.ReadInt32();
                    fs.Seek(peOffset + 4, SeekOrigin.Begin);
                    var machineType = br.ReadUInt16();

                    return machineType switch
                    {
                        0x8664 => "x64",
                        0x014C => "x86",
                        0xAA64 => "arm64",
                        _ => "unknown"
                    };
                }

                // Linux/Mac通过文件命令检测
                var arch = File.ReadAllText("/proc/self/status")
                    .Split('\n')
                    .FirstOrDefault(l => l.StartsWith("Cpu architecture:"))
                    ?.Split(':')[1]
                    ?.Trim();

                return arch switch
                {
                    "9" => "arm64",
                    "3" => "x86",
                    "62" => "x64",
                    _ => "unknown"
                };
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string DetectVendor(string javaHome)
        {
            try
            {
                var releaseFile = Path.Combine(javaHome, "release");
                if (File.Exists(releaseFile))
                {
                    var lines = File.ReadAllLines(releaseFile);
                    var vendorLine = lines.FirstOrDefault(l => l.StartsWith("JAVA_VENDOR="));
                    return vendorLine?.Split('=')[1].Trim('"') ?? string.Empty;
                }

                var dirName = new DirectoryInfo(javaHome).Name.ToLower();
                if (dirName.Contains("corretto")) return "Amazon Corretto";
                if (dirName.Contains("openjdk")) return "OpenJDK";
                if (dirName.Contains("zulu")) return "Zulu";
                return "Oracle";
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool DetectJDK(string javaHome)
        {
            try
            {
                // 检查传统JDK特征
                if (File.Exists(Path.Combine(javaHome, "lib", "tools.jar"))) return true;

                // 检查新版本JDK特征
                if (File.Exists(Path.Combine(javaHome, "bin", GetJavacExeName()))) return true;

                // 检查jmods目录
                return Directory.Exists(Path.Combine(javaHome, "jmods"));
            }
            catch
            {
                return false;
            }
        }

        private static string DetectName(string javaHome)
        {
            try
            {
                var releaseFile = Path.Combine(javaHome, "release");
                if (File.Exists(releaseFile))
                {
                    var lines = File.ReadAllLines(releaseFile);
                    var nameLine = lines.FirstOrDefault(l => l.StartsWith("IMPLEMENTOR="));
                    if (nameLine != null)
                    {
                        var vendor = nameLine.Split('=')[1].Trim('"');
                        var version = GetJavaVersionInfo(javaHome).majorVersion;
                        return $"{vendor} JDK {version}";
                    }
                }

                var dirName = new DirectoryInfo(javaHome).Name;
                var cleanName = System.Text.RegularExpressions.Regex.Replace(
                    dirName,
                    @"([a-z])([A-Z])",
                    "$1 $2"
                ).Replace("_", " ");

                var versionMatch = System.Text.RegularExpressions.Regex.Match(cleanName, @"\d+");
                return versionMatch.Success
                    ? $"{cleanName.Replace(versionMatch.Value, "").Trim()} {versionMatch.Value}"
                    : cleanName;
            }
            catch
            {
                return "Java Runtime";
            }
        }

        private static string NormalizePath(string path) =>
            Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        #endregion
    }
}
