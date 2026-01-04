# Failure Entry Points

## Application Lifecycle
| Entry Point | Location | Current Handling | Risk |
| :--- | :--- | :--- | :--- |
| **Startup** | `App.OnLaunched` | `try-catch` with Logging and Window fallback. | Low |
| **Constructor** | `App()` | `try-catch` loops around `InitializeComponent` and `Host` build. Rethrows. | Moderate (Rethrow might crash before UI) |
| **Global Unhandled** | `App.UnhandledException` | **FLAWED**. Catches, logs, but has empty `catch` block that swallows logging failures. | **CRITICAL** |

## UI Interactions
| Entry Point | Location | Current Handling | Risk |
| :--- | :--- | :--- | :--- |
| **Navigation** | `MainWindow.OnItemInvoked` | **NONE**. `async void` with no try-catch. | **CRITICAL** |
| **Commands** | `ViewModel` RelayCommands | Varies. `AsyncRelayCommand` captures exceptions, but `RelayCommand` (sync) might crash. | Moderate |
| **Dialogs** | `NavigationService.ShowDialogAsync` | Throws `InvalidOperationException` on timeout/null root. Unverified caller handling. | High |

## Background Tasks
| Entry Point | Location | Current Handling | Risk |
| :--- | :--- | :--- | :--- |
| **Clock Timer** | `MainWindow` Timer | Lambda `(s,e) => ...` has no error handling. | Low (Simple format) |
| **Auth State** | `MainWindow.UpdateUiAuthState` | Dispatched. No explicit catch in dispatch lambda. | Moderate |
| **Table Map** | `TableMapViewModel.Refresh` | `try-catch` with Debug.WriteLine. (Silent Failure). | Moderate (Fixed in recent task but relies on log) |
