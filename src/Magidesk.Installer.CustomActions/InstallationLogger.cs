using System;
using System.IO;
using System.Text;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Structured logging for installation operations.
/// Writes timestamped log entries to %TEMP%\MagideskInstall\install_YYYY-MM-DD_HHmmss.log
/// </summary>
public class InstallationLogger : IDisposable
{
    private readonly string _logFilePath;
    private readonly StreamWriter _writer;
    private readonly object _lock = new();
    private bool _disposed;

    public InstallationLogger()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "MagideskInstall");
        Directory.CreateDirectory(tempDir);

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        _logFilePath = Path.Combine(tempDir, $"install_{timestamp}.log");

        _writer = new StreamWriter(_logFilePath, append: false, Encoding.UTF8)
        {
            AutoFlush = true
        };

        LogInfo("Installation", "Installation started", null);
    }

    public string LogFilePath => _logFilePath;

    public void LogDebug(string phase, string message, object? context = null)
    {
        WriteLog(LogLevel.Debug, phase, message, context);
    }

    public void LogInfo(string phase, string message, object? context = null)
    {
        WriteLog(LogLevel.Info, phase, message, context);
    }

    public void LogWarning(string phase, string message, object? context = null)
    {
        WriteLog(LogLevel.Warning, phase, message, context);
    }

    public void LogError(string phase, string message, object? context = null)
    {
        WriteLog(LogLevel.Error, phase, message, context);
    }

    public void LogCritical(string phase, string message, object? context = null)
    {
        WriteLog(LogLevel.Critical, phase, message, context);
    }

    private void WriteLog(LogLevel level, string phase, string message, object? context)
    {
        if (_disposed) return;

        lock (_lock)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logEntry = $"[{timestamp}] [{level,-8}] [{phase,-20}] {message}";

            _writer.WriteLine(logEntry);

            if (context != null)
            {
                _writer.WriteLine($"  Context: {context}");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        lock (_lock)
        {
            LogInfo("Installation", "Installation log closed", null);
            _writer?.Dispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}
