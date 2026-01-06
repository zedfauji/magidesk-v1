# Silent Failure Eliminations

This document tracks the specific code paths where "Silence" was detected and the strategy to eliminate it.

## 1. Startup Silence (F-SA-001)
- **Path**: `App.xaml.cs` -> `HandleFatalStartupError`
- **Issue**: If `MessageBox` (P/Invoke) fails/throws, the catch block swallows it.
- **Fix**: Recursive safety. If `MessageBox` fails, try writing to a "fatal_error.txt" file on Desktop as last resort, or spin a tight loop beep. (Acceptable: just ensuring `catch` doesn't swallow without *trying*).

## 2. The "Red Text" Trap (F-VM-001, F-VM-002)
- **Path**: `LoginViewModel`
- **Issue**: Using `ErrorMessage` property for System Errors (DB Connection) blends in with "Invalid PIN".
- **Fix**: Check Exception type. If `InvalidPin` -> Text. If `Exception` -> Dialog.

## 3. The "Void Command" Trap (F-VM-004..009)
- **Path**: `SwitchboardViewModel` Command Handlers
- **Issue**: `AsyncRelayCommand` delegates catch exceptions to `Debug.WriteLine`.
- **Fix**: All Command Handlers must use `try/catch` that calls `_navigationService.ShowErrorAsync`.

## 4. The "Status Bar" Mirage (F-VM-010..012)
- **Path**: `SystemConfigViewModel`
- **Issue**: Errors written to a Status Bar at the bottom of a scrolling page are effectively invisible.
- **Fix**: Critical actions (Save/Restore) must be **Modal**. Success/Fail = Dialog.
