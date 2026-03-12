namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Configuration for E2E test execution.
/// </summary>
public class TestConfiguration
{
    public string DatabaseConnectionString { get; set; } = string.Empty;
    public string? ApplicationPath { get; set; }
    public double TimeoutMultiplier { get; set; } = 1.0;
    public string ArtifactsDirectory { get; set; } = "TestResults/";
    public int PropertyTestIterations { get; set; } = 100;
    public bool EnableTestTracking { get; set; } = true;
    public bool EnableParallelExecution { get; set; } = false;
    public int MaxParallelTests { get; set; } = 1;
    public string[] PriorityFilter { get; set; } = Array.Empty<string>();
    public string[] CategoryFilter { get; set; } = Array.Empty<string>();

    public static TestConfiguration LoadFromEnvironment()
    {
        return new TestConfiguration
        {
            DatabaseConnectionString = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION") 
                ?? "Host=localhost;Database=magidesk_test;Username=postgres;Password=postgres",
            ApplicationPath = Environment.GetEnvironmentVariable("TEST_APP_PATH"),
            TimeoutMultiplier = double.TryParse(Environment.GetEnvironmentVariable("TEST_TIMEOUT_MULTIPLIER"), 
                out var multiplier) ? multiplier : 1.0,
            EnableParallelExecution = bool.TryParse(Environment.GetEnvironmentVariable("TEST_PARALLEL"), 
                out var parallel) && parallel
        };
    }
}
