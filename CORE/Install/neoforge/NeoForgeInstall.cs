using LMCMLCore.CORE.data;
using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static LMCMLCore.CORE.Install.fabric.FabricVerJson;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMCMLCore.CORE.Install.neoforge
{
    /// <summary>
    /// neoforge安装器
    /// </summary>
    public class NeoForgeInstall
    {
        /// <summary>
        /// install处理
        /// </summary>
        private InstallProFileManage IPFManage;
        /// <summary>
        /// 安装器名称
        /// </summary>
        private string InstallName;
        /// <summary>
        /// 客户端路径
        /// </summary>
        private string JarPath;
        /// <summary>
        /// 版本文件夹路径
        /// </summary>
        private string VersionPath;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="installProFileManage">installProFile初步处理后的数据</param>
        /// <param name="installName">安装器名称</param>
        /// <param name="jarPath">客户端路径</param>
        /// <param name="versionPath">版本文件夹路径</param>
        public NeoForgeInstall(InstallProFileManage installProFileManage, string installName, string jarPath, string versionPath)
        {
            IPFManage = installProFileManage;
            InstallName = installName;
            JarPath = jarPath;
            VersionPath = versionPath;
        }
        /// <summary>
        /// 启动安装
        /// </summary>
        /// <returns>是否安装成功</returns>
        public bool Run()
        {
            try
            {
                Setdata();
                SetProcessors();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {

            }
        }
        /// <summary>
        /// 再次处理data
        /// </summary>
        void Setdata()
        {
            IPFManage.data.Add("SIDE", IPFManage.IsClient ? "client" : "server");//Side
            IPFManage.data.Add("MINECRAFT_JAR", JarPath);//添加客户端jar路径
            IPFManage.data.Add("MINECRAFT_VERSION", IPFManage.GetMincraft());//添加mc版本
            IPFManage.data.Add("ROOT", VersionPath);//添加版本文件夹
            IPFManage.data.Add("INSTALLER", Path.Combine(PATH.GNEOFORGE,InstallName));//添加安装器路径
            IPFManage.data.Add("LIBRARY_DIR", PATH.LIBRARIES);//添加lib文件夹
        }
        void SetProcessors()
        {
            foreach (var item in IPFManage.processors)
            {
                var AllNames = item.jar.Split(":");//分割名称
                string AllName = $"{AllNames[0]}:{AllNames[1]}";//全名
                if (AllNames[1] == "installertools")
                {
                    var taskvalue = item.args[item.args.IndexOf("--task") + 1];//获取task的值（也就是在args中的下一个）
                    AllName += $" -> {taskvalue}";
                    if (taskvalue == "DOWNLOAD_MOJMAPS")//理论上是这样的
                    {
                        continue;
                    }
                }
                Dictionary<string, string> outputs = new();
                if (item.outputs != null && item.outputs.Count > 0)
                {
                    bool miss = false;
                    foreach (var oute in item.outputs)
                    {
                        var key = oute.Key;
                        if (key.StartsWith("[") && key.EndsWith("]"))
                        {
                            key = InstallProFileManage.Getzykh(key);
                        }
                        else
                        {
                            key = SetValueData(key);
                        }
                        var value = oute.Value;
                        if (value != null)
                        {
                            value = SetValueData(value);
                        }
                        if (key == null || value == null)
                        {
                            return;
                        }
                        outputs.Add(key, value);
                        if (!File.Exists(key))
                        {
                            miss = true;
                            continue;
                        }
                        if (Hash.FileHash(key) == value)
                        {
                            continue;
                        }
                        miss = true;
                        File.Delete(key);
                    }
                    if (!miss)
                    {
                        continue;
                    }
                }
                string jar = Path.Combine(PATH.LIBRARIES, Universal.GetUrl_Path_name(item.jar));
                string MainClass;
                if (!GetJarMain(jar, out MainClass))//没有main
                {
                    return;
                }
                List<string> classpath = new();
                classpath.Add(jar);
                foreach (var classpathi in item.classpath)
                {
                    classpath.Add(InstallProFileManage.Getzykh($"[{classpathi}]"));
                }
                List<string> args = new();
                foreach (var argsi in item.args)
                {
                    if (argsi.StartsWith("[") && argsi.EndsWith("]"))
                    {
                        args.Add(InstallProFileManage.Getzykh(argsi));
                    }
                    else
                    {
                        args.Add(SetValueData(argsi));
                    }
                }
                //这里需要启动JVM用于运行
                //构建启动参数
                string argument = "-cp \"";
                foreach (var classi in classpath)
                {
                    argument += $"{classi}{DATA.PATHCUT}";
                }
                argument += $"\" {MainClass}";
                foreach (var argsi in args)
                {
                    argument += $" {argsi}";
                }
                //
                var startInfo = new ProcessStartInfo
                {
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    FileName = PATH.JAVA_EXE,
                    Arguments = argument, // 注意：路径含空格时需用引号包裹
                    UseShellExecute = false,          // 必须为 false 才能重定向流
                    RedirectStandardOutput = true,    // 捕获标准输出
                    RedirectStandardError = true,     // 捕获错误输出
                    CreateNoWindow = true,            // 不显示控制台窗口
                };
                // 启动并等待进程
                using Process process = new Process { StartInfo = startInfo };
                process.Start();
                process.WaitForExit();
                foreach (var outputi in outputs)
                {
                    if (!File.Exists(outputi.Key))
                    {
                        continue;
                    }
                    if (Hash.FileHash(outputi.Key) == outputi.Value)
                    {
                        continue;
                    }
                    return;
                }
            }
        }
        private static string MANIFESTMF = "META-INF/MANIFEST.MF";//jar包的MF文件位置
        bool GetJarMain(string jarpath, out string mainClass)
        {
            mainClass = null;
            try
            {
                // 打开ZIP文件
                using (ZipArchive archive = ZipFile.OpenRead(jarpath))
                {
                    // 不区分大小写查找清单文件
                    var targetEntry = archive.Entries.FirstOrDefault(entry =>
                        string.Equals(entry.FullName, MANIFESTMF, StringComparison.OrdinalIgnoreCase));

                    if (targetEntry == null)
                        return false;

                    using (Stream entryStream = targetEntry.Open())
                    using (StreamReader reader = new StreamReader(entryStream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // 跳过空行和续行
                            if (line.Length == 0 || char.IsWhiteSpace(line[0]))
                                continue;

                            // 检查是否以"Main-Class:"开头（不区分大小写）
                            if (line.TrimStart().StartsWith("Main-Class:", StringComparison.OrdinalIgnoreCase))
                            {
                                // 提取冒号后的值（处理可能存在的空格）
                                var parts = line.Split(new[] { ':' }, 2);
                                if (parts.Length == 2)
                                {
                                    mainClass = parts[1].Trim();
                                    return true;
                                }
                                return false;
                            }
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        string SetValueData(string value)
        {
            var valueSpan = value.AsSpan();
            var buf = new StringBuilder((int)(value.Length * 1.5));

            for (int x = 0; x < valueSpan.Length; x++)
            {
                char c = valueSpan[x];
                if (c == '\\')
                {
                    if (++x >= valueSpan.Length) return null;
                    buf.Append(valueSpan[x]);
                }
                else if (c == '{' || c == '\'')
                {
                    int start = x++;
                    while (x < valueSpan.Length && valueSpan[x] != (c == '{' ? '}' : '\'')) x++;
                    if (x >= valueSpan.Length) return null;

                    var key = valueSpan.Slice(start + 1, x - start - 1).ToString();
                    if (!IPFManage.data.TryGetValue(key, out var val)) return null;
                    buf.Append(val);
                }
                else
                {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }
    }
    public class InstallProFileManage
    {
        /// <summary>
        /// 原版install数据
        /// </summary>
        public InstallProFileInfo InstallProFileInfo { get; set; }
        /// <summary>
        /// 处理后data
        /// </summary>
        public Dictionary<string,string> data { get; set; }
        /// <summary>
        /// 处理后processors
        /// </summary>
        public List<InstallProFileInfo.Processors> processors { get; set; }

        public string InstallPathJson;
        public bool IsClient;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="installPath">install_profile.json路径</param>
        /// <param name="isClient">是否客户端，默认为真</param>
        public InstallProFileManage(string installPath,bool isClient=true)
        {
            InstallPathJson = installPath;
            IsClient = isClient;
            if(File.Exists(installPath))
            {
                string file = File.ReadAllText(installPath);
                InstallProFileInfo = JsonSerializer.Deserialize<InstallProFileInfo>(file,DATA.JSON_OPTIONS);
                Getdata(isClient);
                GetProcessors(isClient);
            }
            else
            {
                InstallProFileInfo = new();
                data = new();
                processors = new();
            }
        }

        public string GetMincraft()
        {
            return InstallProFileInfo.minecraft;
        }
        /// <summary>
        /// 处理data
        /// </summary>
        /// <param name="isClient"></param>
        void Getdata(bool isClient)
        {
            data = new Dictionary<string, string>();
            foreach (var item in InstallProFileInfo.data)
            {
                if(isClient)
                {
                    data.Add(item.Key, GetdataS(item.Value.client));
                }
                else
                {
                    data.Add(item.Key, GetdataS(item.Value.server));
                }
            }
        }
        public static string Getzykh(string value)
        {
            if (value.StartsWith("[") && value.EndsWith("]"))//第一种情况
            {
                value = value[1..^1];//去除首尾
                string[] vals = value.Split(":");//:分割
                string filename = $"{vals[1]}-{vals[2]}";//构建名称
                if (vals.Length > 3)
                {
                    if (vals[3].IndexOf("@") != -1)
                    {
                        filename += $"-{vals[3].Replace("@", ".")}";
                    }
                    else
                    {
                        filename += $"-{vals[3]}.jar";
                    }

                }
                else
                {
                    filename += ".jar";
                }
                return Path.Combine(PATH.LIBRARIES, vals[0].Replace(".", "/"), vals[1], vals[2], filename);
            }
            return null;
        }
        string GetdataS(string value)
        {
            var result = Getzykh(value);
            if (result!= null)
            {
                return result;
            }
            if (value.StartsWith("'") && value.EndsWith("'"))//第二种情况
            {
                value = value[1..^1];//去除首尾
                return value;
            }
            //其余情况
            if(value.StartsWith("/")||value.StartsWith("\\"))
            {
                value = value[1..^0];
            }
            var temppath = Path.Combine(Path.GetDirectoryName(InstallPathJson), value);//构造路径(注意这时候理论上是解压过jar文件了)
            return temppath;
        }
        /// <summary>
        /// 处理processors
        /// </summary>
        /// <param name="isClient"></param>
        void GetProcessors(bool isClient)
        {
            processors = new List<InstallProFileInfo.Processors>();
            foreach (var item in InstallProFileInfo.processors)
            {
                if (item.sides == null || item.sides.Count == 0)//没有限制则添加
                {
                    processors.Add(item);
                }
                if (!isClient &&item.sides[0] == "server")//服务端
                {
                    processors.Add(item);
                }
                if(isClient && item.sides[0] == "client")//客户端
                {
                    processors.Add(item);
                }
            }
        }
    }
    /// <summary>
    /// NeoForge安装Json文件类
    /// </summary>
    public class InstallProFileInfo
    {
        public int spec { get; set; }
        public string profile { get; set; }
        public string version { get; set; }
        public string icon { get; set; }
        public string minecraft { get; set; }
        public string json { get; set; }
        public string logo { get; set; }
        public string path { get; set; }//从源码扒的
        public string urlIcon { get; set; }//从源码扒的
        public string welcome { get; set; }
        public string mirrorList { get; set; }
        public bool hideClient { get; set; }//从源码扒的
        public bool hideServer { get; set; }//从源码扒的
        public bool hideExtract { get; set; }
        public Dictionary<string, Data> data { get; set; }
        public List<Processors> processors { get; set; }
        public List<Libraries> libraries { get; set; }
        public string serverJarPath { get; set; }
        public class Data
        {
            public string client { get; set; }
            public string server { get; set; }
        }
        public class Processors
        {
            public List<string> sides { get; set; }
            public string jar { get; set; }
            public List<string> classpath { get; set; }
            public List<string> args { get; set; }
            public Dictionary<string,string> outputs { get; set; }//从源码扒的
        }
        public class Libraries
        {
            public string name { get; set; }
            public Downloads downloads { get; set; }
            public class Downloads
            {
                public Artifact artifact { get; set; }
                public class Artifact
                {
                    public string sha1 { get; set; }
                    public int size { get; set; }
                    public string url { get; set; }
                    public string path { get; set; }
                }
            }
        }
    }
}
/*
 * 反编译neoforge安装器获取的客户端安装方式
 * 1.检查并安装原版
 * 2.检查并下载neoforge支持库文件
 * 3.检查并进行neoforge的安装
 *      data不为空
 *          处理data
 *              创建临时文件夹
 *              循环处理data列表
 *                  1.如果被[]包围
 *                      手动处理为lib路径后结束本次循环
 *                  2.被\\包围
 *                      去除\\后结束本次循环
 *                  3.其他
 *                      与临时文件夹拼接后从安装jar提取文件到临时文件夹
 *              循环结束
 *      添加到data
 *          客户端（服务端）
 *          客户端jar
 *          mc版本
 *          mc版本文件夹
 *          安装器路径
 *          lib路径
 *      检查本地是否存在mappings.txt（也就是官方的映射文件）
 *      循环处理processors列表--》循环变量为porc
 *          获取全名称（也就是::分割的前两项）
 *          名称为installertools的（非全名（是：第二项））
 *              获取porc的--task项
 *              全名添加上--task的内容
 *              如果task项为DOWNLOAD_MOJMAPS且本地存在mappings.txt
 *                  结束本次循环
 *          创建一个空哈希字典
 *          如果porc的output不为空
 *          循环处理porc的output列表--》循环变量为e
 *              如果e的键为[]包围
 *                  手动处理为lib路径
 *              否则
 *                  使用data中的键值对对e的键进行替换
 *              如果e的值不为空
 *                  使用data中的键值对对e的值进行替换
 *              如果e的键或值为空
 *                  报错且设置上次循环截止量miss为false
 *              更新键值对
 *              根据键构造路径
 *              路径不存在
 *                  跳出本次循环
 *              求路径的哈希值
 *                  哈希值与值相同
 *                  跳出本次循环
 *          如果！miss为真
 *              结束本次循环
 *          构造porc的jar本地lib路径
 *              如果不存在
 *                  结束安装
 *          找到porc的jar的mainclass
 *          如果mainclass不存在
 *              结束安装
 *          创建一个Url列表
 *          将jar路径转为url添加进列表
 *          循环proc的Classpath列表
 *              构造lib路径
 *              存在则转化为url添加进列表
 *          创建一个字符串列表
 *          循环proc的args列表
 *              如果[]包围
 *                  处理成lib路径加入列表
 *              否则
 *              使用data中的键值对值进行替换后加入列表
 *              
 *          切换当前线程的上下文类加载器
 *          反射调用mainclass的main方法
 *          无论是否成功切换回原线程的上下文类加载器
 *          如果output不为空
 *              循环output列表--》循环变量为e
 *                  使用e的键生成路径
 *                  路径文件不存在
 *                      跳出本次循环
 *                  计算哈希
 *                  哈希相同
 *                      跳出本次循环
 *                  报错
 * 4.合并json文件
 */
/*
 * 基于neoforge的安装器的自定义安装逻辑
 * 1.检查并安装原版和neoforge支持库文件
 * 2.检查并进行neoforge的安装
 *      处理数据同原
 *      需要启动jvm调用原反射
 */
/*
 * 基于neoforge的安装器的自定义安装逻辑2
 * 1.检查并安装原版和neoforge支持库文件
 * 2.检查并进行neoforge的安装
 *      构建注入，对原安装程序进行修改
 */