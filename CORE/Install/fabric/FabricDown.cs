using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using LMCMLCore.CORE.Install.mc;
using LMCMLCore.CORE.Json.Mc;
using LMCMLCore.CORE.LOGGER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.fabric
{
    /*
     * Fabric 安装核心类
     * 所有有关Fabric 安装的操作都在这里进行
     * 请勿绕过这个类进行Fabric的安装操作
     */
    /*
     * 流程示例
     * 用户现在需要安装一个mc实例
     * --->启动器调用McCore获取mc的版本列表给用户选择
     * ------->用户选择了一个mc版本
     * --->启动器通过各个加载器核心获取这个mc版本拥有的mod加载器给用户选择
     * ------->用户选择了一个Fabric加载器（其他mod加载器详见对应核心类）
     * --->启动器调用FabricCore获取这个版本的Fabric Loader列表给用户选择
     * ------->用户选择了一个Fabric Loader
     * --->启动器调用FabricCore获取这个Fabric Loader对应的Fabric Api列表给用户选择（或许还要添加OptiFabric）
     * ------->用户选择了一个Fabric Api
     * =====备注=====
     * 以上请写成公开静态方法，在不创建实例的情况下使用
     * 以上的文件会被下载到临时文件夹，在获取时会检查是否有对应文件，如果有则不进行下载，如果没有则下载，但请注意需要给用户一个选择不使用存储在临时文件夹的缓存文件的选项
     * 以上过程中在主线程进行，在进行获取时会阻断主线程，所以需要添加超时检测，超时则提示用户网络异常，等待重试
     * =====结束 备注=====
     * <-------------------用户选择完毕,进入自动安装------------------->
     * --->启动器调用FabricCore获取Fabric Installer列表
     * --->启动器调用FabricCore下载最新版的Fabric Installer
     * --->启动器调用FabricCore下载对应版本的Fabric Loader Json并处理为lib库下载列表
     * --->启动器调用FabricCore下载对应版本的Fabric Loader
     * --->启动器调用FabricCore启动Fabric Installer进行安装，同时调用ModDownCore进行Fabric Api的下载
     * --->等待Fabric Installer安装完成，和Fabric Api的下载完成
     * =====备注=====
     * 以上请写成私有非静态方法，在创建实例的情况下使用，且参数在构造时传入，中途除取消操作外不允许更改
     * 自动安装过程中不需要用户干预。且不能阻断主线程，所以会启动一个新的线程进行安装操作
     * 为防止用户误操作导致同时安装多个实例，在进入自动安装前，需要检查是否有同配置（mc，Fabric Loader，Fabric Api均相同）实例正在安装
     * 如果有，需要提示用户是否继续安装，并推荐请不要安装相同实例
     * 请注意整合包和用户自定义包互不干扰，所以不需要检查整合包是否安装相同实例
     * =====结束 备注=====
     * <-------------------安装完成,告知用户------------------->
     * 结束一次安装需求
     */
    /*
     * 方法列表
     * 获取fabric Installer列表
     * 获取fabric Loader列表
     * 获取fabric Api列表
     * 获取特定版本fabric Library列表
     * 下载fabric Installer
     * 下载fabric Library
     * 安装带有fabric的mc版本
     */
    /// <summary>
    /// Fabric核心类
    /// <para>获取Fabric下载各种信息</para>
    /// <para>下载一个带有Fabric的MC实例</para>
    /// </summary>
    class FabricDown
    {
        private string Name;
        private string Ver;
        private McVersionInfo Mcver;
        private string Apiver;
        /// <summary>
        /// 下载进度
        /// </summary>
        public double Projress { get; set; } = 0.00;
        public FabricDown(McVersionInfo mcVer,string ver,string apiVer,string name) 
        { 
            Mcver = mcVer;
            Apiver = apiVer;
            Name = name;
            Ver = ver;
        }
        /// <summary>
        /// 启动下载
        /// </summary>
        public void Run()
        { 
            Task.Run(async () =>
            {
                //第一步mc下载
                
                //第二步loader下载
                //if (!(await FabricCore.GetFabricJsonBmcl(Ver, Mcver.id)))//尝试bmcl源
                //{
                //    if (!(await FabricCore.GetFabricJson(Ver, Mcver.id)))//尝试官方源
                //    {
                //        return;//均失败
                //    }
                //}//获取json
                string verjson = File.ReadAllText(Path.Combine(PATH.GFRABRIC,$"{Ver}-{Mcver.id}.json"));
                List<DownLoadTask> downLoads = new List<DownLoadTask>();
                var jsonDoc=JsonNode.Parse(verjson);
                var libraries = jsonDoc?["libraries"]?.AsArray().Select(
                    node => (name: node?["name"]?.GetValue<string>(),url: node?["url"]?.GetValue<string>())).Where(t => !string.IsNullOrEmpty(t.name) && !string.IsNullOrEmpty(t.url)).ToList();
                foreach(var jlib in libraries)
                {
                    downLoads.Add(new DownLoadTask(jlib.url + Universal.GetUrl_name(jlib.name), Path.Combine(PATH.LIBRARIES, Universal.GetUrl_Path_name(jlib.name))));//创建下载任务列表
                }
                //DownLoadManager downLoadManager = new DownLoadManager();
                //downLoadManager.AddTask(downLoads);
                //downLoadManager.Run();//下载lib库
                ////进度检查循环
                //while (true)
                //{
                //    Projress = downLoadManager.Projress();
                //    Logger.Info("当前进度", $"进度：{downLoadManager.Projress().ToString()}%");
                //    Console.WriteLine($"进度：{downLoadManager.Projress().ToString()}%");
                //    if (downLoadManager.Projress() >= 100.00)
                //    {
                //        await Task.Delay(5000);
                //        break;
                //    }
                //    await Task.Delay(1000);
                //}
                //第三步api下载//这部分需要调用mod下载流程暂未实现

            });
        }
    }
}
