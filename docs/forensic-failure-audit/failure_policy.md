# System Failure Policy

## Core Axioms
1.  **No Silence**: Every error that interrupts a user workflow MUST result in a UI notification (Dialog, Toast, or Banner).
2.  **Fail Safe**: If state cannot be guaranteed (e.g. partial payment), the system must explicitly block further actions until resolved or reset.
3.  **Process Integrity**: Background threads must never crash the process silently. Global handlers must intercept and log/notify.

## Exception Handling Strategy

### 1. Global Safety Net (App.xaml.cs, Program.cs)
-   **Requirement**: Bind `AppDomain.CurrentDomain.UnhandledException`.
-   **Requirement**: Bind `TaskScheduler.UnobservedTaskException`.
-   **Behavior**:
    -   Log Fatal Error to Disk (`crash_log.txt`).
    -   Attempt to show Native Fatal Dialog.
    -   Terminate Process (Reset).

### 2. ViewModel Command Handlers
-   **Requirement**: All `AsyncRelayCommand` delegates must be wrapped in a specific `SafeExecuteAsync` helper or equivalent top-level `try/catch`.
-   **Behavior**:
    -   Catch `Exception`.
    -   Log `Exception`.
    -   **Show Retry/Dismiss Dialog** to User via `NavigationService`.
    -   Do **NOT** swallow.

### 3. Service Layer
-   **Requirement**: Throw exceptions for recoverable failures (Auth, IO, DB) so ViewModels can decide flow.
-   **Exception**: If returning `bool` (Success/Fail), the failure reason MUST be exposed (out param, Result object, or property) AND checked by the caller.
-   **Preferred**: Return `Result<T>` pattern instead of throwing for expected flow deviations.

### 4. Background Tasks
-   **Requirement**: `Task.Run` must have a top-level `try/catch` inside the lambda.
-   **Behavior**: On error, dispatch to UI thread to notify user if possible, or log to critical audit log.

## Dialog Standards
-   **Critical/Blocker**: Modal `ContentDialog`, user must acknowledge ("OK").
-   **Retryable**: `ContentDialog` with "Retry" / "Cancel".
-   **Info**: Toast/Banner (Non-blocking).

## Verification Checklist
- [ ] Is exception caught?
- [ ] Is user notified?
- [ ] Is state consistent?
- [ ] Is it logged?
