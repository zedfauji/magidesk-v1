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
│   └── BaseE2ETest.cs            # Base class for all E2E tests
└── Tests/
    └── SmokeTests.cs             # Basic smoke tests
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
- Helper methods for waiting (`WaitUntil`, `WaitForElement`)
- Automatic cleanup after each test

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
        var button = MainWindow.FindFirstDescendant(cf => 
            cf.ByAutomationId("MyButtonId"));

        // Act
        button.Click();

        // Assert
        var result = WaitForElement(
            () => MainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResultId")),
            TimeSpan.FromSeconds(5));
        
        Assert.NotNull(result);
    }
}
```

### Best Practices

1. **Use AutomationIds**: Always prefer AutomationId over Name or ClassName for element lookup
2. **Wait for elements**: Use `WaitForElement` or `WaitUntil` instead of `Thread.Sleep`
3. **Isolate tests**: Each test gets a fresh application instance
4. **Clean state**: Override `ResetDatabase()` to ensure clean test state
5. **Meaningful assertions**: Assert on behavior, not implementation details

## Notes

- Tests run with UIA3 automation provider (most reliable for WinUI 3)
- Each test launches a new application instance
- Application process is forcefully terminated after each test
- Executable path is resolved relative to test assembly location
- Database reset is a placeholder - implement as needed for your test scenarios
