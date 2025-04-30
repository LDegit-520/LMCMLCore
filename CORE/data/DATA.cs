using LMCMLCore.CORE.datainfo;
using LMCMLCore.CORE.LOGGER;
using LMCMLCore.CORE.OS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.data
{
    /*
     * 数据类 4.0版本
     * 这个类主要用来存储各种数据杂七杂八的都有
     * 这个类还有一个静态方法需要在程序开始时进行调用
     * 因为部分数据存在外部json里
     */
    public class DATA
    {
        //新
        public static string OSNAME = "windows";
        //



        #region 无需存取的项
        /// <summary>
        /// Json序列化选项
        /// </summary>
        public static JsonSerializerOptions JSON_OPTIONS { get; } = new JsonSerializerOptions
        {
            Encoder= System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,//忽略null
            WriteIndented = true // 格式化输出
        };
        public static string PATHCUT { get; set; } = ";";
        /// <summary>
        /// 默认。控制启动game参数是否添加
        /// </summary>
        public static Dictionary<string, bool> ARGUMENT_GAME { get; set; } = new()
        {
            ["is_demo_user"] = false,//是否演示模式
            ["is_fullscreen"]= false,//是否全屏
            ["has_custom_resolution"] = true,//是否自定义分辨率
            ["has_quick_plays_support"] = false,//是否快速进入游戏
            ["is_quick_play_singleplayer"] = false,//是否快速进入单机游戏
            ["is_quick_play_multiplayer"] = false,//是否快速进入联机游戏
            ["is_quick_play_realms"] = false,//是否快速进入realms
            ["is_server"] = false//是否快速进入服务器，老板本
        };
        public static Dictionary<string, string> ARGUMENT_GAME_DZ { get; set; } = new() 
        {
            ["is_demo_user"] = "--demo",
            ["is_fullscreen"]= "--fullscreen",
            ["has_custom_resolution"] = "--width ${resolution_width} --height ${resolution_height}",
            ["has_quick_plays_support"]= "${quickPlayPath}",
            ["is_quick_play_singleplayer"] = "${quickPlaySingleplayer}",
            ["is_quick_play_multiplayer"] = "${quickPlayMultiplayer}",
            ["is_quick_play_realms"] = "${quickPlayRealms}",
            ["is_server"] = "--server ${server} --port ${port}",
        };
        /// <summary>
        /// 启动game参数对应的值
        /// </summary>
        public static Dictionary<string, string> ARGUMENT_GAME_VALUE { get; set; } = new()
        {
            ["${resolution_width}"] = "854",
            ["${resolution_height}"] = "480",
            ["${quickPlayPath}"] = string.Empty,
            ["${quickPlaySingleplayer}"] = string.Empty,
            ["${quickPlayMultiplayer}"] = string.Empty,
            ["${quickPlayRealms}"] = string.Empty,
            ["${server}"] = string.Empty,
            ["${port}"] = string.Empty,
        };
        /// <summary>
        /// 默认，控制jvm参数是否为新版的启动参数，（true代表新旧版均要添加，false代表仅有旧版添加）
        /// </summary>
        public static Dictionary<string, bool> ARGUMENT_JVM { get; set; } = new()
        {
            ["-Xmn{minmemory}m"]=true,
            ["-Xmx{maxmemory}m"]=true,
            ["-XX:+UseG1GC"] = true,
            ["-XX:-UseAdaptiveSizePolicy"] = true,
            ["-XX:-OmitStackTraceInFastThrow"] =true,
            ["-Djdk.lang.Process.allowAmbiguousCommands=true"] =true,
            ["-Dfml.ignoreInvalidMinecraftCertificates=True"] =true,
            ["-Dfml.ignorePatchDiscrepancies=True"] =true,
            ["-Dlog4j2.formatMsgNoLookups=true"]=true,
            ["-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump"]=false,
            ["-Djava.library.path=${natives_directory}"] =false,
            ["-Djna.tmpdir=${natives_directory}"]=false,
            ["-Dorg.lwjgl.system.SharedLibraryExtractPath=${natives_directory}"] =false,
            ["-Dio.netty.native.workdir=${natives_directory}"] =false,
            ["-Dminecraft.launcher.brand=${launcher_name}"] =false,
            ["-Dminecraft.launcher.version=${launcher_version}"] =false,
            ["-cp ${classpath}"] =false,
        };
        /// <summary>
        /// 启动参数对应的值,这里存储常态化的值
        /// </summary>
        public static Dictionary<string, string> ARGUMENT_GAME_JAM_VALUE { get; set; } = new()
        {
            //game参数
            ["${auth_player_name}"]="",//玩家名称，储存在玩家数据中
            ["${version_name}"]="",//版本名称
            ["${game_directory}"]="",//游戏目录
            ["${assets_root}"]=PATH.ASSETS,//资源文件主目录，默认为本启动器自带目录
            ["${assets_index_name}"]="",//资源文件索引
            ["${auth_uuid}"]="",//玩家id，储存在玩家数据中
            ["${auth_access_token}"]="",//访问令牌，正版由mojang服务器返回，离线玩家自动生成,储存在玩家数据中
            //极其不建议修改的game参数
            ["${clientid}"] = "${clientid}",//遥测客户端id，此值建议保留
            ["${auth_xuid}"]= "${auth_xuid}",//遥测玩家id，此值建议保留
            ["${user_type}"] ="msa",//玩家类型，固定值，不应该再次对其进行修改。此值代表微软登录
            ["${version_type}"] =CORE_DATA.NAME,//调试显示版本类型
            ["${user_properties}"] =" {}",//这个不能动，改了就报错
            //jvm参数
            ["${natives_directory}"] = "",//natives目录
            ["${classpath}"] ="",//库文件
            //极其不建议修改的jvm参数
            ["${launcher_name}"] = CORE_DATA.NAME,//启动器名称
            ["${launcher_version}"] = CORE_DATA.VERSION,//启动器版本
        };
        #endregion
        #region 需要存取的项，也就是需要存到外部文件中
        /// <summary>
        /// 操作系统信息
        /// </summary>
        public static OSInfo OS { get; set; }
        public static List<JavaVMInfo> JAVAEXE_LIST { get; set; }
        /// <summary>
        /// 离线玩家信息
        /// </summary>
        public static List<UserInfo>? USERS_DATA { get; set; }
        /// <summary>
        /// 启动器数据
        /// </summary>
        public static Launcherinfo LAUNCHERINFO { get; set; }
        #endregion
        /// <summary>
        /// 读取json数据，用来初始化数据
        /// </summary>
        public static void DATAJSON()
        {
            if (File.Exists(PATH.USER_JSON))//玩家数据是否存在
            {
                Logger.Info(nameof(DATA), "读取离线玩家数据");
                USERS_DATA = JsonSerializer.Deserialize<List<datainfo.UserInfo>>(File.ReadAllText(PATH.USER_JSON), JSON_OPTIONS);
            }
            else
            {
                Logger.Info(nameof(DATA), "数据不存在正在创建离线玩家数据");
                USERS_DATA = new List<datainfo.UserInfo>() { new datainfo.UserInfo("user")};
                File.WriteAllText(PATH.USER_JSON, JsonSerializer.Serialize(USERS_DATA, JSON_OPTIONS));
            }
        }

        public static void CDATA()
        {
             OS = OSInfo.COSInfo();//OS信息
            if (File.Exists(PATH.JAVA_JSON)) { JAVAEXE_LIST = JsonSerializer.Deserialize<List<JavaVMInfo>>(File.ReadAllText(PATH.JAVA_JSON), JSON_OPTIONS);}
            else { JAVAEXE_LIST = JavaFinder.FindJava(true).ToList();/*这里使用常规搜素*/ File.WriteAllText(PATH.JAVA_JSON,JsonSerializer.Serialize(JAVAEXE_LIST,JSON_OPTIONS)); }//java信息

        }
    } 
}
