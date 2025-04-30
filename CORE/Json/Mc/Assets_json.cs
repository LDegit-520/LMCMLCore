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
    public class Assets_json
    {
        public AssetsJson assetsInfo;
        public Assets_json(string path)
        {
            assetsInfo = JsonSerializer.Deserialize<AssetsJson>(File.ReadAllText(path));
        }
        public List<DownLoadTask> GetAssetsDownLoadTasks()
        {
            List<DownLoadTask> tasks = new();
            foreach (var item in assetsInfo.objects)
            {
                tasks.Add(new DownLoadTask(URL.GetAssetsbmcl($"/{item.Value.hash.Substring(0, 2)}/{item.Value.hash}"),
                    Path.Combine(PATH.OBJECTS, item.Value.hash.Substring(0, 2), item.Value.hash),
                    item.Value.size,
                    item.Value.hash,
                    HashInfo.SHA1//mc资源索引为SHA1
                    ));
            }
            return tasks;
        }
    }
    public class AssetsIndex
    {
        public string id { get; set; }
        public string hash { get; set; }
        public HashInfo hashinfo { get; set; } = HashInfo.SHA1;
        public long size { get; set; }
        public long totalSize { get; set; }
        public string url { get; set; }
    }
    public class AssetsJson
    {
        public Dictionary<string, asshash> objects { get; set; }//资源字典
        public class asshash
        {
            public string hash { get; set; }//哈希值
            public long size { get; set; }//文件大小
        }
    }
}
