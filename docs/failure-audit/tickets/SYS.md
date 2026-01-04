# Startup/System Failure (SYS) Tickets

| ID | Title | Severity | Status | Owner |
| :--- | :--- | :--- | :--- | :--- |
| **SYS-001** | Implement Fatal Startup Error Dialog | **HIGH** | DONE | Antigravity |

## Details

### SYS-001: Implement Fatal Startup Error Dialog
*   **Location**: `App.xaml.cs`, Constructor `App()`
*   **Failure**: If `InitializeComponent` or `Host` builder fails, it logs (using `StartupLogger`) and rethrows. The rethrow likely results in a silent process exit or generic Windows Event Viewer entry.
*   **Requirement**: Before rethrowing, attempt to show a native `MessageBox` (P-Invoke) or a dedicated fallback `Window` with the error trace, so the operator knows *why* it failed.
