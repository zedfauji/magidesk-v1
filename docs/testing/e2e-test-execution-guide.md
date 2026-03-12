# E2E Test Execution Guide

## Overview

This guide explains how to execute the Magidesk POS E2E test suite using FlaUI automation.

## Prerequisites

- Windows 10/11 with .NET 8.0 SDK
- PostgreSQL database server
- Magidesk POS application built and available

## Test Infrastructure Setup

### Database Configuration

Set the database connection string via environment variable:

```powershell
$env:TEST_DB_CONNECTION = "Host=localhost;Database=magidesk_test;Username=postgres;Password=postgres"
```

### Application Path

Specify the application path (optional, auto-detected if not set):

```powershell
$env:TEST_APP_PATH = "C:\path\to\Magidesk.Presentation.exe"
```

## Running Tests

### Run All Tests

```powershell
dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj
```

### Run by Priority

```powershell
# P0 - Critical financial safety tests
dotnet test --filter "Priority=P0"

# P1 - Operational integrity tests
dotnet test --filter "Priority=P1"

# P2 - Stability tests
dotnet test --filter "Priority=P2"
```

### Run by Category

```powershell
# Financial safety tests
dotnet test --filter "Category=FinancialSafety"

# Operational integrity tests
dotnet test --filter "Category=OperationalIntegrity"

# Stability tests
dotnet test --filter "Category=Stability"
```

## Test Artifacts

Test failures automatically capture:

- Screenshots: `TestResults/Screenshots/`
- UI trees: `TestResults/UITrees/`
- Database snapshots: `TestResults/DatabaseSnapshots/`
- Process state: `TestResults/ProcessState/`

## CI/CD Integration

Tests run automatically via GitHub Actions on push/PR. P0 test failures block the build.

## Troubleshooting

- Ensure database is accessible and reset before tests
- Verify application path is correct
- Check test execution logs in TestResults directory
