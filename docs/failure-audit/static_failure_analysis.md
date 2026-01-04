# Static Failure Analysis

## Methodology
Codebase scan using `grep` for known anti-patterns:
- `async void`
- `catch { }` (Empty Catch)
- `throw new Exception` (Generic Throw)

## Findings

### 1. Async Void (Crash Risk)
`async void` methods cannot be awaited and exceptions thrown inside them will crash the application immediately, bypassing the TaskScheduler.

| File | Line | Method | Status | Mitigation |
| :--- | :--- | :--- | :--- | :--- |
| `App.xaml.cs` | 153 | `OnLaunched` | **MITIGATED** | Wrapped in `try-catch` block. |
| `MainWindow.xaml.cs` | 101 | `OnItemInvoked` | **CRITICAL** | NO error handling. Navigation errors will CRASH the app. |
| `ShiftStartDialog.xaml.cs` | 18 | `ShiftStartDialog_Opened` | **UNKNOWN** | Needs inspection. Likely missing handler. |
| `TableDesignerPage.xaml.cs` | 25 | `OnNavigatedTo` | **UNKNOWN** | Needs inspection. |

### 2. Empty Catch Blocks (Silent Failure)
Exceptions swallowed here leave no trace in logs.

| File | Line | Context | Status | Justification |
| :--- | :--- | :--- | :--- | :--- |
| `App.xaml.cs` | 240 | `App_UnhandledException` | **CRITICAL** | If logging fails (e.g. permission deny), the original crash is hidden. app exits silently. |
| `StartupLogger.cs` | N/A | Logging fallback | **ACCEPTABLE** | Preventing log failure from crashing app. |
| `LoginViewModel.cs` | 51 | `ShutdownCommand` | **ACCEPTABLE** | Best-effort exit. |

### 3. Fire-and-Forget (Race Conditions)
tasks launched without `await` or result observation.

| File | Line | Context | Status |
| :--- | :--- | :--- | :--- |
| `MainWindow.xaml.cs` | 48 | `DispatcherQueue.TryEnqueue` | **RISK** | Lambda passed to dispatcher is fire-and-forget. Exceptions inside `UpdateUiAuthState` are unobserved. |
