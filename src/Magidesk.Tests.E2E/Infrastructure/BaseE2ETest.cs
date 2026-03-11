using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using Magidesk.Tests.Workflows.Infrastructure;
using System.Diagnostics;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Base class for E2E tests. Handles application launch, database reset, test execution tracking, and cleanup.
/// </summary>
public abstract class BaseE2ETest : IDisposable
{
    private readonly ConfigurationManager _config;
    private readonly DatabaseResetEngine _dbResetEngine;
    private readonly FailureCaptureSystem _failureCapture;
    private readonly TestExecutionTracker? _testTracker;
    private readonly ITestOutputHelper? _output;
    private bool _testFailed;
    private Exception? _testException;
    private Guid _executionId;

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
        
        // Initialize test execution tracker
        try
        {
            _testTracker = new TestExecutionTracker(_config.DatabaseConnectionString);
        }
        catch (Exception ex)
        {
            // Log warning but don't fail test if tracker initialization fails
            _output?.WriteLine($"Warning: Failed to initialize test execution tracker: {ex.Message}");
            _testTracker = null;
        }

        // Setup runs before each test
        Setup();
    }

    private void Setup()
    {
        try
        {
            // Start test execution tracking
            if (_testTracker != null)
            {
                var (testName, category, priority) = GetTestMetadata();
                _executionId = _testTracker.StartTestExecutionAsync(testName, category, priority)
                    .GetAwaiter()
                    .GetResult();
            }

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
    /// Extracts test metadata (name, category, priority) from the calling test method using reflection.
    /// </summary>
    private (string testName, string category, string priority) GetTestMetadata()
    {
        // Get the stack trace to find the test method
        var stackTrace = new StackTrace();
        MethodBase? testMethod = null;

        // Walk up the stack to find a method with [Fact] or [Theory] attribute
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            var method = frame?.GetMethod();
            
            if (method != null)
            {
                var factAttribute = method.GetCustomAttribute<FactAttribute>();
                var theoryAttribute = method.GetCustomAttribute<TheoryAttribute>();
                
                if (factAttribute != null || theoryAttribute != null)
                {
                    testMethod = method;
                    break;
                }
            }
        }

        // Default values if test method not found
        string testName = GetType().Name;
        string category = "Unknown";
        string priority = "P2";

        if (testMethod != null)
        {
            // Get test name from method
            testName = $"{testMethod.DeclaringType?.Name}.{testMethod.Name}";

            // Extract category from [Trait("Category", "...")] attributes
            var traitAttributes = testMethod.GetCustomAttributes<TraitAttribute>();
            foreach (var trait in traitAttributes)
            {
                if (trait.Name.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    category = trait.Value;
                }
                else if (trait.Name.Equals("Priority", StringComparison.OrdinalIgnoreCase))
                {
                    priority = trait.Value;
                }
            }
        }

        return (testName, category, priority);
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

        // Walk up from the test assembly directory to find the 'src' folder
        // (depth varies by platform: x86 = 5 levels, x64/AnyCPU = 4 levels)
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && currentDir.Name != "src")
            currentDir = currentDir.Parent;

        if (currentDir == null)
        {
            throw new FileNotFoundException(
                "Could not locate the 'src' directory by walking up from the test assembly location. " +
                "Ensure the test project is in the correct location relative to the Presentation project.");
        }

        var presentationBinPath = Path.Combine(currentDir.FullName, "Magidesk.Presentation", "bin");
        const string tfm = "net8.0-windows10.0.19041.0";

        // Probe in priority order: x86 → x64 → any-CPU → legacy path
        var possiblePaths = new[]
        {
            Path.Combine(presentationBinPath, "x86", "Debug", tfm, "win-x86", "Magidesk.Presentation.exe"),
            Path.Combine(presentationBinPath, "Debug", tfm, "win-x64", "Magidesk.Presentation.exe"),
            Path.Combine(presentationBinPath, "x64", "Debug", tfm, "win-x64", "Magidesk.Presentation.exe"),
            Path.Combine(presentationBinPath, "Debug", tfm, "Magidesk.Presentation.exe"),
            Path.Combine(presentationBinPath, "Debug", "net8.0-windows", "Magidesk.Presentation.exe"),
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
            // Complete test execution tracking
            if (_testTracker != null && _executionId != Guid.Empty)
            {
                try
                {
                    var result = _testFailed ? TestResult.Failed : TestResult.Passed;
                    var failureReason = _testException != null
                        ? $"{_testException.Message}\n---\n{_testException.StackTrace}"
                        : null;

                    _testTracker.CompleteTestExecutionAsync(_executionId, result, failureReason)
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    // Log warning but don't fail test if tracker completion fails
                    _output?.WriteLine($"Warning: Failed to complete test execution tracking: {ex.Message}");
                }
            }

            // Capture failure artifacts if test failed
            if (_testFailed && _testException != null)
            {
                var testName = GetType().Name;
                _failureCapture.CaptureFailureArtifacts(
                    testName,
                    _testException,
                    MainWindow,
                    _config.DatabaseConnectionString,
                    _executionId);
            }

            // Dispose application launcher
            Launcher?.Dispose();
        }
    }
}
