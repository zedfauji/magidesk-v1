# Table Click Confirmation Implementation

## Overview

This document describes the implementation of a confirmation dialog when clicking on tables in the Table Map, along with checking for existing open tickets.

## Problem Statement

Previously, clicking on an available table would immediately create a new ticket without user confirmation. This could lead to:
- Accidental ticket creation
- Orphaned empty tickets if users navigated away
- Multiple tickets for the same table if the system didn't properly track table status

## Solution

Implemented a two-step confirmation process:

### 1. Check for Existing Open Tickets

Before showing the confirmation dialog, the system checks if there's already an open ticket for the selected table that is not:
- Paid
- Voided
- Refunded
- Held

### 2. Show Confirmation Dialog

**If existing ticket found:**
- Shows warning that an existing ticket was found
- Primary button: "Open Existing Ticket" - navigates to the existing ticket
- Secondary button: "Cancel" - closes dialog without action

**If no existing ticket:**
- Asks user: "Would you like to open a new ticket for this table?"
- Primary button: "Yes, Open Ticket" - creates new ticket and navigates to order entry
- Secondary button: "No, Just View Table" - stays on table map without creating ticket
- Close button: "Cancel" - closes dialog without action

## Implementation Details

### Files Modified

1. **Magidesk.Application/Interfaces/ITicketRepository.cs**
   - Added `GetOpenTicketByTableNumberAsync()` method to interface

2. **Magidesk.Infrastructure/Repositories/TicketRepository.cs**
   - Implemented `GetOpenTicketByTableNumberAsync()` method
   - Queries for tickets that are not Paid, Voided, Refunded, or Held
   - Filters by table number from the TableNumbers collection

3. **ViewModels/TableMapViewModel.cs**
   - Updated `SelectTableAsync()` method to:
     - Check for existing open tickets before showing confirmation
     - Show appropriate dialog based on whether existing ticket found
     - Handle user's choice (create new, open existing, or cancel)

### Files Created

1. **Views/Dialogs/OpenTicketConfirmationDialog.xaml**
   - ContentDialog with three buttons (Primary, Secondary, Close)
   - Shows table information
   - Conditional InfoBar for existing ticket warning

2. **Views/Dialogs/OpenTicketConfirmationDialog.xaml.cs**
   - Code-behind with `Initialize()` method
   - Configures dialog based on whether existing ticket found
   - Properties: TableNumber, HasExistingTicket, ExistingTicketId

## User Flow

### Scenario 1: No Existing Ticket

1. User clicks on available table
2. System checks for existing open tickets → None found
3. Confirmation dialog appears: "Would you like to open a new ticket for this table?"
4. User clicks "Yes, Open Ticket"
5. New ticket is created
6. User is navigated to Order Entry page with new ticket

### Scenario 2: Existing Ticket Found

1. User clicks on available table
2. System checks for existing open tickets → Ticket found
3. Warning dialog appears: "This table already has an open ticket"
4. User clicks "Open Existing Ticket"
5. User is navigated to Order Entry page with existing ticket

### Scenario 3: User Cancels

1. User clicks on available table
2. Confirmation dialog appears
3. User clicks "No, Just View Table" or "Cancel"
4. Dialog closes, user remains on Table Map page
5. No ticket is created

## Benefits

1. **Prevents Accidental Ticket Creation**: Users must explicitly confirm before creating a ticket
2. **Prevents Duplicate Tickets**: System checks for existing open tickets before allowing new ticket creation
3. **Better User Experience**: Users can view table information without committing to opening a ticket
4. **Data Integrity**: Reduces orphaned empty tickets in the system
5. **Flexibility**: Users can choose to just view the table without creating a ticket

## Technical Notes

### GetOpenTicketByTableNumberAsync Implementation

```csharp
public async Task<Ticket?> GetOpenTicketByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default)
{
    // Get tickets that are not Paid, Voided, Refunded, or Held
    var ticket = await _context.Tickets
        .AsNoTracking()
        .Where(t => t.Status != Domain.Enumerations.TicketStatus.Paid 
                 && t.Status != Domain.Enumerations.TicketStatus.Voided 
                 && t.Status != Domain.Enumerations.TicketStatus.Refunded
                 && t.Status != Domain.Enumerations.TicketStatus.Held)
        .Include(t => t.OrderLines)
        .Include(t => t.Payments)
        .Include(t => t.Discounts)
        .Include(t => t.Gratuity)
        .ToListAsync(cancellationToken);

    // Filter by table number (TableNumbers is a collection)
    var matchingTicket = ticket.FirstOrDefault(t => t.TableNumbers.Contains(tableNumber));
    
    // Load related entities...
    
    return matchingTicket;
}
```

### Why Check These Statuses?

- **Paid**: Ticket is complete and closed
- **Voided**: Ticket was cancelled
- **Refunded**: Ticket was refunded
- **Held**: Ticket is temporarily on hold

Any ticket in these states should not prevent opening a new ticket for the table.

## Future Enhancements

1. **Auto-cleanup**: Implement a background job to automatically void empty tickets after a timeout period (e.g., 30 minutes)
2. **Table Details View**: Create a dedicated page to view table information without opening a ticket
3. **Merge Tickets**: Allow merging multiple tickets for the same table
4. **Ticket History**: Show history of previous tickets for the table in the confirmation dialog

## Testing Recommendations

1. **Test existing ticket detection**: Create an open ticket for a table, then try to click on it again
2. **Test confirmation flow**: Verify all three button options work correctly
3. **Test with different ticket statuses**: Ensure Paid, Voided, Refunded, and Held tickets don't block new ticket creation
4. **Test cancellation**: Verify no ticket is created when user cancels
5. **Test "Just View Table" option**: Verify user stays on table map without ticket creation

## Related Files

- `.kiro/specs/category-c-billing-payments/tasks.md` - Updated with concurrency and XAML fixes
- `TASK_2_1_15_CONCURRENCY_FIX.md` - Previous fix documentation
- `App.xaml` - Fixed BooleanToVisibilityConverter resource

## Date

January 15, 2026
