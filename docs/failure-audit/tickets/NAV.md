# Navigation Failure (NAV) Tickets

| ID | Title | Severity | Status | Owner |
| :--- | :--- | :--- | :--- | :--- |
| **NAV-001** | Fix `NavigationService.ShowDialogAsync` Timeout Crash | **BLOCKER** | DONE | Antigravity |

## Details

### NAV-001: Fix `NavigationService.ShowDialogAsync` Timeout Crash
*   **Location**: `NavigationService.cs`, Method `ShowDialogAsync`
*   **Failure**: Throws raw `InvalidOperationException` if `XamlRoot` is not found within timeout. This uncaught exception will crash the caller (often `async void` handlers).
*   **Requirement**: 
    1. Increase resilience of `XamlRoot` lookup.
    2. Do NOT throw `InvalidOperationException` blindly.
    3. Return a `Result` object OR throw a custom `DialogFailureException` that is documented.
    4. Ideally, log the failure and return `DialogResult.None` (swallow safely) or show an alternative window if possible (fallback).
