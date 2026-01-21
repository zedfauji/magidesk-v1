# Task 2.1.15: Concurrency Exception Fix

## Problem

When applying discounts to tickets in the SettlePage, users were encountering `DbUpdateConcurrencyException` errors. The error message was:

```
Exception thrown: 'Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException' in System.Private.CoreLib.dll
Exception thrown: 'Magidesk.Domain.Exceptions.ConcurrencyException' in Magidesk.Infrastructure.dll
```

## Root Cause

The issue was caused by improper handling of EF Core's concurrency token (`Version` property) when using `AsNoTracking()` queries. Here's what was happening:

1. **SettleViewModel** loads the ticket using `GetByIdAsync()` with `AsNoTracking()`
2. User clicks "Apply Discount" button
3. **ApplyDiscountCommandHandler** loads the ticket again with `AsNoTracking()`
4. The handler modifies the ticket (which increments `Version` from N to N+1)
5. **TicketRepository.UpdateAsync()** attaches the ticket and marks it as Modified
6. When `SaveChangesAsync()` is called, EF Core generates:
   ```sql
   UPDATE Tickets SET ... WHERE Id = @id AND Version = @version
   ```
7. **Problem**: EF Core uses the current `Version` value (N+1) in the WHERE clause instead of the original value (N)
8. The UPDATE affects 0 rows because the database has Version=N, not Version=N+1
9. EF Core throws `DbUpdateConcurrencyException`

## Solution

### 1. Fixed TicketRepository.UpdateAsync()

Modified the `UpdateAsync` method to properly set the `OriginalValue` for the `Version` property when working with detached entities:

```csharp
if (entry.State == EntityState.Detached)
{
    // Attach the ticket
    _context.Tickets.Attach(ticket);
    entry = _context.Entry(ticket);
    
    // Mark as Modified
    entry.State = EntityState.Modified;
    
    // CRITICAL: Set OriginalValues for concurrency check
    var currentVersion = ticket.Version;
    var originalVersion = currentVersion - 1;
    
    entry.Property(nameof(Ticket.Version)).OriginalValue = originalVersion;
    entry.Property(nameof(Ticket.Version)).CurrentValue = currentVersion;
}
```

**Why this works:**
- When a ticket is loaded with `AsNoTracking()`, EF Core doesn't track the original values
- When we attach it, EF Core sets `OriginalValues = CurrentValues`
- Since the domain method (e.g., `ApplyDiscount`) already incremented `Version`, both Original and Current are N+1
- We manually set `OriginalValue = CurrentVersion - 1` to restore the correct original version
- Now EF Core generates: `WHERE Version = N` (correct!)

### 2. Enhanced ApplyDiscountCommandHandler Logging

Added comprehensive debug logging to track the discount application flow:

```csharp
System.Diagnostics.Debug.WriteLine($"[ApplyDiscount] Loaded ticket {ticket.Id}, TicketNumber={ticket.TicketNumber}, Version={ticket.Version}");
System.Diagnostics.Debug.WriteLine($"[ApplyDiscount] Before ApplyDiscount: Version={ticket.Version}, SubtotalAmount={ticket.SubtotalAmount.Amount}");
System.Diagnostics.Debug.WriteLine($"[ApplyDiscount] After ApplyDiscount: Version={ticket.Version}, DiscountAmount={ticket.DiscountAmount.Amount}");
```

This helps diagnose concurrency issues in the future.

### 3. Improved Retry Logic

Enhanced the retry logic to properly handle exceptions and provide better error messages:

```csharp
Exception? lastException = null;

while (retryCount < maxRetries)
{
    try
    {
        // ... apply discount logic ...
        return;
    }
    catch (Domain.Exceptions.ConcurrencyException ex)
    {
        lastException = ex;
        retryCount++;
        
        if (retryCount >= maxRetries)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                $"Failed to apply discount after {maxRetries} attempts due to concurrent modifications. Please try again.",
                ex);
        }
        
        System.Diagnostics.Debug.WriteLine($"[ApplyDiscount] Concurrency conflict detected. Retry {retryCount}/{maxRetries}");
        
        await Task.Delay(100 * (int)Math.Pow(2, retryCount - 1), cancellationToken);
    }
}
```

## Testing

To verify the fix:

1. Open a ticket in SettlePage
2. Click "Apply Discount"
3. Select a discount and apply it
4. Verify that the discount is applied successfully without concurrency exceptions
5. Check the Debug output window for logging messages

## Files Modified

1. **Magidesk.Infrastructure/Repositories/TicketRepository.cs**
   - Fixed `UpdateAsync()` to properly handle concurrency tokens with AsNoTracking()

2. **Magidesk.Application/Services/ApplyDiscountCommandHandler.cs**
   - Enhanced logging for debugging
   - Improved retry logic and error handling

## Related Tasks

- **Task 2.1.15**: Integrate discount into SettlePage
- **Task 2.1.13**: Create DiscountSelectionViewModel
- **Task 2.1.14**: Create DiscountSelectionDialog view

## Notes

- The `Version` property is configured as a concurrency token in `TicketConfiguration.cs`
- EF Core automatically checks the Version in the WHERE clause of UPDATE statements
- Using `AsNoTracking()` is important for performance and preventing stale data issues
- The fix ensures that concurrency checks work correctly even with AsNoTracking()
- The retry logic handles transient concurrency conflicts (e.g., multiple users modifying the same ticket)

## Future Improvements

1. Consider using optimistic concurrency with a timestamp/rowversion column instead of an integer
2. Add integration tests for concurrent ticket modifications
3. Implement a more sophisticated conflict resolution strategy (e.g., merge changes)
4. Add user-friendly error messages when concurrency conflicts cannot be resolved
