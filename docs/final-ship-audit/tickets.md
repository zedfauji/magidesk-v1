# Final Audit Tickets

## Critical System Infrastructure

### T-001: Safe-Fail Startup Handlers (F-SA-001)
- **File**: `App.xaml.cs`
- **Goal**: Ensure `HandleFatalStartupError` cannot fail silently.
- **Requirement**: Wrap P/Invoke `MessageBox` in `try/catch`. If it fails, fallback to `System.IO.File.WriteAllText` on Desktop "FATAL_ERROR.txt".
- **Severity**: CRITICAL

## Login & Auth

### T-002: Login System Error Visibility (F-VM-002)
- **File**: `LoginViewModel.cs`
- **Goal**: Differentiate partial auth failure from system crash.
- **Requirement**: If `Exception` is NOT `InvalidCredentials` (or generic logic), show `ShowErrorAsync` dialog. Keep `ErrorMessage` only for user mistakes.
- **Severity**: HIGH

### T-003: Clock-In Error Visibility (F-VM-001)
- **File**: `LoginViewModel.cs`
- **Goal**: Ensure staff knows if Clock-In failed.
- **Requirement**: If `ClockInHandler` throws, show Blocking Dialog.
- **Severity**: HIGH

### T-004: Switchboard Clock-In Visibility (F-VM-009)
- **File**: `SwitchboardViewModel.cs`
- **Goal**: Same as T-003 but for Switchboard context.
- **Requirement**: Add `try/catch` with `ShowErrorAsync`.
- **Severity**: HIGH

### T-005: Nav Routing Fallback Feedback (F-VM-003)
- **File**: `LoginViewModel.cs`
- **Goal**: Inform user if Default View is broken.
- **Requirement**: If `Navigate` throws or fallback triggers, Show `ShowMessageAsync` "Default view unavailable...".
- **Severity**: MEDIUM

## Switchboard Operations (BLOCKERS)

### T-006: Create Ticket Failure (F-VM-006)
- **File**: `SwitchboardViewModel.cs`
- **Goal**: Prevent "Click New Ticket -> Nothing Happens".
- **Requirement**: `NewTicketAsync` catch block MUST use `ShowErrorAsync` with "Failed to create ticket".
- **Severity**: BLOCKER

### T-007: Drawer Operations Failure (F-VM-008)
- **File**: `SwitchboardViewModel.cs`
- **Goal**: Prevent silent failure of Cash Drops / Open Drawer.
- **Requirement**: All Drawer commands (`PerformOpenDrawer`, `ShowDrawerBalance`, `PerformDrawerOperation`) MUST use `ShowErrorAsync`.
- **Severity**: BLOCKER

### T-008: Admin/Manager Auth Failure (F-VM-004, F-VM-005)
- **File**: `SwitchboardViewModel.cs`
- **Goal**: Ensure Adminknows why they are denied.
- **Requirement**: `BackOfficeAsync` and `ManagerFunctionsAsync` catch blocks must show Dialog.
- **Severity**: HIGH

### T-009: Switchboard Ticket Load Failure (F-VM-007)
- **File**: `SwitchboardViewModel.cs`
- **Goal**: Ensure user knows if ticket list is stale.
- **Requirement**: `LoadTicketsAsync` catch block -> Warning Banner or Dialog.
- **Severity**: HIGH

## System Configuration

### T-010: Backup Restore Visibility (F-VM-012)
- **File**: `SystemConfigViewModel.cs`
- **Goal**: Explicit Pass/Fail for critical data restore.
- **Requirement**: `RestoreBackupAsync` catch block -> Error Dialog. detailed message.
- **Severity**: BLOCKER

### T-011: Config Save Visibility (F-VM-011)
- **File**: `SystemConfigViewModel.cs`
- **Goal**: Ensure setting saves are confirmed.
- **Requirement**: `Save...` commands -> Success Dialog or Error Dialog. No Status Bar.
- **Severity**: HIGH

### T-012: Config Load Visibility (F-VM-010)
- **File**: `SystemConfigViewModel.cs`
- **Goal**: Ensure blank screen is explained.
- **Requirement**: `Load...` methods -> Error Dialog if DB fails.
- **Severity**: MEDIUM
