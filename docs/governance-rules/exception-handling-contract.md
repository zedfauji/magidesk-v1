# Exception Handling Contract Rule

**Category**: MANDATORY  
**Enforcement**: Code Review + Architectural Pattern  
**Violations**: BLOCKING

---

## RULE STATEMENT

**Catching ≠ Handling.**  
**UI surfacing is REQUIRED.**  
**Severity classification is ENFORCED.**

---

## RULE 1: Catching ≠ Handling

### Definition
**Catching an exception does NOT mean you have handled it.**

Handling requires:
1. ✅ Logging (optional but recommended)
2. ✅ UI notification (MANDATORY)
3. ✅ User-friendly message (MANDATORY)

### Examples

#### ❌ VIOLATION: Caught But Not Handled
```csharp
try {
    await SaveAsync();
} catch (Exception ex) {
    _logger.LogError(ex, "Save failed");
    // NO UI NOTIFICATION - NOT HANDLED
}
```

#### ✅ COMPLIANT: Caught AND Handled
```csharp
try {
    await SaveAsync();
} catch (Exception ex) {
    _logger.LogError(ex, "Save failed");
    await ShowErrorAsync("Save Failed", ex.Message); // HANDLED
}
```

---

## RULE 2: UI Surfacing is REQUIRED

### Requirement
**EVERY caught exception MUST be surfaced to the operator.**

### Surfacing Methods

#### Method 1: Error Dialog (Action Failures)
```csharp
await _navigationService.ShowErrorAsync("Operation Failed", ex.Message);
```

#### Method 2: Error Banner (Background Failures)
```csharp
MainWindow.ShowErrorBanner(ex);
```

#### Method 3: Result Object (Business Logic)
```csharp
return new Result { 
    Success = false, 
    ErrorMessage = "User-friendly message" 
};
```

#### Method 4: Domain Exception (Repositories)
```csharp
throw new Domain.Exceptions.ConcurrencyException(
    "Entity was modified by another process. Please refresh and try again.", 
    ex);
```

### Anti-Pattern: Log-Only
```csharp
// ❌ VIOLATION
catch (Exception ex) {
    _logger.LogError(ex, "Failed");
    // NO UI SURFACING
}
```

---

## RULE 3: Severity Classification

### Severity Levels

| Severity | Criteria | UI Element | Blocking |
|----------|----------|-----------|----------|
| **FATAL** | App cannot continue | Fatal Dialog + Exit | YES |
| **CRITICAL** | User action failed | Error Dialog | YES |
| **WARNING** | Degraded service | Warning Banner | NO |
| **INFO** | Informational | Toast | NO |

### Examples

#### FATAL: Startup Failure
```csharp
try {
    await InitializeSystemAsync();
} catch (Exception ex) {
    ShowFatalDialog("Startup Failed", ex.Message);
    Environment.Exit(1); // FATAL - EXIT APP
}
```

#### CRITICAL: Action Failure
```csharp
try {
    await ProcessPaymentAsync();
} catch (Exception ex) {
    await ShowErrorAsync("Payment Failed", ex.Message); // CRITICAL - BLOCK ACTION
}
```

#### WARNING: Degraded Service
```csharp
try {
    await ConnectToPaymentGatewayAsync();
} catch (Exception ex) {
    MainWindow.ShowWarningBanner("Payment Gateway Offline", 
        "Credit card payments unavailable."); // WARNING - CONTINUE
}
```

#### INFO: Success Notification
```csharp
ShowToast("Ticket Saved", "Ticket #1234 saved successfully."); // INFO
```

---

## EXCEPTION HANDLING PATTERNS

### Pattern 1: Result-Based (Recommended)
**USE**: For business logic and command handlers

```csharp
public async Task<Result> ProcessPaymentAsync(Payment payment)
{
    if (payment.Amount <= 0)
    {
        return new Result { 
            Success = false, 
            ErrorMessage = "Payment amount must be greater than zero." 
        };
    }
    
    try {
        await _gateway.ProcessAsync(payment);
        return new Result { Success = true };
    } catch (Exception ex) {
        _logger.LogError(ex, "Payment processing failed");
        return new Result { 
            Success = false, 
            ErrorMessage = $"Payment processing failed: {ex.Message}" 
        };
    }
}

// ViewModel
var result = await _paymentService.ProcessPaymentAsync(payment);
if (!result.Success) {
    await ShowErrorAsync("Payment Failed", result.ErrorMessage);
}
```

### Pattern 2: Exception-Based (Acceptable)
**USE**: For domain logic and repositories

```csharp
public async Task ProcessPaymentAsync(Payment payment)
{
    if (payment.Amount <= 0)
    {
        throw new BusinessRuleViolationException(
            "Payment amount must be greater than zero.");
    }
    
    await _gateway.ProcessAsync(payment);
}

// ViewModel MUST catch
try {
    await _paymentService.ProcessPaymentAsync(payment);
} catch (BusinessRuleViolationException ex) {
    await ShowErrorAsync("Payment Failed", ex.Message);
} catch (Exception ex) {
    await ShowErrorAsync("Payment Failed", $"Unexpected error: {ex.Message}");
}
```

### Pattern 3: Hybrid (Best of Both)
**USE**: For complex operations

```csharp
public async Task<Result> ProcessPaymentAsync(Payment payment)
{
    try {
        // Validation
        if (payment.Amount <= 0)
        {
            return new Result { 
                Success = false, 
                ErrorMessage = "Payment amount must be greater than zero." 
            };
        }
        
        // Domain operation (may throw)
        await _gateway.ProcessAsync(payment);
        
        return new Result { Success = true };
    } catch (BusinessRuleViolationException ex) {
        return new Result { 
            Success = false, 
            ErrorMessage = ex.Message 
        };
    } catch (Exception ex) {
        _logger.LogError(ex, "Payment processing failed");
        return new Result { 
            Success = false, 
            ErrorMessage = $"Payment processing failed: {ex.Message}" 
        };
    }
}
```

---

## LAYER-SPECIFIC CONTRACTS

### ViewModels
**CONTRACT**: Catch ALL exceptions from command handlers and surface to UI

```csharp
try {
    var result = await _handler.HandleAsync(command);
    // Process result
} catch (Exception ex) {
    await ShowErrorAsync("Operation Failed", ex.Message); // MANDATORY
}
```

### Services (Command/Query Handlers)
**CONTRACT**: Either return error result OR throw domain exception

**Option 1**: Result-based
```csharp
return new Result { Success = false, ErrorMessage = "..." };
```

**Option 2**: Exception-based
```csharp
throw new BusinessRuleViolationException("...");
```

### Repositories
**CONTRACT**: Wrap database exceptions in domain exceptions

```csharp
try {
    await _context.SaveChangesAsync();
} catch (DbUpdateConcurrencyException ex) {
    throw new Domain.Exceptions.ConcurrencyException(
        "Entity was modified by another process.", ex);
}
```

### Converters
**CONTRACT**: Return safe fallback value + log to Debug

```csharp
try {
    return ConvertValue(value);
} catch (Exception ex) {
    Debug.WriteLine($"Conversion failed: {ex.Message}");
    return SafeFallbackValue;
}
```

---

## ENFORCEMENT MECHANISMS

### 1. Code Review
**Checklist**:
- [ ] Are all exceptions caught?
- [ ] Are all caught exceptions handled (UI surfacing)?
- [ ] Is severity classification appropriate?
- [ ] Are error messages user-friendly?

### 2. Architectural Pattern Verification
**Verify**:
- ViewModels catch handler exceptions ✅ (VERIFIED)
- Handlers use result-based or exception-based ✅ (VERIFIED)
- Repositories wrap database exceptions ✅ (VERIFIED)

### 3. Static Analysis (Future)
**Detect**:
- Catch blocks without UI surfacing
- Missing exception handling in ViewModels
- Inappropriate severity classification

---

## VIOLATION SEVERITY

| Violation Type | Severity | Response |
|----------------|----------|----------|
| Caught but not handled | HIGH | Same day fix |
| No UI surfacing | HIGH | Same day fix |
| Inappropriate severity | MEDIUM | Next sprint |
| Non-user-friendly message | LOW | Backlog |

---

## COMPLIANCE VERIFICATION

### Question 1: Is the exception caught?
- ✅ YES → Proceed to Q2
- ❌ NO → Verify global handlers catch it

### Question 2: Is the exception handled (UI surfaced)?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

### Question 3: Is the severity appropriate?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION (adjust severity)

---

## SUMMARY

**GOLDEN RULE**: Catching ≠ Handling. UI surfacing is MANDATORY.

**PATTERNS**: Result-based (recommended) OR Exception-based (acceptable)

**ENFORCEMENT**: Code review + Architectural verification + Static analysis (future)

---

**Status**: MANDATORY  
**Enforcement**: ACTIVE  
**Violations**: FORBIDDEN
