using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Json.Mc
{
    public class VersionList_json
    {
        /// <summary>
        /// 正式版 key为版本id，value为版本数据
        /// </summary>
        public  Dictionary<string, McVersionInfo> release { get; set; } = new Dictionary<string, McVersionInfo>();
        /// <summary>
        /// 快照版 key为版本id，value为版本数据
        /// </summary>
        public Dictionary<string, McVersionInfo> snapshot { get; set; } = new Dictionary<string, McVersionInfo>();
        /// <summary>
        /// beta版 key为版本id，value为版本数据
        /// </summary>
        public Dictionary<string, McVersionInfo> old_beta { get; set; } = new Dictionary<string, McVersionInfo>();
        /// <summary>
        /// alpha版 key为版本id，value为版本数据
        /// </summary>
        public Dictionary<string, McVersionInfo> old_alpha { get; set; } = new Dictionary<string, McVersionInfo>();
        /// <summary>
        /// 最新版 key为版本id，value为版本数据
        /// </summary>
        public Dictionary<string, McVersionInfo> latest { get; set; } = new Dictionary<string, McVersionInfo>();
        /// <summary>
        /// 愚人节版 key为版本id，value为版本数据
        /// </summary>
        public Dictionary<string, McVersionInfo> aprilfool { get; set; } = new Dictionary<string, McVersionInfo>();
        public VersionList_json(string path)
        {
            GetVerList(path);
        }
        public VersionList_json()
        {
            DownLoadTask downLoadTask = new DownLoadTask(URL.GetVersionManifest(), Path.Combine(PATH.TEMP, "version_manifest.json"));
            FILE.FileDelete.TimeOutDelete(downLoadTask.Path, 1 * 60 * 24);
            if (!File.Exists(downLoadTask.Path))
            {
                DownLoadCore downLoadCore = new DownLoadCore();
                downLoadCore.StartDownloadAsync(new List<DownLoadTask> { downLoadTask }).Wait();
            }
            GetVerList(downLoadTask.Path);
        }
        /// <summary>
        /// 用来获取url中的sha1
        /// </summary>
        /// <param name="url">Versions的url</param>
        /// <returns>sha1值</returns>
        public static string SetUrlSha(string url)
        {
            string result = url.Substring(0, url.LastIndexOf('/')); ;
            result = result.Substring(result.LastIndexOf('/') + 1);
            return result;
        }
        /// <summary>
        /// 生成版本列表字典
        /// </summary>
        public void GetVerList(string path)
        {
            string file = File.ReadAllText(path);
            var versJson = JsonSerializer.Deserialize<McVersionListJson>(file);
            for (int i = 0; i < versJson.versions.Count; i++)
            {
                versJson.versions[i].hash = SetUrlSha(versJson.versions[i].url);//添加哈希值
                if (versJson.versions[i].id == versJson.latest.release)//最新正式版
                {
                    latest.Add(versJson.versions[i].id, versJson.versions[i]);
                }
                if (versJson.versions[i].id == versJson.latest.snapshot)//最新快照版
                {
                    latest.Add(versJson.versions[i].id, versJson.versions[i]);
                }
                #region 愚人节版本处理//很烦人，这玩意只能手动处理
                if (
                    versJson.versions[i].id == "25w14craftmine" ||
                    versJson.versions[i].id == "24w14potato" ||
                    versJson.versions[i].id == "23w13a_or_b" ||
                    versJson.versions[i].id == "22w12oneblockatatime" ||
                    versJson.versions[i].id == "20w14infinite" ||
                    versJson.versions[i].id == "3D Shareware v1.34" ||
                    versJson.versions[i].id == "1.RV-Pre1" ||
                    versJson.versions[i].id == "15w14a"
                    )
                {
                    aprilfool.Add(versJson.versions[i].id, versJson.versions[i]);
                }
                #endregion
                switch (versJson.versions[i].type)//其他版本
                {
                    case "release":
                        {
                            release.Add(versJson.versions[i].id, versJson.versions[i]);
                            break;
                        }
                    case "snapshot":
                        {
                            snapshot.Add(versJson.versions[i].id, versJson.versions[i]);
                            break;
                        }
                    case "old_beta":
                        {
                            old_beta.Add(versJson.versions[i].id, versJson.versions[i]);
                            break;
                        }
                    case "old_alpha":
                        {
                            old_alpha.Add(versJson.versions[i].id, versJson.versions[i]);
                            break;
                        }
                }

            }
            //return result;
        }
    }
    /// <summary>
    /// 版本信息
    /// </summary>
    public class McVersionInfo
    {
        /// <summary>
        /// 版本id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 版本类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 版本json下载地址（包含sha1）
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 版本更新
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 版本发布时间//不太确定
        /// </summary>
        public string releaseTime { get; set; }
       /// <summary>
       /// 哈希值
       /// </summary>
        public string hash { get; set; }
        /// <summary>
        /// 哈希类型
        /// </summary>
        public HashInfo hashinfo { get; set; } = HashInfo.SHA1;
        /// <summary>
        /// 文件大小，json文件中没有但估计值为300kb左右
        /// </summary>
        public long size { get; set; } = 3 * 100 * 1024;
    }
    public class McVersionListJson
    {
        /// <summary>
        /// 包含最新版本和最新快照版本
        /// </summary>
        public Latest latest { get; set; }
        /// <summary>
        /// 所有版本
        /// </summary>
        public List<McVersionInfo> versions { get; set; }
        /// <summary>
        /// 最新版本
        /// </summary>
        public class Latest
        {
            /// <summary>
            /// 最新正式版
            /// </summary>
            public string release { get; set; }
            /// <summary>
            /// 最新快照版
            /// </summary>
            public string snapshot { get; set; }
        }
    }
}
