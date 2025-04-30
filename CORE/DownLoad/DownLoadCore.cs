using LMCMLCore.CORE.FILE;
using LMCMLCore.CORE.LOGGER;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.DownLoad
{

    /// <summary>
    /// 多线程下载核心管理器
    /// </summary>
    public class DownLoadCore
    {
        #region 字段与事件

        /// <summary>
        /// 取消令牌源（用于优雅终止下载任务）
        /// </summary>
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// 下载进度变更事件（百分比进度）
        /// </summary>
        public event EventHandler<DownloadProgressEventArgs> DownloadProgressChanged;

        /// <summary>
        /// 完成后详细下载状态变更事件（包含每个任务细节）
        /// </summary>
        public event EventHandler<AllDownloadProgressEventArgs> AllDownloadProgressChanged;

        /// <summary>
        /// 总下载量（字节）
        /// </summary>
        private long _totalSize = 1; // 初始值避免除以零

        /// <summary>
        /// 当前已下载量（原子操作保证线程安全）
        /// </summary>
        private long _completedSize;

        /// <summary>
        /// 完成的任务队列（线程安全字典）
        /// Key: 下载任务对象
        /// Value: 是否成功
        /// </summary>
        private readonly ConcurrentDictionary<DownLoadTask, string> _completedTasks = new();

        private Dictionary<DownLoadTask,string> AlldownLoadTasks;

        #endregion

        #region 公共方法

        /// <summary>
        /// 启动下载任务
        /// </summary>
        /// <param name="tasks">需要下载的任务列表</param>
        /// <returns>表示异步操作的Task</returns>
        public Task StartDownloadAsync(List<DownLoadTask> tasks)
        {
            return Task.Run(async () =>
            {
                try
                {
                    // 预处理阶段（单线程操作）
                    var validTasks = PreprocessTasks(tasks);
                    //Logger.Info(nameof(DownLoadCore), $"共有{validTasks.Count}个任务需要下载");//临时
                    if(validTasks.Count == 0)
                    {
                        Logger.Info(nameof(DownLoadCore), "没有需要下载的任务");
                        return;//注意：此处使用return，但不是直接返回，而是会执行finally块
                    }
                    // 任务分配算法
                    var balancedGroups = BalanceTaskLoad(validTasks, maxConcurrency: 8);

                    // 创建并执行下载任务组
                    var downloadTasks = balancedGroups
                        .Where(group => group.tasks.Any()) // 过滤空任务组
                        .Select(group => ProcessTaskGroupAsync(group.tasks));
                    //Logger.Info(nameof(DownLoadCore), $"已创建{downloadTasks.Count()}个下载任务组");
                    await Task.WhenAll(downloadTasks);
                    //Logger.Info(nameof(DownLoadCore), "所有任务已完成");
                }
                catch (OperationCanceledException)
                {
                    Logger.Info(nameof(DownLoadCore), "下载任务已被取消");
                }
                catch (Exception ex)
                {
                    Logger.Error("下载管理器发生未处理异常", ex.ToString());
                }
                finally
                {
                    //Logger.Info(nameof(DownLoadCore), "lscs3");
                    // 最终状态通知
                    OnAllDownloadProgressChanged();
                    _cts.Dispose(); // 释放取消令牌资源
                }
            }, _cts.Token);
        }

        /// <summary>
        /// 请求停止下载操作
        /// </summary>
        public void RequestStop()
        {
            _cts.Cancel();
        }
        private bool Onall = false;
        /// <summary>
        /// 如果最终事件已经触发但是没有接送到最终事件，则手动触发一次
        /// </summary>
        public void OnEvent()
        {
            if(Onall)
            {
                //OnAllDownloadProgressChanged();
            }
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 预处理下载任务
        /// 1. 去重处理
        /// 2. 本地文件校验
        /// </summary>
        private List<DownLoadTask> PreprocessTasks(List<DownLoadTask> tasks)
        {
            // 使用Url去重
            var distinctTasks = tasks
                .DistinctBy(t => t.Url)
                .ToList();

            AlldownLoadTasks = distinctTasks.ToDictionary(keySelector:item=>item,elementSelector:item=> "null");

            // 过滤需要实际下载的任务
            return distinctTasks
                .Where(t => NeedsDownload(t))
                .ToList();
        }

        /// <summary>
        /// 判断任务是否需要下载
        /// </summary>
        private bool NeedsDownload(DownLoadTask task)
        {
            // 文件不存在时需要下载
            if (!File.Exists(task.Path)) return true;

            // 存在哈希校验需求时进行校验
            if (!string.IsNullOrEmpty(task.Hash))
            {
                var currentHash = Hash.FileHash(task.Path, task.HashInfo);
                if (currentHash != task.Hash)
                {
                    File.Delete(task.Path);
                    return true;
                }
            }
            //Logger.Info(nameof(DownLoadCore), $"{task.Url}已存在，无需下载");//临时
            AlldownLoadTasks[task] = "exist";
            return false;
        }

        /// <summary>
        /// 处理单个任务组的下载
        /// </summary>
        private async Task ProcessTaskGroupAsync(List<DownLoadTask> tasks)
        {
            // 每个任务组使用独立的HttpClient实例
            using var httpClient = new HttpClient();
            foreach (var task in tasks)
            {
                await ProcessSingleTaskAsync(httpClient, task);
            }
        }

        /// <summary>
        /// 处理单个下载任务（包含重试逻辑）
        /// </summary>
        private async Task ProcessSingleTaskAsync(HttpClient httpClient, DownLoadTask task)
        {
            const int maxRetries = 3;
            var tempPath = Path.Combine(data.PATH.TEMPDOEN, task.Id.ToString());

            try
            {
                for (int retryCount = 1; retryCount <= maxRetries; retryCount++)
                {
                    if (_cts.IsCancellationRequested) return;

                    if (await TryDownloadFileAsync(httpClient, task, tempPath))
                    {
                        if (ValidateAndMoveFile(task, tempPath))
                        {
                            _completedTasks.TryAdd(task, "true");
                            Logger.Info(nameof(DownLoadCore), $"{task.Url}下载成功");
                            UpdateProgress(task.Size);
                            return;
                        }
                    }

                    CleanupTempFile(tempPath);
                    await ApplyRetryDelay(retryCount, baseDelay: 1000);
                }

                // 所有重试失败后标记为失败
                MarkTaskFailed(task);
            }
            catch (Exception ex)
            {
                Logger.Error($"任务[{task.Id}]处理失败", ex.ToString());
                MarkTaskFailed(task);
            }
            finally
            {
                CleanupTempFile(tempPath);
            }
        }

        /// <summary>
        /// 尝试下载文件到临时路径
        /// </summary>
        private async Task<bool> TryDownloadFileAsync(
            HttpClient httpClient,
            DownLoadTask task,
            string tempPath)
        {
            try
            {
                using var response = await httpClient.GetAsync(
                    task.Url,
                    HttpCompletionOption.ResponseHeadersRead,
                    _cts.Token);

                response.EnsureSuccessStatusCode();

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(
                    tempPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 8192,
                    useAsync: true);

                await contentStream.CopyToAsync(fileStream, _cts.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 校验并移动文件到目标位置
        /// </summary>
        private bool ValidateAndMoveFile(DownLoadTask task, string tempPath)
        {
            try
            {
                // 哈希校验
                if (!string.IsNullOrEmpty(task.Hash))
                {
                    var actualHash = Hash.FileHash(tempPath, task.HashInfo);
                    if (actualHash != task.Hash) return false;
                }

                // 确保目标目录存在
                var targetDir = Path.GetDirectoryName(task.Path);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                File.Move(tempPath, task.Path, overwrite: true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 应用指数退避延迟（带随机抖动）
        /// </summary>
        /// <param name="retryCount">当前重试次数（从1开始）</param>
        /// <param name="baseDelay">基础延迟（毫秒）</param>
        private async Task ApplyRetryDelay(int retryCount, int baseDelay)
        {
            if (_cts.IsCancellationRequested) return;

            var maxDelay = 10000; // 10秒上限
            var delay = (int)Math.Min(baseDelay * Math.Pow(2, retryCount), maxDelay);
            var jitter = new Random().Next(200, 500);

            await Task.Delay(delay + jitter, _cts.Token);
        }

        /// <summary>
        /// 更新下载进度（线程安全）
        /// </summary>
        private void UpdateProgress(long downloadedBytes)
        {
            Interlocked.Add(ref _completedSize, downloadedBytes);
            //var progress = (int)(_completedSize / (double)_totalSize * 100);
            //DownloadProgressChanged?.Invoke(this, new DownloadProgressEventArgs($"[{_completedSize}]/[{_totalSize}]"));
            DownloadProgressChanged?.Invoke(this,new DownloadProgressEventArgs(_totalSize,_completedSize));
        }

        /// <summary>
        /// 标记任务失败
        /// </summary>
        private void MarkTaskFailed(DownLoadTask task)
        {
            _completedTasks.TryAdd(task, "false");
            //OnAllDownloadProgressChanged();
        }

        /// <summary>
        /// 清理临时文件
        /// </summary>
        private void CleanupTempFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                Logger.Warning($"临时文件清理失败: {path}", ex.Message);
            }
        }

        #endregion

        #region 事件触发方法

        private void OnAllDownloadProgressChanged()
        {
            //Logger.Info(nameof(DownLoadCore), $"通知下载完成");
            Onall = true;
            AllDownloadProgressChanged?.Invoke(this, new AllDownloadProgressEventArgs(AlldownLoadTasks,_completedTasks));
            //Logger.Info(nameof(DownLoadCore), $"通知完成");
        }

        #endregion

        #region 任务分配算法

        /// <summary>
        /// 基于最小负载的任务分配算法
        /// </summary>
        /// <param name="tasks">需要分配的任务列表</param>
        /// <param name="maxConcurrency">最大并发组数</param>
        /// <returns>平衡后的任务组列表（按负载升序排列）</returns>
        private List<(long totalSize, List<DownLoadTask> tasks)> BalanceTaskLoad(
            List<DownLoadTask> tasks,
            int maxConcurrency)
        {
            // 初始化优先队列（最小堆）
            var priorityQueue = new PriorityQueue<DownloadGroup, long>();
            for (int i = 0; i < maxConcurrency; i++)
            {
                priorityQueue.Enqueue(new DownloadGroup(), 0);
            }

            // 按任务大小降序排列
            foreach (var task in tasks.OrderByDescending(t => t.Size))
            {
                Interlocked.Add(ref _totalSize, task.Size);
                var group = priorityQueue.Dequeue();
                group.AddTask(task);
                priorityQueue.Enqueue(group, group.TotalSize);
            }

            // 提取结果并排序
            var result = new List<DownloadGroup>();
            while (priorityQueue.Count > 0)
            {
                result.Add(priorityQueue.Dequeue());
            }

            return result
                .OrderBy(g => g.TotalSize)
                .Select(g => (g.TotalSize, g.Tasks))
                .ToList();
        }

        /// <summary>
        /// 下载任务组（内部辅助类）
        /// </summary>
        private class DownloadGroup
        {
            public long TotalSize { get; private set; }
            public List<DownLoadTask> Tasks { get; } = new();

            public void AddTask(DownLoadTask task)
            {
                Tasks.Add(task);
                TotalSize += task.Size;
            }
        }

        #endregion
    }

    /// <summary>
    /// 下载进度事件参数
    /// </summary>
    public class DownloadProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 当前下载进度格式化的
        /// </summary>
        public string ProgressPercentage { get; }

        public DownloadProgressEventArgs(string progress)
        {
            ProgressPercentage = progress;
        }
        public long Longsize { get; }
        public long Size { get; }
        public DownloadProgressEventArgs(long longsize, long size)
        {
            Longsize = longsize;
            Size = size;
            ProgressPercentage = $"[{size}]/[{longsize}]";
        }
    }
    /// <summary>
    /// 下载完成事件参数
    /// </summary>
    public class AllDownloadProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 任务完成情况，true完成，false未完成，null未开始，exist存在无需下载
        /// </summary>
        public Dictionary<DownLoadTask,string> DownloadTasks{ get; }
        public AllDownloadProgressEventArgs(Dictionary<DownLoadTask,string> All, ConcurrentDictionary<DownLoadTask, string> completedTasks)
        {
            //Logger.Info(nameof(AllDownloadProgressEventArgs), $"初始化");
            DownloadTasks = All;
            var down = completedTasks.ToDictionary();
            foreach (var item in down)
            {
                DownloadTasks[item.Key] = item.Value;
            }
            //Logger.Info("cs","cs");
            /////这玩意最开始版本就是狗屎，硬生生的把这个简单的处理干成几分钟跑不出来，搞得我以为是我下载写有问题，没想到是这个吊玩意搞得，气死我了    ___*( ￣皿￣)/#____        (╯°□°）╯︵ ┻━┻
        }
    }
}

