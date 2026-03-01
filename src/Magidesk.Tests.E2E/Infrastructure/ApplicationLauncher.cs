using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.Diagnostics;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Launches the Magidesk.Presentation executable and provides FlaUI automation access.
/// </summary>
public sealed class ApplicationLauncher : IDisposable
{
    private readonly string _executablePath;
    private Process? _process;
    private Application? _application;
    private UIA3Automation? _automation;

    public ApplicationLauncher(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
            throw new ArgumentException("Executable path cannot be null or empty.", nameof(executablePath));

        if (!File.Exists(executablePath))
            throw new FileNotFoundException($"Magidesk executable not found at: {executablePath}", executablePath);

        _executablePath = executablePath;
    }

    /// <summary>
    /// Launches the application and returns the FlaUI Application instance.
    /// </summary>
    public Application Launch()
    {
        if (_application != null)
            throw new InvalidOperationException("Application is already running.");

        _automation = new UIA3Automation();
        
        var processStartInfo = new ProcessStartInfo(_executablePath)
        {
            UseShellExecute = false
        };
        
        _process = Process.Start(processStartInfo);
        if (_process == null)
            throw new InvalidOperationException($"Failed to start process: {_executablePath}");

        _application = Application.Attach(_process);

        return _application;
    }

    /// <summary>
    /// Gets the main window of the application with retry logic.
    /// </summary>
    public Window GetMainWindow(TimeSpan timeout)
    {
        if (_application == null)
            throw new InvalidOperationException("Application has not been launched.");

        var endTime = DateTime.UtcNow.Add(timeout);
        
        while (DateTime.UtcNow < endTime)
        {
            try
            {
                var mainWindow = _application.GetMainWindow(_automation);
                if (mainWindow != null)
                    return mainWindow;
            }
            catch
            {
                // Window not ready yet
            }

            Task.Delay(100).Wait();
        }

        throw new TimeoutException($"Main window did not appear within {timeout.TotalSeconds} seconds.");
    }

    public void Dispose()
    {
        try
        {
            _application?.Close();
            
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.WaitForExit(5000);
            }
        }
        catch
        {
            // Best effort cleanup
        }
        finally
        {
            _process?.Dispose();
            _automation?.Dispose();
        }
    }
}
