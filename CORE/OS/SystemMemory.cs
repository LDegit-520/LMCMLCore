using LMCMLCore.CORE.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.OS
{
    /// <summary>
    /// 获取系统内存信息
    /// </summary>
    public static class SystemMemory
    {
        #region Windows Implementation
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYINFO
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYINFO lpBuffer);

        private static (double availableGB, double totalGB) GetWindowsMemory()
        {
            try
            {
                var mi = new MEMORYINFO { dwLength = (uint)Marshal.SizeOf<MEMORYINFO>() };
                if (!GlobalMemoryStatusEx(ref mi))
                {
                    Console.WriteLine($"Windows API错误: 0x{Marshal.GetLastWin32Error():X8}");
                    return (-1, -1);
                }
                return (mi.ullAvailPhys / 1073741824.0, mi.ullTotalPhys / 1073741824.0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Windows异常: {ex.Message}");
                return (-1, -1);
            }
        }
        #endregion

        #region Linux Implementation
        private static (double availableGB, double totalGB) GetLinuxMemory()
        {
            try
            {
                var memInfo = File.ReadAllText("/proc/meminfo");

                var availableMatch = Regex.Match(memInfo, @"MemAvailable:\s*(\d+)\s*kB");
                var totalMatch = Regex.Match(memInfo, @"MemTotal:\s*(\d+)\s*kB");

                if (!availableMatch.Success || !totalMatch.Success)
                    return (-1, -1);

                double available = double.Parse(availableMatch.Groups[1].Value) / 1048576.0;
                double total = double.Parse(totalMatch.Groups[1].Value) / 1048576.0;
                return (available, total);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Linux异常: {ex.Message}");
                return (-1, -1);
            }
        }
        #endregion

        #region macOS Implementation
        private static (double availableGB, double totalGB) GetMacOSMemory()
        {
            try
            {
                // 获取总内存
                var totalOutput = ExecuteCommand("sysctl -n hw.memsize");
                if (!long.TryParse(totalOutput.Trim(), out long totalBytes))
                    return (-1, -1);
                double totalGB = totalBytes / 1073741824.0;

                // 获取可用内存
                var vmStat = ExecuteCommand("vm_stat");
                var free = GetVmStatValue(vmStat, "Pages free") + GetVmStatValue(vmStat, "Pages inactive");
                var pageSize = GetVmStatValue(vmStat, "page size of", 4096);
                double availableGB = free * pageSize / 1073741824.0;

                return (availableGB, totalGB);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"macOS异常: {ex.Message}");
                return (-1, -1);
            }
        }

        private static string ExecuteCommand(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            return process.StandardOutput.ReadToEnd();
        }

        private static long GetVmStatValue(string vmStat, string key, long defaultValue = 0)
        {
            var match = Regex.Match(vmStat, $@"{key}:\s*([\d,]+)");
            return match.Success ? long.Parse(match.Groups[1].Value.Replace(",", "")) : defaultValue;
        }
        #endregion

        /// <summary>
        /// 获取可用内存（GB）
        /// </summary>
        /// <returns></returns>
        public static double GetAvailableGB()
        {
            var result = DATA.OS.id switch
            {
                "windows" => GetWindowsMemory(),
                "linux" => GetLinuxMemory(),
                "osx" => GetMacOSMemory(),
                _ => (-1,-1)
            };
            return result.availableGB;
        }

        /// <summary>
        /// 获取系统总物理内存（GB）
        /// </summary>
        public static double GetTotalGB()
        {
            return DATA.OS.id switch
            {
                "windows" => GetWindowsMemory().totalGB,
                "linux" => GetLinuxMemory().totalGB,
                "osx" => GetMacOSMemory().totalGB,
                _ => -1
            };
        }

        /// <summary>
        /// 同时获取可用内存和总内存
        /// </summary>
        public static (double availableGB, double totalGB) GetMemoryInfo()
        {
            return DATA.OS.id switch
            {
                "windows" => GetWindowsMemory(),
                "linux" => GetLinuxMemory(),
                "osx" => GetMacOSMemory(),
                _ => (-1, -1)
            };
        }
    }
}
