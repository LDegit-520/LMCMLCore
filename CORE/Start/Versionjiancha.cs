using LMCMLCore.CORE.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Start
{
    public class Versionjiancha
    {
        /// <summary>
        /// 获取指定文件夹内部的所有版本实例文件夹
        /// </summary>
        /// <param name="Verpath">指定文件夹</param>
        /// <returns>版本实例文件夹列表</returns>
        public static List<string> GetVersions(string Verpath)
        {
            var list = new List<string>();
            string[] subFolders = Directory.GetDirectories(Verpath, "*", SearchOption.TopDirectoryOnly);
            foreach (string subFolder in subFolders)
            {
                if (File.Exists(Path.Combine(subFolder,PATH._START_JSON)))
                {
                    list.Add(Path.Combine(subFolder, PATH._START_JSON));
                }
            }
            return list;
        }
        /// <summary>
        /// 获取默认文件夹内部的所有版本实例文件夹（）也就是.mincraft/versions
        /// </summary>
        /// <returns>版本实例文件夹列表</returns>
        public static List<string> GetVersions()
        {
            return GetVersions(PATH.VERSIONS);
        }
    }
}
