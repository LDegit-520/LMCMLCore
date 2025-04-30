using LMCMLCore.CORE.data;
using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.forge
{
    /* 
    * forge安装逻辑
    * 1.版本不同的不一致
    * 2.对于存在安装版的使用安装版
    * 3.对于不存在安装版的使用解压版
    * 版本
    * 1.13及以上的使用安装直接安装
    *       安装器启动安装器即可
    * 1.5.2及以上的使用解压版
    *       1.文件会含有一个jar文件这个需要放入到lib文件夹
    *       2.文件会含有一个maven文件夹这个加入到lib文件夹
    * 1.5.2以下不提供安装
    */

    /*
     * 
     */
    class ForgeIntall
    {
        public void Install(string path)
        {
            #region 1.13及以上的使用安装直接安装

            #endregion
            #region 1.5.2及以上的使用解压版
            string Epath = FileExtractor.ExtractFile(path);//解压文件到文件夹
            var ijson=JsonNode.Parse(File.ReadAllText(Path.Combine(Epath, "install_profile.json")));//读取安装json文件
            string verjson= "";
            if (ijson["install"]!=null)//jar文件版
            {
                var jarpath = GetJarPath(ijson["install"]["path"].ToString());//获取lib文件夹的路径
                Directory.CreateDirectory(Path.GetDirectoryName(jarpath));//创建文件夹
                File.Copy(Path.Combine(Epath,ijson["install"]["filepath"].ToString()), jarpath);//粘贴jar文件
                //File.Copy(Path.Combine(Epath, "install_profile.json"), Path.GetFileNameWithoutExtension(path)+".json");

                verjson = ijson["versionInfo"].ToString();
            }
            else//maven文件夹版
            {
                FileMove.MoveDirectory(Path.Combine(Epath, "maven"), PATH.LIBRARIES);//移动maven文件夹

                verjson = ijson.ToString();
            }

            #endregion
        }
        /// <summary>
        /// 获取jar文件的路径
        /// </summary>
        /// <param name="jpath"></param>
        /// <returns></returns>
        private string GetJarPath(string jpath)
        {
            var paths = jpath.Split(":").ToList();
            string path = PATH.LIBRARIES;
            foreach (var item in paths[0].Split("."))
            {
                path = Path.Combine(path, item);
            }
            path = Path.Combine(path, paths[1], paths[2], $"{paths[1]}-{paths[2]}.jar");//生成最终路径
            return path;
        }
    }
}
