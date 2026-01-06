# Magidesk Workflow Tests

This project contains isolated, ViewModel-based workflow tests for the Magidesk POS.
These tests simulate real user interactions via the ViewModel layer to verify complete business processes.

## Isolation Guarantees

1.  **Separate Configuration**: Tests MUST NOT rely on production `appsettings.json`. Configuration is provided by `TestConfiguration`.
2.  **Separate Database**: Tests MUST run against an isolated test database or in-memory provider (if applicable for integration scenarios).
3.  **No Static Leakage**: Tests MUST ensure static state is reset or isolated.
4.  **No UI Dependency**: Tests run primarily on the background thread or a simulated dispatcher, without rendering XAML.

## Non-Regression Guarantee

-   Any change that breaks a workflow test constitutes a REGRESSION.
-   Silent behavior changes are FORBIDDEN.
-   All workflows functionality must be verified by at least one test.

## Running Tests

```powershell
dotnet test Magidesk.Tests.Workflows
```
