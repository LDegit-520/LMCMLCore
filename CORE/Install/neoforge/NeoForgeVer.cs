using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.neoforge
{
    class NeoForgeVer
    {
        /// <summary>
        /// 获取neoforge版本列表
        /// </summary>
        /// <param name="isdown">是否强制获取（不添加这个对于1.20.1会7天获取一次其他会1天获取一次）</param>
        /// <returns></returns>
        public static async Task<Dictionary<string,List<string>>> getNeoForgeVerUrl(bool isdown=false)
        {
            var dictver = new Dictionary<string, List<string>>();
            //1.20.1列表
            //string url20_1 = "https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/forge";
            string urlPath20_1 = Path.Combine(PATH.TEMPNEOFORGE,"1201.json");
            //1.20.1以上列表
            //string url20_1up = "https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/neoforge";
            string urlPath20_1up = Path.Combine(PATH.TEMPNEOFORGE, "1201up.json");
            FileDelete.RepeatFile(urlPath20_1,isdown, 1 * 60 * 24 * 7);
            FileDelete.RepeatFile(urlPath20_1up, isdown);
            //await DownLoadManager.RunTask(new DownLoadTask(URL.GetNeoForgeListforge(), urlPath20_1));
            //await DownLoadManager.RunTask(new DownLoadTask(URL.GetNeoForgeListneoforge(), urlPath20_1up));
            //列表获取完成，开始处理列表
            // 读取JSON文件并解析
            var L201 = JsonNode.Parse(File.ReadAllText(urlPath20_1))!;
            // 提取versions数组并填充到listver
            if (L201["versions"] is JsonArray versions)
            {
                var ListVer=versions.Select(node => node!.ToString()).ToList();
                for(int i=0;i<ListVer.Count;i++)
                {
                    ListVer[i] = ListVer[i].Replace("1.20.1-", "");//去除1.20.1-前缀只保留版本号
                }
                dictver.Add("1.20.1", ListVer);
            }
            var L201u = JsonNode.Parse(File.ReadAllText(urlPath20_1up))!;
            if (L201u["versions"] is JsonArray Uversions)
            {
                var ListVer=Uversions.Select(node => node!.ToString()).ToList();
                foreach (var ver in ListVer)
                {
                    var Lver = ver.Split(".");
                    if (dictver.ContainsKey($"1.{Lver[0]}.{Lver[1]}"))//存在版本键
                    {
                        dictver[$"1.{Lver[0]}.{Lver[1]}"].Add(ver);
                    }
                    else
                    {
                        dictver.Add($"1.{Lver[0]}.{Lver[1]}", new List<string> { ver });
                    }
                }
            }
            return dictver;
        }
        /// <summary>
        /// 获取neoforge版本字典
        /// </summary>
        /// <param name="mcver">mc版本</param>
        /// <param name="ver">neoforge版本</param>
        /// <returns>字典键为meoforge版本，值为版本信息</returns>
        public static Dictionary<string, NeoForgeInfo> getNeoForgeVer(string mcver,string ver)
        {
            return new Dictionary<string, NeoForgeInfo>() 
            { 
                { 
                    ver.Replace("-beta", ""), //去除-beta后缀
                    new NeoForgeInfo() 
                    { 
                        McverID = mcver, //mc版本
                        Ver = ver, //neoforge版本
                        Url = URL.GetNeoForge(mcver, ver), //下载地址
                        isbeta = ver.Contains("-beta") //是否为beta版
                    } 
                } 
            };
        }
        /// <summary>
        /// 下载neoforge
        /// </summary>
        /// <param name="verinfo">neoforge信息</param>
        public static async Task NeoForgeDown(NeoForgeInfo verinfo,bool isdown=false)
        {
            string url = verinfo.Url;//下载地址
            string path = Path.Combine(PATH.GNEOFORGE, $"{verinfo.McverID}-{verinfo.Ver}.jar");//保存地址
            if(isdown)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            var listtask = new List<DownLoadTask>() { new DownLoadTask(url,path) };
            //listtask = Taskjiancha.ClearTask(listtask);
            if (listtask.Count != 0)//下载列表不为空
            {
                //using (var manager = new DownLoadManager(listtask))
                //{
                //    manager.ProgressChanged += (s, progress) => Console.WriteLine($"总进度: {progress:P0}");
                //    await manager.StartAsync();
                //}
                //DownLoadManager downLoadManager = new DownLoadManager();
                //downLoadManager.AddTask(listtask);
                //downLoadManager.Run();
            }
        }
    }
    public class NeoForgeInfo
    {
        /// <summary>
        /// mc版本id
        /// </summary>
        public string McverID { get; set; }
        /// <summary>
        /// neoforge版本
        /// </summary>
        public string Ver { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 是否为beta版
        /// </summary>
        public bool isbeta { get; set; }
    }
}




