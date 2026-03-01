using FlaUI.Core;
using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Base class for E2E tests. Handles application launch, database reset, and cleanup.
/// </summary>
public abstract class BaseE2ETest : IDisposable
{
    protected ApplicationLauncher? Launcher { get; private set; }
    protected Application? App { get; private set; }
    protected Window? MainWindow { get; private set; }

    protected BaseE2ETest()
    {
        // Setup runs before each test
        Setup();
    }

    private void Setup()
    {
        // Reset database before each test
        ResetDatabase();

        // Resolve executable path relative to test assembly
        var executablePath = ResolveExecutablePath();

        // Launch application
        Launcher = new ApplicationLauncher(executablePath);
        App = Launcher.Launch();

        // Wait for main window with timeout
        MainWindow = Launcher.GetMainWindow(TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Resets the database to a clean state before each test.
    /// Override this method to implement database reset logic.
    /// </summary>
    protected virtual void ResetDatabase()
    {
        // TODO: Implement database reset logic
        // This could call a script, use EF Core migrations, or restore a snapshot
    }

    /// <summary>
    /// Resolves the path to the Magidesk.Presentation executable.
    /// </summary>
    private static string ResolveExecutablePath()
    {
        // Get the directory of the test assembly
        var testAssemblyPath = AppContext.BaseDirectory;
        
        // Navigate up to src directory and then to Magidesk.Presentation
        var srcDirectory = Path.GetFullPath(Path.Combine(testAssemblyPath, "..", "..", "..", ".."));
        var presentationBinPath = Path.Combine(srcDirectory, "Magidesk.Presentation", "bin", "Debug", "net8.0-windows");
        
        // Look for the executable
        var exePath = Path.Combine(presentationBinPath, "Magidesk.Presentation.exe");
        
        if (!File.Exists(exePath))
        {
            throw new FileNotFoundException(
                $"Magidesk.Presentation.exe not found at expected path: {exePath}. " +
                "Please ensure the Presentation project has been built in Debug configuration.");
        }

        return exePath;
    }

    /// <summary>
    /// Waits for a condition to be true with a timeout.
    /// </summary>
    protected static void WaitUntil(Func<bool> condition, TimeSpan timeout, string? errorMessage = null)
    {
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            if (condition())
                return;

            Task.Delay(100).Wait();
        }

        throw new TimeoutException(errorMessage ?? $"Condition was not met within {timeout.TotalSeconds} seconds.");
    }

    /// <summary>
    /// Waits for an element to exist with a timeout.
    /// </summary>
    protected static T WaitForElement<T>(Func<T?> elementGetter, TimeSpan timeout) where T : class
    {
        T? element = null;
        WaitUntil(
            () => (element = elementGetter()) != null,
            timeout,
            "Element did not appear within the timeout period.");
        
        return element!;
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
            Launcher?.Dispose();
        }
    }
}
