using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Base class for E2E tests. Handles application launch, database reset, and cleanup.
/// </summary>
public abstract class BaseE2ETest : IDisposable
{
    private readonly ConfigurationManager _config;
    private readonly DatabaseResetEngine _dbResetEngine;
    private readonly FailureCaptureSystem _failureCapture;
    private readonly ITestOutputHelper? _output;
    private bool _testFailed;
    private Exception? _testException;

    protected ApplicationLauncher? Launcher { get; private set; }
    protected Application? App { get; private set; }
    protected Window? MainWindow { get; private set; }

    protected BaseE2ETest(ITestOutputHelper? output = null)
    {
        _output = output;
        _config = ConfigurationManager.Load();
        _config.Validate();

        _dbResetEngine = new DatabaseResetEngine(_config.DatabaseConnectionString);
        _failureCapture = new FailureCaptureSystem(_config.ArtifactsDirectory);

        // Setup runs before each test
        Setup();
    }

    private void Setup()
    {
        try
        {
            // Reset database before each test
            ResetDatabase();

            // Resolve executable path
            var executablePath = ResolveExecutablePath();

            // Launch application
            Launcher = new ApplicationLauncher(executablePath);
            App = Launcher.Launch();

            // Wait for main window with timeout
            MainWindow = Launcher.GetMainWindow(TimeSpan.FromSeconds(30));
        }
        catch (Exception ex)
        {
            _testFailed = true;
            _testException = ex;
            throw;
        }
    }

    /// <summary>
    /// Resets the database to a clean state before each test.
    /// Override this method to implement custom database reset logic.
    /// </summary>
    protected virtual void ResetDatabase()
    {
        _dbResetEngine.ResetDatabase();
    }

    /// <summary>
    /// Marks the test as failed. Call this from catch blocks in derived test classes.
    /// </summary>
    protected void MarkTestFailed(Exception exception)
    {
        _testFailed = true;
        _testException = exception;
    }

    /// <summary>
    /// Resolves the path to the Magidesk.Presentation executable.
    /// </summary>
    private string ResolveExecutablePath()
    {
        // Check environment variable first
        var envPath = _config.ApplicationPath;
        if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
        {
            return envPath;
        }

        // Fall back to relative path
        var testAssemblyPath = AppContext.BaseDirectory;
        var srcDirectory = Path.GetFullPath(Path.Combine(testAssemblyPath, "..", "..", "..", ".."));
        var presentationBinDebugPath = Path.Combine(srcDirectory, "Magidesk.Presentation", "bin", "Debug");
        
        // Try net8.0-windows10.0.19041.0 first (full TFM), then fall back to net8.0-windows
        var possiblePaths = new[]
        {
            Path.Combine(presentationBinDebugPath, "net8.0-windows10.0.19041.0", "Magidesk.Presentation.exe"),
            Path.Combine(presentationBinDebugPath, "net8.0-windows", "Magidesk.Presentation.exe")
        };

        foreach (var exePath in possiblePaths)
        {
            if (File.Exists(exePath))
            {
                return exePath;
            }
        }

        throw new FileNotFoundException(
            $"Magidesk.Presentation.exe not found at any expected path. Tried: {string.Join(", ", possiblePaths)}. " +
            "Please ensure the Presentation project has been built in Debug configuration. " +
            "Alternatively, set the MAGIDESK_APP_PATH environment variable.");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Capture failure artifacts if test failed
            if (_testFailed && _testException != null)
            {
                var testName = GetType().Name;
                _failureCapture.CaptureFailureArtifacts(
                    testName,
                    _testException,
                    MainWindow,
                    _config.DatabaseConnectionString);
            }

            // Dispose application launcher
            Launcher?.Dispose();
        }
    }
}
