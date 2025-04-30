using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Mod.Modtinth
{
    /// <summary>
    /// 用于处理版本josn数据
    /// </summary>
    public class ModrinthVersionJson
    {
        public List<ModrinthVersiomJsonInfo> versiomJsonInfos { get; set; }
        private Dictionary<string, List<ModrinthVersiomJsonInfo>> Game_Version;
        private Dictionary<string, List<ModrinthVersiomJsonInfo>> Game_Loaders;
        public List<ModrinthVersiomJsonInfo> GetVersion(List<string> versions, List<string> loaders)
        {
            List<ModrinthVersiomJsonInfo> result = new();
            foreach (var item in versions)
            {
                if (Game_Version.ContainsKey(item))
                {
                    result.AddRange(Game_Version[item]);
                }
            }
            return result;
        }
        public List<ModrinthVersiomJsonInfo> GetVesion_V(List<string> versions)
        {
            List<ModrinthVersiomJsonInfo> result = new();
            foreach (var item in versions)
            {
                if(Game_Version.ContainsKey(item))
                {
                    result.AddRange(Game_Version[item]);
                }
            }
            return result;
        }
        public List<ModrinthVersiomJsonInfo> GetVesion_L(List<string> loaders)
        {
            List<ModrinthVersiomJsonInfo> result = new();
            foreach (var item in loaders)
            {
                if (Game_Loaders.ContainsKey(item))
                {
                    result.AddRange(Game_Loaders[item]);
                }
            }
            return result;
        }
        private void setversion()
        {

        }
        private void setLoader()
        {

        }
    }
    /// <summary>
    /// 表示 Modrinth 版本的 JSON 信息类，包含了关于 Modrinth 上某个项目版本的详细信息。
    /// </summary>
    public class ModrinthVersiomJsonInfo
    {
        /// <summary>
        /// 获取或设置该版本所支持的游戏版本列表。
        /// </summary>
        public List<string> game_versions { get; set; }

        /// <summary>
        /// 获取或设置该版本所支持的加载器列表。
        /// </summary>
        public List<string> loaders { get; set; }

        /// <summary>
        /// 获取或设置该版本的唯一标识符。
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 获取或设置该版本所属项目的唯一标识符。
        /// </summary>
        public string project_id { get; set; }

        /// <summary>
        /// 获取或设置该版本作者的唯一标识符。
        /// </summary>
        public string author_id { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该版本是否为特色版本。
        /// </summary>
        public bool featured { get; set; }

        /// <summary>
        /// 获取或设置该版本的名称。
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 获取或设置该版本的版本号。
        /// </summary>
        public string version_number { get; set; }

        /// <summary>
        /// 获取或设置该版本的更新日志内容。
        /// </summary>
        public string changelog { get; set; }

        /// <summary>
        /// 获取或设置该版本更新日志的 URL。
        /// </summary>
        public string changelog_url { get; set; }

        /// <summary>
        /// 获取或设置该版本的发布日期。
        /// </summary>
        public string date_published { get; set; }

        /// <summary>
        /// 获取或设置该版本的下载次数。
        /// </summary>
        public long downloads { get; set; }

        /// <summary>
        /// 获取或设置该版本的类型（如 release、beta、alpha 等）。
        /// </summary>
        public string version_type { get; set; }

        /// <summary>
        /// 获取或设置该版本的状态。
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 获取或设置该版本请求的状态。
        /// </summary>
        public string requested_status { get; set; }

        /// <summary>
        /// 获取或设置与该版本相关的文件列表。
        /// </summary>
        public List<File> files { get; set; }

        /// <summary>
        /// 获取或设置该版本的依赖项列表。
        /// </summary>
        public List<Dependencies> dependencies { get; set; }

        /// <summary>
        /// 表示与 Modrinth 版本相关的文件信息类。
        /// </summary>
        public class File
        {
            /// <summary>
            /// 获取或设置文件的哈希值信息。
            /// </summary>
            public Hashes hashes { get; set; }

            /// <summary>
            /// 获取或设置文件的下载 URL。
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// 获取或设置文件的名称。
            /// </summary>
            public string filename { get; set; }

            /// <summary>
            /// 获取或设置一个值，指示该文件是否为主文件。
            /// </summary>
            public bool primary { get; set; }

            /// <summary>
            /// 获取或设置文件的大小。
            /// </summary>
            public long size { get; set; }

            /// <summary>
            /// 获取或设置文件的类型。
            /// </summary>
            public string file_type { get; set; }

            /// <summary>
            /// 表示文件的哈希值信息类，包含 SHA-1 和 SHA-512 哈希值。
            /// </summary>
            public class Hashes
            {
                /// <summary>
                /// 获取或设置文件的 SHA-1 哈希值。
                /// </summary>
                public string sha1 { get; set; }

                /// <summary>
                /// 获取或设置文件的 SHA-512 哈希值。
                /// </summary>
                public string sha512 { get; set; }
            }
        }

        /// <summary>
        /// 表示 Modrinth 版本的依赖项信息类。
        /// </summary>
        public class Dependencies
        {
            /// <summary>
            /// 获取或设置依赖项的版本唯一标识符。
            /// </summary>
            public string version_id { get; set; }

            /// <summary>
            /// 获取或设置依赖项所属项目的唯一标识符。
            /// </summary>
            public string project_id { get; set; }

            /// <summary>
            /// 获取或设置依赖项的文件名。
            /// </summary>
            public string file_name { get; set; }

            /// <summary>
            /// 获取或设置依赖项的类型。
            /// </summary>
            public string dependency_type { get; set; }
        }
    }
}
