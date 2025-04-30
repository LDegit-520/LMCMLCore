using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.OS
{
    public class OSInfo
    {
        /// <summary>
        /// id（windows，osx，linux）
        /// </summary>
        public string id{ get; set;}
        /// <summary>
        /// 系统描述
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 系统版本
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 系统架构
        /// </summary>
        public string arch { get; set; }
        /// <summary>
        /// 系统平台
        /// </summary>
        public string platform { get; set; }
        /// <summary>
        /// 是否为64位系统
        /// </summary>
        public string is64bit { get; set; }
        public static OSInfo COSInfo()
        {
            string osid = "none";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osid= "windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osid = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osid = "osx";
            }
            return new OSInfo()
            {
                id = osid,
                name = RuntimeInformation.OSDescription,
                arch = RuntimeInformation.ProcessArchitecture.ToString(),
                version = Environment.OSVersion.Version.ToString(),
                platform = Environment.OSVersion.Platform.ToString(),
                is64bit = Environment.Is64BitOperatingSystem ? "64" : "32"
            };
        }
    }
}
