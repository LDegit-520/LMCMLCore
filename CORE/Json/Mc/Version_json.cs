using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using LMCMLCore.CORE.FILE;
using System.Xml.Linq;
using LMCMLCore.CORE.data;
using System.Text.Unicode;
using LMCMLCore.CORE.LOGGER;

namespace LMCMLCore.CORE.Json.Mc
{
    /// <summary>
    /// 通用库数据，可以存储lib和nat数据，既可以用于生成下载任务，也可以用于生成启动参数
    /// </summary>
    public class UnLibraries
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string url;
        /// <summary>
        /// 本地保存路径
        /// </summary>
        public string path;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long size;
        /// <summary>
        /// 哈希值
        /// </summary>
        public string hash;
        /// <summary>
        /// 哈希类型
        /// </summary>
        public HashInfo hashinfo;
        /// <summary>
        /// 在有不属于上面的数据，但有比较重要的使用这个记录下来
        /// </summary>
        public object? SpecialData;
    }
    /// <summary>
    /// mc日志配置数据类
    /// </summary>
    public class McLogging
    {
        public string ver;
        public string id;
        public string url;
        public string type;
        public long size;
        public string argument;
        public string hash;
        public HashInfo hashinfo;
    }
    /// <summary>
    /// mc本体jar下载数据类
    /// </summary>
    public class McDown
    {
        public Down client { get; set; }
        public Down client_mappings { get; set; }
        public Down server { get; set; }
        public Down server_mappings { get; set; }
        public Down windows_server { get; set; }
        public Dictionary<string,object> other{ get; set; }
        public class Down
        {
            public HashInfo hashinfo { get; set; }
            public string? hash { get; set; }
            public long size { get; set; }
            public string? url { get; set; }
        }
    }
    public class Version_json
    {
        public ArgumentsJson argumentsJson;
        public List<UnLibraries> Libraries { get; set; }
        public List<UnLibraries> Natives { get; set; }
        public McLogging McLogging { get; set; }
        public  McDown McDown { get; set; }
        public AssetsIndex AssetsIndex { get; set; }
        public List<string> Arg_Game { get; set;}
        public List<string> Arg_Jvm { get; set; }
        public int GetJavaVersion()
        {
            return argumentsJson.javaVersion?.majorVersion??0;
        }
        public string GetMainclass()
        {
            return argumentsJson.mainClass;
        }
        /// <summary>
        /// 版本清单json构造函数
        /// </summary>
        /// <param name="path">版本清单json</param>
        /// <param name="_GameArg">Game参数主要为那些带规则参数和是否添加</param>
        /// <param name="_JvmArg">Jvm参数为自定义的jvm参数，值的真假代表是否为通用jvm参数（真代表新旧版均要添加，假代表仅有旧版添加）</param>
        /// <param name="OSname">当前系统名称windows，osx，linux</param>
        /// <param name="OSis64">当前系统位数（主要给windows使用）</param>
        public Version_json(string path, Dictionary<string, bool> _GameArg,Dictionary<string, bool> _JvmArg, string OSname, string OSis64)
        {
            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,//忽略null
                WriteIndented = true, // 格式化输出
                Converters = {
                    new ArgumentsJson.Arguments.EitherConverter<string, ArgumentsJson.Arguments.GJRules>(),
                    new ArgumentsJson.Arguments.EitherConverter<string, List<string>>(),
                }
            };
            argumentsJson = JsonSerializer.Deserialize<ArgumentsJson>(json, options);
            Logger.Info(nameof(Version_json), "读取文件");
            DisArguments(_GameArg,_JvmArg, OSname);
            Logger.Info(nameof(Version_json), "读取Arg");
            DisAsserts();
            Logger.Info(nameof(Version_json), "读取asserts");
            DisDownload();
            Logger.Info(nameof(Version_json), "读取down");
            DisLogging();
            Logger.Info(nameof(Version_json), "读取logging");
            Dislibraries(OSname, OSis64);
            Logger.Info(nameof(Version_json), "读取lib");
        }
        /// <summary>
        /// 版本清单json构造函数
        /// </summary>
        /// <param name="Ajson">已经读取的版本清单json</param>
        /// <param name="_GameArg">Game参数主要为那些带规则参数和是否添加</param>
        /// <param name="_JvmArg">Jvm参数为自定义的jvm参数，值的真假代表是否为通用jvm参数（真代表新旧版均要添加，假代表仅有旧版添加）</param>
        /// <param name="OSname">当前系统名称windows，osx，linux</param>
        /// <param name="OSis64">当前系统位数（主要给windows使用）</param>
        public Version_json(ArgumentsJson Ajson, Dictionary<string, bool> _GameArg, Dictionary<string, bool> _JvmArg, string OSname, string OSis64)
        {
            argumentsJson = Ajson;
            Logger.Info(nameof(Version_json), "读取文件");
            DisArguments(_GameArg, _JvmArg, OSname);
            Logger.Info(nameof(Version_json), "读取Arg");
            DisAsserts();
            Logger.Info(nameof(Version_json), "读取asserts");
            DisDownload();
            Logger.Info(nameof(Version_json), "读取down");
            DisLogging();
            Logger.Info(nameof(Version_json), "读取logging");
            Dislibraries(OSname, OSis64);
            Logger.Info(nameof(Version_json), "读取lib");
        }
        public void Dislibraries(string OSname,string OSis64)
        {
            var libs = argumentsJson.libraries;
            var relib = new List<UnLibraries>();//lib
            var renat = new List<UnLibraries>();//nat
            for(int i=0;i<libs.Count; i++)
            {
                Logger.Info(nameof(Version_json), $"读取lib[{libs[i].name}]");
                var lib = libs[i];
                var Temp = new UnLibraries();
                //情况一，存在rules
                if(lib.rules != null)
                {
                    bool isnot = false;
                    for(int j = 0; j < lib.rules.Count; j++)//遍历rules（虽然实际上大概率就一项）
                    {
                        if (lib.rules[j].action == "allow")//允许
                        {
                            if(lib.rules[j].os != null && lib.rules[j].os?.name != OSname)//不是当前系统
                            {
                                isnot= true;
                            }
                        }
                        else if (lib.rules[j].action == "disallow")//拒绝
                        {
                            if(lib.rules[j].os?.name == OSname)//是当前系统
                            {
                                isnot= true;
                            }
                        }
                    }
                    if(isnot)
                    {
                        continue;//结束本次循环，规则判断出此文件非本系统文件
                    }
                }
                //情况二，存在natives
                if(lib.natives != null)//存在natives
                {
                    string natname = lib.natives[OSname];//获取名称
                    natname = natname.Replace("${arch}",OSis64);//替换系统位数
                    var natclass= lib.downloads?.classifiers?[natname];//取出nat数据
                    if (natclass == null) { }//错误
                    if (natclass != null)//不为空
                    {
                        Temp.url = natclass.url;
                        Temp.path =Path.Combine(PATH.LIBRARIES,natclass.path);//存放到库文件夹
                        Temp.hash = natclass.sha1;
                        Temp.size = natclass.size;
                        Temp.hashinfo = HashInfo.SHA1;//mc默认为sha1
                        Temp.SpecialData = ("不需要解压的文件目录",lib.extract?.exclude);//这里存储了不需要解压的文件目录
                        renat.Add(Temp);
                        //continue;//结束本次循环,不能结束，因为后面可能还有下载
                    }
                }
                //情况三
                var Temp2 = new UnLibraries();
                if(lib.downloads?.artifact != null)//存在下载
                {
                    Temp2.url = lib.downloads.artifact.url;
                    Temp2.path = Path.Combine(PATH.LIBRARIES,lib.downloads.artifact.path);//存放到库文件夹
                    Temp2.hash = lib.downloads.artifact.sha1;
                    Temp2.size = lib.downloads.artifact.size;
                    Temp2.hashinfo = HashInfo.SHA1;//mc的默认为sha1
                    relib.Add(Temp2);
                }
            }
            Libraries=relib;
            Natives=renat;
        }
        public void DisLogging()
        {
            McLogging = new McLogging()
            {
                ver = "client",//这个就只有客户端
                id = argumentsJson.logging.client.file.id,
                url = argumentsJson.logging.client.file.url,
                type = argumentsJson.logging.client.type,
                size = argumentsJson.logging.client.file.size,
                argument = argumentsJson.logging.client.argument,
                hash = argumentsJson.logging.client.file.sha1,
                hashinfo = HashInfo.SHA1//mc的默认为sha1
            };
        }
        public void DisDownload()
        {
            McDown = new McDown()
            {
                client = new McDown.Down()
                {
                    url=argumentsJson.downloads?.client?.url,
                    hash=argumentsJson.downloads?.client?.sha1,
                    size=argumentsJson.downloads?.client?.size??2*10*1024*1024,
                    hashinfo=HashInfo.SHA1//mc的默认为sha1
                },
                client_mappings = new McDown.Down()
                {
                    url=argumentsJson.downloads?.client_mappings?.url,
                    hash=argumentsJson.downloads?.client_mappings?.sha1,
                    size=argumentsJson.downloads?.client_mappings?.size??2*10*1024*1024,
                    hashinfo=HashInfo.SHA1//mc的默认为sha1
                },
                server = new McDown.Down()
                { 
                    url=argumentsJson.downloads?.server?.url,
                    hash=argumentsJson.downloads?.server?.sha1,
                    size=argumentsJson.downloads?.server?.size??2*10*1024*1024,
                    hashinfo=HashInfo.SHA1//mc的默认为sha1
                },
                server_mappings = new McDown.Down()
                {
                    url=argumentsJson.downloads?.server_mappings?.url,
                    hash=argumentsJson.downloads?.server_mappings?.sha1,
                    size=argumentsJson.downloads?.server_mappings?.size??2*10*1024*1024,
                    hashinfo=HashInfo.SHA1//mc的默认为sha1
                },
                windows_server = new McDown.Down()
                {
                    url=argumentsJson.downloads?.windows_server?.url,
                    hash=argumentsJson.downloads?.windows_server?.sha1,
                    size=argumentsJson.downloads?.windows_server?.size??2*10*1024*1024,
                    hashinfo=HashInfo.SHA1//mc的默认为sha1
                },
            };
        }
        public void DisAsserts()
        {
            AssetsIndex = new AssetsIndex()
            {
                id = argumentsJson.assetIndex.id,
                hash = argumentsJson.assetIndex.sha1,

                size = argumentsJson.assetIndex.size,
                totalSize = argumentsJson.assetIndex.totalSize,
                url = argumentsJson.assetIndex.url
            };
        }
        public void DisArguments(Dictionary<string,bool> _GameArg,Dictionary<string,bool> _JvmArg,string OSname)
        {
            List<string> Dgame = new List<string>();
            List<string> Djvm = new List<string>();
            for(int i=0;i<argumentsJson.arguments?.game.Count;i++)
            {
                var Lg = argumentsJson.arguments.game[i];
                if(Lg.Value1!=null)//为字符串
                {
                    Dgame.Add(Lg.Value1);
                }
                else//为自定义类
                {
                    for(int j=0;j<Lg.Value2.rules.Count;j++)//遍历规则，虽然实际这里只有一个规则
                    {
                        var _rule = Lg.Value2.rules[j].features.First();//获取规则值。这里直接取字典的第一个元素，在mojang没改动结构前应该是不会出问题的
                        if (Lg.Value2.rules[j].action=="allow")//允许
                        {
                            bool tb;
                            if(_GameArg.TryGetValue(_rule.Key,out tb))//如果外部参数存在这个键，则使用外部参数
                            {
                                if(tb)//为真，则添加到列表中
                                {
                                    if(Lg.Value2.value.Value1!=null)//为字符串
                                    {
                                        Dgame.Add(Lg.Value2.value.Value1);
                                    }
                                    else//为字符串列表
                                    {
                                        Dgame.AddRange(Lg.Value2.value.Value2);
                                    }
                                }
                            }
                            else//外部参数不存在这个键，则使用自带参数
                            {
                                if (_rule.Value)//为真，则添加到列表中
                                {
                                    if (Lg.Value2.value.Value1 != null)//为字符串
                                    {
                                        Dgame.Add(Lg.Value2.value.Value1);
                                    }
                                    else//为字符串列表
                                    {
                                        Dgame.AddRange(Lg.Value2.value.Value2);
                                    }
                                }
                            }
                        }
                        else if(Lg.Value2.rules[j].action=="disallow")//拒绝,虽然大概率这个不会用到
                        {
                            bool tb;
                            if (_GameArg.TryGetValue(_rule.Key, out tb))//如果外部参数存在这个键，则使用外部参数
                            {
                                if (!tb)//为假，则添加到列表中
                                {
                                    if (Lg.Value2.value.Value1 != null)//为字符串
                                    {
                                        Dgame.Add(Lg.Value2.value.Value1);
                                    }
                                    else//为字符串列表
                                    {
                                        Dgame.AddRange(Lg.Value2.value.Value2);
                                    }
                                }
                            }
                            else//外部参数不存在这个键，则使用自带参数
                            {
                                if (!_rule.Value)//为假，则添加到列表中
                                {
                                    if (Lg.Value2.value.Value1 != null)//为字符串
                                    {
                                        Dgame.Add(Lg.Value2.value.Value1);
                                    }
                                    else//为字符串列表
                                    {
                                        Dgame.AddRange(Lg.Value2.value.Value2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < argumentsJson.arguments?.jvm.Count; i++)
            {
                var Lg = argumentsJson.arguments.jvm[i];
                if (Lg.Value1 != null)//为字符串
                {
                    Djvm.Add(Lg.Value1);
                }
                else//为自定义类
                {
                    for (int j = 0; j < Lg.Value2.rules.Count; j++)//遍历规则，虽然实际这里只有一个规则
                    {
                        var _rule = Lg.Value2.rules[j].os.First();//获取规则值。这里直接取字典的第一个元素，在mojang没改动结构前应该是不会出问题的
                        if (Lg.Value2.rules[j].action == "allow")//允许
                        {
                            if(_rule.Value==OSname)//于本系统相同
                            {
                                if (Lg.Value2.value.Value1 != null)//为字符串
                                {
                                    Djvm.Add(Lg.Value2.value.Value1);
                                }
                                else//为字符串列表
                                {
                                    Djvm.AddRange(Lg.Value2.value.Value2);
                                }
                            }
                        }
                        else if (Lg.Value2.rules[j].action == "disallow")//拒绝,虽然大概率这个不会用到
                        {
                            if (_rule.Value != OSname)//于本系统不相同
                            {
                                if (Lg.Value2.value.Value1 != null)//为字符串
                                {
                                    Djvm.Add(Lg.Value2.value.Value1);
                                }
                                else//为字符串列表
                                {
                                    Djvm.AddRange(Lg.Value2.value.Value2);
                                }
                            }
                        }
                    }
                }
            }
            if(Dgame.Count==0)//代表为旧版
            {
                Dgame=argumentsJson.minecraftArguments?.Split(" ").ToList()??new();
                foreach (var item in _GameArg)
                {
                    if (item.Value)
                    {
                        Dgame.Add(item.Key);
                    }
                }
            }
            if(Djvm.Count==0)//代表为旧版
            {
                foreach (var item in _JvmArg)//全部添加
                {
                    Djvm.Add(item.Key);
                }
            }
            else
            {
                foreach (var item in _JvmArg)
                {
                    if (item.Value)//添加通用
                    {
                        Djvm.Add(item.Key);
                    }
                }
            }
            Arg_Game=Dgame;
            Arg_Jvm=Djvm;
        }
    }
    /// <summary>
    /// 请注意使用时请为 JsonSerializerOptions.Converters 添加转换器,不加必报错
    /// <para>new ArgumentsJson.Arguments.EitherConverter{string, ArgumentsJson.Arguments.GJRules}()</para>
    /// <para>new ArgumentsJson.Arguments.EitherConverter{string, List{string}}()</para>
    /// </summary>
    public class ArgumentsJson
    {
        public Arguments? arguments { get; set; }
        public AssetIndex? assetIndex { get; set; }
        public string? assets { get; set; }
        public long complianceLevel { get; set; }
        public Download? downloads { get; set; }
        public string? id { get; set; }
        public JavaVersion? javaVersion { get; set; }
        public List<Librarie>? libraries { get; set; }
        public Logging? logging { get; set; }
        public string? mainClass { get; set; }
        public string? minecraftArguments { get; set; }
        public long minimumLauncherVersion { get; set; }
        public string? releaseTime { get; set; }
        public string? time { get; set; }
        public string? type { get; set; }
        public string? clientVersion { get; set; }
        #region 子类定义
        public class Arguments
        {
            public List<Either<string, GJRules>> game { get; set; }
            public List<Either<string, GJRules>> jvm { get; set; }
            public class GJRules
            {
                public List<Rule> rules { get; set; }
                public Either<string, List<string>> value { get; set; }
                public class Rule
                {
                    public string action { get; set; }
                    public Dictionary<string, string> os { get; set; }
                    public Dictionary<string, bool> features { get; set; }
                }
            }
            // josn二选一基类
            public class Either<T1, T2>
            {
                public T1? Value1 { get; set; }
                public T2? Value2 { get; set; }
                public Either() { }
                public Either(T1 value) => Value1 = value;
                public Either(T2 value) => Value2 = value;
            }
            public class EitherConverter<T1, T2> : JsonConverter<Either<T1, T2>>
            {
                public EitherConverter() { }
                public override Either<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    var element = JsonDocument.ParseValue(ref reader).RootElement;
                    var json = element.GetRawText();

                    try
                    {
                        var value1 = JsonSerializer.Deserialize<T1>(json, options);
                        //Console.WriteLine($"{value1}解析为{typeof(T1)}");
                        return new Either<T1, T2>(value1);
                    }
                    catch (JsonException)
                    {
                        try
                        {
                            var value2 = JsonSerializer.Deserialize<T2>(json, options);
                            //Console.WriteLine($"{value2}解析为{typeof(T2)}");
                            return new Either<T1, T2>(value2);
                        }
                        catch (JsonException)
                        {
                            throw new JsonException($"无法将JSON值{json.ToString()}解析为{typeof(T1)}或{typeof(T2)}");
                        }
                    }
                }

                public override void Write(Utf8JsonWriter writer, Either<T1, T2> value, JsonSerializerOptions options)
                {
                    if (value.Value1 != null)
                    {
                        JsonSerializer.Serialize(writer, value.Value1, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, value.Value2, options);
                    }
                }
            }
        }
        public class AssetIndex
        {
            public string? id { get; set; }
            public string? sha1 { get; set; }
            public long size { get; set; }
            public long totalSize { get; set; }
            public string? url { get; set; }
        }
        public class Download
        {
            public Type? client { get; set; }
            public Type? client_mappings { get; set; }
            public Type? server { get; set; }
            public Type? server_mappings { get; set; }
            public Type? windows_server { get; set; }
            public class Type
            {
                public string? sha1 { get; set; }
                public long size { get; set; }
                public string? url { get; set; }
            }
        }
        public class JavaVersion
        {
            public string? component { get; set; }
            public int majorVersion { get; set; }
        }
        public class Librarie
        {
            public Downloads? downloads { get; set; }
            public Extract? extract { get; set; }
            public string? name { get; set; }
            public List<Rule>? rules { get; set; }
            public Dictionary<string,string>? natives { get; set; }

            //public string? url { get; set; }
            //public List<string>? checksums { get; set; }
            //public bool? serverreq { get; set; }
            //public bool? clientreq { get; set; }
            public class Extract
            {
                public List<string>? exclude { get; set; }
            }
            public class Downloads
            {
                public Artifact? artifact { get; set; }
                public Dictionary<string,Artifact>? classifiers { get; set; }
                public class Artifact
                {
                    public string? path { get; set; }
                    public string? sha1 { get; set; }
                    public long size { get; set; }
                    public string? url { get; set; }
                }

                //public class Classifiers
                //{
                //    [JsonPropertyName("natives-linux")]
                //    public Artifact? linux { get; set; }
                //    [JsonPropertyName("natives-osx")]
                //    public Artifact? osx { get; set; }
                //    [JsonPropertyName("natives-windows")]
                //    public Artifact? windows { get; set; }
                //    [JsonPropertyName("natives-windows-32")]
                //    public Artifact? windows32 { get; set; }
                //    [JsonPropertyName("natives-windows-64")]
                //    public Artifact? windows64 { get; set; }
                //    [JsonPropertyName("javadoc")]
                //    public Artifact? javadoc { get; set; }
                //    [JsonPropertyName("sources")]
                //    public Artifact? sources { get; set; }
                //}
            }
            public class Rule
            {
                public string? action { get; set; }
                public OS? os { get; set; }
                public class OS
                {
                    public string? name { get; set; }
                }
            }
        }
        public class Logging
        {
            public Client? client { get; set; }
            public class Client
            {
                public string? argument { get; set; }
                public File? file { get; set; }
                public string? type { get; set; }
                public class File
                {
                    public string? id { get; set; }
                    public string? sha1 { get; set; }
                    public long size { get; set; }
                    public string? url { get; set; }
                }
            }
        }
        #endregion
    }
}
