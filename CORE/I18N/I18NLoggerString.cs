using LMCMLCore.CORE.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.I18N
{
    /*
     * 这是国际化的一部分
     * 这里存储日志用到的所有文本
     * 默认文本为中文
     */
    /// <summary>
    /// 这里用来存放国际化的字符串
    /// 请注意，对于没有必要设为占位符的请不要设置占位符，例如可以放在最前面和最后面的就无必要
    /// 命名规则为 class_xxx 类名_解释
    /// <para>请注意{0}等为占位符</para>
    /// </summary>
    public class I18NLoggerString
    {
        public static string PATH_EXEPATHWARRING = "请注意你现在正在使用Windows平台进行游玩，但是你的exe路径长度大于130字符，为了防止在以后的运行中出现错误，请及时修改,现在提供一下几种解决方法\r\n" +
            "1.关闭启动器，打开exe文件路径{0}，将exe文件和.minecraft文件夹和LMCML文件夹进行剪切移动。如果你的电脑上存在D盘可以在D盘根目录建立一个纯英文文件夹，将前面剪切的文件放入，如果不存在，那就在C盘根目录创建\r\n" +
            "2.关闭启动器，使用 Win+R 组合键打开运行窗口，输入 gpedit.msc 回车打开 本地组策略编辑器 依次打开 计算机配置 > 管理模板 > 系统 > 文件系统 找到 启用 Win32 长路径 这个选项更改为已启用然后确认后将电脑进行重启\r\n" +
            "3.关闭启动器，使用 Win+R 组合键打开运行窗口，输入 regedit 回车打开 注册表编辑器 依次打开 HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\FileSystem " +
            "在右侧找到名为 LongPathsEnabled 的项。如果没有该项，则需要新建一个DWORD（32位）值，并命名为 LongPathsEnabled 然后将其值设置为 1 关闭注册表编辑器并重启电脑\r\n" +
            "4.关闭启动器，打开PowerShell输入下面的命令并运行\r\nNew-ItemProperty -Path \"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\FileSystem\" -Name \"LongPathsEnabled\" -Value 1 -PropertyType DWORD –Force\r\n运行完成后重启电脑";
        public static string PATH_wenjiancuowu = "文件夹不存在，且文件夹创建失败。错误信息为：";
        public static string PATH_wenjianchuangjian = "文件夹已创建：";
        public static string PATH_wenjianwancheng = "文件夹检查完成，均已存在";

        public static string DownLoadManager_Fail = "任务 {0} 第{1}次失败，正在重试";
        public static string DownLoadManager_Error = "任务出错";
        public static string DownLoadManager_Disposed = "实例资源已清理，无法调用";
        public static string DownLoadManager_TaskFinish = "下载任务已完成，保存到：";
        public static string DownLoadManager_TaskAnagerError = "无法获取文件信息（尝试了 {0} 次）";
        public static string DownLoadManager_tempfilenull = "临时文件不存在";
        public static string DownLoadManager_downtimeout = "下载超时（{0} 分钟）";
        public static string DownLoadManager_filesize = "文件大小不匹配:";
        public static string DownLoadManager_noRangerError = "服务器不支持分段下载.状态码：";
        public static string DownLoadManager_HttpError = "请求服务器获取文件头出错";
        public static string DownLoadManager_KaishiTask = "开始下载";
        public static string DownLoadManager_TAskCanel = "任务取消";
        public static string DownLoadManager_TAskError = "下载出现错误";
        public static string DownLoadManager_DisposedError = "等待任务退出时发生错误:";

        public static string McDown_VErsionJsonError = "版本清单下载失败";

        /// <summary>
        /// 设置国际化文本
        /// <paramref name="path"/>
        /// </summary>
        /// <param name="path">json文件路径</param>
        public static void SetI18N(string path)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new StaticClassConverter<I18NLoggerString>() },
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,//忽略null
                WriteIndented = true // 可选：格式化JSON
            };
            string jsontxt = File.ReadAllText(path);
            JsonSerializer.Deserialize<I18NLoggerString>(jsontxt,options);
        }
        /// <summary>
        /// 将此类json化到指定文件
        /// </summary>
        /// <param name="path">文件地址</param>
        public static void GetI18N(string path)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new StaticClassConverter<I18NLoggerString>() },
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,//忽略null
                WriteIndented = true // 可选：格式化JSON
            };
            string jsontxt = JsonSerializer.Serialize<I18NLoggerString>(new I18NLoggerString(),options);
            //Console.WriteLine(jsontxt);
            File.WriteAllText(path,jsontxt);
        }
        public static void I18NJSON(string  path)
        {
            Dictionary<string, string> jsontext = new() { 
                { "I18N",PATH.I18N_ZH_CN_JSON},
                { "备注:" , "请注意如果需要根据zh_cn.json生成其他语言的文档时请保留 '{0}'等这些是程序文档中的占位符"}

            };
            
            File.WriteAllText(path, JsonSerializer.Serialize<Dictionary<string, string>>(jsontext,DATA.JSON_OPTIONS));
        }
    }
    public class StaticClassConverter<T> : JsonConverter<T> where T : class
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader);
            foreach (var kvp in obj)
            {
                var field = typeof(T).GetField(kvp.Key, BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                {
                    field.SetValue(null, Convert.ChangeType(kvp.Value.ToString(), field.FieldType));
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            writer.WriteStartObject();
            foreach (var field in fields)
            {
                writer.WritePropertyName(field.Name);
                JsonSerializer.Serialize(writer, field.GetValue(null), field.FieldType, options);
            }
            writer.WriteEndObject();
        }
    }
}
