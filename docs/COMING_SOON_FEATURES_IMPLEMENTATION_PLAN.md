# Coming Soon Features - Implementation Plan

## Overview
This document tracks the implementation of all "Coming Soon" placeholder features in the redesigned Order and Settle pages.

## Status Summary

### OrderPageViewModel Features
| Feature | Status | Command Handler | Dialog | Notes |
|---------|--------|----------------|--------|-------|
| Select Table | ✅ Available | N/A | TableSelectionDialog | Need to wire up |
| Split Order | ✅ Available | SplitTicketCommand | N/A | Need to implement |
| Merge Order | ✅ Available | MergeTicketsCommand | N/A | Need to implement |
| Add Note | ⚠️ Partial | AddOrderLineInstructionCommand | N/A | Need note dialog |
| Print Order | ✅ Available | PrintToKitchenCommand | N/A | Need to implement |
| Pay Now | ⚠️ Complex | ProcessPaymentCommand | N/A | Quick payment flow |
| Start Session | ✅ Available | OpenCashSessionCommand | N/A | Need to implement |
| End Session | ✅ Available | CloseCashSessionCommand | N/A | Need to implement |
| Reprint | ✅ Available | PrintReceiptCommand | N/A | Need to implement |
| Void Ticket | ✅ Available | VoidTicketCommand | VoidTicketDialog | Need to wire up |
| Apply Discount | ✅ Available | ApplyDiscountCommand | DiscountSelectionDialog | Need to wire up |
| Fire Ticket | ✅ Available | PrintToKitchenCommand | N/A | Need to implement |

### SettlePageViewModel Features
| Feature | Status | Command Handler | Dialog | Notes |
|---------|--------|----------------|--------|-------|
| Add Tip | ✅ Available | ApplyGratuityCommand | GratuitySelectionDialog | Need to wire up |
| Split Payment | ✅ Available | ProcessSplitPaymentCommand | SplitPaymentDialog | Need to wire up |
| Apply Discount | ✅ Available | ApplyDiscountCommand | DiscountSelectionDialog | Need to wire up |
| Print Receipt | ✅ Available | PrintReceiptCommand | N/A | Need to implement |

## Implementation Priority

### Phase 1: High Priority (Core POS Operations)
1. ✅ Fire Ticket (Send to kitchen)
2. ✅ Start/End Session (Cash drawer management)
3. ✅ Void Ticket (Cancel orders)
4. ✅ Print Receipt (Customer receipt)
5. ✅ Add Tip (Gratuity)

### Phase 2: Medium Priority (Payment & Discounts)
6. ✅ Apply Discount (Both pages)
7. ✅ Split Payment (Divide payment)
8. ✅ Select Table (Table assignment)

### Phase 3: Lower Priority (Advanced Features)
9. ✅ Split Order (Divide by seat/item)
10. ✅ Merge Order (Combine tickets)
11. ✅ Add Note (Special instructions)
12. ✅ Print Order (Kitchen ticket)
13. ✅ Reprint (Reprint last ticket)
14. ✅ Pay Now (Quick payment)

## Implementation Notes

### Common Patterns
- All dialogs need XamlRoot set from current window
- All async operations need error handling with user feedback
- All commands need proper DI resolution via ServiceScopeFactory
- All operations need logging
- All operations need to reload ticket data after completion

### Dialog Initialization Pattern
```csharp
// Get required data
var data = await LoadDataAsync();

// Create ViewModel
var viewModel = new SomeDialogViewModel(dependencies, data);

// Create Dialog
var dialog = new SomeDialog(viewModel);

// Set XamlRoot
if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
{
    dialog.XamlRoot = element.XamlRoot;
}

// Show and handle result
await dialog.ShowAsync();

if (viewModel.IsConfirmed)
{
    // Process result
    await ProcessResultAsync(viewModel.Result);
    await LoadTicketAsync(); // Reload ticket
}
```

### Command Handler Pattern
```csharp
try
{
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SomeCommand, SomeResult>>();
        
        var command = new SomeCommand
        {
            // Set properties
        };
        
        var result = await handler.HandleAsync(command);
        
        // Handle result
        await LoadTicketAsync(); // Reload ticket
    }
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    await _dialogService.ShowErrorAsync("Error", $"Operation failed: {ex.Message}");
}
```

## Next Steps
1. Implement each feature following the priority order
2. Test each feature individually
3. Update this document as features are completed
4. Remove "coming soon" messages as features are implemented
