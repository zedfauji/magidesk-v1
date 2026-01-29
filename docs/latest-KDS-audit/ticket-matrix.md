# KDS Ticket Matrix

**Project**: Magidesk POS - KDS Production Readiness  
**Epic**: Real-Time Kitchen Display System  
**Date**: 2026-01-28  
**Status**: ACTIVE

---

## Ticket Summary

| Priority | Count | Description |
|----------|-------|-------------|
| **BLOCKER** | 2 | Must fix before ANY production deployment |
| **REQUIRED** | 0 | Must fix before General Availability |
| **OPTIONAL** | 1 | Technical debt, can defer to v1.1 |

---

## Tickets

### KDS-001: Add NotifyOrderCreatedAsync Method to IOrderNotificationService

**Priority**: **BLOCKER** 🔴  
**Component**: `Magidesk.Application/Interfaces/IOrderNotificationService.cs`  
**Estimated Effort**: 2 hours

**Description**:
The `IOrderNotificationService` interface lacks a method for notifying about NEW order creation. It only has methods for status changes (`NotifyOrderStatusChangeAsync`) and order ready notifications (`NotifyOrderReadyAsync`). This architectural gap prevents the command handler from notifying KDS about new orders.

**Evidence**:
- File: `Magidesk.Application/Interfaces/IOrderNotificationService.cs`
- Current interface has 4 methods, none for order creation
- `KitchenStatusService` successfully uses existing methods for status changes
- SignalR infrastructure is functional (proven by working status notifications)

**Acceptance Criteria**:
1. Add method signature: `Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName)`
2. Implement method in `OrderNotificationService` to create `OrderNotification` with `Type = NotificationType.OrderCreated`
3. Add `OrderCreated` enum value to `NotificationType`
4. Method must call `_publisher.PublishAsync(notification)` to trigger SignalR broadcast
5. Method must log notification using `ILogger`
6. Method must handle exceptions gracefully (log but don't throw)

**Implementation Notes**:
- Follow existing pattern from `NotifyOrderReadyAsync` and `NotifyOrderStatusChangeAsync`
- Ensure notification includes all required fields: `KitchenOrderId`, `TableNumber`, `ServerName`, `Timestamp`
- SignalR will broadcast to all connected KDS clients via `OrderUpdated` event

**Release Impact**: **BLOCKER** - Cannot notify KDS without this method

---

### KDS-002: Inject and Call Notification Service in PrintToKitchenCommandHandler

**Priority**: **BLOCKER** 🔴  
**Component**: `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`  
**Estimated Effort**: 3 hours  
**Depends On**: KDS-001

**Description**:
The `PrintToKitchenCommandHandler` successfully persists orders to the database but does NOT trigger any notification to the Kitchen Display System. This causes 60-second latency (polling interval) before orders appear on KDS screens.

**Evidence**:
- File: `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`, Method: `HandleAsync`
- Handler has NO `IOrderNotificationService` dependency
- `KitchenRoutingService.RouteToKitchenAsync` returns `List<Guid>` (kitchen order IDs) but these are never used
- Contrast: `KitchenStatusService.BumpOrderAsync` DOES properly call notification service

**Acceptance Criteria**:
1. Add `IOrderNotificationService` to constructor parameters
2. Store service in private readonly field `_notificationService`
3. After successful `RouteToKitchenAsync` call, iterate through returned kitchen order IDs
4. For each kitchen order ID, call `await _notificationService.NotifyOrderCreatedAsync(kitchenOrderId, tableNumber, serverName)`
5. Wrap notification calls in try/catch to ensure notification failures don't break order persistence
6. Log notification failures but continue processing
7. Update DI registration in `ServiceCollectionExtensions` if needed
8. Write unit test verifying notification service is called with correct parameters

**Implementation Notes**:
```csharp
// After line 45 in HandleAsync:
var kitchenOrderIds = await _kitchenRoutingService.RouteToKitchenAsync(ticketDto, ...);

// Add notification loop:
foreach (var kitchenOrderId in kitchenOrderIds)
{
    try
    {
        await _notificationService.NotifyOrderCreatedAsync(
            kitchenOrderId,
            ticket.TableNumbers.FirstOrDefault() ?? "Unknown",
            "Server Name" // TODO: Get from ticket or user context
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to notify KDS about kitchen order {KitchenOrderId}", kitchenOrderId);
        // Don't throw - notification failure shouldn't break order persistence
    }
}
```

**Testing Strategy**:
1. Unit test: Mock `IOrderNotificationService`, verify `NotifyOrderCreatedAsync` called with correct parameters
2. Integration test: Create order via UI, verify KDS screen updates within 2 seconds
3. Failure test: Simulate SignalR failure, verify order still persists to database

**Release Impact**: **BLOCKER** - Core functionality broken without this fix

---

### KDS-003: Remove Unused IOrderNotificationService from OrderEntryViewModel

**Priority**: **OPTIONAL** 🟡  
**Component**: `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`  
**Estimated Effort**: 30 minutes

**Description**:
`OrderEntryViewModel` injects `IOrderNotificationService` in its constructor but never uses it. This is dead code that misleads developers into thinking notifications are handled at the UI layer.

**Evidence**:
- File: `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`, Line 48
- Field `_orderNotificationService` is declared and assigned
- Grep search confirms ZERO usage in any method
- Notifications should be handled in Application layer (command handler), not Presentation layer

**Acceptance Criteria**:
1. Remove `IOrderNotificationService` parameter from constructor
2. Remove `_orderNotificationService` field declaration
3. Verify application still compiles and runs
4. Verify DI container can still resolve `OrderEntryViewModel`
5. Update any unit tests that mock this dependency

**Implementation Notes**:
- Simple cleanup task
- No functional impact (field was never used)
- Improves code clarity

**Release Impact**: **OPTIONAL** - Can defer to v1.1, does not affect functionality

---

## Release Gate Mapping

| Gate | Required Tickets | Status |
|------|------------------|--------|
| **GATE-03: Real-Time Notification** | KDS-001, KDS-002 | ❌ BLOCKED |
| **GATE-04: Code Quality** | KDS-003 | 🟡 OPTIONAL |

---

## Implementation Order

1. **KDS-001** (Interface method) - MUST complete first
2. **KDS-002** (Handler integration) - Depends on KDS-001
3. **KDS-003** (Cleanup) - Can be done anytime, independent

**Estimated Total Effort**: 5.5 hours (excluding testing and code review)

---

## Verification Checklist

After implementing KDS-001 and KDS-002:

- [ ] Launch POS application (Main Window)
- [ ] Launch KDS application (separate window or machine)
- [ ] Create new ticket with food items
- [ ] Click "Send to Kitchen" button
- [ ] **Verify**: KDS screen updates within 2 seconds (no manual refresh)
- [ ] **Verify**: Order appears with correct table number and items
- [ ] **Verify**: Bump order on KDS, verify status change notification works
- [ ] **Verify**: Create another order, verify it also appears immediately
- [ ] **Verify**: Check logs for any notification errors
