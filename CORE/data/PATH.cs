using LMCMLCore.CORE.I18N;
using LMCMLCore.CORE.LOGGER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.data
{
    /*
     * 路径类 1.0
     * 这个类原本在数据类里，后来发现太多了
     * 然后就有了这个类
     * 这里存放了所有重要的路径
     * 且大多数仅支持get操作
     * 
     * 在程序运行中多次使用的应该尽量写进这里进行调用
     */
    /// <summary>
    /// 存储程序中所有的路径 || 文件夹均不带_ || 文件均带有_[文件描述]
    /// </summary>
    public class PATH
    {
        /// <summary>
        /// natives文件夹（路径常用名称）
        /// </summary>
        public readonly static string _NATIVES = "natives";
        public readonly static string _START_JSON = "start.json";
        #region 文件夹
        /// <summary>
        /// 获取exe所在目录（不带文件名，末尾自动带斜杠）//会检查windows平台的长度限制
        /// </summary>
        public static string EXE { get; }= AppDomain.CurrentDomain.BaseDirectory;
        //    = ((Func<string>)(() =>
        //{
        //    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        //    if (DATA.OS.Platform == PlatformType.windows && baseDir.Length > 130)//如果是win平台且exe路径大于130
        //    {
        //        Logger.Warning("EXE", $"EXE:{baseDir}--{string.Format(I18.I18LoggerString.PATH_EXEPATHWARRING, $"{baseDir}")}");
        //    }
        //    return baseDir;
        //}))();//AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// .mc目录
        /// </summary>
        public static string MC { get; } = Path.Combine(EXE,".minecraft");
        /// <summary>
        /// 启动器目录
        /// </summary>
        public static string MCL { get; } = Path.Combine(EXE, "LMCML");
        /// <summary>
        /// 版本实例文件夹
        /// </summary>
        public static string VERSIONS { get; } = Path.Combine(MC, "versions");
        /// <summary>
        /// 资源文件夹
        /// </summary>
        public static string ASSETS { get; } = Path.Combine(MC,"assets");
        /// <summary>
        /// 资源文件夹内部indexs文件夹(也就是索引文件文件夹)
        /// </summary>
        public static string INDEXES { get; } = Path.Combine(ASSETS, "indexes");
        /// <summary>
        /// 资源文件夹内部objects文件夹
        /// </summary>
        public static string OBJECTS { get; } = Path.Combine(ASSETS,"objects");
        /// <summary>
        /// 支持库文件夹
        /// </summary>
        public static string LIBRARIES { get; } = Path.Combine(MC,"libraries");
        /// <summary>
        /// 复用资源文件夹
        /// </summary>
        public static string GAMEASS { get; } = Path.Combine(MC, "gameass");
        /// <summary>
        /// 复用资源文件夹内部MC资源文件夹
        /// </summary>
        public static string GJARJSON { get; } = Path.Combine(GAMEASS, "jarjson");
        /// <summary>
        /// 复用资源文件夹内部MC资源文件夹的mc日志配置文件夹
        /// </summary>
        public static string GLOGGING { get; } = Path.Combine(GJARJSON, "logging");
        /// <summary>
        /// 复用资源文件夹内部forge资源文件夹
        /// </summary>
        public static string GFORGE { get; } = Path.Combine(GAMEASS, "forge");
        /// <summary>
        /// 复用资源文件夹内部neoforge资源文件夹
        /// </summary>
        public static string GNEOFORGE { get; } = Path.Combine(GAMEASS, "neoforge");
        /// <summary>
        /// 复用资源文件夹内部fabric资源文件夹
        /// </summary>
        public static string GFRABRIC { get; } = Path.Combine(GAMEASS, "fabric");
        /// <summary>
        /// 启动器信息文件夹
        /// </summary>
        public static string LMCML { get; } = Path.Combine(MCL,"LMCML");
        /// <summary>
        /// I18N（国际化）文件夹
        /// </summary>
        public static string LMI18N { get; } = Path.Combine(MCL, "I18N");
        /// <summary>
        /// OS信息文件夹
        /// </summary>
        public static string OS { get; } = Path.Combine(MCL, "OS");
        /// <summary>
        /// 日志文件夹
        /// </summary>
        public static string LOG { get; } = Path.Combine(MCL, "log");
        /// <summary>
        /// 临时文件文件夹
        /// </summary>
        public static string TEMP { get; } = Path.Combine(MCL, "temp");
        /// <summary>
        /// 临时下载文件文件夹
        /// </summary>
        public static string TEMPDOEN { get; } = Path.Combine(TEMP,"Download");
        /// <summary>
        /// 临时forge文件文件夹
        /// </summary>
        public static string TEMPFORGE { get; } = Path.Combine(TEMP, "forge");
        /// <summary>
        /// 临时neoforge文件文件夹
        /// </summary>
        public static string TEMPNEOFORGE { get; } = Path.Combine(TEMP, "neoforge");
        /// <summary>
        /// 临时fabric文件文件夹
        /// </summary>
        public static string TEMPFABRIC { get; } = Path.Combine(TEMP, "fabric");
        /// <summary>
        /// 用户文件夹
        /// <para>请注意这里为明文存储，这里仅存储离线玩家数据（也就是随机产生的数据）, 不存储正版账号数据</para>
        /// </summary>
        public static string USER { get; } = Path.Combine(MCL,"user");

        #endregion
        #region 文件地址
        ///// <summary>
        ///// OS数据的josn路径
        ///// </summary>
        //public static string OS_JSON { get; } = Path.Combine(OS,"OS.json");
        /// <summary>
        /// i18n配置数据文件路径
        /// </summary>
        public static string I18N_JSON { get; } = Path.Combine(LMI18N, "i18n.json");
        public static string I18N_ZH_CN_JSON { get; } = Path.Combine(LMI18N, "zh_cn.json");
        /// <summary>
        /// Java数据的json路径
        /// </summary>
        public static string JAVA_JSON { get; } = Path.Combine(OS, "java.json");
        /// <summary>
        /// 启动器数据的json路径
        /// </summary>
        public static string LMCML_JSON { get; } = Path.Combine(LMCML, "LMCML.json");
        /// <summary>
        /// 玩家数据的json路径
        /// </summary>
        public static string USER_JSON { get; } = Path.Combine(USER, "user.json");
        #endregion
        #region 可变数据
        /// <summary>
        /// 当前Java路径//初次默认为最高java
        /// </summary>
        public static string JAVA_EXE { get; set; }
        #endregion
        #region 静态函数区
        /// <summary>
        /// 静态构造函数，用于创建不存在的目录
        /// </summary>
        public static void CPATH()
        {
            try
            {
                var properties = typeof(PATH).GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (var prop in properties)
                {
                    if (prop.Name == "EXE")
                    {
                        if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && EXE.Length > 130)//如果是win平台且exe路径大于130
                        {
                            Logger.Warning("EXE", $"EXE:{EXE}--{string.Format(I18N.I18NLoggerString.PATH_EXEPATHWARRING, $"{EXE}")}");
                        }
                        continue;
                    }
                    //跳过非目录
                    if (prop.Name.Contains('_'))
                    {
                        continue;
                    }
                    var path = prop.GetValue(null) as string;
                    if (path == null)
                    {
                        continue;
                    }
                    // 跳过空路径或非绝对路径
                    if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path))
                    {
                        continue;
                    }
                    // 标准化路径
                    path = Path.GetFullPath(path);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Logger.Info(nameof(PATH), $"{I18NLoggerString.PATH_wenjianchuangjian}{path}");
                    }
                }
                Logger.Info(nameof(PATH), I18NLoggerString.PATH_wenjianwancheng);
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(PATH), $"{I18NLoggerString.PATH_wenjiancuowu}{ex}");
            }
        }
        #endregion

    }
}
