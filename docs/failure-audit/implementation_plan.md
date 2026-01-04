# Failure Handling Implementation Plan

## Goal
Eliminate silent failures and unhandled crashes by implementing a robust, layered exception handling strategy.

## Phase 4.1: Infrastructure Foundation
1.  **Global Dialog Service**: Implement `IDialogService` and `WindowsDialogService` using `ContentDialog` that is robust against `XamlRoot` issues.
2.  **Safe Command Wrapper**: Create `SafeAsyncRelayCommand` or similar utility that wraps execution in `try-catch` and logs/shows dialogs automatically.
3.  **Global Handler Hardening**: Fix `App.xaml.cs` to ensure `UnhandledException` *actually* logs, using paranoid try-catch blocks and pure .NET file I/O where possible.

## Phase 4.2: Critical Fixes (Blockers)
1.  **Fix Async Void**: Refactor `MainWindow.OnItemInvoked` to capture exceptions.
2.  **Fix Navigation**: Wrap `NavigationService.ShowDialogAsync` in logic that handles timeout/null-root gracefully (e.g., retry or log-and-abort, don't crash).
3.  **Fix Dispatcher**: Ensure `DispatcherQueue.TryEnqueue` lambdas are safe.

## Phase 4.3: Pattern Application
1.  **Repository Layer**: Ensure `DbContext` exceptions are not swallowed unless specifically handling concurrency (already mostly done).
2.  **ViewModel Layer**: Review all Constructor `async void` or fire-and-forget calls (e.g. `LoadData`) and ensure they use a tailored `SafeFireAndForget` extension method.

## Phase 4.4: Verification
1.  **Crash Test**: Intentionally throw exceptions in:
    - Constructor
    - Async Command
    - Background Timer
    - Navigation
2.  **Verify**: Log entry created AND Dialog shown (if applicable).

## Proposed Architecture

### IDialogService
```csharp
public interface IDialogService {
    Task ShowErrorAsync(string title, string message, string exceptionDetails = null);
    Task ShowWarningAsync(string title, string message);
}
```

### App_UnhandledException
```csharp
private void App_UnhandledException(...) {
    try {
        // 1. Log to File (Paranoid Mode)
        LogToDisk(e.Exception);
    } catch { /* Absolute failure of logging */ }

    // 2. Try to show Unhandled Exception Dialog
    // (requires careful window handling)
}
```
