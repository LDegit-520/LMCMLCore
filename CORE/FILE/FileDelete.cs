using LMCMLCore.CORE.data;
using LMCMLCore.CORE.Install.mc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.FILE
{
    /// <summary>
    /// 这里用来执行文件的删除
    /// </summary>
    class FileDelete
    {
        /// <summary>
        /// 重复文件检查。 默认超时为1天，超时会删除。本方法isDelete参数如果存在则会覆盖全局参数
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="time">文件超时时间，默认为1天</param>
        /// <param name="isDelete">是否强制删除文件,这个会临时覆盖全局设置</param>
        public static void RepeatFile(string path,bool? isDelete=null,int time = 1 * 60 * 24)
        {
            if (isDelete==null&&SETTING.ISMANDATORYDELETION)//没有自定义设置，则使用全局配置
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            if(isDelete==true)//设置为强制删除
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            TimeOutDelete(path,time);//超时判断删除1天前的文件
        }
        /// <summary>
        /// 超时删除
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <param name="time">超时时间</param>
        public static void TimeOutDelete(string path, int time = 1 * 60 * 24)
        {
            if (!File.Exists(path))
            {
                return;
            }
            //获取文件创建时间
            DateTime createTime = File.GetCreationTime(path);
            //获取当前时间
            DateTime nowTime = DateTime.Now;
            //获取时间差
            TimeSpan ts = nowTime - createTime;
            //判断是否超时
            if (ts.TotalMinutes > time)
            {
                //删除文件
                File.Delete(path);
            }
        }
    }
}
