# Magidesk.Tests.E2E

End-to-end UI automation testing framework for Magidesk POS using FlaUI with UIA3 automation.

## Overview

The E2E Testing Framework provides fully automated, deterministic end-to-end testing for the Magidesk POS application through UI automation. The framework ensures zero-manual-intervention testing with automatic failure capture, database reset, and comprehensive reporting.

### Design Goals

- **Determinism**: Tests produce identical results on every execution with the same code
- **Zero Manual Intervention**: Complete automation from launch to cleanup
- **Fast Failure Diagnosis**: Automatic capture of screenshots, UI tree, logs, and database state on failure
- **Test Isolation**: Each test runs with clean state and cannot affect other tests
- **Architectural Integrity**: Tests validate real user behavior without bypassing application boundaries
- **CI/CD Ready**: Designed for automated execution in build pipelines

### Key Constraints

- No references to application projects (Domain, Application, Infrastructure, Presentation)
- No Thread.Sleep - all waits use deterministic polling
- No test hooks or modifications to application code
- UI-only validation - database access only for reset operations
- All tests must be reproducible and non-flaky

## Technology Stack

- **Framework**: .NET 8.0 (Windows 10.0.19041.0)
- **Test Framework**: xUnit 2.5.3
- **UI Automation**: FlaUI 4.0.0 with UIA3 provider
- **Database**: PostgreSQL via Npgsql 8.0.0
- **Target**: Magidesk.Presentation WinUI 3 application
- **Platforms**: x86, x64, ARM64

## Project Structure

```
Magidesk.Tests.E2E/
├── Infrastructure/           # Core framework components
│   ├── ApplicationLauncher.cs    # Manages app process lifecycle
│   ├── BaseE2ETest.cs            # Base class for all E2E tests
│   ├── WaitHelpers.cs            # Deterministic waiting strategies
│   ├── DatabaseResetEngine.cs    # Database reset and seeding
│   ├── FailureCaptureSystem.cs   # Automatic failure artifact capture
│   ├── ConfigurationManager.cs   # Configuration loading and validation
│   └── Exceptions/               # Custom exception hierarchy
├── PageObjects/              # Page Object Model abstractions
│   ├── BasePage.cs              # Base class for all page objects
│   ├── LoginPage.cs             # Login page interactions
│   ├── SwitchboardPage.cs       # Main menu interactions
│   ├── OrderEntryPage.cs        # Order entry interactions
│   ├── SettlementPage.cs        # Payment processing interactions
│   └── CashSessionPage.cs       # Cash session management
├── Tests/                    # Test scenarios organized by priority
│   ├── P0_FinancialSafety/      # Critical financial workflows
│   ├── P1_OperationalIntegrity/ # Essential business operations
│   ├── P2_Stability/            # Stress and edge case tests
│   └── Examples/                # Example tests and patterns
├── Scripts/                  # Database scripts
│   ├── reset-database.sql       # Transactional data cleanup
│   └── seed-test-data.sql       # Baseline test data
├── TestResults/              # Test failure artifacts (generated)
└── appsettings.test.json     # Default configuration
```

## Setup Instructions

### Prerequisites

1. **PostgreSQL Database**: Install PostgreSQL 12 or later
2. **Test Database**: Create a dedicated test database
   ```sql
   CREATE DATABASE magidesk_test;
   ```

3. **Build Magidesk.Presentation**: Build the application in Debug configuration
   ```powershell
   dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj -c Debug
   ```

4. **Environment Variables**: Set required configuration
   ```powershell
   # Required: Database connection string
   $env:MAGIDESK_TEST_DB_CONNECTION = "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres"
   
   # Optional: Custom application path
   $env:MAGIDESK_APP_PATH = "C:\path\to\Magidesk.Presentation.exe"
   
   # Optional: Timeout multiplier for slower machines (default 1.0)
   $env:MAGIDESK_TEST_TIMEOUT_MULTIPLIER = "1.5"
   
   # Optional: Custom artifacts directory (default TestResults/)
   $env:MAGIDESK_TEST_ARTIFACTS_DIR = "C:\TestArtifacts\"
   ```

### Configuration

The framework reads configuration from:
1. **Environment variables** (highest priority)
2. **appsettings.test.json** (fallback)
3. **Default values** (last resort)

Edit `appsettings.test.json` to set default values:
```json
{
  "TestConfiguration": {
    "DatabaseConnectionString": "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres",
    "ApplicationPath": null,
    "TimeoutMultiplier": 1.0,
    "ArtifactsDirectory": "TestResults/"
  }
}
```

## Running Tests

### Execute All Tests

```powershell
# Run all E2E tests
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj

# Run with verbose output
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj -v detailed
```

### Filter Tests by Priority

```powershell
# Run only P0 (critical financial) tests
dotnet test --filter "Priority=P0"

# Run only P1 (operational integrity) tests
dotnet test --filter "Priority=P1"

# Run only P2 (stability) tests
dotnet test --filter "Priority=P2"
```

### Filter Tests by Category

```powershell
# Run only financial safety tests
dotnet test --filter "Category=FinancialSafety"

# Run only operational integrity tests
dotnet test --filter "Category=OperationalIntegrity"

# Run only stability tests
dotnet test --filter "Category=Stability"
```

## Key Components

### ApplicationLauncher

Manages the lifecycle of the Magidesk.Presentation.exe process.

**Responsibilities**:
- Locates executable relative to test assembly or from environment variable
- Starts process and waits for main window (30s timeout)
- Provides FlaUI Application and Window instances
- Forcefully terminates process on disposal
- Ensures no orphaned processes remain

**Usage**:
```csharp
using var launcher = new ApplicationLauncher(executablePath);
var app = launcher.Launch();
var mainWindow = launcher.GetMainWindow(TimeSpan.FromSeconds(30));
// Use mainWindow for UI automation
// Dispose automatically kills process
```

### WaitHelpers

Provides deterministic, polling-based wait operations for UI automation.

**Key Features**:
- All methods use 100ms polling interval
- All methods accept timeout parameter
- Throws TimeoutException with element context on timeout
- Timeout messages include AutomationId, Name, and ControlType
- No use of Thread.Sleep anywhere

**Available Methods**:
```csharp
// Wait for custom condition
WaitHelpers.WaitUntil(() => condition, timeout, "error message");

// Wait for element by AutomationId (preferred)
var element = WaitHelpers.WaitForElementByAutomationId(parent, "ButtonId", timeout);

// Wait for element by Name
var element = WaitHelpers.WaitForElementByName(parent, "Button Text", timeout);

// Wait for element by ControlType
var element = WaitHelpers.WaitForElementByControlType(parent, ControlType.Button, timeout);

// Wait for element to become enabled
WaitHelpers.WaitForElementEnabled(element, timeout);

// Wait for element to disappear
WaitHelpers.WaitForElementToDisappear(() => element, timeout, "Loading spinner");

// Wait for window by title
var window = WaitHelpers.WaitForWindowByTitle(app, "Window Title", timeout);
```

### DatabaseResetEngine

Restores database to clean baseline state before each test.

**Responsibilities**:
- Deletes all transactional data (tickets, payments, cash sessions, etc.)
- Preserves configuration data (menu items, modifiers, payment methods, etc.)
- Seeds minimum required data (admin user, default terminal, tax rates)
- Executes within 5 seconds for typical operations
- Throws descriptive exception if reset fails

**Usage**:
```csharp
var resetEngine = new DatabaseResetEngine(connectionString);
resetEngine.ResetDatabase(); // Synchronous
await resetEngine.ResetDatabaseAsync(); // Asynchronous
```

### FailureCaptureSystem

Captures comprehensive failure artifacts when tests fail.

**Captured Artifacts**:
- Screenshot in PNG format
- UI automation tree in XML format
- Process state (memory, CPU, threads) in JSON
- Database snapshot as SQL dump
- Test metadata (name, timestamp, exception) in JSON

**Artifact Location**:
```
TestResults/
└── {TestName}_{Timestamp}/
    ├── screenshot.png
    ├── ui-tree.xml
    ├── process-state.json
    ├── database-snapshot.sql
    └── failure-info.json
```

### BaseE2ETest

Base class providing automatic setup and teardown for all E2E tests.

**Lifecycle**:
1. **Constructor**: Resets database → Launches application → Waits for main window
2. **Test Execution**: Test code runs with MainWindow available
3. **Dispose**: Terminates application → Captures failure artifacts (if test failed)

**Usage**:
```csharp
public class MyFeatureTests : BaseE2ETest
{
    [Fact]
    public void MyTest()
    {
        // MainWindow is already available
        var button = WaitHelpers.WaitForElementByAutomationId(
            MainWindow!, "MyButtonId", TimeSpan.FromSeconds(10));
        
        button.Click();
        
        // Assert on UI state
    }
    
    // Optional: Override for custom database reset logic
    protected override void ResetDatabase()
    {
        base.ResetDatabase();
        // Additional custom reset logic
    }
}
```

### Page Object Model

Abstracts UI interactions into reusable, maintainable components.

**BasePage** provides common functionality:
- Element lookup using WaitHelpers
- Common actions (ClickButton, EnterText, GetText, IsElementEnabled)
- Timeout management with multiplier support
- Descriptive exceptions when elements not found

**Available Page Objects**:
- **LoginPage**: Login page interactions (EnterUsername, EnterPassword, ClickLogin)
- **SwitchboardPage**: Main menu navigation (NavigateToOrderEntry, NavigateToSettlement, etc.)
- **OrderEntryPage**: Order entry interactions (SelectMenuItem, AddModifier, ApplyDiscount, etc.)
- **SettlementPage**: Payment processing (SelectPaymentMethod, ProcessPayment, SplitPayment, etc.)
- **CashSessionPage**: Cash session management (OpenSession, CloseSession, RecordCashDrop, etc.)

**Usage Example**:
```csharp
public class LoginTests : BaseE2ETest
{
    [Fact]
    public void SuccessfulLogin()
    {
        var loginPage = new LoginPage(MainWindow!);
        
        loginPage.EnterUsername("admin");
        loginPage.EnterPassword("admin123");
        loginPage.ClickLogin();
        
        var switchboard = new SwitchboardPage(MainWindow!);
        Assert.Equal("admin", switchboard.GetCurrentUserName());
    }
}
```

## Writing Tests

### Basic Test Structure

```csharp
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class PaymentTests : BaseE2ETest
{
    [Fact]
    public void ProcessCashPayment_UpdatesCashDrawerBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        loginPage.EnterUsername("admin");
        loginPage.EnterPassword("admin123");
        loginPage.ClickLogin();
        
        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToOrderEntry();
        
        var orderEntry = new OrderEntryPage(MainWindow!);
        orderEntry.SelectMenuItem("Coffee");
        var ticketTotal = orderEntry.GetTicketTotal();
        orderEntry.NavigateToSettlement();
        
        // Act
        var settlement = new SettlementPage(MainWindow!);
        settlement.SelectPaymentMethod("Cash");
        settlement.EnterPaymentAmount(ticketTotal);
        settlement.ProcessPayment();
        
        // Assert
        Assert.Equal(0m, settlement.GetAmountDue());
        Assert.Equal(ticketTotal, settlement.GetAmountPaid());
    }
}
```

### Best Practices

1. **Use AutomationIds**: Always prefer AutomationId over Name or ClassName for element lookup
   ```csharp
   // Good
   var button = WaitHelpers.WaitForElementByAutomationId(MainWindow!, "LoginButton", timeout);
   
   // Avoid
   var button = WaitHelpers.WaitForElementByName(MainWindow!, "Login", timeout);
   ```

2. **Use WaitHelpers**: NEVER use `Thread.Sleep` - always use WaitHelpers methods
   ```csharp
   // Good
   WaitHelpers.WaitForElementEnabled(button, TimeSpan.FromSeconds(5));
   
   // NEVER do this
   Thread.Sleep(5000);
   ```

3. **Fail Fast**: WaitHelpers provide detailed error messages with element context
   ```csharp
   // Timeout exception includes:
   // - Element AutomationId, Name, ControlType
   // - Parent element information
   // - Timeout duration
   // - Actionable guidance
   ```

4. **Isolate Tests**: Each test gets a fresh application instance and clean database
   ```csharp
   // No need to clean up state - BaseE2ETest handles it
   ```

5. **Use Page Objects**: Encapsulate UI interactions in page objects
   ```csharp
   // Good
   var loginPage = new LoginPage(MainWindow!);
   loginPage.EnterUsername("admin");
   
   // Avoid
   var usernameBox = WaitHelpers.WaitForElementByAutomationId(...);
   usernameBox.AsTextBox().Text = "admin";
   ```

6. **Meaningful Assertions**: Assert on behavior, not implementation details
   ```csharp
   // Good
   Assert.Equal(0m, settlement.GetAmountDue());
   
   // Avoid
   Assert.True(settlement.ProcessPaymentButton.IsEnabled == false);
   ```

7. **Tag Tests Appropriately**: Use Trait attributes for filtering
   ```csharp
   [Trait("Priority", "P0")]        // P0 = Critical financial
   [Trait("Category", "FinancialSafety")]
   ```

### Common Pitfalls and How to Avoid Them

1. **Pitfall**: Using Thread.Sleep for timing
   - **Solution**: Use WaitHelpers with appropriate timeouts

2. **Pitfall**: Hardcoding element names that change frequently
   - **Solution**: Use AutomationIds which are stable

3. **Pitfall**: Not waiting for elements to be ready
   - **Solution**: Use WaitForElementEnabled before interacting

4. **Pitfall**: Tests depending on execution order
   - **Solution**: Each test must be independent with clean state

5. **Pitfall**: Catching exceptions and continuing
   - **Solution**: Let exceptions propagate for proper failure capture

6. **Pitfall**: Not disposing resources
   - **Solution**: Use `using` statements or inherit from BaseE2ETest

7. **Pitfall**: Testing implementation details instead of user behavior
   - **Solution**: Focus on what users see and do in the UI

### Troubleshooting Guide

#### Test Fails with "Element not found"

1. Check if AutomationId is correct in the XAML
2. Verify the application navigated to the expected page
3. Increase timeout if element takes longer to appear
4. Check failure artifacts (screenshot, UI tree) to see actual UI state

#### Test Fails with "Application did not start"

1. Verify Magidesk.Presentation is built in Debug configuration
2. Check executable path is correct
3. Verify no other instance is running
4. Check application logs for startup errors

#### Test Fails with "Database reset failed"

1. Verify PostgreSQL is running
2. Check database connection string is correct
3. Verify test database exists
4. Check database user has sufficient permissions

#### Tests Are Slow

1. Check TimeoutMultiplier setting (should be 1.0 for fast machines)
2. Verify database reset completes within 5 seconds
3. Check for network latency to database
4. Consider running tests on faster hardware

#### Tests Are Flaky

1. Verify you're using WaitHelpers, not Thread.Sleep
2. Check for race conditions in UI updates
3. Increase timeouts if necessary
4. Review failure artifacts to identify patterns

## Architecture Overview

### Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     Test Execution Layer                     │
│  ┌──────────────┐              ┌──────────────┐            │
│  │ xUnit Runner │─────────────▶│ Test Classes │            │
│  └──────────────┘              └──────────────┘            │
└─────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
┌─────────────────────────────────────────────────────────────┐
│                 Test Infrastructure Layer                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  BaseE2ETest │  │ WaitHelpers  │  │ Config Mgr   │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ App Launcher │  │ DB Reset Eng │  │ Failure Cap  │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
┌─────────────────────────────────────────────────────────────┐
│                   Page Object Layer                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  LoginPage   │  │ Switchboard  │  │ OrderEntry   │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│  ┌──────────────┐  ┌──────────────┐                        │
│  │  Settlement  │  │ CashSession  │                        │
│  └──────────────┘  └──────────────┘                        │
└─────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
┌─────────────────────────────────────────────────────────────┐
│                  External Dependencies                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │    FlaUI     │  │    Npgsql    │  │  Magidesk    │     │
│  │   + UIA3     │  │              │  │ Presentation │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                           │                   │             │
│                           ▼                   ▼             │
│                    ┌──────────┐      ┌──────────┐          │
│                    │PostgreSQL│      │  WinUI 3 │          │
│                    │ Database │      │    App   │          │
│                    └──────────┘      └──────────┘          │
└─────────────────────────────────────────────────────────────┘
```

### Test Execution Flow

1. **Test Runner** discovers and executes test classes
2. **BaseE2ETest Constructor** runs before each test:
   - Resets database to clean state
   - Launches Magidesk.Presentation.exe
   - Waits for main window to appear
3. **Test Method** executes with MainWindow available
4. **BaseE2ETest Dispose** runs after each test:
   - Terminates application process
   - Captures failure artifacts (if test failed)

## CI/CD Integration

### Build Pipeline Steps

```yaml
steps:
  - name: Build Magidesk.Presentation
    run: dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj -c Debug
  
  - name: Restore E2E Test Dependencies
    run: dotnet restore src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj
  
  - name: Set Environment Variables
    run: |
      echo "MAGIDESK_TEST_DB_CONNECTION=Host=localhost;Port=5432;Database=magidesk_test;Username=test;Password=test" >> $GITHUB_ENV
  
  - name: Run Database Migrations
    run: powershell -File Scripts/apply_migrations.ps1
  
  - name: Execute E2E Tests
    run: dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --logger "trx;LogFileName=test-results.trx"
  
  - name: Publish Test Results
    uses: actions/upload-artifact@v3
    if: always()
    with:
      name: test-results
      path: TestResults/
  
  - name: Fail Build on P0 Test Failure
    run: |
      if (dotnet test --filter "Priority=P0" --no-build) { exit 0 } else { exit 1 }
```

## Notes

- Tests run with UIA3 automation provider (most reliable for WinUI 3)
- Each test launches a new application instance
- Application process is forcefully terminated after each test
- Executable path is resolved relative to test assembly location
- Database reset is automatic before each test
- Failure artifacts are captured automatically on test failure
- Tests are serial by default for determinism (can be parallelized later)

## Future Enhancements

- Parallel test execution with database-per-test-instance isolation
- Video recording of test execution
- Performance profiling integration
- Flaky test detection and reporting
- Visual regression testing
- Accessibility testing automation
- Load testing with concurrent users
- Network simulation for offline mode testing

---

**Framework Version**: 1.0  
**Last Updated**: 2026-01-29  
**Status**: Active Development
