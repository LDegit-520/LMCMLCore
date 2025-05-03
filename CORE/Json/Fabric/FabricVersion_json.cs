using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Json.Fabric
{
    /// <summary>
    /// 作用处理Fabricjson并根据mc版本号提供Fabricjson数据
    /// </summary>
    public class FabricVersion_json
    {
        private static string FabricVersionJsonPath = Path.Combine(PATH.TEMPFABRIC, "fabric.json");
        public FabricVerJson VerJson { get; set; } = null;
        public FabricVersion_json()
        {
            //流程 检查Fabricjson文件是否存在，不存在则下载，存在则检查是否超时 1天为超时时间 如果超时则重新下载 如果未超时则使用缓存数据
            //不存在情况构造下载
            //读取文件
            //处理文件
            FILE.FileDelete.TimeOutDelete(FabricVersionJsonPath, 1 * 60 * 24);//检查是否超时
            if(!File.Exists(FabricVersionJsonPath))//文件不存在
            {
                var downtask=new DownLoadTask(URL.GetFabricList(),FabricVersionJsonPath,1 * 1024 * 1024);
                DownLoadCore downLoadCore =new();
                downLoadCore.StartDownloadAsync(new() { downtask }).Wait();
            }
            VerJson= JsonSerializer.Deserialize<FabricVerJson>(File.ReadAllText(FabricVersionJsonPath), DATA.JSON_OPTIONS) ?? null;
        }
        public List<FabricVerJson.Loader> GetFabricVersions(string mcVersion)
        {
            //根据mc获取Fabric版本
            //返回Fabric版本列表
            //不存在返回null
            if(VerJson.game.Any(item => item.version==mcVersion))
            {
                return VerJson.loader;
            }
            else
            {
                return null;
            }
        }
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
