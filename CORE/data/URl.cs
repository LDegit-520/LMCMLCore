using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.data
{
    /*
     * 链接 2.0
     * 对于部分不需要方法的为了好看都改成了方法（对资源的消耗差别不是很大）
     * 感谢bmclapi和bangbang93提供的下载源
     * 
     * 程序的所有链接都应该写在这里
     */
    /// <summary>
    /// 存放网址官网源无后缀，bmcl源添加bmcl后缀
    /// </summary>
    class URL
    {
        #region 原版mc
        /// <summary>
        /// 返回版本清单1的URL
        /// </summary>
        /// <returns>版本清单1的URL</returns>
        public static string GetVersionManifest()
        {
            return "https://piston-meta.mojang.com/mc/game/version_manifest.json";
        }

        /// <summary>
        /// 返回版本清单1的备用URL
        /// </summary>
        /// <returns>版本清单1的备用URL</returns>
        public static string GetVersionManifest1au()
        {
            return "http://launchermeta.mojang.com/mc/game/version_manifest.json";
        }

        /// <summary>
        /// 返回版本清单1的bmcl源URL
        /// </summary>
        /// <returns>版本清单1的另一个源URL</returns>
        public static string GetVersionManifestbmcl()
        {
            return "https://bmclapi2.bangbang93.com/mc/game/version_manifest.json";
        }

        /// <summary>
        /// 返回带sha1的版本清单2的URL
        /// </summary>
        /// <returns>带sha1的版本清单2的URL</returns>
        public static string GetVersionManifest2()
        {
            return "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        }

        /// <summary>
        /// 返回带sha1的版本清单2的备用URL
        /// </summary>
        /// <returns>带sha1的版本清单2的备用URL</returns>
        public static string GetVersionManifest1au2()
        {
            return "http://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
        }

        /// <summary>
        /// 返回带sha1的版本清单2的bmcl源URL
        /// </summary>
        /// <returns>带sha1的版本清单2的另一个源URL</returns>
        public static string GetVersionManifestbmcl2()
        {
            return "https://bmclapi2.bangbang93.com/mc/game/version_manifest_v2.json";
        }
        /// <summary>
        /// ass的json下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://piston-meta.mojang.com </para>
        /// 把完整的url替换为piston-meta开头
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetAssetsInpi(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://piston-meta.mojang.com" + url; }
            return url.Replace("http://launchermeta.", "https://piston-meta.").Replace("https://launcher.", "https://piston-meta.");
        }
        /// <summary>
        /// ass的json下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://launchermeta.mojang.com </para>
        /// 把完整的url替换launchermeta为开头
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetAssestsInme(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://launchermeta.mojang.com" + url; }
            return url.Replace("https://piston-meta.", "http://launchermeta.").Replace("https://launcher.", "http://launchermeta.");
        }
        /// <summary>
        /// ass的json下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://launcher.mojang.com </para>
        /// 把完整的url替换launcher为开头
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetAssetsInla(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://launcher.mojang.com" + url; }
            return url.Replace("https://piston-meta.", "http://launcher.").Replace("https://launchermeta.", "http://launcher.");
        }
        /// <summary>
        /// ass的json下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://bmclapi2.bangbang93.com </para>
        /// 把完整的url替换bmcl源
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetAssetsInbmcl(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://bmclapi2.bangbang93.com" + url; }
            return url.Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://launcher.mojang.com", "https://bmclapi2.bangbang93.com");
        }
        /// <summary>
        /// ass下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://resources.download.minecraft.net </para>
        /// 把完整的url替换为官方
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetAssets(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://resources.download.minecraft.net" + url; }
            return url.Replace("https://bmclapi2.bangbang93.com/assets", "https://resources.download.minecraft.net");
        }
        /// <summary>
        /// ass下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://bmclapi2.bangbang93.com/assets </para>
        /// 把完整的url替换为bmcl
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetAssetsbmcl(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://bmclapi2.bangbang93.com/assets" + url; }
            return url.Replace("http://resources.download.minecraft.net", "https://bmclapi2.bangbang93.com/assets");
        }

        /// <summary>
        /// lib库下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://libraries.minecraft.net </para>
        /// 把完整的url替换为官方
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetLib(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://libraries.minecraft.net" + url; }
            return url.Replace("https://bmclapi2.bangbang93.com/maven", "https://libraries.minecraft.net");
        }
        /// <summary>
        /// lib前缀网址 主要用于快速获取官方lib前缀 （用于启动参数构建）
        /// </summary>
        public static string getlib { get; } = "https://libraries.minecraft.net";
        /// <summary>
        /// lib库下载地址
        /// <para>把不完整的url添加上（请注意不带/） https://bmclapi2.bangbang93.com/maven </para>
        /// 把完整的url替换为bmcl
        /// </summary>
        /// <param name="url">需要转换的网址</param>
        /// <returns>转换完成的网址</returns>
        public static string GetLibbmcl(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://"))) { return "https://bmclapi2.bangbang93.com/maven" + url; }
            return url.Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/maven");
        }
        /// <summary>
        /// 版本json下载地址，官方源。这个其实是多次一举，但是为了好看，这一点点影响不大
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <returns>下载地址</returns>
        public static string GetJson(string url)
        {
            return url; 
        }
        /// <summary>
        /// 版本json下载地址，bmcl源。
        /// </summary>
        /// <param name="url">这个请注意要填的是版本号，不是下载地址</param>
        /// <returns>下载地址</returns>
        public static string GetJsonbmcl(string url)
        {
            return $"https://bmclapi2.bangbang93.com/version/{url}/json";
        }
        /// <summary>
        /// 客户端下载地址，官方源。这个其实是多次一举，但是为了好看，这一点点影响不大
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <returns>下载地址</returns>
        public static string Getclient(string url)
        {
            return url;
        }
        /// <summary>
        /// 客户端下载地址，bmcl源。
        /// </summary>
        /// <param name="url">这个请注意要填的是版本号，不是下载地址</param>
        /// <returns>下载地址</returns>
        public static string Getclientbmcl(string url)
        {
            return $"https://bmclapi2.bangbang93.com/version/{url}/client";
        }
        /// <summary>
        /// 服务端下载地址，官方源。这个其实是多次一举，但是为了好看，这一点点影响不大
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <returns>下载地址</returns>
        public static string Getserver(string url)
        {
            return url;
        }
        /// <summary>
        /// 服务端下载地址，bmcl源。
        /// </summary>
        /// <param name="url">这个请注意要填的是版本号，不是下载地址</param>
        /// <returns>下载地址</returns>
        public static string Getserverbmcl(string url)
        {
            return $"https://bmclapi2.bangbang93.com/version/{url}/server";
        }
        #endregion
        #region Forge
        /// <summary>
        /// forge下载地址，官方源。主要为了好看，这一点点影响不大
        /// </summary>
        /// <param name="url">下载地址（不包含前缀）</param>
        /// <returns>完成下载地址</returns>
        public static string GetForge(string url)
        {
            return $"https://files.minecraftforge.net/net/minecraftforge/forge/{url}";
        }
        #endregion
        #region NeoForge
        /// <summary>
        /// neoforge版本列表获取地址（1.20.1）
        /// </summary>
        /// <returns>下载地址</returns>
        public static string GetNeoForgeListforge()
        {
            return "https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/forge";
        }
        /// <summary>
        /// neoforge版本列表获取地址（1.20.1以上，不包含）
        /// </summary>
        /// <returns>下载地址</returns>
        public static string GetNeoForgeListneoforge()
        {
            return "https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/neoforge";
        }
        /// <summary>
        /// neoforge下载地址(完整地址)
        /// </summary>
        /// <param name="mcversion">mc版本</param>
        /// <param name="version">neoforge版本</param>
        /// <returns></returns>
        public static string GetNeoForge(string mcversion,string version)
        {
            string curl = "neoforge";
            if (mcversion=="1.20.1")
            {
                curl += "forge/";
            }
            return $"https://maven.neoforged.net/releases/net/neoforged/{curl}/{version}/{curl}-{version}-installer.jar";
        }
        #endregion
        #region Fabric
        /// <summary>
        /// fabric版本列表获取地址
        /// </summary>
        /// <returns>下载网址</returns>
        public static string GetFabricList()
        {
            return "https://meta.fabricmc.net/v2/versions";
        }
        /// <summary>
        /// 获取fabric的对应mc版本json
        /// </summary>
        /// <param name="fabricVer">fabric版本</param>
        /// <param name="mcVer">mc版本</param>
        /// <returns>下载地址</returns>
        public static string GetFabricVersionJson(string fabricVer,string mcVer)
        {
            return $"https://meta.fabricmc.net/v2/versions/loader/{mcVer}/{fabricVer}/profile/json";
        }
        /// <summary>
        /// 获取fabric的对应mc版本json bmcl源
        /// </summary>
        /// <param name="fabricVer">fabric版本</param>
        /// <param name="mcVer">mc版本</param>
        /// <returns>下载地址</returns>
        public static string GetFabricVersionJsonbmcl(string fabricVer, string mcVer)
        {
            return $"https://bmclapi2.bangbang93.com/fabric-meta/v2/versions/loader/{mcVer}/{fabricVer}/profile/json";
        }
        #endregion
        #region modrinth
        /// <summary>
        /// 获取Modrinth的搜索基地址
        /// </summary>
        /// <returns></returns>
        public static string GetModrinthSearch()
        {
            return "https://api.modrinth.com/v2/search";
        }
        /// <summary>
        /// 获取项目信息地址
        /// </summary>
        /// <param name="name">项目名称（id,slug）</param>
        /// <returns></returns>
        public static string GetModrinthProject(string name)
        {
            return $"https://api.modrinth.com/v2/project/{name}";
        }
        /// <summary>
        /// 获取项目版本列表地址
        /// </summary>
        /// <param name="name">项目名称（id,slug）</param>
        /// <returns></returns>
        public static string GetModrinthProjectVersion(string name)
        {
            return $"https://api.modrinth.com/v2/project/{name}/version";
        }
        /// <summary>
        /// 获取项目依赖列表地址
        /// </summary>
        /// <param name="name">项目名称（id,slug）</param>
        /// <returns></returns>
        public static string GetModrinthProjectDependencies(string name)
        {
            return $"https://api.modrinth.com/v2/project/{name}/dependencies";
        }
        #endregion
    }
}
