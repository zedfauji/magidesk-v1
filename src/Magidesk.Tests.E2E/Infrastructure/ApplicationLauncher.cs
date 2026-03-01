using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Magidesk.Tests.E2E.Infrastructure.Exceptions;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Manages the lifecycle of the Magidesk.Presentation.exe process for E2E testing.
/// Handles application launch, main window detection, and process termination.
/// </summary>
public sealed class ApplicationLauncher : IDisposable
{
    private const int MainWindowTimeoutSeconds = 30;
    private const int GracefulExitTimeoutSeconds = 5;

    private readonly string _executablePath;
    private Process? _process;
    private Application? _application;
    private Window? _mainWindow;
    private bool _isLaunched;
    private bool _disposed;

    /// <summary>
    /// Gets the FlaUI Application instance.
    /// </summary>
    public Application? Application => _application;

    /// <summary>
    /// Gets the main window of the application.
    /// </summary>
    public Window? Window => _mainWindow;

    /// <summary>
    /// Initializes a new instance of the ApplicationLauncher class.
    /// </summary>
    /// <param name="executablePath">Path to the Magidesk.Presentation.exe executable.</param>
    /// <exception cref="ArgumentException">Thrown when executable path is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when executable file does not exist.</exception>
    public ApplicationLauncher(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
            throw new ArgumentException("Executable path cannot be null or empty.", nameof(executablePath));

        if (!File.Exists(executablePath))
            throw new FileNotFoundException($"Executable not found at path: {executablePath}", executablePath);

        _executablePath = executablePath;
    }

    /// <summary>
    /// Starts the Magidesk.Presentation.exe process and returns the FlaUI Application instance.
    /// </summary>
    /// <returns>The FlaUI Application instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when Launch() is called more than once.</exception>
    /// <exception cref="ApplicationLaunchException">Thrown when the process fails to start.</exception>
    public Application Launch()
    {
        if (_isLaunched)
            throw new InvalidOperationException("Launch() has already been called. Cannot launch the application twice.");

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _executablePath,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(_executablePath)
            };

            _process = Process.Start(startInfo);

            if (_process == null || _process.HasExited)
            {
                throw new ApplicationLaunchException(
                    $"Failed to start process. The process exited immediately or could not be started.",
                    _executablePath);
            }

            _application = FlaUI.Core.Application.Attach(_process.Id);
            _isLaunched = true;

            return _application;
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not ApplicationLaunchException)
        {
            // Clean up if launch failed
            CleanupProcess();
            throw new ApplicationLaunchException(
                $"Failed to launch application at path: {_executablePath}. {ex.Message}",
                _executablePath);
        }
    }

    /// <summary>
    /// Waits for the main window to appear within the specified timeout.
    /// </summary>
    /// <param name="timeout">Maximum time to wait for the main window.</param>
    /// <returns>The main window.</returns>
    /// <exception cref="InvalidOperationException">Thrown when Launch() has not been called.</exception>
    /// <exception cref="TimeoutException">Thrown when the main window does not appear within the timeout.</exception>
    public Window GetMainWindow(TimeSpan timeout)
    {
        if (!_isLaunched || _application == null)
            throw new InvalidOperationException("Launch() must be called before GetMainWindow().");

        try
        {
            // Poll for main window with 100ms intervals
            var endTime = DateTime.UtcNow.Add(timeout);

            while (DateTime.UtcNow < endTime)
            {
                try
                {
                    var windows = _application.GetAllTopLevelWindows(new UIA3Automation());
                    
                    // Look for the main window (typically the first visible window)
                    _mainWindow = windows.FirstOrDefault(w => w.IsAvailable && !string.IsNullOrEmpty(w.Title));
                    
                    if (_mainWindow != null)
                        return _mainWindow;
                }
                catch
                {
                    // Window not ready yet, continue polling
                }

                // Check if process has exited
                if (_process?.HasExited == true)
                {
                    throw new TimeoutException(
                        $"Application process exited before main window appeared. " +
                        $"Exit code: {_process.ExitCode}. " +
                        $"Executable: {_executablePath}");
                }

                Task.Delay(100).Wait();
            }

            // Timeout occurred
            throw new TimeoutException(
                $"Main window did not appear within {timeout.TotalSeconds:F1} seconds. " +
                $"Executable: {_executablePath}. " +
                $"Ensure the application is built and can start successfully.");
        }
        catch (TimeoutException)
        {
            // Clean up and re-throw
            CleanupProcess();
            throw;
        }
    }

    /// <summary>
    /// Disposes the ApplicationLauncher, forcefully terminating the application process.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        CleanupProcess();
        _disposed = true;
    }

    /// <summary>
    /// Cleans up the application process and ensures no orphaned processes remain.
    /// </summary>
    private void CleanupProcess()
    {
        try
        {
            // Clear FlaUI resources (Window doesn't implement IDisposable)
            _mainWindow = null;

            _application?.Dispose();
            _application = null;

            // Terminate the process
            if (_process != null && !_process.HasExited)
            {
                try
                {
                    // Try graceful exit first
                    _process.CloseMainWindow();
                    
                    if (!_process.WaitForExit(GracefulExitTimeoutSeconds * 1000))
                    {
                        // Force kill if graceful exit fails
                        _process.Kill();
                        _process.WaitForExit();
                    }
                }
                catch
                {
                    // If graceful exit fails, force kill
                    try
                    {
                        if (!_process.HasExited)
                        {
                            _process.Kill();
                            _process.WaitForExit();
                        }
                    }
                    catch
                    {
                        // Process already exited or cannot be killed
                    }
                }
            }

            _process?.Dispose();
            _process = null;

            // Check for orphaned processes
            CheckForOrphanedProcesses();
        }
        catch
        {
            // Suppress exceptions during cleanup
        }
    }

    /// <summary>
    /// Checks for and terminates any orphaned Magidesk.Presentation processes.
    /// </summary>
    private void CheckForOrphanedProcesses()
    {
        try
        {
            var processName = Path.GetFileNameWithoutExtension(_executablePath);
            var orphanedProcesses = Process.GetProcessesByName(processName)
                .Where(p => p.MainModule?.FileName?.Equals(_executablePath, StringComparison.OrdinalIgnoreCase) == true);

            foreach (var orphan in orphanedProcesses)
            {
                try
                {
                    if (!orphan.HasExited)
                    {
                        orphan.Kill();
                        orphan.WaitForExit();
                    }
                    orphan.Dispose();
                }
                catch
                {
                    // Process already exited or cannot be killed
                }
            }
        }
        catch
        {
            // Suppress exceptions during orphan cleanup
        }
    }

    /// <summary>
    /// Resolves the executable path from environment variable or relative path.
    /// </summary>
    /// <returns>The resolved executable path.</returns>
    /// <exception cref="FileNotFoundException">Thrown when executable cannot be found.</exception>
    public static string ResolveExecutablePath()
    {
        // Check environment variable first
        var envPath = Environment.GetEnvironmentVariable("MAGIDESK_APP_PATH");
        if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
            return envPath;

        // Fall back to relative path
        var testAssemblyDir = AppContext.BaseDirectory;
        
        // Navigate up to src/ directory (typically 4 levels up from test assembly)
        var currentDir = new DirectoryInfo(testAssemblyDir);
        
        // Go up until we find the src directory or reach the root
        while (currentDir != null && currentDir.Name != "src")
        {
            currentDir = currentDir.Parent;
        }

        if (currentDir == null)
        {
            throw new FileNotFoundException(
                "Could not locate src directory. Ensure the test project is in the correct location relative to the Presentation project.");
        }

        // Build path to Presentation executable
        var exePath = Path.Combine(
            currentDir.FullName,
            "Magidesk.Presentation",
            "bin",
            "Debug",
            "net8.0-windows10.0.19041.0",
            "win-x64",
            "Magidesk.Presentation.exe");

        if (!File.Exists(exePath))
        {
            // Try without platform-specific subdirectory
            exePath = Path.Combine(
                currentDir.FullName,
                "Magidesk.Presentation",
                "bin",
                "Debug",
                "net8.0-windows10.0.19041.0",
                "Magidesk.Presentation.exe");
        }

        if (!File.Exists(exePath))
        {
            throw new FileNotFoundException(
                $"Magidesk.Presentation.exe not found at expected location: {exePath}. " +
                "Ensure the Presentation project is built in Debug configuration. " +
                "Alternatively, set the MAGIDESK_APP_PATH environment variable to the executable path.",
                exePath);
        }

        return exePath;
    }
}

