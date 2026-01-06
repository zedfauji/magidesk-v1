# EXTENDED FORENSIC FAILURE AUDIT - ASYNC FAILURE MAP

## CRITICAL ASYNC FAILURES

### A001: App.xaml.cs - ASYNC VOID ONLAUNCHED
**File**: App.xaml.cs  
**Method**: OnLaunched  
**Line**: 179  
**Async Pattern**: async void (DANGEROUS)  
**Failure Type**: App Crash  
**Trigger**: Any exception in OnLaunched  
**Current Behavior**: Unhandled exception crashes app  
**Current Visibility**: CRASH  
**Required Visibility**: FATAL DIALOG  
**Severity**: BLOCKER  
**Evidence**: `protected override async void OnLaunched(LaunchActivatedEventArgs args)`

### A002: MainWindow.xaml.cs - ASYNC VOID NAVIGATION
**File**: MainWindow.xaml.cs  
**Method**: OnItemInvoked  
**Line**: 77  
**Async Pattern**: async void (DANGEROUS)  
**Failure Type**: App Crash  
**Trigger**: Exception during navigation  
**Current Behavior**: Unhandled exception crashes app  
**Current Visibility**: CRASH  
**Required Visibility**: ERROR DIALOG  
**Severity**: BLOCKER  
**Evidence**: `private async void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)`

### A003: MainWindow.xaml.cs - DISPATCHER QUEUE ASYNC LAMBDA
**File**: MainWindow.xaml.cs  
**Method**: UserChanged event handler  
**Line**: 46  
**Async Pattern**: Async lambda in dispatcher  
**Failure Type**: Silent Exception  
**Trigger**: Exception in UI update  
**Current Behavior**: Exception caught and logged only  
**Current Visibility**: LOG only  
**Required Visibility**: WARNING BANNER  
**Severity**: MEDIUM  
**Evidence**: `DispatcherQueue.TryEnqueue(() => { try { UpdateUiAuthState(u); } catch (Exception ex) { StartupLogger.Log($"Auth UI Update Failed: {ex}"); } });`

### A004: OrderEntryViewModel.cs - FIRE-AND-FORGET ASYNC OPERATIONS
**File**: ViewModels/OrderEntryViewModel.cs  
**Methods**: LoadGroupsAsync, LoadItemsAsync  
**Lines**: Multiple  
**Async Pattern**: Fire-and-forget (DANGEROUS)  
**Failure Type**: Silent Crash  
**Trigger**: Exception in background loading  
**Current Behavior**: Exceptions lost, state inconsistent  
**Current Visibility**: NONE  
**Required Visibility**: ERROR DIALOG  
**Severity**: BLOCKER  
**Evidence**: `_ = LoadGroupsAsync(value);` and `_ = LoadItemsAsync(value);`

### A005: LoginViewModel.cs - ASYNC COMMAND WITHOUT OBSERVATION
**File**: ViewModels/LoginViewModel.cs  
**Methods**: ClockInOutAsync, LoginAsync  
**Lines**: 85, 130  
**Async Pattern**: AsyncRelayCommand (proper)  
**Failure Type**: Partial Exception Handling  
**Trigger**: Exception in async operations  
**Current Behavior**: Some exceptions handled, but not all surfaced properly  
**Current Visibility**: UI (partial)  
**Required Visibility**: ERROR DIALOG  
**Severity**: MEDIUM  
**Evidence**: Try-catch blocks that only set ErrorMessage property

### A006: PaymentViewModel.cs - ASYNC COMMAND WITH LIMITED ERROR HANDLING
**File**: ViewModels/PaymentViewModel.cs  
**Methods**: LoadTicketAsync, CashPayAsync  
**Lines**: 95, 115  
**Async Pattern**: AsyncRelayCommand (proper)  
**Failure Type**: Partial Exception Handling  
**Trigger**: Exception in payment operations  
**Current Behavior**: Errors shown in property only  
**Current Visibility**: UI (partial)  
**Required Visibility**: ERROR DIALOG  
**Severity**: MEDIUM  
**Evidence**: `catch (Exception ex) { Error = ex.Message; }`

---

## ASYNC FAILURE PATTERNS

### PATTERN A1: ASYNC VOID METHODS
**Root Cause**: async void methods don't allow proper exception handling
**Impact**: Exceptions crash app without proper handling
**Files**: App.xaml.cs, MainWindow.xaml.cs
**Fix Required**: Convert to async Task and add proper exception handling

### PATTERN A2: FIRE-AND-FORGET ASYNC
**Root Cause**: Async operations not awaited or observed
**Impact**: Silent failures in background
**Files**: OrderEntryViewModel.cs
**Fix Required**: Proper async/await with exception handling

### PATTERN A3: INSUFFICIENT ASYNC ERROR HANDLING
**Root Cause**: Async operations catch exceptions but don't surface properly
**Impact**: Users may not see critical errors
**Files**: LoginViewModel.cs, PaymentViewModel.cs
**Fix Required**: All async exceptions must surface to UI with dialogs

---

## ASYNC BEST PRACTICES VIOLATIONS

### VIOLATION V1: ASYNC VOID
**Pattern**: `async void MethodName()`
**Risk**: Exceptions cannot be caught
**Count**: 2 instances
**Fix**: Change to `async Task MethodName()`

### VIOLATION V2: FIRE-AND-FORGET
**Pattern**: `var task = MethodAsync();` without await
**Risk**: Exceptions lost
**Count**: Multiple instances
**Fix**: `await MethodAsync();` or proper observation

### VIOLATION V3: ASYNC EXCEPTION SWALLOWING
**Pattern**: try-catch that only logs or sets property
**Risk**: Users unaware of failures
**Count**: Multiple instances
**Fix**: Surface all exceptions to UI with appropriate dialogs

---

**TOTAL ASYNC FAILURES**: 6
**BLOCKER ASYNC FAILURES**: 3
**MEDIUM ASYNC FAILURES**: 3

**ASYNC SAFETY ASSESSMENT**: CRITICAL - Multiple async patterns that can cause silent crashes