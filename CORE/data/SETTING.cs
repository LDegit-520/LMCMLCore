using LMCMLCore.CORE.datainfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml;

namespace LMCMLCore.CORE.data
{
    /*
     * 这里计划存储所有不是超强关联的设置
     * 并提供修改，保存，初始化等
     */
    /*
     * 设置应该包含两项，设置名和数据
     */
    /*
     * 实现思路
     * 对于存在修改的存储在json文件中，
     * 静态类存储所有默认值
     * 然后读取json对静态类进行修改
     * 如果在运行时修改了某些属性，则修改json和静态类
     */
    /// <summary>
    /// 这是设置区域，每次启动时会检查是否具有设置文件，如果有则读取设置文件，如果没有则把默认配置生成为配置文件。本数据类的所有数据均可在使用中修改
    /// <para>请注意本类应该全部具有默认值</para>
    /// </summary>
    class SETTING
    {
        /// <summary>
        /// 是否强制删除，默认为否，但是这个可以自行设置，在读取设置配置后可能不为否
        /// </summary>
        public static bool ISMANDATORYDELETION { get; set; } = false;
        /// <summary>
        /// 全局game参数，在没有为游戏实例自定义时，使用这个
        /// </summary>
        public SetGameInfo OverallSetGameInfo { get; set; } = new SetGameInfo();
        /// <summary>
        /// 事件主线最大容量
        /// </summary>
        public static int EventThreadDataSize { get; set; } = 32768;
    }

    public class sett
    {
        #region 方法与初始化区
        /// <summary>
        /// 不可变副本
        /// </summary>
        private static sett noset;
        /// <summary>
        /// 可变副本
        /// </summary>
        private static sett yesset;
        /// <summary>
        /// 静态构造
        /// </summary>
        static sett() { noset = new sett(); yesset = new sett(); }
        /// <summary>
        /// 私有构造
        /// </summary>
        private sett() {  }
        /// <summary>
        /// 外部获取可变副本
        /// </summary>
        /// <returns>可变副本</returns>
        public static sett GetSett() { return yesset; }
        /// <summary>
        /// 根据外部json修改副本
        /// </summary>
        /// <param name="jsonpath">json路径</param>
        public void SetJson(string jsonpath)
        {
            yesset = JsonSerializer.Deserialize<sett>(File.ReadAllText(jsonpath));
        }
        /// <summary>
        /// 将有修改的保存进json
        /// </summary>
        /// <param name="jsonpath">json路径</param>
        public void AJson(string jsonpath)
        {
            string json = ObjectComparer.GetDifferencesToJson(noset, yesset);
            File.WriteAllText(jsonpath, json);
        }
        public static class ObjectComparer
        {
            public static string GetDifferencesToJson<T>(T obj1, T obj2) where T : class
            {
                // 获取所有公共实例属性
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var differences = new Dictionary<string, object>();

                foreach (var prop in properties)
                {
                    // 获取两个对象的属性值
                    var value1 = prop.GetValue(obj1);
                    var value2 = prop.GetValue(obj2);

                    // 比较属性值是否不同
                    if (!Equals(value1, value2))
                    {
                        // 将A2的属性值添加到差异字典中
                        differences.Add(prop.Name, value2);
                    }
                }
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,//忽略null
                    WriteIndented = true // 格式化输出
                };
                return JsonSerializer.Serialize(differences, options);
            }
        }

        #endregion
        #region 属性区

        #endregion
    }
    /*
     *一个类维护两个实例 
     *一个完整实例
     *一个不完整实例
     *获取值时有限获取不完整的不完整不存在在获取完整的
     *保存时保存不完整的完整的不用动
     *修改时修改不完整的，如果不完整不存在则增加
     */

}
