# File-by-File Forensic Audit

| File | Status | Findings Count | Critical | High | Medium | Low |
|------|--------|----------------|----------|------|--------|-----|
| `App.xaml.cs` | **AUDITED** | 3 | 1 | 2 | 0 | 0 |
| `Magidesk.Api/Program.cs` | **AUDITED** | 2 | 0 | 1 | 1 | 0 |
| `OrderEntryViewModel.cs` | **AUDITED** | 13 | 3 | 8 | 2 | 0 |
| `SettleViewModel.cs` | **AUDITED** | 6 | 0 | 5 | 1 | 0 |
| `NavigationService.cs` | **AUDITED** | 2 | 0 | 1 | 1 | 0 |
| `PrintingService.cs` | **AUDITED** | 1 | 0 | 0 | 1 | 0 |
| `KitchenPrintService.cs` | **AUDITED** | 3 | 1 | 2 | 0 | 0 |

## App.xaml.cs
### [CRITICAL] Missing Background Exception Handlers
- **Line**: Constructor (42)
- **Finding**: Only subscribes to `this.UnhandledException` (UI Thread). Missing `AppDomain.CurrentDomain.UnhandledException` and `TaskScheduler.UnobservedTaskException`.
- **Failure Type**: Silent / Crash
- **Why**: Exceptions on ThreadPool threads terminate the process instantly without UI notification.
- **Trigger**: Any unawaited Task or worker thread exception (e.g., DB Connect failure on background).

### [HIGH] Missing Custom Program.cs
- **Line**: N/A (File Missing)
- **Finding**: WinUI 3 project lacks logical entry point (`Program.cs`) to hook `AppDomain` exceptions *before* XAML runtime spins up.
- **Failure Type**: Bypass
- **Why**: Startup crashes (assembly load, jitter) might occur before `App()` constructor, leading to silent exit.

### [HIGH] Native MessageBox Reliability
- **Line**: 176, 285
- **Finding**: Uses `DllImport("user32.dll")` for fatal error dialogs.
- **Failure Type**: Risk of Silent Failure
- **Why**: If the window station is shutting down or the process is in a restricted state (e.g. stack overflow), this native call might fail, leading to no UI feedback.

## Magidesk.Api/Program.cs
### [MEDIUM] Missing Global Exception Middleware
- **Line**: 18-22
- **Finding**: Only configures `UseSwagger` in Development. Missing `app.UseExceptionHandler()` for Production.
- **Failure Type**: Silent / 500
- **Why**: In production, unhandled exceptions will result in generic 500 errors without structured logging or client-safe error details unless relying on default ASP.NET behavior which might be insufficient for POS resilience.

## OrderEntryViewModel.cs
### [BLOCKER] Systemic Silent Failures (Command Handlers)
- **Lines**: 738, 787, 812, 906, 930, 950, 1029, 1225, 1281, 1326, 1362
- **Finding**: Async methods (PayNow, Settle, Print, etc.) catch `Exception` but only log to `Debug.WriteLine`.
- **Failure Type**: Silent Failure
- **Why**: Critical operations fail without notifying the user, leading to financial loss (unprocessed payments), operational chaos (kitchen not notified), or confused users (item not deleted).
- **Trigger**: Any exception during these operations.

### [HIGH] AddItemAsync Crash Potential
- **Line**: 1036
- **Finding**: `AddItemAsync` lacks top-level try/catch.
- **Failure Type**: Crash
- **Why**: If adding an item fails (DB constraint, network), `AsyncRelayCommand` will propagate the exception to the UI thread, crashing the app (handled by global handler if present, but currently creates a crash dialog -> exit).

## SettleViewModel.cs
### [HIGH] Soft Silent Failures (Property Only)
- **Lines**: 267, 330, 448
- **Finding**: Catches exceptions and sets `Error` string property.
- **Failure Type**: UI Visibility Risk
- **Why**: Logic relies on View binding to `Error`. If View does not display it prominently (or user ignores text), financial errors (Payment Failed) are missed.
- **Trigger**: Payment Gateway Timeout, DB disconnect.

### [HIGH] Unsafe Background Task
- **Line**: 486
- **Finding**: `Task.Run(...)` fire-and-forget without inner try/catch.
- **Failure Type**: Crash / Process Termination
- **Why**: If the lambda throws before dispatching, the background thread crashes the process (due to missing global handlers).

## NavigationService.cs
### [HIGH] Silent Dialog Failure
- **Line**: 80, 94
- **Finding**: If XamlRoot is missing or `ShowAsync` throws, returns `ContentDialogResult.None` and logs to Debug.
- **Failure Type**: Silent Failure
- **Why**: User expects a dialog (e.g. "Confirm Payment"). Nothing happens. Caller thinks user cancelled.
- **Trigger**: Race condition during navigation or window resizing.

### [MEDIUM] Silent Navigation Block
- **Line**: 56
- **Finding**: Returns `false` if `CurrentUser` is null (except for Login page).
- **Failure Type**: Silent Bypass
- **Why**: Method returns bool, but most ViewModels call it via `RelayCommand` which ignores the return value. Button clicks result in no action.

## PrintingService.cs
### [MEDIUM] Synchronous Print Blocking
- **Line**: 69
- **Finding**: `printDoc.Print()` is called on the UI thread.
- **Failure Type**: UI Freeze
- **Why**: Large print jobs or driver delays will freeze the POS UI processing.

## KitchenPrintService.cs
### [BLOCKER] Silent Print Failure
- **Line**: 146
- **Finding**: Catches exception, logs error, returns `false`.
- **Failure Type**: Silent Failure
- **Why**: `OrderEntryViewModel` calls routing service which calls this. The return value is ignored. Kitchen never gets the order, FOH assumes sent.
- **Trigger**: Network error, Printer Offline.

### [HIGH] Missing Mapping Silence
- **Line**: 100
- **Finding**: Logs warning if printer mapping missing. Returns partial success (false for that group).
- **Failure Type**: Silent Failure
- **Why**: Config errors result in missing items in kitchen with no feedback to user.

### [HIGH] Empty Terminal ID
- **Line**: 66
- **Finding**: Returns false if Terminal ID is empty.
- **Failure Type**: Silent Bypass
- **Why**: If terminal context is lost, printing stops working silently.




