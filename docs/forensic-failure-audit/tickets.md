# Forensic Remediation Tickets

## Category: STARTUP-EX (Startup & Global Handling)

### T-001: Global Background Exception Handlers
- **Status**: **EXECUTED**
- **Finding ID**: SF-001 (App.xaml.cs)
- **Severity**: CRITICAL
- **Files**: `App.xaml.cs`
- **Requirement**: Hook `AppDomain.CurrentDomain.UnhandledException` and `TaskScheduler.UnobservedTaskException`.
- **Behavior**: Log fatal error to `crash_log.txt`, show native MessageBox, terminate process.
- **Verification**: Throw exception on background thread -> Verify dialog appears -> Verify log file.

## Category: PRINT-EX (Printing Reliability)

### T-002: Kitchen Print Silent Failure
- **Status**: **EXECUTED**
- **Finding ID**: KP-001 (KitchenPrintService.cs)
- **Severity**: BLOCKER
- **Files**: `KitchenPrintService.cs`, `OrderEntryViewModel.cs`
- **Requirement**: `PrintSpecificLinesAsync` must returning detailed Result/Exception, OR ViewModel must check `false` return and show Dialog.
- **Fix Strategy**: Change Service to return `Result<bool>` with Error Message. Update VM to show error dialog if failed.
- **Verification**: Disconnect printer/Simulate error -> Verify Error Dialog appears in Order Entry.

### T-003: Printing Service UI Freeze
- **Status**: **EXECUTED**
- **Finding ID**: PrintingService.cs (Line 69)
- **Severity**: MEDIUM
- **Files**: `PrintingService.cs`
- **Requirement**: Wrap `printDoc.Print()` in `Task.Run` to offload GDI+ blocking call.
- **Verification**: Print large job -> UI remains responsive.

## Category: VM-EX (ViewModel Silent Failures)

### T-004: Order Entry Command Handlers (Payment/Settle)
- **Status**: **EXECUTED**
- **Finding ID**: SF-002, SF-007, SF-009, SF-014 (OrderEntryViewModel.cs)
- **Severity**: BLOCKER
- **Files**: `OrderEntryViewModel.cs`
- **Requirement**: Remove `Debug.WriteLine` swallows. Wrap in `SafeExecuteAsync` or add `try/catch` with `DialogService.ShowError`.
- **Target Methods**: `PayNowAsync`, `SettleAsync`, `PrintTicketAsync`, `QuickPayAsync`.
- **Verification**: Trigger Payment Exception -> Verify Dialog.

### T-005: Order Entry Data Loading
- **Status**: **EXECUTED**
- **Finding ID**: SF-003, SF-005, SF-006, SF-010 (OrderEntryViewModel.cs)
- **Severity**: HIGH
- **Files**: `OrderEntryViewModel.cs`
- **Requirement**: Handle DB errors during `InitializeAsync`, `LoadTicketAsync`. Show "Retry/Cancel" dialog or "System Error" overlay.
- **Verification**: Kill DB -> Open Order Entry -> Verify Error UI (not empty screen).

### T-006: Settle ViewModel Payment Logic
- **Status**: **EXECUTED**
- **Finding ID**: SettleViewModel soft-silence
- **Severity**: HIGH
- **Files**: `SettleViewModel.cs`
- **Requirement**: `ProcessPaymentAsync` must show Modal BlockING Error Dialog on failure, not just set string property.
- **Verification**: Fail payment -> Verify Modal Dialog.

### T-007: Settle ViewModel Background Crash
- **Status**: **EXECUTED**
- **Finding ID**: SettleViewModel Unsafe Task
- **Severity**: CRITICAL
- **Files**: `SettleViewModel.cs`
- **Requirement**: Wrap `Task.Run` logic in try/catch or use `SafeFireAndForget`.
- **Verification**: Force exception in lambda -> Verify App handles gracefully (Dialog) instead of crash log.

## Category: NAV-EX (Navigation)

### T-008: Navigation Service Dialog Failure
- **Status**: **EXECUTED**
- **Finding ID**: NavigationService.cs (Line 80)
- **Severity**: HIGH
- **Files**: `NavigationService.cs`
- **Requirement**: If `ShowDialogAsync` fails (no root), Log AND return specific `Failure` result, or Retry. Do not return `None` silently if meaningful interaction required.
- **Verification**: Call ShowDialog before Window loads -> Verify handled or retried.

### T-009: Navigation Service Auth Bypass
- **Status**: **EXECUTED**
- **Finding ID**: NavigationService.cs (Line 56)
- **Severity**: MEDIUM
- **Files**: `NavigationService.cs`
- **Requirement**: If `Navigate` returns false due to Auth, trigger `ShowError` or `Login` redirection explicitly if not already handling.
- **Verification**: Logout -> Click Secured Button -> Verify Login Prompt/Error (not silence).
