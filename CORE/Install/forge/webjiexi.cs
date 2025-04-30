using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.forge
{
    /*
     * 输入mc版本，解析生成forge访问路径
     * 下载gorge网页，解析网页，获取forge信息（这里应该使用类进行存储）
     * 把forge信息存储到本地
     */
    //class webjiexi
    //{
    //    /// <summary>
    //    /// 官方源，下载网页，解析网页，获取forge列表
    //    /// </summary>
    //    /// <param name="mcver"></param>
    //    public static bool GetForgeInfo_OFFICIAL(string mcver)
    //    {
    //        string url = DownDATA.OFFICIAL["forge"] + $"/maven/net/minecraftforge/forge/index_{mcver.Replace("-", "_")}.html";
    //        var tasks = new DownloadTask(url, Path.Combine(data.DATA.EXEPATH, data.DATA.LMCML, "temp"), "forge" + mcver.Replace("-", "_") + ".html");//版本网页
    //        if (!xiazai.GetForgeInfo(tasks, nameof(GetForgeInfo_OFFICIAL)))
    //        {
    //            return false;
    //        }//官方源获取失败
    //         //数据解析
    //        #region 数据解析
    //        try
    //        {
    //            string html = File.ReadAllText(Path.Combine(data.DATA.EXEPATH, data.DATA.LMCML, "temp", "forge" + mcver.Replace("-", "_") + ".html"));//读取网页

    //        }
    //        catch (Exception e)
    //        {
    //            Logger.Error(nameof(webjiexi), "解析forge网页失败");
    //            return false;
    //        }
    //        #endregion
    //        return true;
    //    }
    //    /// <summary>
    //    /// bmcl源获取forge列表
    //    /// </summary>
    //    /// <param name="mcver">mc版本</param>
    //    /// <returns>forge列表</returns>
    //    public static bool GetForgeInfo_BMCLAPI(string mcver)
    //    {
    //        string url = DownDATA.BMCLAPI["forge"] + $"/forge/minecraft/{mcver.Replace("-", "_")}";
    //        var tasks = new DownloadTask(url, Path.Combine(data.DATA.EXEPATH, data.DATA.MC_ZHU, "gameass","forge"), "bmcl" + mcver.Replace("-", "_") + ".json");//版本json
    //        if (!xiazai.GetForgeInfo(tasks, nameof(GetForgeInfo_OFFICIAL)))
    //        {
    //            return false;
    //        }//BMCL源获取失败
    //        #region 数据解析
    //        string sfile = File.ReadAllText(Path.Combine(data.DATA.EXEPATH, data.DATA.MC_ZHU, "gameass", "forge", "bmcl" + mcver.Replace("-", "_") + ".json"));//读取文件
    //        List<forgevers_man> foman = JsonSerializer.Deserialize<List<forgevers_man>>(sfile);//解析成数据类
    //        List<forgevers_down> fodown = new List<forgevers_down>();//解析为下载列表
    //        for(int i=0;i<foman.Count;i++)//循环判断
    //        {
    //            var _= new forgevers_down();
    //            _.category = null;
    //            for (int j = 0; j < foman[i].files.Count;j++)
    //            {
    //                if (foman[i].files[j].category == "installer")//优先添加installer
    //                {
    //                    _.format = foman[i].files[j].format;
    //                    _.category = foman[i].files[j].category;
    //                    _.hash = foman[i].files[j].hash;
    //                    break;//结束循环
    //                } else if (foman[i].files[j].category == "universal")//其次添加universal
    //                {
    //                    _.format = foman[i].files[j].format;
    //                    _.category = foman[i].files[j].category;
    //                    _.hash = foman[i].files[j].hash;
    //                } else if (_.category == null&&foman[i].files[j].category == "client")//最后添加client
    //                {
    //                    _.format = foman[i].files[j].format;
    //                    _.category = foman[i].files[j].category;
    //                    _.hash = foman[i].files[j].hash;
    //                }
    //            }
    //            _.mcversion = foman[i].mcversion;
    //            _.version = foman[i].version;
    //            _.modified = foman[i].modified;
    //            _.branch = foman[i].branch == null? null: foman[i].branch.ToString();//临时
    //            fodown.Add(_);
    //        }
    //        #endregion
    //        string str = JsonSerializer.Serialize<List<forgevers_down>>(fodown,data.DATA.JSON_OPTIONS);
    //        Console.WriteLine(str);
    //        return true;
    //    }
    //}
    /// <summary>
    /// forge下载数据类
    /// </summary>
    public class forgevers_down
    {
        public string mcversion { get; set; }
        public string version { get; set; }
        public string branch { get; set; }
        public string category { get; set; }
        public string format { get; set; }
        public string modified { get; set; }
        public string hash { get; set; }
    }
    /// <summary>
    /// bmcl数据类
    /// </summary>
    public class forgevers_man
    {
        public string _id { get; set; }
        public int build { get; set; }
        public int __v { get; set; }
        public string version { get; set; }
        public string modified { get; set; }
        public string mcversion { get; set; }
        public List<File> files { get; set; }
        public object branch { get; set; }
        public class File
        {
            public string format { get; set; }
            public string category { get; set; }
            public string hash { get; set; }
        }
    }


}
