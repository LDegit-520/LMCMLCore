using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LMCMLCore.CORE.data;

namespace LMCMLCore.CORE.LOGGER
{

    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug,
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
    /// <summary>
    /// 日志记录器
    /// </summary>
    public static class Logger
    {
        private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private static readonly object _fileLock = new object();
        private static string _logDirectory = PATH.LOG; // 默认日志文件夹
        private static string _logFileName = "log.log"; // 最新日志文件名
        private static int _maxLogFiles = 10; // 最大日志文件数量
        private static long _maxLogFileSize = 10 * 1024 * 1024; // 单个日志文件最大大小（默认 1MB）
        private static int _batchSize = 10; // 批量写入的大小
        private static Timer _timer; // 定时器，用于定期写入日志

        static Logger()
        {
            // 确保日志目录存在
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            // 初始化定时器，每隔 5 秒触发一次批量写入
            _timer = new Timer(FlushLogs, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            // 注册程序退出事件
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnCancelKeyPress;
        }
        // 程序退出时刷新日志
        private static void OnProcessExit(object sender, EventArgs e)
        {
            FlushLogs();
        }

        // 控制台按下 Ctrl+C 时刷新日志
        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            FlushLogs();
        }

        // 设置日志文件夹（可选）
        public static void SetLogDirectory(string directory)
        {
            _logDirectory = directory;
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        // 设置最大日志文件数量（可选）
        public static void SetMaxLogFiles(int maxLogFiles)
        {
            _maxLogFiles = maxLogFiles;
        }

        // 设置单个日志文件最大大小（可选）
        public static void SetMaxLogFileSize(long maxLogFileSize)
        {
            _maxLogFileSize = maxLogFileSize;
        }

        // 设置批量写入的大小（可选）
        public static void SetBatchSize(int batchSize)
        {
            _batchSize = batchSize;
        }

        // 记录日志
        public static void Log(LogLevel level, string sourceClass, string message)
        {
            string logMessage = FormatLogMessage(level, sourceClass, message);

            // 输出到控制台
            //Console.WriteLine(logMessage);

            // 将日志消息加入队列
            _logQueue.Enqueue(logMessage);

            // 如果队列大小达到批量写入的大小，立即触发写入
            if (_logQueue.Count >= _batchSize)
            {
                Task.Run(() => FlushLogs());
            }
        }

        // 格式化日志消息
        private static string FormatLogMessage(LogLevel level, string sourceClass, string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{sourceClass}] [{level}] [{message}]";
        }

        // 批量写入日志到文件
        private static void FlushLogs(object state = null)
        {
            lock (_fileLock)
            {
                try
                {
                    // 检查当前日志文件大小
                    string currentLogFilePath = Path.Combine(_logDirectory, _logFileName);
                    if (File.Exists(currentLogFilePath) && new FileInfo(currentLogFilePath).Length >= _maxLogFileSize)
                    {
                        RotateLogFiles();
                    }

                    // 写入日志
                    using (StreamWriter writer = new StreamWriter(currentLogFilePath, true))
                    {
                        while (_logQueue.TryDequeue(out string logMessage))
                        {
                            writer.WriteLine(logMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }
        }

        // 日志文件轮转
        private static void RotateLogFiles()
        {
            try
            {
                // 获取所有日志文件，按数字后缀排序
                var logFiles = Directory.GetFiles(_logDirectory, "log*.log")
                                        .OrderByDescending(f => GetLogFileNumber(f))
                                        .ToList();

                // 删除最旧的文件（如果超过最大数量）
                if (logFiles.Count >= _maxLogFiles)
                {
                    var oldestFile = logFiles.Last();
                    File.Delete(oldestFile);
                    Console.WriteLine($"Deleted old log file: {oldestFile}");
                    logFiles.Remove(oldestFile);
                }

                // 重命名文件，依次加一
                foreach (var file in logFiles)
                {
                    int fileNumber = GetLogFileNumber(file);
                    string newFilePath = Path.Combine(_logDirectory, $"log{fileNumber + 1}.log");
                    File.Move(file, newFilePath);
                    Console.WriteLine($"Renamed log file: {file} -> {newFilePath}");
                }

                // 将当前日志文件重命名为 log1.log
                string currentLogFilePath = Path.Combine(_logDirectory, _logFileName);
                string newLogFilePath = Path.Combine(_logDirectory, "log1.log");
                File.Move(currentLogFilePath, newLogFilePath);
                Console.WriteLine($"Renamed current log file: {currentLogFilePath} -> {newLogFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to rotate log files: {ex.Message}");
            }
        }

        // 获取日志文件的数字后缀
        private static int GetLogFileNumber(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName == "log")
            {
                return 0;
            }
            if (int.TryParse(fileName.Substring(3), out int number))
            {
                return number;
            }
            return 0;
        }

        // 快捷方法
        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="sourceClass">调用者</param>
        /// <param name="message">消息</param>
        public static void Debug(string sourceClass, string message) => Log(LogLevel.Debug, sourceClass, message);
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="sourceClass">调用者</param>
        /// <param name="message">消息</param>
        public static void Info(string sourceClass, string message) => Log(LogLevel.Info, sourceClass, message);
        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="sourceClass">调用者</param>
        /// <param name="message">消息</param>
        public static void Warning(string sourceClass, string message) => Log(LogLevel.Warning, sourceClass, message);
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="sourceClass">调用者</param>
        /// <param name="message">消息</param>
        public static void Error(string sourceClass, string message) => Log(LogLevel.Error, sourceClass, message);
    }
}
