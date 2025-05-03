using LMCMLCore.CORE.data;
using LMCMLCore.CORE.DownLoad;
using LMCMLCore.CORE.Json.Mc;
using LMCMLCore.CORE.LOGGER;
using LMCMLCore.CORE.Start;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.mc
{
    public class MCversioninstall
    {
        public McVersionInfo McVersion;
        public string Vername;
        private bool isstart = false;
        public MCversioninstall(McVersionInfo mcVersion, string vername)
        {
            McVersion = mcVersion;
            Vername = vername;
        }
        public async Task<DownLoadCore> Run()
        {
            //构造版本清单下载任务
            var mcversionjsonpath = Path.Combine(PATH.GJARJSON, McVersion.id + ".json");//构造版本清单保存路径
            DownLoadTask mcversionjson = new DownLoadTask(McVersion.url,
                mcversionjsonpath,
                McVersion.size,
                McVersion.hash,
                McVersion.hashinfo
                );
            //下载版本清单
            DownLoadCore downLoadCore = new DownLoadCore();
            var task = downLoadCore.StartDownloadAsync(new() { mcversionjson });
            await Task.WhenAll(task);
            Logger.Info(nameof(MCversioninstall), $"下载版本清单{McVersion.id}.json完成");
            //处理版本清单
            Version_json version_json = new Version_json(mcversionjsonpath,
                DATA.ARGUMENT_GAME,
                DATA.ARGUMENT_JVM,
                DATA.OS.id,
                DATA.OS.is64bit
                );
            Logger.Info(nameof(MCversioninstall), $"开始处理版本清单{McVersion.id}.json");
            var assetsJson = Path.Combine(PATH.INDEXES, version_json.AssetsIndex.id + ".json");//构造资源索引保存路径
            var jarpath = Path.Combine(PATH.GJARJSON, McVersion.id + ".jar");//构造jar保存路径
            var loggingpath = Path.Combine(PATH.GLOGGING, version_json.McLogging.id);//构造日志保存路径
            Logger.Info(nameof(MCversioninstall), $"开始处理资源索引{version_json.AssetsIndex.id}.json");
            DownLoadTask assetsjsonTask = new DownLoadTask(
                version_json.AssetsIndex.url,
                assetsJson,
                version_json.AssetsIndex.size,
                version_json.AssetsIndex.hash,
                version_json.AssetsIndex.hashinfo
                );
            DownLoadCore downLoadCore2 = new DownLoadCore();
            Logger.Info(nameof(MCversioninstall), $"开始下载资源索引{version_json.AssetsIndex.id}.json");
            var task2 = downLoadCore2.StartDownloadAsync(new() { assetsjsonTask });
            await Task.WhenAll(task2);
            Logger.Info(nameof(MCversioninstall), $"下载资源索引{version_json.AssetsIndex.id}.json完成");
            //构造所有下载任务
            List<DownLoadTask> downLoadTasks = new List<DownLoadTask>()
                    {
                        new(version_json.McDown.client.url,jarpath,version_json.McDown.client.size,version_json.McDown.client.hash,version_json.McDown.client.hashinfo),//jar
                        new(version_json.McLogging.url,loggingpath,version_json.McLogging.size,version_json.McLogging.hash,version_json.McLogging.hashinfo)//logging
                    };
            //lib
            for (int i = 0; i < version_json.Libraries.Count; i++)
            {
                downLoadTasks.Add(new(version_json.Libraries[i].url, version_json.Libraries[i].path, version_json.Libraries[i].size, version_json.Libraries[i].hash, version_json.Libraries[i].hashinfo));
            }
            //nat
            for (int i = 0; i < version_json.Natives.Count; i++)
            {
                downLoadTasks.Add(new(version_json.Natives[i].url, version_json.Natives[i].path, version_json.Natives[i].size, version_json.Natives[i].hash, version_json.Natives[i].hashinfo));
            }
            //ass
            downLoadTasks.AddRange(new Assets_json(assetsJson).GetAssetsDownLoadTasks());
            Logger.Info(nameof(MCversioninstall), $"开始下载所有文件");
            DownLoadCore downLoadCore1 = new DownLoadCore();
            downLoadCore1.AllDownloadProgressChanged += (sender, e) =>
            {
                Logger.Info("当前进度", $"文件前检查");
                if(!isstart)
                {
                    Logger.Info("当前进度", $"创建文件%");
                    //生成启动json
                    StartJsonInfo startJsonInfo = new StartJsonInfo()
                    {
                        Name = Vername,
                        StartJarPath = jarpath.Replace(PATH.EXE,PATH._LMCML_STR),
                        GameJsonPath = mcversionjsonpath.Replace(PATH.EXE, PATH._LMCML_STR),
                        AssetsJsonPath = assetsJson.Replace(PATH.EXE, PATH._LMCML_STR),
                        LoggingPath = loggingpath.Replace(PATH.EXE, PATH._LMCML_STR),
                    };
                    //构造相关文件夹
                    Directory.CreateDirectory(Path.Combine(PATH.VERSIONS, Vername));//创建版本实例文件夹
                    Directory.CreateDirectory(Path.Combine(PATH.VERSIONS, Vername, PATH._NATIVES));//创建natives文件夹
                    File.WriteAllText(Path.Combine(PATH.VERSIONS, Vername, PATH._START_JSON), JsonSerializer.Serialize(startJsonInfo, DATA.JSON_OPTIONS));//创建启动文件
                    //解压复制nat
                    foreach (var item in version_json.Natives)
                    {
                        Logger.Info("当前进度", $"{version_json.Natives.Count}");
                        Logger.Info("当前进度", $"解压{item.path}");
                        //补充代码//
                        //这里需要将item.path记录的文件解压到natives文件夹（已经定义了string形变量）
                        ZipFile.ExtractToDirectory(item.path,Path.Combine(PATH.VERSIONS, Vername, PATH._NATIVES));
                        if (item.SpecialData!=null)
                        {
                            (string,List<string>)? data = item.SpecialData as (string, List<string>)?;
                            if(data!=null)
                            {
                                for(int i=0;i<data.Value.Item2.Count;i++)
                                {
                                    if(Directory.Exists(Path.Combine(PATH.VERSIONS, Vername, PATH._NATIVES, data.Value.Item2[i])))
                                    {
                                        Directory.Delete(Path.Combine(PATH.VERSIONS, Vername, PATH._NATIVES, data.Value.Item2[i]), true);
                                    }
                                }
                            }
                        }
                    }
                    isstart = true;
                }
            };
            downLoadCore1.StartDownloadAsync(downLoadTasks);
            return downLoadCore1;
        }
    }
}
