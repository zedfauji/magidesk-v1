# Magidesk.Tests.E2E

End-to-end UI automation tests for Magidesk POS using FlaUI with UIA3 automation.

## Overview

This project contains automated UI tests that launch the full Magidesk.Presentation application and interact with it through the UI Automation framework.

## Technology Stack

- **Framework**: .NET 8.0 (Windows)
- **Test Framework**: xUnit
- **UI Automation**: FlaUI with UIA3 provider
- **Target**: Magidesk.Presentation WinUI 3 application

## Project Structure

```
Magidesk.Tests.E2E/
├── Infrastructure/
│   ├── ApplicationLauncher.cs    # Launches and manages the app process
│   ├── BaseE2ETest.cs            # Base class for all E2E tests
│   └── WaitHelpers.cs            # Deterministic waiting strategies
└── Tests/
    ├── SmokeTests.cs             # Basic smoke tests
    └── WaitHelpersExampleTests.cs # Examples of proper wait patterns
```

## Key Components

### ApplicationLauncher

Responsible for:
- Locating and launching the Magidesk.Presentation executable
- Managing the application process lifecycle
- Providing FlaUI Application and Window instances
- Clean process termination on disposal

### BaseE2ETest

Provides:
- Automatic application launch before each test
- Database reset capability (override `ResetDatabase()`)
- Main window access via `MainWindow` property
- Automatic cleanup after each test

### WaitHelpers

Deterministic waiting strategies with detailed error messages:
- `WaitUntil` - Wait for custom condition
- `WaitForElementByAutomationId` - Find element by AutomationId
- `WaitForElementByName` - Find element by Name
- `WaitForElementByControlType` - Find element by ControlType
- `WaitForElementEnabled` - Wait for element to become enabled
- `WaitForElementToDisappear` - Wait for element to disappear
- `WaitForWindowByTitle` - Find window by title

All methods use retry + timeout pattern with 100ms polling interval and fail fast with actionable error messages.

## Running Tests

### Prerequisites

1. Build Magidesk.Presentation in Debug configuration:
   ```powershell
   dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj -c Debug
   ```

2. Ensure the executable exists at:
   ```
   src/Magidesk.Presentation/bin/Debug/net8.0-windows/Magidesk.Presentation.exe
   ```

### Execute Tests

```powershell
# Run all E2E tests
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj

# Run with verbose output
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj -v detailed
```

## Writing Tests

### Basic Test Structure

```csharp
using Magidesk.Tests.E2E.Infrastructure;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

public class MyFeatureTests : BaseE2ETest
{
    [Fact]
    public void MyTest()
    {
        // Arrange - MainWindow is already available
        var button = WaitHelpers.WaitForElementByAutomationId(
            MainWindow!,
            "MyButtonId",
            TimeSpan.FromSeconds(10));

        // Act
        button.Click();

        // Assert
        var result = WaitHelpers.WaitForElementByAutomationId(
            MainWindow!,
            "ResultId",
            TimeSpan.FromSeconds(5));
        
        Assert.NotNull(result);
    }
}
```

### Best Practices

1. **Use AutomationIds**: Always prefer AutomationId over Name or ClassName for element lookup
2. **Use WaitHelpers**: NEVER use `Thread.Sleep` - always use WaitHelpers methods
3. **Fail fast**: WaitHelpers provide detailed error messages with element context
4. **Isolate tests**: Each test gets a fresh application instance
5. **Clean state**: Override `ResetDatabase()` to ensure clean test state
6. **Meaningful assertions**: Assert on behavior, not implementation details

### WaitHelpers Examples

```csharp
// Wait for element by AutomationId (preferred)
var button = WaitHelpers.WaitForElementByAutomationId(
    MainWindow!, "LoginButton", TimeSpan.FromSeconds(10));

// Wait for element by Name
var label = WaitHelpers.WaitForElementByName(
    MainWindow!, "Welcome", TimeSpan.FromSeconds(5));

// Wait for custom condition
WaitHelpers.WaitUntil(
    () => MainWindow!.Title.Contains("Ready"),
    TimeSpan.FromSeconds(10),
    "Window title did not update to Ready state");

// Wait for element to become enabled
WaitHelpers.WaitForElementEnabled(button, TimeSpan.FromSeconds(5));

// Wait for element to disappear
WaitHelpers.WaitForElementToDisappear(
    () => MainWindow!.FindFirstDescendant(cf => cf.ByAutomationId("LoadingSpinner")),
    TimeSpan.FromSeconds(10),
    "Loading spinner");
```

## Notes

- Tests run with UIA3 automation provider (most reliable for WinUI 3)
- Each test launches a new application instance
- Application process is forcefully terminated after each test
- Executable path is resolved relative to test assembly location
- Database reset is a placeholder - implement as needed for your test scenarios
