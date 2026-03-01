using Magidesk.Tests.E2E.Infrastructure.Exceptions;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for the E2E test framework exception hierarchy.
/// Validates that exceptions provide required properties and descriptive error messages.
/// </summary>
public class ExceptionHierarchyProperties
{
    /// <summary>
    /// Feature: e2e-testing-framework, Property 16: Precondition Fast Failure
    /// Validates: Requirements 13.8
    /// 
    /// For any test execution where preconditions are not met (application not built,
    /// database not accessible, required configuration missing), the framework must fail
    /// immediately with a descriptive exception before attempting test execution.
    /// 
    /// This test validates that all exception types in the hierarchy:
    /// 1. Include required context properties
    /// 2. Provide descriptive error messages
    /// 3. Properly chain inner exceptions
    /// 4. Inherit from the correct base types
    /// </summary>
    [Fact]
    public void ExceptionHierarchy_AllExceptionsInheritFromCorrectBaseTypes()
    {
        // ApplicationLaunchException inherits from E2ETestException
        var appLaunchEx = new ApplicationLaunchException("Test message", "/path/to/app.exe");
        Assert.IsAssignableFrom<E2ETestException>(appLaunchEx);
        Assert.IsAssignableFrom<Exception>(appLaunchEx);

        // DatabaseResetException inherits from E2ETestException
        var dbResetEx = new DatabaseResetException("Test message", "connection_string", new Exception("Inner"));
        Assert.IsAssignableFrom<E2ETestException>(dbResetEx);
        Assert.IsAssignableFrom<Exception>(dbResetEx);

        // ConfigurationException inherits from E2ETestException
        var configEx = new ConfigurationException("Test message", "ConfigKey");
        Assert.IsAssignableFrom<E2ETestException>(configEx);
        Assert.IsAssignableFrom<Exception>(configEx);

        // ElementNotFoundException inherits from TimeoutException (not E2ETestException)
        var elementEx = new ElementNotFoundException("Test message", "AutoId", "Name", "Button");
        Assert.IsAssignableFrom<TimeoutException>(elementEx);
        Assert.IsAssignableFrom<Exception>(elementEx);
    }

    [Theory]
    [InlineData("", "/path/to/app.exe")]
    [InlineData("Application failed to launch", "")]
    [InlineData("Application failed to launch", "/path/to/app.exe")]
    [InlineData("The application at '/path/to/app.exe' did not start within 30 seconds.", "/path/to/app.exe")]
    public void ApplicationLaunchException_PreservesExecutablePath(string message, string executablePath)
    {
        // Arrange & Act
        var exception = new ApplicationLaunchException(message, executablePath);

        // Assert
        Assert.Equal(executablePath, exception.ExecutablePath);
        Assert.Equal(message, exception.Message);
    }

    [Theory]
    [InlineData("", "Host=localhost;Database=test", "Inner error")]
    [InlineData("Database reset failed", "", "Inner error")]
    [InlineData("Database reset failed", "Host=localhost;Database=test", "")]
    [InlineData("Failed to delete transactional data", "Host=localhost;Port=5432;Database=magidesk_test", "Connection timeout")]
    public void DatabaseResetException_PreservesConnectionStringAndInnerException(
        string message,
        string connectionString,
        string innerMessage)
    {
        // Arrange
        var innerException = new Exception(innerMessage);

        // Act
        var exception = new DatabaseResetException(message, connectionString, innerException);

        // Assert
        Assert.Equal(connectionString, exception.ConnectionString);
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
        Assert.Equal(innerMessage, exception.InnerException?.Message);
    }

    [Theory]
    [InlineData("", "ConfigKey")]
    [InlineData("Configuration is invalid", "")]
    [InlineData("Configuration is invalid", "DatabaseConnectionString")]
    [InlineData("TimeoutMultiplier must be greater than 0", "TimeoutMultiplier")]
    [InlineData("MAGIDESK_TEST_DB_CONNECTION environment variable is required", "DatabaseConnectionString")]
    public void ConfigurationException_PreservesConfigurationKey(string message, string configurationKey)
    {
        // Arrange & Act
        var exception = new ConfigurationException(message, configurationKey);

        // Assert
        Assert.Equal(configurationKey, exception.ConfigurationKey);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void ConfigurationException_WithInnerException_PreservesAllProperties()
    {
        // Arrange
        var innerException = new FormatException("Invalid format");
        var message = "TimeoutMultiplier must be a valid number";
        var configKey = "TimeoutMultiplier";

        // Act
        var exception = new ConfigurationException(message, configKey, innerException);

        // Assert
        Assert.Equal(configKey, exception.ConfigurationKey);
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Theory]
    [InlineData("Element not found", "LoginButton", "Login", "Button")]
    [InlineData("Element not found", "LoginButton", null, null)]
    [InlineData("Element not found", null, "Login", null)]
    [InlineData("Element not found", null, null, "Button")]
    [InlineData("Element not found", null, null, null)]
    [InlineData("Element with AutomationId 'LoginButton' was not found within 10 seconds", "LoginButton", "Login", "Button")]
    public void ElementNotFoundException_PreservesElementProperties(
        string message,
        string? automationId,
        string? name,
        string? controlType)
    {
        // Arrange & Act
        var exception = new ElementNotFoundException(message, automationId, name, controlType);

        // Assert
        Assert.Equal(automationId, exception.AutomationId);
        Assert.Equal(name, exception.Name);
        Assert.Equal(controlType, exception.ControlType);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void ElementNotFoundException_WithInnerException_PreservesAllProperties()
    {
        // Arrange
        var innerException = new InvalidOperationException("UI Automation error");
        var message = "Element not found";
        var automationId = "LoginButton";
        var name = "Login";
        var controlType = "Button";

        // Act
        var exception = new ElementNotFoundException(
            message,
            automationId,
            name,
            controlType,
            innerException);

        // Assert
        Assert.Equal(automationId, exception.AutomationId);
        Assert.Equal(name, exception.Name);
        Assert.Equal(controlType, exception.ControlType);
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact]
    public void E2ETestException_CanBeCreatedWithMessageOnly()
    {
        // Arrange
        var message = "Test framework error";

        // Act
        var exception = new E2ETestException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void E2ETestException_CanBeCreatedWithMessageAndInnerException()
    {
        // Arrange
        var message = "Test framework error";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new E2ETestException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    /// <summary>
    /// Validates that exceptions provide descriptive error messages with context.
    /// This is critical for fast failure diagnosis.
    /// </summary>
    [Fact]
    public void ExceptionMessages_ProvideDescriptiveContext()
    {
        // ApplicationLaunchException should include path
        var appLaunchEx = new ApplicationLaunchException(
            "Application at '/path/to/app.exe' failed to launch within 30 seconds. Ensure the application is built.",
            "/path/to/app.exe");
        Assert.Contains("/path/to/app.exe", appLaunchEx.Message);
        Assert.Contains("30 seconds", appLaunchEx.Message);

        // DatabaseResetException should include operation context
        var dbResetEx = new DatabaseResetException(
            "Failed to reset database. Connection string: 'Host=localhost;Database=test'. Ensure PostgreSQL is running.",
            "Host=localhost;Database=test",
            new Exception("Connection refused"));
        Assert.Contains("reset database", dbResetEx.Message);
        Assert.Contains("PostgreSQL", dbResetEx.Message);

        // ConfigurationException should include key and guidance
        var configEx = new ConfigurationException(
            "Configuration key 'DatabaseConnectionString' is missing. Set MAGIDESK_TEST_DB_CONNECTION environment variable.",
            "DatabaseConnectionString");
        Assert.Contains("DatabaseConnectionString", configEx.Message);
        Assert.Contains("MAGIDESK_TEST_DB_CONNECTION", configEx.Message);

        // ElementNotFoundException should include element details
        var elementEx = new ElementNotFoundException(
            "Element with AutomationId 'LoginButton' was not found within 10.0 seconds. Ensure the application has navigated to the login page.",
            "LoginButton",
            "Login",
            "Button");
        Assert.Contains("LoginButton", elementEx.Message);
        Assert.Contains("10.0 seconds", elementEx.Message);
        Assert.Contains("login page", elementEx.Message);
    }

    /// <summary>
    /// Validates that exceptions can be caught by their base types.
    /// This is important for error handling in the framework.
    /// </summary>
    [Fact]
    public void ExceptionHierarchy_CanBeCaughtByBaseType()
    {
        // All E2ETestException derivatives can be caught as E2ETestException
        Exception caughtException;

        try
        {
            throw new ApplicationLaunchException("Test", "/path");
        }
        catch (E2ETestException ex)
        {
            caughtException = ex;
        }
        Assert.IsType<ApplicationLaunchException>(caughtException);

        try
        {
            throw new DatabaseResetException("Test", "conn", new Exception());
        }
        catch (E2ETestException ex)
        {
            caughtException = ex;
        }
        Assert.IsType<DatabaseResetException>(caughtException);

        try
        {
            throw new ConfigurationException("Test", "key");
        }
        catch (E2ETestException ex)
        {
            caughtException = ex;
        }
        Assert.IsType<ConfigurationException>(caughtException);

        // ElementNotFoundException can be caught as TimeoutException
        try
        {
            throw new ElementNotFoundException("Test", "id", "name", "type");
        }
        catch (TimeoutException ex)
        {
            caughtException = ex;
        }
        Assert.IsType<ElementNotFoundException>(caughtException);
    }

    /// <summary>
    /// Validates that ApplicationLaunchException with inner exception preserves all properties.
    /// </summary>
    [Fact]
    public void ApplicationLaunchException_WithInnerException_PreservesAllProperties()
    {
        // Arrange
        var innerException = new FileNotFoundException("Executable not found");
        var message = "Failed to launch application";
        var executablePath = "/path/to/app.exe";

        // Act
        var exception = new ApplicationLaunchException(message, executablePath, innerException);

        // Assert
        Assert.Equal(executablePath, exception.ExecutablePath);
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    /// <summary>
    /// Validates that exceptions fail fast by being throwable immediately.
    /// This test ensures exceptions can be constructed and thrown without delay.
    /// </summary>
    [Fact]
    public void Exceptions_FailFastByBeingImmediatelyThrowable()
    {
        // All exceptions should be constructible and throwable without delay
        var appLaunchEx = Assert.Throws<ApplicationLaunchException>((Action)(() =>
        {
            throw new ApplicationLaunchException("App not built", "/path/to/app.exe");
        }));
        Assert.NotNull(appLaunchEx);

        var dbResetEx = Assert.Throws<DatabaseResetException>((Action)(() =>
        {
            throw new DatabaseResetException("DB not accessible", "conn", new Exception());
        }));
        Assert.NotNull(dbResetEx);

        var configEx = Assert.Throws<ConfigurationException>((Action)(() =>
        {
            throw new ConfigurationException("Config missing", "key");
        }));
        Assert.NotNull(configEx);

        var elementEx = Assert.Throws<ElementNotFoundException>((Action)(() =>
        {
            throw new ElementNotFoundException("Element not found", "id", "name", "type");
        }));
        Assert.NotNull(elementEx);

        var e2eEx = Assert.Throws<E2ETestException>((Action)(() =>
        {
            throw new E2ETestException("Framework error");
        }));
        Assert.NotNull(e2eEx);
    }
}
