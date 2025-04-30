using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.data
{
    /// <summary>
    /// 核心数据
    /// </summary>
    class CORE_DATA
    {
        /// <summary>
        /// 启动器名称
        /// </summary>
        public static string NAME { get; } = "LMCML";
        /// <summary>
        /// 启动器版本
        /// </summary>
        public static string VERSION { get; } = "1.0.0.38";
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public static string EMAIL { get; } = "";
        /// <summary>
        /// 版本介绍。可以在其中填入以下几种情况
        /// <para>1.介绍文本 {str=}</para>
        /// <para>2.介绍链接 {path=}</para>
        /// <para>3.介绍链接+介绍文本 {path=,str=}</para>
        /// </summary>
        public static string VersionIntroduce = "str=开发版本暂无介绍";
        /// <summary>
        /// 私有构造禁止实例化
        /// </summary>
        private CORE_DATA() { }
        /// <summary>
        /// 特殊解释文本
        /// </summary>
        private string Prohibit = "本文本禁止修改禁止删除";
        /*
         * 衍生作品请手动将上面进行修改为自己的信息
         * 并将原LMCML项目的信息加入LMCML_VERSION
         * 如果为衍生的衍生则将原信息加入Dependencies
         */
        /// <summary>
        /// 用于标识基于LMCML项目的版本，用于衍生项目标识原LMCML项目版本
        /// </summary>
        //private static CORE_DATA LMCML_VERSION = null;
        /// <summary>
        /// 给衍生的衍生使用
        /// </summary>
        //private static List<CORE_DATA> Dependencies = null;
    }
}
