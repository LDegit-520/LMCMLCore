using LMCMLCore.CORE.data;
using LMCMLCore.CORE.FILE;
using LMCMLCore.CORE.Json.Mc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Json.Fabric
{
    public class Fabric_json : IModLoaderJson
    {
        public FabricjsonInfo VerJson { get; set; } = null;
        public Fabric_json(string jsonpath)
        {
            VerJson = JsonSerializer.Deserialize<FabricjsonInfo>(File.ReadAllText(jsonpath),DATA.JSON_OPTIONS);
        }
        public List<string> GetArgumentsGame()
        {
            return VerJson.arguments.game;
        }

        public List<string> GetArgumentsJvm()
        {
            return VerJson.arguments.jvm;
        }

        public List<UnLibraries> GetLibraries()
        {
            List<UnLibraries> list = new();
            for(int i=0;i<VerJson.libraries.Count;i++)
            {
                var names= VerJson.libraries[i].name.Split(":");
                list.Add(new()
                {
                    url = $"{VerJson.libraries[i].url}{names[0].Replace(".", "/")}/{names[1]}/{names[2]}/{names[1]}-{names[2]}.jar",
                    path = Path.Combine(PATH.LIBRARIES,$"{names[0].Replace(".", "/")}/{names[1]}/{names[2]}/{names[1]}-{names[2]}.jar"),
                    size = VerJson.libraries[i].size,
                    hash = VerJson.libraries[i].sha1,
                    hashinfo = HashInfo.SHA1
                });
            }
            return list;
        }

        public string GetMainclass()
        {
            return VerJson.mainClass;
        }
    }

    public class FabricjsonInfo
    {
        public string id { get; set; }
        public string inheritsFrom { get; set; }
        public string releaseTime { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public string mainClass { get; set; }
        public Arguments arguments { get; set; }
        public List<Library> libraries { get; set; }

        public class Arguments
        {
            public List<string> game { get; set; }
            public List<string> jvm { get; set; }
        }

        public class Library
        {
            public string name { get; set; }
            public string url { get; set; }
            public string md5 { get; set; }
            public string sha1 { get; set; }
            public string sha256 { get; set; }
            public string sha512 { get; set; }
            public long size { get; set; }
        }
    }
}
