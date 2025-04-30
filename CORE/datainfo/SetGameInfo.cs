using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.datainfo
{
    /// <summary>
    /// 启动game配置，这个是为了简化生成操作和方便自定义而设置的
    /// </summary>
    public class SetGameInfo
    {
        /// <summary>
        /// 是否demo（也就是预览版）
        /// </summary>
        public bool isdemo { get; set; } = false;
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool isfullScreen { get; set; } = false;
        /// <summary>
        /// 宽度
        /// </summary>
        public string? width { get; set; } = "873";
        /// <summary>
        /// 高度
        /// </summary>
        public string? height { get; set; } = "494";
        /// <summary>
        /// 是否调试
        /// </summary>
        public bool isquickPlayPath { get; set; } = false;
        /// <summary>
        /// 调试参数
        /// </summary>
        public string quickPlayPath { get; set; }
        /// <summary>
        /// 是否单人游戏
        /// </summary>
        public bool isquickPlaySingleplayer { get; set; } = false;
        /// <summary>
        /// 单人游戏（指定存档）
        /// </summary>
        public string quickPlaySingleplayer { get; set; }
        /// <summary>
        /// 是否多人游戏
        /// </summary>
        public bool isquickPlayMultiplayer { get; set; } = false;
        /// <summary>
        /// 多人游戏链接地址
        /// </summary>
        public string quickPlayMultiplayer { get; set; }
        /// <summary>
        /// 是否局域网游戏
        /// </summary>
        public bool isquickPlayRealms { get; set; } = false;
        /// <summary>
        /// 局域网游戏地址
        /// </summary>
        public string quickPlayRealms { get; set; }
        /// <summary>
        /// 是否服务器，老版本
        /// </summary>
        public bool isserver { get; set; } = false;
        /// <summary>
        /// 服务器链接
        /// </summary>
        public string server { get; set; }
        /// <summary>
        /// 端口名
        /// </summary>
        public string port { get; set; }
    }
}
