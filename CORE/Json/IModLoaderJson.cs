using LMCMLCore.CORE.Json.Mc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Json
{
    /// <summary>
    /// mod加载器应该实现此接口,如果mod加载器没有对应的数据会返回null
    /// </summary>
    public interface IModLoaderJson
    {
        /// <summary>
        /// 获取模组加载器需要的库
        /// </summary>
        /// <returns></returns>
        public List<UnLibraries> GetLibraries();
        /// <summary>
        /// 获取模组加载器需要的启动game参数
        /// </summary>
        /// <returns></returns>
        public List<string> GetArgumentsGame();
        /// <summary>
        /// 获取模组加载器需要的启动jvm参数
        /// </summary>
        /// <returns></returns>
        public List<string> GetArgumentsJvm();
        /// <summary>
        /// 获取模组加载器需要的启动main
        /// </summary>
        /// <returns></returns>
        public string GetMainclass();
    }
}
