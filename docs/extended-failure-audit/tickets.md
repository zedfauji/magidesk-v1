# EXTENDED FORENSIC FAILURE AUDIT - GRANULAR TICKETS

## BLOCKER TICKETS (Must be fixed first)

### T001: ASYNC VOID ONLAUNCHED CRASH
**Ticket ID**: T001  
**Finding ID**: F001  
**Severity**: BLOCKER  
**Area**: Backend (App.xaml.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: NO  
**Required UI Surfacing**: FATAL DIALOG  
**Exact Behavior After Fix**: OnLaunched exceptions show error dialog and allow graceful app termination  
**Verification Steps**: 
1. Cause exception during app launch
2. Verify error dialog appears instead of crash
3. Verify dialog provides meaningful error information

### T002: SERVICE RESOLUTION CRASH
**Ticket ID**: T002  
**Finding ID**: F003  
**Severity**: BLOCKER  
**Area**: Backend (App.xaml.cs, MainWindow.xaml.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: NO  
**Required UI Surfacing**: FATAL DIALOG  
**Exact Behavior After Fix**: Service resolution failures show error dialog instead of crashing app  
**Verification Steps**:
1. Remove or misconfigure a critical service registration
2. Start app
3. Verify error dialog appears instead of crash

### T003: FIRE-AND-FORGET SILENT CRASHES
**Ticket ID**: T003  
**Finding ID**: F011  
**Severity**: BLOCKER  
**Area**: Backend (OrderEntryViewModel.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: NO  
**Required UI Surfacing**: ERROR DIALOG  
**Exact Behavior After Fix**: Background async operation failures show error dialog to user  
**Verification Steps**:
1. Trigger background data loading failure
2. Verify error dialog appears instead of silent failure
3. Verify UI state remains consistent

### T004: NULL DEPENDENCY CRASHES
**Ticket ID**: T004  
**Finding ID**: F013  
**Severity**: BLOCKER  
**Area**: Backend (OrderEntryViewModel.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: NO  
**Required UI Surfacing**: FATAL DIALOG  
**Exact Behavior After Fix**: Null dependency detection shows error dialog and prevents component initialization  
**Verification Steps**:
1. Simulate null dependency injection
2. Verify error dialog appears during component creation
3. Verify component fails gracefully

---

## HIGH PRIORITY TICKETS

### T005: ASYNC VOID NAVIGATION CRASH
**Ticket ID**: T005  
**Finding ID**: F005  
**Severity**: HIGH  
**Area**: Frontend (MainWindow.xaml.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: ERROR DIALOG  
**Exact Behavior After Fix**: Navigation exceptions show error dialog instead of crashing app  
**Verification Steps**:
1. Trigger navigation exception
2. Verify error dialog appears
3. Verify app remains stable

### T006: TIMER EXCEPTION CRASH
**Ticket ID**: T006  
**Finding ID**: B001  
**Severity**: HIGH  
**Area**: Frontend (MainWindow.xaml.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: WARNING BANNER  
**Exact Behavior After Fix**: Timer exceptions show warning banner instead of crashing app  
**Verification Steps**:
1. Cause timer callback exception
2. Verify warning banner appears
3. Verify clock shows error state

### T007: CONVERTER SILENT FAILURES
**Ticket ID**: T007  
**Finding ID**: S001  
**Severity**: HIGH  
**Area**: Frontend (All converters)  
**Backend Changes Required**: NO  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: WARNING BANNER  
**Exact Behavior After Fix**: Converter exceptions show warning banner and return fallback value  
**Verification Steps**:
1. Provide invalid input to converter
2. Verify warning banner appears
3. Verify UI shows fallback value

### T008: DIALOG FAILURE SILENT LOGGING
**Ticket ID**: T008  
**Finding ID**: S002, S003  
**Severity**: HIGH  
**Area**: Frontend (NavigationService.cs)  
**Backend Changes Required**: NO  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: ERROR DIALOG  
**Exact Behavior After Fix**: Dialog failures show error dialog instead of silent logging  
**Verification Steps**:
1. Cause dialog system failure
2. Verify error dialog appears
3. Verify user gets clear error message

---

## MEDIUM PRIORITY TICKETS

### T009: AUTH STATE UPDATE LOGGING ONLY
**Ticket ID**: T009  
**Finding ID**: F004  
**Severity**: MEDIUM  
**Area**: Frontend (MainWindow.xaml.cs)  
**Backend Changes Required**: NO  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: WARNING BANNER  
**Exact Behavior After Fix**: Auth state update failures show warning banner  
**Verification Steps**:
1. Cause auth state update exception
2. Verify warning banner appears
3. Verify auth state remains consistent

### T010: STARTUP STATIC ERROR MESSAGES
**Ticket ID**: T010  
**Finding ID**: SS003  
**Severity**: MEDIUM  
**Area**: Backend (App.xaml.cs)  
**Backend Changes Required**: YES  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: ERROR DIALOG  
**Exact Behavior After Fix**: Startup failures show interactive error dialog instead of static message  
**Verification Steps**:
1. Cause startup initialization failure
2. Verify interactive error dialog appears
3. Verify dialog provides recovery options

### T011: PARTIAL ASYNC ERROR HANDLING
**Ticket ID**: T011  
**Finding ID**: F008, A006  
**Severity**: MEDIUM  
**Area**: Frontend (LoginViewModel.cs, PaymentViewModel.cs)  
**Backend Changes Required**: NO  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: ERROR DIALOG  
**Exact Behavior After Fix**: Async operation errors show dialog instead of property-only errors  
**Verification Steps**:
1. Cause async operation failure
2. Verify error dialog appears
3. Verify error message is clear and actionable

---

## LOW PRIORITY TICKETS

### T012: SHUTDOWN ERROR HANDLING
**Ticket ID**: T012  
**Finding ID**: SS009  
**Severity**: LOW  
**Area**: Frontend (LoginViewModel.cs)  
**Backend Changes Required**: NO  
**Frontend Changes Required**: YES  
**Required UI Surfacing**: INFO TOAST  
**Exact Behavior After Fix**: Shutdown failures show info toast notification  
**Verification Steps**:
1. Cause shutdown failure
2. Verify info toast appears
3. Verify user gets shutdown status

---

## TICKET EXECUTION ORDER

### PHASE 1: CRITICAL INFRASTRUCTURE (T001-T004)
1. T001: Fix async void OnLaunched
2. T002: Fix service resolution guards
3. T003: Fix fire-and-forget async operations
4. T004: Fix null dependency checks

### PHASE 2: UI STABILITY (T005-T008)
1. T005: Fix async void navigation
2. T006: Fix timer exception handling
3. T007: Fix converter exception handling
4. T008: Fix dialog failure handling

### PHASE 3: USER EXPERIENCE (T009-T012)
1. T009: Fix auth state update visibility
2. T010: Fix startup error dialogs
3. T011: Fix async error dialog handling
4. T012: Fix shutdown error handling

---

**TOTAL TICKETS**: 12
**BLOCKER TICKETS**: 4
**HIGH TICKETS**: 4
**MEDIUM TICKETS**: 3
**LOW TICKETS**: 1

**EXECUTION STRATEGY**: Fix by severity level, with pattern-level fixes before individual tickets