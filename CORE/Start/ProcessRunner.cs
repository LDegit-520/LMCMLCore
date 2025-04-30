using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Start
{


    /// <summary>
    /// 跨平台进程管理器（支持Windows/Linux/macOS）
    /// </summary>
    public sealed class ProcessRunner : IDisposable
    {
        #region 常量与字段
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_ACCESS_DENIED = 5;

        private readonly Process _process;
        private readonly StringBuilder _outputBuffer = new StringBuilder();
        private readonly StringBuilder _errorBuffer = new StringBuilder();
        private readonly object _bufferLock = new object();
        private bool _isDisposed=false;
        #endregion

        #region 属性
        /// <summary>
        /// 标准输出内容
        /// </summary>
        public string StandardOutput
        {
            get { lock (_bufferLock) { return _outputBuffer.ToString(); } }
        }

        /// <summary>
        /// 错误输出内容
        /// </summary>
        public string StandardError
        {
            get { lock (_bufferLock) { return _errorBuffer.ToString(); } }
        }

        /// <summary>
        /// 进程是否正在运行
        /// </summary>
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// 进程退出代码（仅在退出后有效）
        /// </summary>
        public int ExitCode => _process.HasExited ? _process.ExitCode : -1;
        #endregion

        #region 事件
        /// <summary>
        /// 标准输出接收事件
        /// </summary>
        public event EventHandler<string> OutputReceived;

        /// <summary>
        /// 错误输出接收事件
        /// </summary>
        public event EventHandler<string> ErrorReceived;

        /// <summary>
        /// 进程退出事件
        /// </summary>
        public event EventHandler Exited;
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建进程管理器实例
        /// </summary>
        /// <param name="executable">可执行文件或命令（也就时java.exe）</param>
        /// <param name="arguments">启动参数</param>
        /// <param name="workingDirectory">工作目录</param>
        /// <param name="customAppDataPath">自定义APPDATA路径</param>
        /// <param name="iswindows">是否隐藏命令行</param>
        public ProcessRunner(
            string executable,
            string arguments = "",
            string customAppDataPath = "",
            string workingDirectory = null,
            bool iswindows = true)
        {
            ValidateExecutable(executable);

            var startInfo = new ProcessStartInfo
            {
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                FileName = executable,
                Arguments = arguments,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = iswindows,
            };
            // 
            startInfo.EnvironmentVariables["APPDATA"] = customAppDataPath;

            // Windows系统特殊处理
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.Verb = "runas";
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            _process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _process.Exited += (s, e) => Exited?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 启动进程
        /// </summary>
        public void Start()
        {
            EnsureNotDisposed();

            if (IsRunning)
                throw new InvalidOperationException("Process is already running");

            try
            {
                _process.Start();
                IsRunning = true;
                BeginAsyncOutputReading();
            }
            catch (Exception ex) when (IsStartFailureException(ex))
            {
                throw CreatePlatformException(ex);
            }
        }

        /// <summary>
        /// 安全终止进程
        /// </summary>
        /// <param name="timeoutMs">等待退出的超时时间（毫秒）</param>
        public void Stop(int timeoutMs = 5000)
        {
            EnsureNotDisposed();
            if (!IsRunning) return;

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (!_process.CloseMainWindow())
                        _process.Kill();
                }
                else
                {
                    SendUnixTerminationSignal();
                }

                if (!_process.WaitForExit(timeoutMs))
                    throw new TimeoutException("Process termination timeout");
            }
            catch (InvalidOperationException) { /* 进程已退出 */ }
        }

        /// <summary>
        /// 向进程发送输入
        /// </summary>
        public void SendInput(string input)
        {
            EnsureNotDisposed();

            if (!IsRunning)
                throw new InvalidOperationException("Process is not running");

            try
            {
                _process.StandardInput.WriteLine(input);
                _process.StandardInput.Flush();
            }
            catch (IOException ex)
            {
                throw new ProcessOperationException("Input operation failed", ex);
            }
        }

        /// <summary>
        /// 等待进程退出
        /// </summary>
        public bool WaitForExit(int timeoutMs = -1)
        {
            EnsureNotDisposed();
            return _process.WaitForExit(timeoutMs);
        }
        #endregion

        #region 私有方法
        private void ValidateExecutable(string executable)
        {
            if (string.IsNullOrWhiteSpace(executable))
                throw new ArgumentException("Executable path cannot be empty");

            if (File.Exists(executable)) return;
            if (ExistsInPath(executable)) return;

            throw new FileNotFoundException($"Executable not found: {executable}");
        }

        private bool ExistsInPath(string fileName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return false;

            var pathVar = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(pathVar)) return false;

            foreach (var path in pathVar.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return true;
            }
            return false;
        }

        private void BeginAsyncOutputReading()
        {
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            _process.OutputDataReceived += (s, e) => AppendOutput(e.Data);
            _process.ErrorDataReceived += (s, e) => AppendError(e.Data);
        }

        private void AppendOutput(string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            lock (_bufferLock)
            {
                _outputBuffer.AppendLine(data);
            }
            OutputReceived?.Invoke(this, data);
        }

        private void AppendError(string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            lock (_bufferLock)
            {
                _errorBuffer.AppendLine(data);
            }
            ErrorReceived?.Invoke(this, data);
        }

        private void SendUnixTerminationSignal()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                try
                {
                    using var killProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "kill",
                            Arguments = $"-TERM {_process.Id}",
                            UseShellExecute = false
                        }
                    };
                    killProcess.Start();
                    killProcess.WaitForExit(1000);
                }
                catch { /* 回退到强制终止 */ }
            }

            if (!_process.HasExited)
                _process.Kill();
        }

        private bool IsStartFailureException(Exception ex)
        {
            return ex is Win32Exception ||
                   ex is FileNotFoundException ||
                   ex is DirectoryNotFoundException ||
                   ex is PlatformNotSupportedException;
        }

        private ProcessOperationException CreatePlatformException(Exception ex)
        {
            var message = ex switch
            {
                Win32Exception winEx => GetWindowsErrorMessage(winEx),
                FileNotFoundException fnfEx => $"File not found: {fnfEx.FileName}",
                _ => ex.Message
            };

            return new ProcessOperationException($"Process start failed: {message}", ex);
        }

        private string GetWindowsErrorMessage(Win32Exception ex)
        {
            return ex.NativeErrorCode switch
            {
                ERROR_FILE_NOT_FOUND => "The system cannot find the file specified",
                ERROR_ACCESS_DENIED => "Access is denied",
                _ => ex.Message
            };
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
        #endregion

        #region 资源释放
        public void Dispose()
        {
            if (_isDisposed) return;

            try
            {
                if (IsRunning)
                {
                    try { Stop(); }
                    catch { /* 忽略终止异常 */ }
                }

                _process.Dispose();
            }
            finally
            {
                _isDisposed = true;
            }
        }
        #endregion
    }

    /// <summary>
    /// 进程操作异常
    /// </summary>
    public class ProcessOperationException : Exception
    {
        public ProcessOperationException(string message, Exception inner)
            : base(message, inner) { }
    }
}
