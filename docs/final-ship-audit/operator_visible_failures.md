# Operator Visibility Classification

All findings from the "Final Pre-Ship Forensic Audit" are classified here.

| Finding ID | Component | Failure Condition | Current Visibility | **REQUIRED Visibility** | Severity |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-SA-001** | `App.xaml.cs` | Startup Fatal Error (Native Dialog Fails) | Log Only (Silent) | **Native Safe-Fail Loop** | CRITICAL |
| **F-VM-001** | `LoginViewModel` | Clock-In System Failure | Red Text (Weak) | **ERROR DIALOG** | HIGH |
| **F-VM-002** | `LoginViewModel` | Login System Failure (DB) | "Login Error" Text | **ERROR DIALOG** | HIGH |
| **F-VM-003** | `LoginViewModel` | Default View Routing Failed | Silent Redirect | **WARNING TOAST** | MEDIUM |
| **F-VM-004** | `Switchboard` | Back Office Auth Error | Log Only | **ERROR DIALOG** | HIGH |
| **F-VM-005** | `Switchboard` | Manager Func Auth Error | Log Only | **ERROR DIALOG** | HIGH |
| **F-VM-006** | `Switchboard` | **Create Ticket Failure** | Log Only | **FATAL DIALOG** | **BLOCKER** |
| **F-VM-007** | `Switchboard` | Load Open Tickets Error | Log Only | **WARNING BANNER** | HIGH |
| **F-VM-008** | `Switchboard` | Drawer Ops (Open/Balance) | Log Only | **ERROR DIALOG** | **BLOCKER** |
| **F-VM-009** | `Switchboard` | Switchboard Clock-In Error | Log Only | **ERROR DIALOG** | HIGH |
| **F-VM-010** | `SystemConfig` | Load Config Failure | Status Bar Text | **ERROR DIALOG** | MEDIUM |
| **F-VM-011** | `SystemConfig` | Save Config Failure | Status Bar Text | **ERROR DIALOG** | HIGH |
| **F-VM-012** | `SystemConfig` | Restore Backup Failure | Status Bar Text | **ERROR DIALOG** | **BLOCKER** |

## Universal Rules
1.  **Blocker/Critical** failures must use `NavigationService.ShowDialogAsync` (or Native Fallback if needed).
2.  **High** failures must use `NavigationService.ShowErrorAsync`.
3.  **Medium** failures can use `ShowMessageAsync` or prominent Banners.
4.  **Log-Only** is strictly forbidden for any catch block.
