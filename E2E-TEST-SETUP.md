# E2E Test Setup Guide

This guide explains how to set up and run the E2E tests for Magidesk POS.

## Prerequisites

1. PostgreSQL installed and running on localhost:5432
2. PostgreSQL user `postgres` with password `postgres` (or update `appsettings.test.json`)
3. .NET 8 SDK installed

## Step 1: Create Test Database

```powershell
# Create the test database
psql -U postgres -c "DROP DATABASE IF EXISTS magidesk_test;"
psql -U postgres -c "CREATE DATABASE magidesk_test;"
```

## Step 2: Initialize Database Schema

The Magidesk application automatically creates the database schema on first run using EF Core migrations.

### Option A: Run the Application (Recommended)

1. **Update the database configuration** to point to the test database:
   
   Create or edit `src/Magidesk.Presentation/database-config.json`:
   ```json
   {
     "DatabaseConfiguration": {
       "Host": "localhost",
       "Port": 5432,
       "Database": "magidesk_test",
       "Username": "postgres",
       "Password": "postgres"
     }
   }
   ```

2. **Build and run the application**:
   ```powershell
   dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj -c Release
   dotnet run --project src/Magidesk.Presentation --no-build -c Release
   ```

3. **Wait for initialization**:
   - The application will display "System Initialization..." on startup
   - Wait for "System Initialization successful" in the logs
   - The schema is now created

4. **Close the application**

### Option B: Use EF Core Migrations Directly

If you have the `dotnet-ef` tool installed:

```powershell
# Install dotnet-ef if needed
dotnet tool install --global dotnet-ef

# Apply migrations from Infrastructure project
dotnet ef database update `
  --project src/Magidesk.Infrastructure `
  --startup-project src/Magidesk.Presentation `
  --connection "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres"

# Apply migrations from Migrations project
dotnet ef database update `
  --project src/Magidesk.Migrations `
  --connection "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres"
```

## Step 3: Verify Schema Creation

```powershell
# Check that tables were created
psql -U postgres -d magidesk_test -c "\dt"
```

You should see many tables including: `Tickets`, `Payments`, `OrderLines`, `KitchenOrders`, etc.

## Step 4: Run E2E Tests

### Run All E2E Tests
```powershell
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj
```

### Run Specific Test Categories
```powershell
# P0 Financial Safety Tests
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --filter "Category=FinancialSafety"

# P1 Operational Integrity Tests
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --filter "Category=OperationalIntegrity"

# P2 Stability Tests
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --filter "Category=Stability"
```

### Run a Single Test
```powershell
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj `
  --filter "FullyQualifiedName~SingleCashPaymentTests.CompleteCashPayment_ShouldUpdateCashDrawerBalance"
```

## What the E2E Tests Do

1. **Database Reset**: Each test starts by resetting the database to a clean state
2. **Application Launch**: The test framework launches the Magidesk application
3. **UI Automation**: Tests interact with the UI using FlaUI and AutomationIds
4. **Login**: Tests log in using PIN 1234 (manager account)
5. **Workflow Execution**: Tests perform actions like creating orders, processing payments, etc.
6. **Assertions**: Tests verify expected outcomes (payment totals, UI state, etc.)
7. **Cleanup**: Application is closed and database is reset for the next test

## Test Configuration

The E2E tests use `appsettings.test.json` for configuration:

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

- `DatabaseConnectionString`: Connection to the test database
- `ApplicationPath`: Path to Magidesk.exe (auto-detected if null)
- `TimeoutMultiplier`: Multiplier for all timeouts (increase for slow machines)
- `ArtifactsDirectory`: Where to save screenshots and logs on test failure

## Troubleshooting

### Tests fail with "database does not exist"
- Ensure you created the `magidesk_test` database (Step 1)

### Tests fail with "relation does not exist"
- The schema wasn't created. Run the application once (Step 2)

### Tests fail with "Application not found"
- Build the Magidesk.Presentation project in Release mode
- Or set `ApplicationPath` in `appsettings.test.json` to the full path of Magidesk.exe

### Tests fail with "Element not found"
- Ensure all XAML files have AutomationIds (completed in ui-automation-ids spec)
- Check that the application is using the correct UI (not behind a feature flag)

### Tests are slow
- Increase `TimeoutMultiplier` in `appsettings.test.json`
- Check database performance
- Ensure PostgreSQL is running locally (not over network)

## Test Structure

```
Magidesk.Tests.E2E/
├── Infrastructure/          # Test framework components
│   ├── ApplicationLauncher.cs
│   ├── DatabaseResetEngine.cs
│   ├── WaitHelpers.cs
│   └── BaseE2ETest.cs
├── PageObjects/            # Page Object Model
│   ├── LoginPage.cs
│   ├── SwitchboardPage.cs
│   ├── OrderEntryPage.cs
│   └── SettlementPage.cs
├── Tests/
│   ├── P0_FinancialSafety/  # Critical financial tests
│   ├── P1_OperationalIntegrity/  # Business workflow tests
│   └── P2_Stability/        # Stability and performance tests
└── Scripts/
    ├── reset-database.sql   # Cleans transactional data
    └── seed-test-data.sql   # Seeds test configuration
```

## Manager PIN

All tests use PIN **1234** to log in as a manager account.
