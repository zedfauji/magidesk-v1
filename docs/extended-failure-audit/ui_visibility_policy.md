# Unified UI Visibility Policy
## Canonical Rule Set for Operator Notifications

**Authority**: Audit Convergence & Enforcement  
**Status**: ENFORCED  
**Last Updated**: 2026-01-06

---

## POLICY STATEMENT

**Every failure MUST be visible to the operator.**

Logging ≠ Handling. Debug output ≠ Operator notification.

---

## VISIBILITY MATRIX

| Failure Type | UI Element | Behavior | Blocking | Example |
|--------------|-----------|----------|----------|---------|
| **Startup Failure** | FATAL DIALOG | Show + Exit(1) | YES | DI resolution failure |
| **Action Failure** | ERROR DIALOG | Show + Return | YES | Create ticket failed |
| **Background Exception** | ERROR BANNER | Persist + Details | NO | Unobserved task exception |
| **Degraded Service** | WARNING BANNER | Persist + Dismiss | NO | API connection lost |
| **Validation Failure** | ERROR DIALOG | Show + Return | YES | Invalid discount code |
| **Concurrency Conflict** | ERROR DIALOG | Show + Refresh | YES | Entity modified by another user |
| **Informational State** | TOAST | Auto-dismiss | NO | Ticket saved |

---

## RULE 1: Startup Failures → FATAL DIALOG

### When
- DI container resolution fails
- Database connection fails
- Critical service initialization fails
- Configuration validation fails

### UI Element
**ContentDialog** with:
- Title: "Startup Failed"
- Message: User-friendly error description
- Primary Button: "Exit" (only option)

### Behavior
```csharp
ShowFatalDialog("Startup Failed", errorMessage);
Environment.Exit(1); // TERMINATES APP
```

### Examples
**ENFORCED**:
- App.xaml.cs: Service validation failure (TICKET-004)
- App.xaml.cs: InitializeSystemAsync failure (TICKET-002)
- Program.cs: Database connection failure (TICKET-005)

---

## RULE 2: Action Failures → ERROR DIALOG

### When
- User-initiated action fails
- Command handler throws exception
- Business rule violation
- Validation failure

### UI Element
**ContentDialog** with:
- Title: Operation name (e.g., "Create Ticket Failed")
- Message: User-friendly error + technical details
- Primary Button: "OK"

### Behavior
```csharp
await _navigationService.ShowErrorAsync(
    "Create Ticket Failed", 
    $"Critical Error creating ticket:\n{ex.Message}\n\nPlease check database connection.");
```

### Examples
**ENFORCED**:
- SwitchboardViewModel.NewTicketAsync (Line 324-335)
- SwitchboardViewModel.LoadTicketsAsync (Line 346-351)
- SwitchboardViewModel.PerformDrawerOperationAsync (Line 515-519)

---

## RULE 3: Background Exceptions → ERROR BANNER

### When
- Unhandled exception in background thread
- Unobserved task exception
- AppDomain.UnhandledException
- TaskScheduler.UnobservedTaskException

### UI Element
**InfoBar** (persistent) with:
- Severity: Error
- Title: "Background Error Occurred"
- Message: Exception message
- Action Button: "Details" (shows full stack trace)
- IsClosable: True

### Behavior
```csharp
// App.xaml.cs Lines 329-370
public static void HandleCriticalException(Exception ex) {
    MessageBox.Show(ex.Message); // Immediate notification
    MainWindow.ShowErrorBanner(ex); // Persistent UI
}
```

### Examples
**ENFORCED**:
- App.xaml.cs: UnhandledException handler (TICKET-003)
- App.xaml.cs: UnobservedTaskException handler (TICKET-003)
- MainWindow.xaml: ErrorBanner InfoBar (TICKET-003)

---

## RULE 4: Degraded Services → WARNING BANNER

### When
- API connection lost (but app continues)
- Printer offline (but orders can be taken)
- Payment gateway unavailable (but cash accepted)

### UI Element
**InfoBar** (persistent) with:
- Severity: Warning
- Title: Service name
- Message: "Service unavailable. Some features may be limited."
- IsClosable: True

### Behavior
```csharp
MainWindow.ShowWarningBanner("Payment Gateway Offline", 
    "Credit card payments unavailable. Cash and checks only.");
```

### Examples
**NOT YET IMPLEMENTED** (future enhancement)

---

## RULE 5: Validation Failures → ERROR DIALOG

### When
- Invalid input data
- Business rule violation (non-critical)
- Discount not eligible
- Coupon code invalid

### UI Element
**ContentDialog** with:
- Title: "Validation Failed"
- Message: User-friendly explanation
- Primary Button: "OK"

### Behavior
```csharp
await _navigationService.ShowErrorAsync(
    "Invalid Discount", 
    "Discount is not eligible for this item.");
```

### Examples
**ENFORCED**:
- ApplyDiscountCommandHandler throws BusinessRuleViolationException
- ViewModels catch and surface via ShowErrorAsync (VERIFIED)

---

## RULE 6: Concurrency Conflicts → ERROR DIALOG

### When
- DbUpdateConcurrencyException
- Entity modified by another process
- Optimistic concurrency violation

### UI Element
**ContentDialog** with:
- Title: "Concurrency Conflict"
- Message: "Entity was modified by another process. Please refresh and try again."
- Primary Button: "OK"

### Behavior
```csharp
// Repository catches DbUpdateConcurrencyException
throw new Domain.Exceptions.ConcurrencyException(
    "Entity was modified by another process. Please refresh and try again.", 
    ex);

// ViewModel catches ConcurrencyException
await _navigationService.ShowErrorAsync(
    "Concurrency Conflict", 
    ex.Message);
```

### Examples
**ENFORCED**:
- TicketRepository.UpdateAsync (Line 245-250)
- CashSessionRepository.UpdateAsync (Line 78-83)
- (6 repositories total)

---

## RULE 7: Informational States → TOAST

### When
- Operation completed successfully
- Non-critical status update
- Informational message

### UI Element
**TeachingTip** or **Toast** with:
- Auto-dismiss after 3-5 seconds
- Non-blocking
- Optional action button

### Behavior
```csharp
ShowToast("Ticket Saved", "Ticket #1234 saved successfully.");
```

### Examples
**NOT YET IMPLEMENTED** (future enhancement)

---

## ANTI-PATTERNS (FORBIDDEN)

### ❌ Log-Only Error Handling
```csharp
// VIOLATION
catch (Exception ex) {
    _logger.LogError(ex, "Operation failed");
    // NO UI NOTIFICATION
}
```

**REQUIRED**:
```csharp
catch (Exception ex) {
    _logger.LogError(ex, "Operation failed");
    await ShowErrorAsync("Operation Failed", ex.Message); // ✅
}
```

### ❌ Debug-Only Error Handling
```csharp
// VIOLATION
catch (Exception ex) {
    Debug.WriteLine($"Error: {ex.Message}");
    // NO UI NOTIFICATION
}
```

**REQUIRED** (for converters):
```csharp
catch (Exception ex) {
    Debug.WriteLine($"Error: {ex.Message}"); // Developer visibility
    return SafeFallbackValue; // ✅ Visible to operator
}
```

### ❌ Silent Fallbacks
```csharp
// VIOLATION
catch (Exception ex) {
    return Transparent; // Invisible to operator
}
```

**REQUIRED**:
```csharp
catch (Exception ex) {
    Debug.WriteLine($"Error: {ex.Message}");
    return Colors.LightGray; // ✅ Visible fallback
}
```

---

## IMPLEMENTATION CHECKLIST

### For Every Exception Handler

- [ ] Is the error logged? (optional but recommended)
- [ ] Is the error visible to the operator? (MANDATORY)
- [ ] Is the error message user-friendly? (MANDATORY)
- [ ] Does the UI element match the failure type? (MANDATORY)
- [ ] Is the behavior appropriate (blocking vs non-blocking)? (MANDATORY)

### Visibility Verification

**Question**: If this exception occurs, will the operator know?

- ✅ YES → Compliant
- ❌ NO → VIOLATION

---

## ENFORCEMENT

### Structural (Cannot Be Bypassed)
1. ✅ Global exception handlers show error banner (TICKET-003)
2. ✅ ViewModels catch and surface via ShowErrorAsync (VERIFIED)
3. ✅ Repositories wrap concurrency exceptions (VERIFIED)

### Procedural (Requires Discipline)
1. ⚠️ Code review enforces UI visibility
2. ⚠️ Static analysis detects log-only handling
3. ⚠️ Testing verifies operator visibility

---

## SUMMARY

| UI Element | Use Case | Blocking | Auto-Dismiss |
|------------|----------|----------|--------------|
| FATAL DIALOG | Startup failures | YES | NO (Exit app) |
| ERROR DIALOG | Action failures | YES | NO (User dismisses) |
| ERROR BANNER | Background exceptions | NO | NO (User dismisses) |
| WARNING BANNER | Degraded services | NO | NO (User dismisses) |
| TOAST | Informational | NO | YES (3-5 seconds) |

**GOLDEN RULE**: Every failure MUST be visible to the operator.

---

**Status**: ENFORCED  
**Violations**: FORBIDDEN  
**Compliance**: MANDATORY
