using LMCMLCore.CORE.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Reflection;

namespace LMCMLCore.CORE.Mod.Modtinth
{
    /*
     * 有速率限制
     * 每分钟300次
     * 初步认为每0.25秒一次较为合适，既不会使其反应过慢，也可大幅避免出现限速情况，虽然极限是0.2秒一次，但还是不应该一直高速率请求
     */
    /*
     * 需要实现的类
     * 1.搜索
     *      默认搜索
     *      带参搜索
     *  2.获取单个数据
     *      总体数据
     *      版本数据
     *      获取前置
     *  3.下载
     */
    public class ModrinthApi
    {
        /// <summary>
        /// 每分钟最大请求次数，
        /// <para>该数据来源自官方文档</para>
        /// <para>url: https://docs.modrinth.com/api/</para>
        /// <para>获取该数据时间：2025-04-05</para>
        /// </summary>
        public readonly static int MAXIMUM = 300;
        /// <summary>
        /// User-Agent头内容,根据官方文档要求，使用这种可以尽量避免封禁
        /// <para>项目名称/项目版本（联系方式）</para>
        /// </summary>
        public readonly static string USER_AGENTS = $"{CORE_DATA.NAME}/{CORE_DATA.VERSION}({CORE_DATA.EMAIL})";

        private static HttpClient httpClient = new();
        static ModrinthApi()
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
            //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENTS);
        }
    }
    
    public class ModrinthApi_Search
    {
        /// <summary>
        /// 要搜索的查询
        /// </summary>
        public string query { get; set; } = null;
        /// <summary>
        /// 刻面请通过Facets.Tostring()赋值，不要手动赋值
        /// </summary>
        public string facets { get; set; } = null;
        /// <summary>
        /// 指数，请使用Enum_index赋值，不要手动赋值
        /// </summary>
        public string index { get; set; } = null;
        /// <summary>
        /// 偏移量；
        /// </summary>
        public int offset { get; set; } = 0;
        /// <summary>
        /// 返回数量；默认为10，<=100(注，这个默认是指此项不进行设置的情况下)
        /// </summary>
        public int limit { get; set; } = 10;
        /// <summary>
        /// 获取无参搜索
        /// </summary>
        /// <returns></returns>
        public static string GetUrl()
        {
            return URL.GetModrinthSearch();
        }
        /// <summary>
        /// 获取名称搜素
        /// </summary>
        /// <param name="query">搜素关键词</param>
        /// <returns>最终url</returns>
        public static string GetUrl(string query)
        {
            return $"{URL.GetModrinthSearch}?{WebUtility.UrlDecode($"query ={query}")}";
        }
        public static string GetUrl(ModrinthApi_Search search)
        {
            string query = string.IsNullOrEmpty(search.query) ? "" : WebUtility.UrlDecode($"query={search.query}");
            string facets = string.IsNullOrEmpty(search.facets) ? "" : "&" + WebUtility.UrlDecode($"facets={search.facets}");
            string index = string.IsNullOrEmpty(search.index) ? "" : "&" + WebUtility.UrlDecode($"index={search.index}");
            string offset = search.offset == 0 ? "" : "&" + WebUtility.UrlDecode($"offset={search.offset}");
            string limit = search.limit == 10 ? "" : "&" + WebUtility.UrlDecode($"limit={search.limit}");
            return $"{URL.GetModrinthSearch}?{query}{facets}{index}{offset}{limit}";
        }


        #region 内置数据类
        /// <summary>
        /// 参数列表
        /// <para>值有两种可能</para>
        /// <para>不需要操作符的,直接把值添加进即可</para>
        /// <para>另一种是添加下面所示操作符的，包含操作符请在最前面加上%，表示这个值是包含操作符的</para>
        /// <para>最常见的操作是 ：（与=相同 ），但也可以使用 ！= 、》=、》、《= 和 《 </para>
        /// </summary>
        public class Facets
        {
            public List<string> project_type { get; set; } = null;
            public List<string> categories { get; set; } = null;
            public List<string> versions { get; set; } = null;
            public List<string> client_side { get; set; } = null;
            public List<string> server_side { get; set; } = null;
            public List<string> open_source { get; set; } = null;
            public List<string> title { get; set; } = null;
            public List<string> author { get; set; } = null;
            public List<string> follows { get; set; } = null;
            public List<string> project_id { get; set; } = null;
            public List<string> license { get; set; } = null;
            public List<string> downloads { get; set; } = null;
            public List<string> color { get; set; } = null;
            public List<string> created_timestamp { get; set; } = null;
            public List<string> modified_timestamp { get; set; } = null;
            public override string ToString()
            {
                // 获取所有公共实例属性（包括继承的）
                var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var parts = new List<string>();

                foreach (var prop in properties)
                {
                    // 跳过索引器（有参数的属性）
                    if (prop.GetIndexParameters().Length > 0)
                    {
                        continue; 
                    }

                    // 获取属性值
                    object value = prop.GetValue(this);

                    // 忽略空值
                    if (value == null)
                    {
                        continue;
                    }
                    //转为原格式
                    List<string> listValue = (List<string>)value;
                    var processedItems = listValue.Select(item =>
                    {
                        if (item.StartsWith("%"))
                        {
                            return $"\"{prop.Name}{item[1..]}\"";
                        }
                        else
                        {
                            return $"\"{prop.Name}:{item}\"";
                        }
                    });
                    string Lvalue = $"[{string.Join(",", processedItems)}]";
                    parts.Add(Lvalue);
                }

                // 用逗号连接所有非空属性
                return string.Join(", ", parts);
            }
        }
        /// <summary>
        /// index值枚举
        /// </summary>
        public class Enum_index
        {
            /// <summary>
            /// 默认值。relevance
            /// </summary>
            public static string _default = "relevance";
            /// <summary>
            /// 相关
            /// </summary>
            public static string relevance = "relevance";
            /// <summary>
            /// 下载
            /// </summary>
            public static string downloads = "downloads";
            /// <summary>
            /// 如下
            /// </summary>
            public static string follows = "follows";
            /// <summary>
            /// 最新
            /// </summary>
            public static string newest = "newest";
            /// <summary>
            /// 最具更新
            /// </summary>
            public static string updated = "updated";
        }
        #endregion
    }
    public class ModrinthApi_GetData()
    {
        /// <summary>
        /// 项目信息地址
        /// </summary>
        /// <param name="name">项目名称（id,slug）</param>
        /// <returns></returns>
        public static string GetProject(string name)
        {
            return URL.GetModrinthProject(name);
        }
        /// <summary>
        /// 项目列表地址
        /// </summary>
        /// <param name="name">项目名称（id,slug）</param>
        /// <returns></returns>
        public static string GetProjectVersion(string name)
        {
            return URL.GetModrinthProjectVersion(name);
        }
        /// <summary>
        /// 项目依赖地址
        /// </summary>
        /// <param name="name">项目名称（id,slug）</param>
        /// <returns></returns>
        public static string GetProjectDependencies(string name)
        {
            return URL.GetModrinthProjectDependencies(name);
        }
    }
}
