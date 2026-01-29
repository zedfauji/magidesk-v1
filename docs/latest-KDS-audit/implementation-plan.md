# KDS Implementation Plan

**Objective**: Resolve critical gaps preventing real-time KDS notifications  
**Target**: Production readiness for Order → KDS lifecycle  
**Date**: 2026-01-28

---

## Overview

This plan addresses **2 BLOCKER tickets** that must be completed before production deployment. The fixes are architecturally straightforward but require careful implementation to ensure notification failures don't break order persistence.

**Total Estimated Effort**: 5.5 hours (excluding testing and code review)

---

## Phase 1: Interface Extension (BLOCKER)

### Task 1.1: Add NotifyOrderCreatedAsync Method

**Ticket**: KDS-001  
**Priority**: BLOCKER  
**Estimated Time**: 2 hours  
**Dependencies**: None

**Files to Modify**:
1. `Magidesk.Application/Interfaces/IOrderNotificationService.cs`
2. `Magidesk.Application/Services/OrderNotificationService.cs`
3. `Magidesk.Application/Services/OrderNotification.cs` (add enum value)

**Implementation Steps**:

1. **Update Interface** (`IOrderNotificationService.cs`):
   ```csharp
   /// <summary>
   /// Notifies KDS when a new order is created and routed to kitchen.
   /// </summary>
   /// <param name="kitchenOrderId">The newly created kitchen order ID</param>
   /// <param name="tableNumber">The table number for the order</param>
   /// <param name="serverName">The server responsible for the table</param>
   Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName);
   ```

2. **Add Enum Value** (`OrderNotification.cs` or separate enum file):
   ```csharp
   public enum NotificationType
   {
       OrderReady,
       StatusChange,
       OrderCreated  // NEW
   }
   ```

3. **Implement Method** (`OrderNotificationService.cs`):
   ```csharp
   public async Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName)
   {
       _logger.LogInformation("New order notification: Kitchen Order {KitchenOrderId}, Table {TableNumber}, Server {ServerName}", 
           kitchenOrderId, tableNumber, serverName);

       var notification = new OrderNotification
       {
           Id = Guid.NewGuid(),
           Type = NotificationType.OrderCreated,
           KitchenOrderId = kitchenOrderId,
           TableNumber = tableNumber,
           ServerName = serverName,
           Message = $"New order for Table {tableNumber}",
           Timestamp = DateTime.UtcNow
       };

       await BroadcastNotificationAsync(notification);
   }
   ```

4. **Verify Compilation**:
   - Build solution
   - Ensure no breaking changes to existing code
   - Verify `SignalRKitchenNotificationPublisher` still compiles (it uses `OrderNotification` type)

**Acceptance Criteria**:
- [ ] Interface method added with XML documentation
- [ ] Enum value `OrderCreated` added to `NotificationType`
- [ ] Implementation follows existing pattern (matches `NotifyOrderReadyAsync`)
- [ ] Method logs notification details
- [ ] Method calls `BroadcastNotificationAsync` (triggers SignalR)
- [ ] Solution compiles without errors
- [ ] No breaking changes to existing notification methods

---

## Phase 2: Handler Integration (BLOCKER)

### Task 2.1: Inject Notification Service into PrintToKitchenCommandHandler

**Ticket**: KDS-002  
**Priority**: BLOCKER  
**Estimated Time**: 3 hours  
**Dependencies**: Task 1.1 (KDS-001)

**Files to Modify**:
1. `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`
2. Potentially: DI registration (if not auto-discovered)

**Implementation Steps**:

1. **Add Constructor Parameter**:
   ```csharp
   private readonly IOrderNotificationService _notificationService;

   public PrintToKitchenCommandHandler(
       ITicketRepository ticketRepository,
       IKitchenPrintService kitchenPrintService,
       IKitchenRoutingService kitchenRoutingService,
       IAuditEventRepository auditEventRepository,
       IOrderNotificationService notificationService)  // NEW
   {
       _ticketRepository = ticketRepository;
       _kitchenPrintService = kitchenPrintService;
       _kitchenRoutingService = kitchenRoutingService;
       _auditEventRepository = auditEventRepository;
       _notificationService = notificationService;  // NEW
   }
   ```

2. **Add Notification Logic** (after line 45 in `HandleAsync`):
   ```csharp
   // 1. Route to KDS (Database)
   List<Guid> kitchenOrderIds = new();
   try
   {
       var ticketDto = MapToDto(ticket);
       kitchenOrderIds = await _kitchenRoutingService.RouteToKitchenAsync(
           ticketDto, 
           command.OrderLineId.HasValue ? new List<Guid> { command.OrderLineId.Value } : null);
   }
   catch (Exception ex)
   {
       errors.Add($"KDS Routing Failed: {ex.Message}");
   }

   // 1.5. Notify KDS (Real-Time) - NEW SECTION
   if (kitchenOrderIds.Any())
   {
       var tableNumber = ticket.TableNumbers.FirstOrDefault() ?? "Unknown";
       var serverName = "Server"; // TODO: Get from ticket owner or user context

       foreach (var kitchenOrderId in kitchenOrderIds)
       {
           try
           {
               await _notificationService.NotifyOrderCreatedAsync(
                   kitchenOrderId,
                   tableNumber,
                   serverName);
           }
           catch (Exception ex)
           {
               // Log but don't throw - notification failure shouldn't break order persistence
               _logger.LogError(ex, "Failed to notify KDS about kitchen order {KitchenOrderId}", kitchenOrderId);
               errors.Add($"KDS Notification Failed: {ex.Message}");
           }
       }
   }

   // 2. Physical Printing (continues as before)
   ```

3. **Add Logger Field** (if not already present):
   ```csharp
   private readonly ILogger<PrintToKitchenCommandHandler> _logger;
   ```

4. **Verify DI Registration**:
   - Check `ServiceCollectionExtensions.cs` or `Program.cs`
   - Ensure `IOrderNotificationService` is registered
   - Should already be registered (used by `KitchenStatusService`)

5. **Update Audit Message** (optional but recommended):
   ```csharp
   $"Printed {orderLinesPrinted} lines to kitchen. KDS: Notified {kitchenOrderIds.Count} orders."
   ```

**Acceptance Criteria**:
- [ ] `IOrderNotificationService` injected in constructor
- [ ] Notification called for each kitchen order ID returned by routing service
- [ ] Notification failures are caught and logged (don't break order persistence)
- [ ] Table number and server name are passed to notification
- [ ] Solution compiles without errors
- [ ] DI container can resolve `PrintToKitchenCommandHandler`

---

## Phase 3: Testing & Verification (CRITICAL)

### Task 3.1: Unit Tests

**Estimated Time**: 1 hour

**Test Cases**:

1. **Test: Notification Service Called on Successful Routing**
   ```csharp
   [Fact]
   public async Task HandleAsync_SuccessfulRouting_CallsNotificationService()
   {
       // Arrange
       var mockNotificationService = new Mock<IOrderNotificationService>();
       var mockRoutingService = new Mock<IKitchenRoutingService>();
       mockRoutingService.Setup(x => x.RouteToKitchenAsync(It.IsAny<TicketDto>(), null))
           .ReturnsAsync(new List<Guid> { Guid.NewGuid() });
       
       var handler = new PrintToKitchenCommandHandler(
           /* ... other mocks ... */,
           mockNotificationService.Object);
       
       // Act
       await handler.HandleAsync(new PrintToKitchenCommand { TicketId = Guid.NewGuid() });
       
       // Assert
       mockNotificationService.Verify(
           x => x.NotifyOrderCreatedAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
           Times.Once);
   }
   ```

2. **Test: Notification Failure Doesn't Break Order Persistence**
   ```csharp
   [Fact]
   public async Task HandleAsync_NotificationFails_StillReturnsSuccess()
   {
       // Arrange
       var mockNotificationService = new Mock<IOrderNotificationService>();
       mockNotificationService.Setup(x => x.NotifyOrderCreatedAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
           .ThrowsAsync(new Exception("SignalR connection failed"));
       
       // Act
       var result = await handler.HandleAsync(command);
       
       // Assert
       Assert.True(result.Success); // Order still persisted
       Assert.Contains("KDS Notification Failed", result.Errors);
   }
   ```

### Task 3.2: Integration Test (End-to-End)

**Estimated Time**: 1 hour

**Test Procedure**:

1. **Setup**:
   - Launch POS application (Main Window)
   - Launch KDS application (separate window or machine)
   - Ensure SignalR hub is running (API server)
   - Verify KDS shows "Connected" status

2. **Test Scenario 1: New Order Creation**:
   - Create new ticket in POS
   - Add food items (e.g., "Cheeseburger")
   - Click "Send to Kitchen" button
   - **Expected**: KDS screen updates within 2 seconds
   - **Expected**: Order appears with correct table number and items
   - **Expected**: No manual refresh required

3. **Test Scenario 2: Multiple Orders**:
   - Create 3 tickets in rapid succession
   - Send all to kitchen
   - **Expected**: All 3 appear on KDS within 2 seconds each
   - **Expected**: Orders appear in correct chronological order

4. **Test Scenario 3: Status Change (Existing Functionality)**:
   - Bump an order on KDS
   - **Expected**: Status changes immediately (proves existing notifications still work)

5. **Test Scenario 4: SignalR Failure Resilience**:
   - Stop SignalR hub (simulate network failure)
   - Send order to kitchen
   - **Expected**: Order still persists to database
   - **Expected**: Error logged but POS doesn't crash
   - **Expected**: KDS falls back to polling (60-second update)
   - Restart SignalR hub
   - **Expected**: KDS reconnects automatically
   - Send another order
   - **Expected**: Real-time notification resumes

**Acceptance Criteria**:
- [ ] Orders appear on KDS within 2 seconds of "Send to Kitchen"
- [ ] No manual refresh required
- [ ] Multiple orders handled correctly
- [ ] Existing status change notifications still work
- [ ] System resilient to SignalR failures
- [ ] Logs show notification success/failure appropriately

---

## Phase 4: Code Cleanup (OPTIONAL)

### Task 4.1: Remove Unused Service from OrderEntryViewModel

**Ticket**: KDS-003  
**Priority**: OPTIONAL  
**Estimated Time**: 30 minutes  
**Dependencies**: None (can be done anytime)

**Files to Modify**:
1. `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`

**Implementation Steps**:

1. **Remove Constructor Parameter**:
   ```csharp
   // REMOVE THIS:
   // IOrderNotificationService orderNotificationService
   ```

2. **Remove Field Declaration**:
   ```csharp
   // REMOVE THIS:
   // private readonly IOrderNotificationService _orderNotificationService;
   ```

3. **Remove Assignment**:
   ```csharp
   // REMOVE THIS:
   // _orderNotificationService = orderNotificationService;
   ```

4. **Verify Compilation**:
   - Build solution
   - Ensure DI container can still resolve `OrderEntryViewModel`

**Acceptance Criteria**:
- [ ] Constructor parameter removed
- [ ] Field declaration removed
- [ ] Solution compiles without errors
- [ ] Application runs without DI errors

---

## Dependency Graph

```
KDS-001 (Interface)
    ↓
KDS-002 (Handler) → Testing (Phase 3)
    ↓
Production Deployment ✅

KDS-003 (Cleanup) → Independent, can be done anytime
```

---

## Rollback Plan

If issues are discovered after deployment:

1. **Immediate Rollback**:
   - Comment out notification calls in `PrintToKitchenCommandHandler`
   - System reverts to polling mode (60-second updates)
   - Orders still persist correctly

2. **Temporary Fix**:
   ```csharp
   // Wrap entire notification section in feature flag
   if (_configuration.GetValue<bool>("Features:RealtimeKDS", false))
   {
       // ... notification logic ...
   }
   ```

3. **Investigation**:
   - Check SignalR hub logs
   - Check POS application logs
   - Check KDS application logs
   - Verify network connectivity

---

## Success Criteria

**Definition of Done**:
- [ ] KDS-001 implemented and tested
- [ ] KDS-002 implemented and tested
- [ ] All unit tests passing
- [ ] Integration test successful (< 2 second latency)
- [ ] Code reviewed and approved
- [ ] Documentation updated
- [ ] Release gate GATE-03 passes

**Production Readiness**:
- [ ] Orders appear on KDS within 2 seconds
- [ ] System resilient to SignalR failures
- [ ] No regression in existing functionality
- [ ] Logs provide adequate troubleshooting information
