# No Silent Failure Rule

**Category**: MANDATORY  
**Enforcement**: Code Review + Runtime Verification  
**Violations**: BLOCKING

---

## RULE STATEMENT

**Silent crashes are FORBIDDEN.**  
**Log-only handling is FORBIDDEN.**  
**Every failure MUST surface to the operator.**

---

## WHAT IS A SILENT FAILURE?

### Definition
A silent failure occurs when:
1. An exception is caught
2. The error is logged OR ignored
3. **NO UI notification is shown to the operator**

### Examples

#### ❌ VIOLATION: Log-Only Handling
```csharp
try {
    await ProcessPaymentAsync(payment);
} catch (Exception ex) {
    _logger.LogError(ex, "Payment processing failed");
    // NO UI NOTIFICATION - VIOLATION
}
```

#### ❌ VIOLATION: Empty Catch Block
```csharp
try {
    await SaveTicketAsync(ticket);
} catch {
    // Silent failure - VIOLATION
}
```

#### ❌ VIOLATION: Debug-Only Handling
```csharp
try {
    var result = await _handler.HandleAsync(command);
} catch (Exception ex) {
    Debug.WriteLine($"Error: {ex.Message}");
    // NO UI NOTIFICATION - VIOLATION
}
```

---

## WHAT IS REQUIRED?

### Requirement 1: UI Visibility
**EVERY exception MUST be visible to the operator.**

#### ✅ COMPLIANT: Error Dialog
```csharp
try {
    await ProcessPaymentAsync(payment);
} catch (Exception ex) {
    _logger.LogError(ex, "Payment processing failed");
    await ShowErrorAsync("Payment Failed", ex.Message); // ✅ VISIBLE
}
```

#### ✅ COMPLIANT: Error Banner
```csharp
try {
    await BackgroundTaskAsync();
} catch (Exception ex) {
    _logger.LogError(ex, "Background task failed");
    MainWindow.ShowErrorBanner(ex); // ✅ VISIBLE
}
```

#### ✅ COMPLIANT: Result-Based Error
```csharp
if (validationFails) {
    return new Result { 
        Success = false, 
        ErrorMessage = "User-friendly message" // ✅ VISIBLE (ViewModel shows)
    };
}
```

### Requirement 2: User-Friendly Messages
**Error messages MUST be understandable by operators.**

#### ❌ VIOLATION: Technical Message
```csharp
await ShowErrorAsync("Error", ex.ToString()); // Stack trace - NOT user-friendly
```

#### ✅ COMPLIANT: User-Friendly Message
```csharp
await ShowErrorAsync("Payment Failed", 
    $"Unable to process payment:\n{ex.Message}\n\nPlease check network connection.");
```

### Requirement 3: Logging (Optional But Recommended)
**Logging is OPTIONAL but recommended for debugging.**

```csharp
try {
    await ProcessPaymentAsync(payment);
} catch (Exception ex) {
    _logger.LogError(ex, "Payment processing failed"); // Optional
    await ShowErrorAsync("Payment Failed", ex.Message); // MANDATORY
}
```

---

## ENFORCEMENT MECHANISMS

### 1. Code Review
**EVERY pull request MUST be reviewed for silent failures.**

**Checklist**:
- [ ] Are all exceptions caught?
- [ ] Are all caught exceptions surfaced to UI?
- [ ] Are error messages user-friendly?
- [ ] Is logging present (optional)?

### 2. Runtime Verification
**Global exception handlers catch unhandled exceptions.**

**Installed**:
- ✅ App.UnhandledException
- ✅ AppDomain.UnhandledException
- ✅ TaskScheduler.UnobservedTaskException
- ✅ API UseExceptionHandler middleware

### 3. Static Analysis (Future)
**Automated detection of silent failures.**

**Rules**:
- Detect empty catch blocks
- Detect catch blocks without UI notification
- Detect log-only error handling

---

## EXCEPTIONS TO THE RULE

### Exception 1: Converters
**Converters MAY use fallback values instead of UI notification.**

**Requirements**:
- MUST log to Debug output
- MUST use VISIBLE fallback (not Transparent)

```csharp
try {
    return new SolidColorBrush(ColorHelper.FromArgb(...));
} catch (Exception ex) {
    Debug.WriteLine($"Invalid color: {value}, Error: {ex.Message}");
    return new SolidColorBrush(Colors.LightGray); // ✅ VISIBLE
}
```

### Exception 2: Framework Event Handlers
**UI event handlers MAY rely on global exception handlers.**

```csharp
protected override async void OnNavigatedTo(NavigationEventArgs e)
{
    // Global handler catches exceptions
    await LoadDataAsync();
}
```

**Requirement**: Global handlers MUST show UI notification

---

## VIOLATION SEVERITY

| Violation Type | Severity | Response |
|----------------|----------|----------|
| Empty catch block in critical path | CRITICAL | IMMEDIATE FIX |
| Log-only handling in user action | HIGH | Same day fix |
| Debug-only handling in background | MEDIUM | Next sprint |
| Missing user-friendly message | LOW | Backlog |

---

## COMPLIANCE VERIFICATION

### Question 1: Is the error logged?
- ✅ YES → Good (optional)
- ⚠️ NO → Acceptable (logging is optional)

### Question 2: Is the error visible to the operator?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

### Question 3: Is the error message user-friendly?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION (fix message)

---

## SUMMARY

**GOLDEN RULE**: Logging ≠ Handling. Every failure MUST be visible to the operator.

**ENFORCEMENT**: Code review + Runtime verification + Static analysis (future)

**VIOLATIONS**: BLOCKING (must be fixed before merge)

---

**Status**: MANDATORY  
**Enforcement**: ACTIVE  
**Violations**: FORBIDDEN
