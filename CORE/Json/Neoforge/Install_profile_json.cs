using LMCMLCore.CORE.data;
using LMCMLCore.CORE.FILE;
using LMCMLCore.CORE.Json.Mc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LMCMLCore.CORE.Json.Neoforge
{
    /// <summary>
    /// 安装器json文件，注：本json用来提取需要下载的文件，并提前下载到指定目录，，已减少安装器安装时的下载耗时
    /// </summary>
    public class Install_profile_json
    { 
        
        public Install_profile_json_Info info { get; set; } = new Install_profile_json_Info();
        public List<UnLibraries> Libs { get; set; } = new List<UnLibraries>();
        public void DisData()
        {
            foreach (var item in info.data)
            {
                if(item.Value.client.StartsWith("["))//最前面为[
                {
                    string str=item.Value.client.Replace("[","").Replace("]","");
                    var strs=str.Split(":");
                    string astr = Path.Combine(PATH.LIBRARIES ,strs[0].Replace(".", "/"), strs[1], strs[2], $"{strs[1]}-{strs[2]}-{strs[3].Replace("@", ".")}{(strs[3].Contains("@")?"":".jar")}");
                }
            }
        }
        public void DisLib()
        {
            for(int i=0;i<info.libraries.Count;i++)
            {
                Libs.Add(new UnLibraries()
                {
                    url = info.libraries[i].downloads.artifact.url,
                    path = Path.Combine(PATH.LIBRARIES,info.libraries[i].downloads.artifact.path),
                    size = info.libraries[i].downloads.artifact.size,
                    hash = info.libraries[i].downloads.artifact.sha1,
                    hashinfo = HashInfo.SHA1
                });
            }
        }
    }

    public class Install_profile_json_Info
    {
        public int spec { get; set; }
        public string profile { get; set; }
        public string version { get; set; }
        public string icon { get; set; }
        public string minecraft { get; set; }
        public string json { get; set; }
        public string logo { get; set; }
        public string welcome { get; set; }
        public string mirrorList { get; set; }
        public bool hideExtract { get; set; }
        public Dictionary<string,DataInfo> data { get; set; }
        public List<ProcessorInfo> processors { get; set; }
        public List<LibInfo> libraries { get; set; }
        public string serverJarPath { get; set; }
        public class DataInfo
        {
            public string client { get; set; }
            public string server { get; set; }
        }
        public class ProcessorInfo
        {
            public List<string> sides { get; set; }
            public string jar  { get; set; }
            public List<string> classpath { get; set; }
            public List<string> args { get; set; }
        }
        public class LibInfo
        {
            public string name { get; set; }
            public Down downloads { get; set; }
            public class Down
            {
                public Artifact artifact { get; set; }
                public class Artifact
                {
                public string sha1 { get; set; }
                public long size { get; set; }
                public string url { get; set; }
                public string path { get; set; }
            }

            }
        }
    }
}
