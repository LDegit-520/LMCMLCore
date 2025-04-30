using LMCMLCore.CORE.API;
using LMCMLCore.CORE.data;
using LMCMLCore.CORE.datainfo;
using LMCMLCore.CORE.Json.Mc;
using LMCMLCore.CORE.LOGGER;
using LMCMLCore.CORE.OS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Start
{
    public class StartJsonInfo
    {
        /// <summary>
        /// 游戏版本名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 游戏jar路径
        /// </summary>
        public string StartJarPath { get; set;}
        /// <summary>
        /// 版本清单json路径
        /// </summary>
        public string GameJsonPath { get; set; }
        /// <summary>
        /// 资源json路径
        /// </summary>
        public string AssetsJsonPath { get; set; }
        /// <summary>
        /// 日志配置路径
        /// </summary>
        public string LoggingPath { get; set; }
        /// <summary>
        /// mod加载器json路径
        /// </summary>
        public string LoaderJsonPath { get; set; }
        /// <summary>
        /// java路径，如果本项存在，则强制使用本项指定的java运行
        /// </summary>
        public string JavaPath { get; set; } = "";
        /// <summary>
        /// 有改动的游戏启动参数，对于在DATA类里定义的且没有改动不用添加进入这个
        /// </summary>
        public Dictionary<string, bool> ArgumentsGame { get; set; } = new();
        /// <summary>
        /// 有改动的jvm参数，对于在DATA类里定义的且没有改动的不用添加进入这个
        /// </summary>
        public Dictionary<string, bool> ArgumentsJvm { get; set; } = new();
    }
    /// <summary>
    /// 启动游戏
    /// </summary>
    public class StartGame
    {
        private string Appdatapath;
        private string Workpath;
        private string Javaexe;
        private string Arguments;
        /// <summary>
        /// 构造启动参数
        /// <para name="startjsonpath">启动json路径，不能为空</para>
        /// <para name="user">启动用户，不能为空</para>
        /// <para name="egame">额外开启的game参数，可以为new()</para>
        /// <para name="ejvm">额外开启的jvm参数，可以为new()</para>
        /// <para name="egamevalue">额外开启的game参数值，可以为new()但请注意要和额外开启的game进行一一对应</para>
        /// </summary>
        /// <param name="startjsonpath">启动json路径，不能为空</param>
        /// <param name="user">启动用户，不能为空</param>
        /// <param name="egame">额外开启的game参数，可以为new()</param>
        /// <param name="ejvm">额外开启的jvm参数，可以为new()</param>
        /// <param name="egamevalue">额外开启的game参数值，可以为new()但请注意要和额外开启的game进行一一对应</param>
        public StartGame(string startjsonpath,UserInfo user, Dictionary<string, bool> egame, Dictionary<string, bool> ejvm,Dictionary<string,string> egamevalue)
        {
            Appdatapath = Path.GetDirectoryName(startjsonpath);//appdata路径为版本文件夹
            Workpath = Path.GetDirectoryName(startjsonpath);//工作路径为版本文件夹
            StartJsonInfo startJson =JsonSerializer.Deserialize<StartJsonInfo>(File.ReadAllText(startjsonpath));//启动信息类
            //game合并，优先使用本构造函数的参数，在使用存储的参数，最后使用DATA存储的参数
            Dictionary<string,bool> VJgame = new(DATA.ARGUMENT_GAME);
            foreach (var item in startJson.ArgumentsGame)
            {
                if(VJgame.ContainsKey(item.Key))
                {
                    VJgame[item.Key] = item.Value;
                }
                else
                {
                    VJgame.Add(item.Key, item.Value);
                }
            }
            foreach (var item in egame)
            {
                if (VJgame.ContainsKey(item.Key))
                {
                    VJgame[item.Key] = item.Value;
                }
                else
                {
                    VJgame.Add(item.Key, item.Value);
                }
            }
            //jvm合并，优先使用本构造函数的参数，在使用存储的参数，最后使用DATA存储的参数
            Dictionary<string,bool> VJjvm=new(DATA.ARGUMENT_JVM);
            foreach (var item in startJson.ArgumentsJvm)
            {
                if (VJjvm.ContainsKey(item.Key))
                {
                    VJjvm[item.Key] = item.Value;
                }
                else
                {
                    VJjvm.Add(item.Key, item.Value);
                }
            }
            foreach (var item in ejvm)
            {
                if (VJjvm.ContainsKey(item.Key))
                {
                    VJjvm[item.Key] = item.Value;
                }
                else
                {
                    VJjvm.Add(item.Key, item.Value);
                }
            }
            Version_json version_Json = new Version_json(startJson.GameJsonPath,VJgame,VJjvm,DATA.OS.id,DATA.OS.is64bit);
            Logger.Info(nameof(Start), $"开始处理版本清单{startJson.Name}.json");
            //javaexe路径
            if(!string.IsNullOrEmpty(startJson.JavaPath))//如果指定java
            {
                Javaexe = startJson.JavaPath;
            }
            else//没有指定java
            {
                var gamejavaver = DisJavaexe(version_Json.GetJavaVersion());
                if (string.IsNullOrEmpty(gamejavaver))
                {
                    Logger.Warning(nameof(Start), $"没有找到有效的java.exe,启动本版本需要Java{version_Json.GetJavaVersion()},如果不存在请下载，如果存在请手动指定java");
                    throw new Exception($"没有找到有效的java.exe,启动本版本需要Java{version_Json.GetJavaVersion()},如果不存在请下载，如果存在请手动指定java");
                }
                if (gamejavaver.EndsWith("bin\\java.exe") || gamejavaver.EndsWith("bin/java")) { Javaexe = gamejavaver; }//已经定位到程序
                else if (gamejavaver.EndsWith("bin")) { Javaexe = Path.Combine(gamejavaver, "java"); }//在bin目录
                else { Javaexe = Path.Combine(gamejavaver, "bin", "java"); }//在bin目录的父目录，如果这还不在那只能说明最开始获取错了
                if (DATA.OS.id == "windows" && !Javaexe.EndsWith("java.exe")) { Javaexe += ".exe"; }//windows需要加exe
            }
            Logger.Info(nameof(Start), $"启动java为{Javaexe}");
            //处理并生成参数
            StringBuilder Libs = new("\"");
            string fgf = ":";
            if (DATA.OS.id == "windows") { fgf = ";"; }
            for (int i = 0; i < version_Json.Libraries.Count; i++)
            {
                Libs.Append(version_Json.Libraries[i].path);
                Libs.Append(fgf);
            }
            Libs.Append(startJson.StartJarPath + "\"");
            //补全参数
            Dictionary<string, string> gamejvmArg = new()
            {
                ["${auth_player_name}"] = user.name,//玩家名称，储存在玩家数据中
                ["${version_name}"] = startJson.Name,//版本名称
                ["${game_directory}"] = Path.GetDirectoryName(startjsonpath),//游戏目录
                ["${assets_index_name}"] = version_Json.AssetsIndex.id,//资源文件索引
                ["${auth_uuid}"] = user.id,//玩家id，储存在玩家数据中
                ["${auth_access_token}"] = user.token,//访问令牌，正版由mojang服务器返回，离线玩家自动生成,储存在玩家数据中
                ["${natives_directory}"] = Path.Combine(Path.GetDirectoryName(startjsonpath), PATH._NATIVES),//natives目录
                ["${classpath}"] =Libs.ToString(),//类路径
            };
            //合并字典
            foreach (var item in egamevalue)//遍历添加不存在的参数
            {
                if (!gamejvmArg.ContainsKey(item.Key))
                {
                    gamejvmArg.Add(item.Key, item.Value);
                }
            }
            foreach (var item in DATA.ARGUMENT_GAME_JAM_VALUE)//遍历添加不存在的参数
            {
                if(!gamejvmArg.ContainsKey(item.Key))
                {
                    gamejvmArg.Add(item.Key, item.Value);
                }
            }
            foreach (var item in DATA.ARGUMENT_GAME_VALUE)//遍历添加不存在的参数
            {
                if (!gamejvmArg.ContainsKey(item.Key))
                {
                    gamejvmArg.Add(item.Key, item.Value);
                }
            }
            //合并jvm添加主类合并game
            string JMG=string.Join(" ",version_Json.Arg_Jvm)+$" {version_Json.McLogging.argument.Replace("${path}",$"{Path.Combine(PATH.GLOGGING,version_Json.McLogging.id)}")} {version_Json.GetMainclass()} "+string.Join(" ",version_Json.Arg_Game);
            //game替换
            foreach (var item in DATA.ARGUMENT_GAME_DZ)
            {
                JMG = JMG.Replace(item.Key, item.Value);
            }
            //替换参数
            foreach (var item in gamejvmArg)
            {
                JMG = JMG.Replace(item.Key, item.Value);
            }
           Arguments = JMG;
        Logger.Info(nameof(Start), $"启动参数生成完毕 {Arguments}");
        }
        private string DisJavaexe(int ver)
        {
            string javaexe="";
            string minjavaexe="";
            int minjavaver=int.MaxValue;
            for(int i=0;i<DATA.JAVAEXE_LIST.Count;i++)
            {
                int tver = 0;
                int.TryParse(DATA.JAVAEXE_LIST[i].Id,out tver);
                if(ver==tver)
                {
                    if(javaexe=="")
                    {
                        javaexe= DATA.JAVAEXE_LIST[i].Path;
                    }
                }
                else if(ver<tver)
                {
                    if(tver<minjavaver)
                    {
                        minjavaver=tver;
                        minjavaexe= DATA.JAVAEXE_LIST[i].Path;
                    }
                }
            }
            if(javaexe!="")
            {
                return javaexe;
            }else if(minjavaexe!="")
            {
                return minjavaexe;
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 启动游戏
        /// </summary>
        public ProcessRunner Run()
        {
            try
            {
                Logger.Info(nameof(Start), $"开始启动游戏");
                //替换内存参数
                var memo = SystemMemory.GetMemoryInfo();//获取系统总内存和可用内存
                if (memo.availableGB < 1)//可用内存小于1GB则报错停止启动
                {
                    var EventT = EventThread.Instance;
                    var Eventarg = new EventThread.NotEventArgs(new Dictionary<string, string>()             
                    {
                        { "title","内存不足" },
                        { "text","可用内存小于1GB，请检查您的系统内存是否正常,启动已终止" },
                    });
                    EventT.Add(Eventarg);
                    throw new Exception("启动内存小于1GB，已终止启动");
                }
                Arguments = Arguments.Replace("{minmemory}", 1024.ToString()).Replace("{maxmemory}", ((int)(memo.totalGB/2*1024)).ToString());
                Logger.Info(nameof(Start), $"启动参数 {Arguments}|| {Javaexe}|| {Appdatapath}|| {Workpath}");
                ProcessRunner process = new ProcessRunner(Javaexe, Arguments, Appdatapath, Workpath);
                process.OutputReceived += (s,e)=> { Logger.Info(nameof(ProcessRunner),$"[JVM输出] {e}"); };
                process.ErrorReceived += (s,e)=> { Logger.Info(nameof(ProcessRunner),$"[JVM错误] {e}"); };
                process.Start();
                Logger.Info(nameof(Start), $"启动游戏成功");
                return process;
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(ProcessRunner), $"启动 Java 进程时发生错误：{ex.Message}");
                return null;
            }
        }
    }
    /*
     * 现在要启动游戏
     * 首先构造启动参数
     * 然后启动游戏
     * 构造参数需要知道
     * 类型一下面为安装时就已经确定的---写入一个文件中，然后读取
     * 游戏版本所在的文件夹=> %appdata%\.minecraft\versions\版本名\】
     * 游戏资源所在的文件夹=> %appdata%\.minecraft\assets, %appdata%\.minecraft\lib】
     * 游戏主jar所在的文件夹=> %appdata%\.minecraft\verjson\mc\game.jar】
     * java虚拟机所在的文件夹=> %appdata%\.minecraft\jre\bin\java.exe
     * 启动参数=> %appdata%\.minecraft\versions\版本名\version.json】
     * 资源列表=> %appdata%\.minecraft\assets\indexes\index.json】
     * mod加载器参数=> %appdata%\.minecraft\modloader\modloader.json
     * game和jvm
     * 类型二下面为启动时动态获取的
     * 额外game和jvm --- 传入
     * 内存数据=> 临时获取 --- 内部获取
     * 类型三玩家数据---直接传入
     * 游玩的玩家信息=> %appdata%\user.json
     * 
     *
     */
}
