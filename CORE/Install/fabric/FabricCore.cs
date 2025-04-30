using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.fabric
{
    /*测试
       await FabricVer.getFabricVerJson();
       var json =FabricVer.GetJson();
       Console.WriteLine(string.Join("\r\n",json.intermediary.Select(p => p.version).ToList()));
     */
    /*
     * 请注意fabric Loader和fabric api是不一样的
     * 这里是fabric Loader（简称Fabric）的下载
     * Fabric对于每一个mc版本对应的Fabrci Loader版本是一至的
     * 只要支持这个mc版本那所有Fabric Loader都可以加入不进行区分
     * version.json文件中的
     *      loader存储的就是Fabric Loader的所有版本号
     *      game存储的是支持的mc版本号
     */
    class FabricCore
    {
        /// <summary>
        /// FabricVersionJson路径
        /// </summary>
        public static string FabricVersionJsonPath { get; set; } = Path.Combine(PATH.TEMPFABRIC, "fabric.json");
        /// <summary>
        /// 注意，在使用前请检查是否为null，如果为null请运行getFabricVerJson()
        /// </summary>
        public static FabricVerJson fabricVerJson { get; set; } = JsonSerializer.Deserialize<FabricVerJson>(File.ReadAllText(FabricVersionJsonPath), DATA.JSON_OPTIONS) ?? null;
        /// <summary>
        /// 获取FabricVersionJson
        /// </summary>
        /// <param name="isdown">是否删除本地缓存，使其强制更新数据</param>
        /// <returns></returns>
        public static async Task getFabricVerJson(bool isdown = false)
        {
            string url = URL.GetFabricList();//获取fabric版本列表下载地址
            FileDelete.RepeatFile(FabricVersionJsonPath, isdown);//重复文件检查
            //等待修改
            DownLoadCore downLoadCore = new();
            //await DownLoadManager.RunTask(new DownLoadTask(url, FabricVersionJsonPath));//等待下载完成
            //
            string fabricjson = File.ReadAllText(FabricVersionJsonPath);
            fabricVerJson = JsonSerializer.Deserialize<FabricVerJson>(fabricjson, DATA.JSON_OPTIONS);
        }
        ///// <summary>
        ///// 获取fabroc对应的mc版本json
        ///// </summary>
        //public static async Task<bool> GetFabricJson(string Fabricver,string McVer)
        //{
        //    //return await DownLoadManager.RunTask(new DownLoadTask(URL.GetFabricVersionJson(Fabricver,McVer),Path.Combine(PATH.GFRABRIC,$"{Fabricver}-{McVer}.json")));
        //}
        ///// <summary>
        ///// 获取fabroc对应的mc版本json bmcl源
        ///// </summary>
        //public static async Task<bool> GetFabricJsonBmcl(string Fabricver, string McVer)
        //{
        //    //return await DownLoadManager.RunTask(new DownLoadTask(URL.GetFabricVersionJsonbmcl(Fabricver, McVer), Path.Combine(PATH.GFRABRIC, $"{Fabricver}-{McVer}.json")));
        //}
        ///// <summary>
        ///// 下载指定版本安装器
        ///// </summary>
        ///// <param name="InstallVer">指定版本</param>
        //public static async Task<bool> GetINstaller(FabricVerJson.Installer Install)
        //{
        //    //return await DownLoadManager.RunTask(new DownLoadTask(Install.url,Path.Combine(PATH.GFRABRIC,$"installer-{Install.version}.jar")));
        //}
        ///// <summary>
        ///// 下载最新版安装器
        ///// </summary>
        //public static async Task<bool> GetINstaller()
        //{
        //    if (fabricVerJson == null) { await getFabricVerJson(); }
        //    return await GetINstaller(fabricVerJson.installer[0]);
        //}
    }
    /// <summary>
    /// FabricJson数据类
    /// </summary>
    public class FabricVerJson
    {
        public List<Game> game { get; set; }
        public List<Mappings> mappings { get; set; }
        public List<Intermediary> intermediary { get; set; }
        public List<Loader> loader { get; set; }
        public List<Installer> installer { get; set; }
        public class Game
        {
            public string version { get; set; }
            public bool stable { get; set; }
        }
        public class Mappings
        {
            public string gameVersion { get; set; }
            public string separator { get; set; }
            public int build { get; set; }
            public string maven { get; set; }
            public string version { get; set; }
            public bool stable { get; set; }
        }
        public class Intermediary
        {
            public string maven { get; set; }
            public string version { get; set; }
            public bool stable { get; set; }
        }
        public class Loader
        {
            public string separator { get; set; }
            public int build { get; set; }
            public string maven { get; set; }
            public string version { get; set; }
            public bool stable { get; set; }
        }
        public class Installer
        {
            public string url { get; set; }
            public string maven { get; set; }
            public string version { get; set; }
            public bool stable { get; set; }
        }
    }
}
