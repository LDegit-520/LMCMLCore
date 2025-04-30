using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.datainfo
{
    /// <summary>
    /// 游戏的基础数据
    /// </summary>
    public class UpGameData
    {
        public string GameName { get; set; }
        /// <summary>
        /// 启动路径
        /// </summary>
        public string GamePath { get; set; }
        /// <summary>
        /// 启动玩家
        /// </summary>
        public UserInfo User { get; set; }
    }
}
