using HtmlAgilityPack;
using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.forge
{
    /*
     * 获取forge所有的版本页面的下载地址
     * 
     */
    class ForgeVer
    {
        /// <summary>
        /// 获取forge所有的版本页面的下载地址（不包含前缀）
        /// </summary>
        /// <param name="isdown">是否删除本地缓存，使其强制更新数据</param>
        /// <returns>字典值为版本名，键为下载地址（不包含前缀）</returns>
        public static async Task<Dictionary<string,string>> getForgeVerUrl(bool isdown=false)
        {
            //forge1.20版本页面//这个无所谓用哪个都行。//用这个是因为比较少//在后面解析时消耗时间少
            string url = "https://files.minecraftforge.net/net/minecraftforge/forge/index_1.20.html";
            string path = Path.Combine(PATH.TEMPFORGE,"1.20.html");
            FileDelete.RepeatFile(path);
            //await DownLoadManager.RunTask(new DownLoadTask(url, path));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);
            var resultDict = new Dictionary<string, string>();

            // 匹配所有class="nav-collapsible "的ul
            string ulXpath = "//ul[contains(concat(' ', normalize-space(@class), ' '), ' nav-collapsible ')]";
            var ulNodes = doc.DocumentNode.SelectNodes(ulXpath);

            if (ulNodes == null) { return resultDict; }

            foreach (var ul in ulNodes)
            {
                // 提取当前ul下所有层级的li中的a标签
                var aNodes = ul.SelectNodes(".//li//a");
                if (aNodes == null) continue;

                foreach (var a in aNodes)
                {
                    // 提取文本和链接
                    string text = a.InnerText.Trim();
                    string href = a.GetAttributeValue("href", "").Trim();

                    // 有效性检查
                    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(href)) continue;

                    // 处理重复键：跳过已存在的项
                    if (!resultDict.ContainsKey(text))
                    {
                        resultDict.Add(text, href);
                    }
                    else
                    {
                        // 可选：记录警告或抛出异常
                        // Console.WriteLine($"重复键被跳过: {text}");
                    }
                }
            } 

            return resultDict;
        }

        /// <summary>
        /// 获取对应mc版本的forge版本字典
        /// </summary>
        /// <param name="ver">版本名称</param>
        /// <param name="url">下载地址（不含前缀）</param>
        /// <param name="isdown">是否删除本地缓存，使其强制更新数据</param>
        /// <returns>forge版本类型字典</returns>
        public static Dictionary<string, Forgeinfo> getForgeVer(string ver,string url,bool isdown=false)
        {
            url = URL.GetForge(url);
            string path = Path.Combine(PATH.TEMPFORGE, $"{ver}.html");
            FileDelete.RepeatFile(path,isdown,1 * 60);
            var listtask = new List<DownLoadTask>() { new DownLoadTask(url, path) };
            //listtask = Taskjiancha.ClearTask(listtask);
            if (listtask.Count != 0)//下载列表不为空
            {
                //using (var manager = new DownloadManager(listtask))
                //{
                //    manager.ProgressChanged += (s, progress) => Console.WriteLine($"总进度: {progress:P0}");
                //    manager.StartAsync().Wait();
                //}
                //DownLoadManager downLoadManager = new DownLoadManager();
                //downLoadManager.AddTask(listtask);
                //downLoadManager.Run();
            }
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);
            var resultDict = new Dictionary<string, Forgeinfo>();
            //第一部分。最新版和推荐版
            //第二部分。所有版本
            // XPath 定位表格和行
            var xpath = "//table[contains(concat(' ', normalize-space(@class), ' '), ' download-list ')]//tbody/tr";
            var rows = doc.DocumentNode.SelectNodes(xpath);
            if (rows == null) { return resultDict; }
            foreach (var row in rows)
            {
                var forgeinfo = new Forgeinfo();
                forgeinfo.McVer = ver;
                var versionNode = row.SelectSingleNode(".//td[@class='download-version']");
                forgeinfo.Ver = versionNode.InnerText.Trim();
                var timeNode = row.SelectSingleNode(".//td[@class='download-time']");
                forgeinfo.time = timeNode?.GetAttributeValue("title", null) ?? timeNode?.InnerText.Trim();
                var downNodes = row.SelectNodes(".//td[@class='download-files']//li");
                foreach (var li in downNodes)
                {
                    var forgedown = new Forgeinfo.ForgeDown();
                    // 处理链接和类型
                    var firstLink = li.SelectSingleNode(".//a[1]");
                    if (firstLink != null)
                    {
                        if(firstLink.GetAttributeValue("href", "").Contains("adfoc.us"))//去除可能存在的adfoc.us链接
                        {
                            var match = Regex.Match(firstLink.GetAttributeValue("href", ""), @"url=([^&]*)");
                            forgedown.Url = match.Success ? Uri.UnescapeDataString(match.Groups[1].Value) :firstLink.GetAttributeValue("href", "");
                        }
                        else
                        {
                            forgedown.Url = firstLink.GetAttributeValue("href", "");
                        }
                        forgedown.Type = firstLink.InnerText.Trim();
                    }
                    // 提取MD5和SHA1
                    var tooltip = li.SelectSingleNode(".//div[@class='info-tooltip']");
                    if (tooltip != null)
                    {
                        var tooltipText = tooltip.InnerText;
                        forgedown.MD5 = Regex.Match(tooltipText, @"MD5:\s*([a-f0-9]{32})", RegexOptions.IgnoreCase).Groups[1].Value;
                        forgedown.SHA1 = Regex.Match(tooltipText, @"SHA1:\s*([a-f0-9]{40})", RegexOptions.IgnoreCase).Groups[1].Value;
                    }
                    forgeinfo.downs.Add(forgedown);
                }
                resultDict.Add(forgeinfo.Ver, forgeinfo);
            }
            return resultDict;
        }
    }

    class Forgeinfo
    {
        /// <summary>
        /// mc版本
        /// </summary>
        public string McVer { get; set; }
        /// <summary>
        /// forge版本
        /// </summary>
        public string Ver { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 下载地址列表类型
        /// </summary>
        public List<ForgeDown> downs { get; set; } =new List<ForgeDown>();
        /// <summary>
        /// 下载地址类型
        /// </summary>
        public class ForgeDown
        {
            /// <summary>
            /// 类型
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 下载地址（完整地址）
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// MD5值
            /// </summary>
            public string MD5 { get; set; }
            /// <summary>
            /// SHA1值
            /// </summary>
            public string SHA1 { get; set; }
        }
    }
}
