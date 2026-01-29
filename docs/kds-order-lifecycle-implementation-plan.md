# KDS Order Lifecycle Enhancement - Implementation Plan

**Objective**: Implement complete order lifecycle tracking from "Sent to Kitchen" to "Delivered"  
**Priority**: HIGH - Critical for operational efficiency and order tracking  
**Date**: 2026-01-28

---

## Executive Summary

### Current Gaps

1. **No "Sent to Kitchen" Prevention**: Orders can be sent to kitchen multiple times
2. **No "Delivered" Status**: KDS has no way to mark orders as delivered
3. **No Delivery Notification**: POS doesn't know when kitchen completes an order
4. **No Time Tracking**: No measurement of kitchen preparation time

### Proposed Solution

Implement a complete order lifecycle with:
- **Prevent Re-sending**: Mark orders as sent and prevent duplicate sends
- **Delivered Button**: Add "Delivered" button on KDS to mark completion
- **Real-time Delivery Notification**: Notify POS when order is delivered
- **Time Tracking**: Log time from "Sent to Kitchen" to "Delivered"
- **Visual Indicators**: Show delivery status on POS order entry screen

---

## Requirements

### REQ-001: Prevent Duplicate Kitchen Sends

**Priority**: HIGH  
**User Story**: As a server, I should not be able to send the same order to kitchen twice, so that kitchen doesn't receive duplicate orders.

**Acceptance Criteria**:
- [ ] Once "Send to Kitchen" is clicked, the button should be disabled for that order
- [ ] Button text should change to "Sent to Kitchen" with timestamp
- [ ] Attempting to send again should show error message
- [ ] Manager override should allow re-sending if needed (with audit log)

---

### REQ-002: Add "Delivered" Status to KDS

**Priority**: HIGH  
**User Story**: As kitchen staff, I want to mark orders as "Delivered" when they're ready and handed off, so that I can track completion.

**Acceptance Criteria**:
- [ ] Add new `KitchenStatus.Delivered` enum value
- [ ] Add "Delivered" button to KDS order card (after "Done" status)
- [ ] Clicking "Delivered" should update order status to `Delivered`
- [ ] Delivered orders should move to history view
- [ ] Delivered orders should show green checkmark indicator

---

### REQ-003: Real-time Delivery Notification to POS

**Priority**: HIGH  
**User Story**: As a server, I want to see when my orders are delivered from kitchen, so that I know when to pick them up.

**Acceptance Criteria**:
- [ ] When KDS marks order as "Delivered", send SignalR notification to POS
- [ ] POS order entry screen should show "Delivered" indicator for completed items
- [ ] Notification should include kitchen order ID and delivery timestamp
- [ ] Visual indicator: Green checkmark or "Ready" badge on order line
- [ ] Audio notification (optional): Play sound when order is delivered

---

### REQ-004: Kitchen Preparation Time Tracking

**Priority**: MEDIUM  
**User Story**: As a manager, I want to track how long orders take from "Sent to Kitchen" to "Delivered", so that I can monitor kitchen performance.

**Acceptance Criteria**:
- [ ] Add `SentToKitchenAt` timestamp to `KitchenOrder` entity
- [ ] Add `DeliveredAt` timestamp to `KitchenOrder` entity
- [ ] Calculate `PreparationTime` (DeliveredAt - SentToKitchenAt)
- [ ] Log preparation time in audit events
- [ ] Display preparation time on KDS order card
- [ ] Store preparation time for reporting (future: analytics dashboard)

---

### REQ-005: Visual Status Indicators on POS

**Priority**: MEDIUM  
**User Story**: As a server, I want to see the status of my kitchen orders at a glance, so that I know what's cooking and what's ready.

**Acceptance Criteria**:
- [ ] Order lines show status badge: "Sent", "Cooking", "Done", "Delivered"
- [ ] Color coding: Gray (Sent), Yellow (Cooking), Orange (Done), Green (Delivered)
- [ ] Show elapsed time since sent to kitchen
- [ ] Show "Ready for pickup" notification when delivered
- [ ] Update status in real-time via SignalR

---

## Architecture

### Database Schema Changes

#### 1. Add Timestamps to KitchenOrder

```sql
ALTER TABLE "KitchenOrders" 
ADD COLUMN "SentToKitchenAt" timestamp with time zone NOT NULL DEFAULT NOW(),
ADD COLUMN "DeliveredAt" timestamp with time zone NULL;
```

#### 2. Add Delivered Status to Enum

```csharp
public enum KitchenStatus
{
    New,        // Order received by KDS
    Cooking,    // Kitchen started preparing
    Done,       // Kitchen finished preparing
    Delivered,  // Order handed off to server (NEW)
    Void        // Order cancelled
}
```

#### 3. Add Tracking to OrderLine

```sql
ALTER TABLE "OrderLines"
ADD COLUMN "SentToKitchenAt" timestamp with time zone NULL,
ADD COLUMN "DeliveredAt" timestamp with time zone NULL;
```

---

### SignalR Notification Flow

```
┌─────────────┐                    ┌──────────────┐                    ┌─────────────┐
│     POS     │                    │  SignalR Hub │                    │     KDS     │
└──────┬──────┘                    └──────┬───────┘                    └──────┬──────┘
       │                                  │                                   │
       │ 1. Send to Kitchen               │                                   │
       ├─────────────────────────────────►│                                   │
       │                                  │ 2. OrderCreated Notification      │
       │                                  ├──────────────────────────────────►│
       │                                  │                                   │
       │                                  │                                   │
       │                                  │ 3. Mark as Delivered              │
       │                                  │◄──────────────────────────────────┤
       │ 4. OrderDelivered Notification   │                                   │
       │◄─────────────────────────────────┤                                   │
       │                                  │                                   │
       │ 5. Update UI (Show "Ready")      │                                   │
       │                                  │                                   │
```

---

## Implementation Phases

### Phase 1: Database Schema & Domain Model (2 hours)

**Task 1.1: Update KitchenOrder Entity**

**Files to Modify**:
- `Magidesk.Domain/Entities/KitchenOrder.cs`
- `Magidesk.Domain/Enumerations/KitchenStatus.cs`

**Implementation**:

```csharp
// KitchenStatus.cs
public enum KitchenStatus
{
    New,
    Cooking,
    Done,
    Delivered,  // NEW
    Void
}

// KitchenOrder.cs
public class KitchenOrder
{
    // ... existing properties ...
    
    public DateTime SentToKitchenAt { get; private set; }  // NEW
    public DateTime? DeliveredAt { get; private set; }     // NEW
    
    public TimeSpan? PreparationTime => DeliveredAt.HasValue 
        ? DeliveredAt.Value - SentToKitchenAt 
        : null;  // NEW
    
    public KitchenOrder(Guid ticketId, string serverName, string tableNumber, Guid? printerGroupId)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        ServerName = serverName;
        TableNumber = tableNumber;
        PrinterGroupId = printerGroupId;
        Timestamp = DateTime.UtcNow;
        SentToKitchenAt = DateTime.UtcNow;  // NEW
        Status = KitchenStatus.New;
    }
    
    public void MarkAsDelivered()  // NEW METHOD
    {
        if (Status != KitchenStatus.Done)
        {
            throw new BusinessRuleViolationException("Order must be Done before marking as Delivered");
        }
        
        Status = KitchenStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }
}
```

**Task 1.2: Update OrderLine Entity**

**Files to Modify**:
- `Magidesk.Domain/Entities/OrderLine.cs`

**Implementation**:

```csharp
public class OrderLine
{
    // ... existing properties ...
    
    public DateTime? SentToKitchenAt { get; private set; }  // NEW
    public DateTime? DeliveredAt { get; private set; }      // NEW
    
    public void MarkPrintedToKitchen()
    {
        if (!ShouldPrintToKitchen)
        {
            throw new BusinessRuleViolationException("Order line is not configured to print to kitchen.");
        }

        PrintedToKitchen = true;
        SentToKitchenAt = DateTime.UtcNow;  // NEW

        // Also mark modifiers as printed
        foreach (var modifier in _modifiers.Where(m => m.ShouldPrintToKitchen))
        {
            modifier.MarkPrintedToKitchen();
        }
    }
    
    public void MarkAsDelivered()  // NEW METHOD
    {
        if (!PrintedToKitchen)
        {
            throw new BusinessRuleViolationException("Order line must be sent to kitchen before marking as delivered");
        }
        
        DeliveredAt = DateTime.UtcNow;
    }
}
```

**Task 1.3: Create Database Migration**

```bash
cd Magidesk/src/Magidesk.Migrations
dotnet ef migrations add AddKitchenOrderTimestamps
```

**Acceptance Criteria**:
- [ ] `KitchenStatus.Delivered` enum value added
- [ ] `SentToKitchenAt` and `DeliveredAt` properties added to `KitchenOrder`
- [ ] `SentToKitchenAt` and `DeliveredAt` properties added to `OrderLine`
- [ ] `PreparationTime` calculated property added
- [ ] `MarkAsDelivered()` method added to both entities
- [ ] Database migration created and tested
- [ ] Solution compiles without errors

---

### Phase 2: Notification Service Extension (1.5 hours)

**Task 2.1: Add OrderDelivered Notification**

**Files to Modify**:
- `Magidesk.Application/Interfaces/IOrderNotificationService.cs`
- `Magidesk.Application/Services/OrderNotificationService.cs`

**Implementation**:

```csharp
// IOrderNotificationService.cs
public interface IOrderNotificationService
{
    Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName);
    Task NotifyOrderReadyAsync(Guid kitchenOrderId, string tableNumber, string serverName);
    Task NotifyOrderStatusChangeAsync(Guid kitchenOrderId, KitchenStatus newStatus, string tableNumber, string serverName);
    Task NotifyOrderDeliveredAsync(Guid kitchenOrderId, Guid ticketId, string tableNumber, TimeSpan preparationTime);  // NEW
    Task SubscribeToNotificationsAsync(Guid terminalId, Guid userId, string[]? tableNumbers = null);
    Task UnsubscribeFromNotificationsAsync(Guid terminalId);
}

// OrderNotificationService.cs
public enum NotificationType
{
    OrderReady,
    StatusChange,
    OrderCreated,
    OrderDelivered  // NEW
}

public async Task NotifyOrderDeliveredAsync(Guid kitchenOrderId, Guid ticketId, string tableNumber, TimeSpan preparationTime)
{
    _logger.LogInformation(
        "Order delivered notification: Kitchen Order {KitchenOrderId}, Ticket {TicketId}, Table {TableNumber}, Prep Time {PrepTime}s", 
        kitchenOrderId, ticketId, tableNumber, preparationTime.TotalSeconds);

    var notification = new OrderNotification
    {
        Id = Guid.NewGuid(),
        Type = NotificationType.OrderDelivered,
        KitchenOrderId = kitchenOrderId,
        TicketId = ticketId,  // NEW: Need to add this property
        TableNumber = tableNumber,
        Message = $"Order for Table {tableNumber} is ready (Prep time: {preparationTime.TotalMinutes:F1} min)",
        Timestamp = DateTime.UtcNow,
        PreparationTime = preparationTime  // NEW: Need to add this property
    };

    await BroadcastNotificationAsync(notification);
}
```

**Task 2.2: Update OrderNotification DTO**

**Files to Modify**:
- `Magidesk.Application/Services/OrderNotificationService.cs` (or separate DTO file)

**Implementation**:

```csharp
public class OrderNotification
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public Guid KitchenOrderId { get; set; }
    public Guid TicketId { get; set; }  // NEW
    public string TableNumber { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public KitchenStatus? Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan? PreparationTime { get; set; }  // NEW
}
```

**Acceptance Criteria**:
- [ ] `NotifyOrderDeliveredAsync` method added to interface
- [ ] `OrderDelivered` enum value added to `NotificationType`
- [ ] Method implementation follows existing patterns
- [ ] `TicketId` and `PreparationTime` added to `OrderNotification`
- [ ] Structured logging implemented
- [ ] Solution compiles without errors

---

### Phase 3: KDS UI - Add Delivered Button (2 hours)

**Task 3.1: Add Delivered Button to KDS**

**Files to Modify**:
- `Magidesk.Presentation/ViewModels/KitchenDisplayViewModel.cs`
- `Magidesk.Presentation/Views/KitchenDisplayView.xaml`
- `Magidesk.Application/Interfaces/IKitchenStatusService.cs`
- `Magidesk.Application/Services/KitchenStatusService.cs`

**Implementation**:

```csharp
// IKitchenStatusService.cs
public interface IKitchenStatusService
{
    Task BumpOrderAsync(Guid kitchenOrderId);
    Task VoidOrderAsync(Guid kitchenOrderId);
    Task MarkAsDeliveredAsync(Guid kitchenOrderId);  // NEW
}

// KitchenStatusService.cs
public async Task MarkAsDeliveredAsync(Guid kitchenOrderId)
{
    var order = await _repository.GetByIdAsync(kitchenOrderId);
    if (order == null)
    {
        throw new NotFoundException($"Kitchen order {kitchenOrderId} not found");
    }

    // Update status
    order.MarkAsDelivered();
    await _repository.UpdateAsync(order);

    // Calculate preparation time
    var prepTime = order.PreparationTime ?? TimeSpan.Zero;

    // Send notification to POS
    await _notificationService.NotifyOrderDeliveredAsync(
        kitchenOrderId,
        order.TicketId,
        order.TableNumber,
        prepTime);

    // Log audit event
    _logger.LogInformation(
        "Kitchen order {KitchenOrderId} marked as delivered. Prep time: {PrepTime}s",
        kitchenOrderId, prepTime.TotalSeconds);
}

// KitchenDisplayViewModel.cs
public ICommand MarkAsDeliveredCommand { get; }  // NEW

public KitchenDisplayViewModel(...)
{
    // ... existing code ...
    MarkAsDeliveredCommand = new AsyncRelayCommand<KitchenOrderViewModel>(MarkAsDeliveredAsync);
}

private async Task MarkAsDeliveredAsync(KitchenOrderViewModel? vm)
{
    if (vm == null) return;

    try
    {
        await _statusService.MarkAsDeliveredAsync(vm.Id);
        await LoadOrdersAsync();  // Refresh
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to mark order as delivered");
        // Show error dialog
    }
}
```

**Task 3.2: Update KDS UI**

**Files to Modify**:
- `Magidesk.Presentation/Views/KitchenDisplayView.xaml`

**Implementation**:

```xml
<!-- Add Delivered button to order card -->
<Button 
    Content="Delivered" 
    Command="{Binding DataContext.MarkAsDeliveredCommand, ElementName=Root}"
    CommandParameter="{Binding}"
    Visibility="{Binding IsDoneStatus, Converter={StaticResource BoolToVisibilityConverter}}"
    Background="Green"
    Foreground="White"
    Margin="5,0,0,0" />
```

**Acceptance Criteria**:
- [ ] `MarkAsDeliveredAsync` method added to `IKitchenStatusService`
- [ ] Method implementation updates order status and sends notification
- [ ] "Delivered" button added to KDS order card
- [ ] Button only visible when order status is "Done"
- [ ] Clicking button marks order as delivered
- [ ] Delivered orders move to history view
- [ ] Preparation time logged in audit events

---

### Phase 4: POS UI - Prevent Re-sending & Show Delivery Status (2.5 hours)

**Task 4.1: Prevent Duplicate Sends**

**Files to Modify**:
- `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`
- `Magidesk.Presentation/Views/OrderEntryView.xaml`

**Implementation**:

```csharp
// OrderEntryViewModel.cs
public bool CanSendToKitchen => HasUnsentItems;  // NEW

public ICommand SendToKitchenCommand { get; }  // Update existing

private async Task SendToKitchenAsync()
{
    if (Ticket == null || !HasUnsentItems) return;

    try
    {
        var result = await _printToKitchenHandler.HandleAsync(new PrintToKitchenCommand
        {
            TicketId = Ticket.Id
        });

        if (result.Success)
        {
            // Reload ticket to get updated PrintedToKitchen flags
            await LoadTicketAsync(Ticket.Id);
            
            // Show success message
            // TODO: Show toast notification
        }
        else
        {
            // Show error
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send to kitchen");
    }
}
```

**Task 4.2: Add Delivery Status Indicators**

**Files to Modify**:
- `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`
- `Magidesk.Application/DTOs/OrderLineDto.cs`

**Implementation**:

```csharp
// OrderLineDto.cs
public class OrderLineDto
{
    // ... existing properties ...
    
    public DateTime? SentToKitchenAt { get; set; }  // NEW
    public DateTime? DeliveredAt { get; set; }      // NEW
    
    public string KitchenStatusText => DeliveredAt.HasValue 
        ? "Delivered" 
        : SentToKitchenAt.HasValue 
            ? "In Kitchen" 
            : "Not Sent";  // NEW
    
    public string KitchenStatusColor => DeliveredAt.HasValue 
        ? "Green" 
        : SentToKitchenAt.HasValue 
            ? "Orange" 
            : "Gray";  // NEW
}

// OrderEntryViewModel.cs - Subscribe to delivery notifications
private async Task InitializeAsync(Guid? ticketId = null)
{
    // ... existing code ...

    // Subscribe to order notifications
    if (_terminalContext.TerminalId.HasValue && _userService.CurrentUser?.Id != null)
    {
        await _orderNotificationService.SubscribeToNotificationsAsync(
            _terminalContext.TerminalId.Value,
            _userService.CurrentUser.Id);
    }
}

// Handle delivery notifications (add to SignalR listener)
_hubConnection.On<OrderNotification>("OrderUpdated", async (notification) =>
{
    if (notification.Type == NotificationType.OrderDelivered && 
        notification.TicketId == Ticket?.Id)
    {
        // Reload ticket to show delivery status
        await LoadTicketAsync(Ticket.Id);
        
        // Show notification
        // TODO: Play sound, show toast
    }
});
```

**Task 4.3: Update POS UI**

**Files to Modify**:
- `Magidesk.Presentation/Views/OrderEntryView.xaml`

**Implementation**:

```xml
<!-- Update order line display to show status -->
<StackPanel Orientation="Horizontal">
    <TextBlock Text="{Binding MenuItemName}" />
    <Border 
        Background="{Binding KitchenStatusColor}"
        CornerRadius="3"
        Padding="5,2"
        Margin="10,0,0,0">
        <TextBlock 
            Text="{Binding KitchenStatusText}"
            Foreground="White"
            FontSize="10" />
    </Border>
</StackPanel>

<!-- Update Send to Kitchen button -->
<Button 
    Content="Send to Kitchen"
    Command="{Binding SendToKitchenCommand}"
    IsEnabled="{Binding CanSendToKitchen}"
    Visibility="{Binding HasUnsentItems, Converter={StaticResource BoolToVisibilityConverter}}" />

<TextBlock 
    Text="All items sent to kitchen"
    Visibility="{Binding HasUnsentItems, Converter={StaticResource InverseBoolToVisibilityConverter}}"
    Foreground="Gray" />
```

**Acceptance Criteria**:
- [ ] "Send to Kitchen" button disabled after sending
- [ ] Button text changes to "Sent to Kitchen" with timestamp
- [ ] Order lines show status badge (Not Sent, In Kitchen, Delivered)
- [ ] Status badges color-coded (Gray, Orange, Green)
- [ ] POS receives delivery notifications via SignalR
- [ ] UI updates automatically when order is delivered
- [ ] Optional: Audio notification when order is delivered

---

### Phase 5: Testing & Verification (2 hours)

**Task 5.1: Unit Tests**

**Test Cases**:

1. **Test: Cannot send to kitchen twice**
   - Send order to kitchen
   - Verify `PrintedToKitchen` flag is true
   - Attempt to send again
   - Verify error or no-op

2. **Test: Delivered status updates correctly**
   - Create kitchen order
   - Mark as Done
   - Mark as Delivered
   - Verify `DeliveredAt` timestamp set
   - Verify `PreparationTime` calculated

3. **Test: Delivery notification sent**
   - Mark order as delivered
   - Verify `NotifyOrderDeliveredAsync` called
   - Verify notification contains correct data

**Task 5.2: Integration Tests**

**Test Scenarios**:

1. **Scenario: Complete order lifecycle**
   - Create ticket in POS
   - Send to kitchen
   - Verify order appears on KDS
   - Bump to "Cooking"
   - Bump to "Done"
   - Click "Delivered"
   - Verify POS shows "Delivered" status
   - Verify preparation time logged

2. **Scenario: Prevent duplicate sends**
   - Send order to kitchen
   - Verify button disabled
   - Attempt to send again
   - Verify error message or no action

3. **Scenario: Real-time delivery notification**
   - Send order to kitchen
   - Mark as delivered on KDS
   - Verify POS updates within 2 seconds
   - Verify status badge changes to green

**Acceptance Criteria**:
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] End-to-end workflow verified
- [ ] Performance acceptable (< 2 second notification latency)

---

## Database Migration

### Migration Script

```sql
-- Add timestamps to KitchenOrders
ALTER TABLE "KitchenOrders" 
ADD COLUMN "SentToKitchenAt" timestamp with time zone NOT NULL DEFAULT NOW(),
ADD COLUMN "DeliveredAt" timestamp with time zone NULL;

-- Add timestamps to OrderLines
ALTER TABLE "OrderLines"
ADD COLUMN "SentToKitchenAt" timestamp with time zone NULL,
ADD COLUMN "DeliveredAt" timestamp with time zone NULL;

-- Update existing records (backfill)
UPDATE "KitchenOrders" 
SET "SentToKitchenAt" = "Timestamp" 
WHERE "SentToKitchenAt" IS NULL;

-- Add index for performance
CREATE INDEX "IX_KitchenOrders_Status_SentToKitchenAt" 
ON "KitchenOrders" ("Status", "SentToKitchenAt");
```

---

## Rollback Plan

If issues are discovered:

1. **Disable Delivered Button**:
   - Comment out `MarkAsDeliveredCommand` binding in XAML
   - System continues to work with existing statuses

2. **Revert Database Changes**:
   ```sql
   ALTER TABLE "KitchenOrders" 
   DROP COLUMN "SentToKitchenAt",
   DROP COLUMN "DeliveredAt";
   
   ALTER TABLE "OrderLines"
   DROP COLUMN "SentToKitchenAt",
   DROP COLUMN "DeliveredAt";
   ```

3. **Rollback Time**: < 10 minutes

---

## Success Metrics

### Operational Metrics

- **Duplicate Sends**: Reduce to 0 (currently unknown)
- **Order Tracking**: 100% of orders tracked from send to delivery
- **Kitchen Efficiency**: Measure average preparation time
- **Server Satisfaction**: Improved visibility into order status

### Technical Metrics

- **Notification Latency**: < 2 seconds for delivery notifications
- **System Performance**: No degradation in POS or KDS performance
- **Error Rate**: < 1% for notification failures

---

## Timeline

### Day 1: Database & Domain (2 hours)
- Morning: Phase 1 - Database schema and domain model changes

### Day 2: Services & Notifications (1.5 hours)
- Morning: Phase 2 - Notification service extension

### Day 3: KDS UI (2 hours)
- Morning: Phase 3 - Add delivered button to KDS

### Day 4: POS UI (2.5 hours)
- Morning: Phase 4 - Prevent re-sending and show delivery status

### Day 5: Testing (2 hours)
- Morning: Phase 5 - Unit and integration testing

**Total Estimated Effort**: 10 hours (1.25 days)

---

## Dependencies

### External Dependencies
- SignalR hub must be running
- Network connectivity between POS and KDS
- Database must be accessible

### Internal Dependencies
- Phase 2 depends on Phase 1 (database schema)
- Phase 3 depends on Phase 2 (notification service)
- Phase 4 depends on Phase 2 (notification service)
- Phase 5 depends on all previous phases

---

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Database migration fails | LOW | HIGH | Test migration on staging first |
| SignalR notification fails | LOW | MEDIUM | Polling fallback already exists |
| Performance degradation | LOW | MEDIUM | Monitor metrics, optimize queries |
| UI confusion for users | MEDIUM | LOW | Provide training, clear visual indicators |

---

## Future Enhancements

### Phase 6 (Future): Analytics Dashboard
- Average preparation time by item
- Peak kitchen hours
- Slowest items
- Kitchen efficiency trends

### Phase 7 (Future): Mobile Notifications
- Push notifications to server mobile devices
- "Order ready" alerts
- Vibration/sound alerts

### Phase 8 (Future): Customer Display
- Show order status to customers
- Estimated wait time
- "Your order is ready" notification

---

## Approval

**Spec Status**: READY FOR IMPLEMENTATION  
**Approval Required**: Technical Lead, Product Owner  
**Estimated Start Date**: 2026-01-29  
**Estimated Completion**: 2026-01-30

---

**Next Steps**:
1. Review and approve this implementation plan
2. Begin Phase 1: Database schema changes
3. Execute phases sequentially
4. Test thoroughly before production deployment
