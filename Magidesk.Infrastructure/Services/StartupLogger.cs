using System;
using System.IO;

namespace Magidesk.Infrastructure.Services;

public static class StartupLogger
{
    private static readonly string LogPath;

    static StartupLogger()
    {
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDir = Path.Combine(appData, "Magidesk", "Logs");
            Directory.CreateDirectory(logDir);
            LogPath = Path.Combine(logDir, "startup_trace.txt");
        }
        catch
        {
            LogPath = string.Empty;
        }
    }

    public static void Log(string message)
    {
        if (string.IsNullOrEmpty(LogPath)) return;
        try
        {
            File.AppendAllText(LogPath, $"{DateTime.Now:HH:mm:ss.fff}: {message}\n");
        }
        catch { }
    }
}
