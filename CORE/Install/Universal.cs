using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install
{
    /// <summary>
    /// 通用方法类，这里存放在多出类中需要用到，但是这些类又没有强关联的方法
    /// </summary>
    class Universal
    {
        public static string GetUrl_name(string name)
        {
            var paths = name.Split(":");//按照:分成三部分
            string astr = Path.Combine(paths[0].Replace(".", "/"), paths[1], paths[2],$"{paths[1]}-{paths[2]}.jar");//生成下载路径
            return astr;
        }
        public static string GetUrl_Path_name(string name)
        {
            var paths = name.Split(":");//按照:分成三部分
            string astr = Path.Combine(paths[0].Replace(".", "/"), paths[1], paths[2], $"{paths[1]}-{paths[2]}.jar");//生成本地保存路径
            return astr;
        }
    }
}
