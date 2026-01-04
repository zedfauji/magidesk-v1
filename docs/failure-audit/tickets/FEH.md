# Frontend Exception Handling (FEH) Tickets

| ID | Title | Severity | Status | Owner |
| :--- | :--- | :--- | :--- | :--- |
| **FEH-001** | Fix `MainWindow.OnItemInvoked` Async Void Crash | **BLOCKER** | DONE | Antigravity |
| **FEH-002** | Harden `App.UnhandledException` Logging | **BLOCKER** | DONE | Antigravity |
| **FEH-003** | Fix `ShiftStartDialog` Async Void Handler | **HIGH** | DONE | Antigravity |
| **FEH-004** | Fix `TableDesignerPage` Async Void Handler | **HIGH** | DONE | Antigravity |
| **FEH-005** | Fix `MainWindow.UpdateUiAuthState` Fire-and-Forget | **HIGH** | DONE | Antigravity |
| **FEH-006** | Implement `IDialogService` Infrastructure | **BLOCKER** | DONE | Antigravity |

## Details

### FEH-001: Fix `MainWindow.OnItemInvoked` Async Void Crash
*   **Location**: `MainWindow.xaml.cs`, Method `OnItemInvoked`
*   **Failure**: `async void` method triggered by Navigation View. Any exception thrown during navigation lookup or execution will bypass the Task Scheduler and crash the process.
*   **Requirement**: Wrap entire body in `try { ... } catch (Exception ex) { HandleFatal(ex); }`.
*   **Dependency**: None (use `MessageBox` or `Debug` if Service not ready, or implement FEH-006 first).

### FEH-002: Harden `App.UnhandledException` Logging
*   **Location**: `App.xaml.cs`, Method `App_UnhandledException`
*   **Failure**: The catch block is empty (`catch { }`). If disk is full, permissions denied, or path invalid, the original exception is lost and the app exits silently.
*   **Requirement**: Use nested try-catch. If primary log fails, attempt `Debug.WriteLine`. Ensure the user is notified if possible.

### FEH-003: Fix `ShiftStartDialog_Opened` Async Void
*   **Location**: `ShiftStartDialog.xaml.cs`
*   **Failure**: `async void` event handler.
*   **Requirement**: Wrap body in `try-catch`. Display error via `ContentDialog` if `Opened` logic fails.

### FEH-004: Fix `TableDesignerPage_OnNavigatedTo` Async Void
*   **Location**: `TableDesignerPage.xaml.cs`
*   **Failure**: `async void` override.
*   **Requirement**: Wrap body in `try-catch`.

### FEH-005: Fix `MainWindow.UpdateUiAuthState` Fire-and-Forget
*   **Location**: `MainWindow.xaml.cs`
*   **Failure**: `DispatcherQueue.TryEnqueue(() => UpdateUiAuthState(u))` uses a lambda that returns void. Exceptions inside `UpdateUiAuthState` are swallowed or crash thread.
*   **Requirement**: Use `DispatcherQueue.EnqueueAsync` extension (if avail) or wrap lambda in `try-catch`.

### FEH-006: Implement `IDialogService` Infrastructure
*   **Location**: `Magidesk.Infrastructure.Services.DialogService.cs` (New)
*   **Failure**: Lack of standardized way to show errors from ViewModels/Services leads to swallowed exceptions or `Debug.WriteLine`.
*   **Requirement**: Implement `IDialogService`. Register in DI. Ensure it handles `XamlRoot` resolution robustly.
