using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.datainfo
{
    /// <summary>
    /// 用户数据类
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string? name { get; set; }
        /// <summary>
        /// uuid
        /// </summary>
        public string? id { get; set; }
        /// <summary>
        /// accesstoken
        /// </summary>
        public string? token { get; set; }

        public UserInfo() { }
        /// <summary>
        /// 创建一个用户信息//uuid和accesstoken会自动生成
        /// </summary>
        /// <param name="name">名称</param>
        public UserInfo(string name)
        {
            this.name = name;
            id = Guid.NewGuid().ToString("N");
            token = Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 创建一个用户信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="id">uuid</param>
        /// <param name="token">accesstoken</param>
        public UserInfo(string name, string id, string token)
        {
            this.name = name;
            this.id = id;
            this.token = token;
        }
    }
}
