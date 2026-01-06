# Phase 2b: ViewModel Analysis - Initial Findings
## Extended Forensic Failure Audit

**Analysis Date**: 2026-01-06  
**Scope**: 71 ViewModels  
**Status**: 🔄 IN PROGRESS (Pattern Analysis Complete)

---

## PATTERN SEARCH RESULTS

### Pattern 1: Async Void Methods
**Total Found**: 2 occurrences  
**Severity**: HIGH  
**Risk**: Exceptions cannot be caught by caller, silent failures

#### F-VM-001: SwitchboardViewModel.DrawerPull (HIGH)
**File**: `ViewModels/SwitchboardViewModel.cs`  
**Line**: 376  
**Code**:
```csharp
private async void DrawerPull()
{
    var dialog = new Magidesk.Presentation.Views.DrawerPullReportDialog();
    dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
    await _navigationService.ShowDialogAsync(dialog);
}
```

**Issue**: Async void method with NO exception handling  
**Risk**: If ShowDialogAsync throws, exception is unhandled  
**Current Visibility**: NONE (crashes app or triggers global handler)  
**Required Fix**: Wrap in try-catch OR convert to AsyncRelayCommand

---

#### F-VM-002: NotesDialogViewModel.Save (MEDIUM - Has Try-Catch)
**File**: `ViewModels/Dialogs/NotesDialogViewModel.cs`  
**Line**: 57  
**Code**:
```csharp
private async void Save()
{
    if (_ticketId == null) return;
    try
    {
        IsBusy = true;
        // ... command execution ...
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
    catch (Exception)
    {
        // Simple failure handling for now
        // Cannot use HasError/ErrorMessage as they are not in ViewModelBase
    }
    finally
    {
        IsBusy = false;
    }
}
```

**Issue**: Async void with try-catch BUT empty catch block (silent failure)  
**Risk**: Operator has no indication save failed  
**Current Visibility**: NONE (exception swallowed)  
**Required Fix**: Set error property or show dialog in catch block

---

### Pattern 2: Task.Run Usage
**Total Found**: 1 occurrence  
**Severity**: MEDIUM  
**Risk**: Fire-and-forget, exceptions may be unobserved

#### F-VM-003: SettleViewModel.TestWaitAsync (MEDIUM - Has Try-Catch)
**File**: `ViewModels/SettleViewModel.cs`  
**Line**: 505  
**Code**:
```csharp
var closeTask = Task.Run(async () =>
{
    try
    {
        await Task.Delay(3000);
        _navigationService.DispatcherQueue.TryEnqueue(() =>
        {
            dialog.ViewModel.AllowClose();
            dialog.Hide();
        });
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"TestWaitAsync bg error: {ex}");
    }
});
```

**Issue**: Task.Run is fire-and-forget (not awaited)  
**Risk**: If exception occurs, it's only logged to Debug (invisible to operator)  
**Current Visibility**: Debug output only  
**Required Fix**: Either await the task OR add to tracked background tasks

---

### Pattern 3: Exception Handling
**Total Found**: 126+ catch blocks across ViewModels  
**Analysis**: Most have proper error handling (set Error property)

#### Good Pattern (Majority):
```csharp
catch (Exception ex)
{
    Error = ex.Message;  // ✅ GOOD - Operator sees error
}
```

#### Bad Pattern Found: 1 occurrence
**File**: `SwitchboardViewModel.cs`  
**Line**: 131  
**Code**:
```csharp
ShutdownCommand = new RelayCommand(() => { 
    try { 
        Microsoft.UI.Xaml.Application.Current.Exit(); 
    } catch {} 
});
```

**Issue**: Empty catch block on app shutdown  
**Risk**: Shutdown failure is silent  
**Severity**: LOW (shutdown is best-effort anyway)

---

## FINDINGS SUMMARY

| Finding ID | File | Severity | Category | Status |
|------------|------|----------|----------|--------|
| F-VM-001 | SwitchboardViewModel | HIGH | Async Void (no try-catch) | 📋 NEW |
| F-VM-002 | NotesDialogViewModel | MEDIUM | Async Void (empty catch) | 📋 NEW |
| F-VM-003 | SettleViewModel | MEDIUM | Fire-and-Forget Task.Run | 📋 NEW |
| F-VM-004 | SwitchboardViewModel | LOW | Empty Catch (shutdown) | 📋 NEW |

---

## POSITIVE FINDINGS

### ✅ Good Exception Handling Patterns
- **126+ catch blocks** properly set Error property
- **AsyncRelayCommand** used extensively (good async pattern)
- **IsBusy** flags used consistently
- **Error/StatusMessage** properties used for operator feedback

### ✅ No ConfigureAwait(false) Issues
- Zero occurrences found (GOOD)
- All async operations stay on UI thread
- No deadlock risks

---

## ESTIMATED REMAINING WORK

**ViewModels Analyzed**: Pattern search complete (71 files scanned)  
**Deep Analysis Required**: 4 files with findings  
**Estimated Additional Findings**: 5-10 (from deep analysis)

### Next Steps
1. Deep analysis of 4 files with findings
2. Sample analysis of high-risk ViewModels (OrderEntryViewModel, PaymentViewModel)
3. Generate tickets for findings

---

## RECOMMENDATIONS

### Immediate Action
1. **F-VM-001**: Add try-catch to SwitchboardViewModel.DrawerPull
2. **F-VM-002**: Add error visibility to NotesDialogViewModel.Save catch block

### Pattern Fix
Create standard async void wrapper pattern:
```csharp
private async void MethodName()
{
    try
    {
        // Async logic
    }
    catch (Exception ex)
    {
        Error = ex.Message;
        // OR show dialog for critical failures
    }
}
```

---

**Audit Status**: Phase 2b Pattern Analysis COMPLETE | Deep Analysis PENDING  
**Last Updated**: 2026-01-06 12:15 CST
