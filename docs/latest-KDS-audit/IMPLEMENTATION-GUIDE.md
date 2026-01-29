# KDS Implementation Guide: Real-Time Notifications

**Date**: 2026-01-28  
**Objective**: Implement real-time KDS notifications for new orders  
**Status**: READY TO START  
**Estimated Time**: 1 day (implementation + testing)

---

## Overview

This guide provides step-by-step instructions to fix the critical gaps preventing real-time KDS notifications. Follow these steps in order.

---

## Prerequisites

- [ ] Read [AUDIT-SUMMARY.md](AUDIT-SUMMARY.md) to understand the problem
- [ ] Review [gap-analysis.md](gap-analysis.md) for technical details
- [ ] Ensure development environment is set up
- [ ] Ensure you can build and run the solution
- [ ] Ensure you have access to both POS and KDS applications

---

## Phase 1: Add Interface Method (KDS-001)

**Estimated Time**: 2 hours  
**Priority**: BLOCKER  
**Dependencies**: None

### Step 1.1: Add Enum Value

**File**: `Magidesk.Application/Services/OrderNotification.cs`  
**Location**: Find the `NotificationType` enum

**Current Code**:
```csharp
public enum NotificationType
{
    OrderReady,
    StatusChange
}
```

**Change To**:
```csharp
public enum NotificationType
{
    OrderReady,
    StatusChange,
    OrderCreated  // NEW: For notifying KDS about new orders
}
```

**Verification**:
- [ ] Enum compiles without errors
- [ ] No breaking changes to existing code

---

### Step 1.2: Add Interface Method

**File**: `Magidesk.Application/Interfaces/IOrderNotificationService.cs`  
**Location**: Add method after `NotifyOrderStatusChangeAsync`

**Add This Method**:
```csharp
/// <summary>
/// Notifies KDS when a new order is created and routed to kitchen.
/// This triggers real-time updates on kitchen display screens.
/// </summary>
/// <param name="kitchenOrderId">The newly created kitchen order ID</param>
/// <param name="tableNumber">The table number for the order</param>
/// <param name="serverName">The server responsible for the table</param>
/// <returns>Task representing the async operation</returns>
Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName);
```

**Verification**:
- [ ] Interface compiles without errors
- [ ] Method signature is correct
- [ ] XML documentation is complete

---

### Step 1.3: Implement Interface Method

**File**: `Magidesk.Application/Services/OrderNotificationService.cs`  
**Location**: Add method after `NotifyOrderStatusChangeAsync` implementation

**Add This Implementation**:
```csharp
public async Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName)
{
    _logger.LogInformation(
        "New order notification: Kitchen Order {KitchenOrderId}, Table {TableNumber}, Server {ServerName}", 
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

**Verification**:
- [ ] Implementation compiles without errors
- [ ] Follows same pattern as `NotifyOrderReadyAsync`
- [ ] Logs notification details
- [ ] Calls `BroadcastNotificationAsync` to trigger SignalR

---

### Step 1.4: Build and Verify

**Commands**:
```bash
cd Magidesk
dotnet build
```

**Verification**:
- [ ] Solution builds successfully
- [ ] No compilation errors
- [ ] No breaking changes to existing code

---

## Phase 2: Inject and Call Notification Service (KDS-002)

**Estimated Time**: 3 hours  
**Priority**: BLOCKER  
**Dependencies**: Phase 1 complete

### Step 2.1: Add Constructor Parameter

**File**: `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`  
**Location**: Constructor

**Current Constructor**:
```csharp
private readonly ITicketRepository _ticketRepository;
private readonly IKitchenPrintService _kitchenPrintService;
private readonly IKitchenRoutingService _kitchenRoutingService;
private readonly IAuditEventRepository _auditEventRepository;

public PrintToKitchenCommandHandler(
    ITicketRepository ticketRepository,
    IKitchenPrintService kitchenPrintService,
    IKitchenRoutingService kitchenRoutingService,
    IAuditEventRepository auditEventRepository)
{
    _ticketRepository = ticketRepository;
    _kitchenPrintService = kitchenPrintService;
    _kitchenRoutingService = kitchenRoutingService;
    _auditEventRepository = auditEventRepository;
}
```

**Change To**:
```csharp
private readonly ITicketRepository _ticketRepository;
private readonly IKitchenPrintService _kitchenPrintService;
private readonly IKitchenRoutingService _kitchenRoutingService;
private readonly IAuditEventRepository _auditEventRepository;
private readonly IOrderNotificationService _notificationService;  // NEW
private readonly ILogger<PrintToKitchenCommandHandler> _logger;  // NEW (if not present)

public PrintToKitchenCommandHandler(
    ITicketRepository ticketRepository,
    IKitchenPrintService kitchenPrintService,
    IKitchenRoutingService kitchenRoutingService,
    IAuditEventRepository auditEventRepository,
    IOrderNotificationService notificationService,  // NEW
    ILogger<PrintToKitchenCommandHandler> logger)  // NEW (if not present)
{
    _ticketRepository = ticketRepository;
    _kitchenPrintService = kitchenPrintService;
    _kitchenRoutingService = kitchenRoutingService;
    _auditEventRepository = auditEventRepository;
    _notificationService = notificationService;  // NEW
    _logger = logger;  // NEW (if not present)
}
```

**Verification**:
- [ ] Fields added
- [ ] Constructor parameters added
- [ ] Assignments added
- [ ] Code compiles

---

### Step 2.2: Add Notification Logic

**File**: `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`  
**Method**: `HandleAsync`  
**Location**: After line 45 (after routing to KDS)

**Current Code** (lines 37-45):
```csharp
// 1. Route to KDS (Database)
try
{
    var ticketDto = MapToDto(ticket);
    await _kitchenRoutingService.RouteToKitchenAsync(ticketDto, command.OrderLineId.HasValue ? new List<Guid> { command.OrderLineId.Value } : null);
}
catch (Exception ex)
{
    errors.Add($"KDS Routing Failed: {ex.Message}");
}
```

**Change To**:
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
            
            _logger.LogInformation(
                "Successfully notified KDS about kitchen order {KitchenOrderId} for table {TableNumber}",
                kitchenOrderId, tableNumber);
        }
        catch (Exception ex)
        {
            // Log but don't throw - notification failure shouldn't break order persistence
            _logger.LogError(ex, 
                "Failed to notify KDS about kitchen order {KitchenOrderId}. Order was still saved to database.",
                kitchenOrderId);
            errors.Add($"KDS Notification Failed: {ex.Message}");
        }
    }
}
```

**Verification**:
- [ ] Return value from routing service is captured
- [ ] Notification called for each kitchen order ID
- [ ] Notification failures are caught and logged
- [ ] Notification failures don't break order persistence
- [ ] Code compiles

---

### Step 2.3: Update Audit Message (Optional)

**File**: `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`  
**Location**: Audit logging section (around line 85)

**Current Code**:
```csharp
$"Printed {orderLinesPrinted} lines to kitchen. KDS: Sent.",
```

**Change To**:
```csharp
$"Printed {orderLinesPrinted} lines to kitchen. KDS: Notified {kitchenOrderIds.Count} orders.",
```

**Verification**:
- [ ] Audit message updated
- [ ] Code compiles

---

### Step 2.4: Build and Verify

**Commands**:
```bash
cd Magidesk
dotnet build
```

**Verification**:
- [ ] Solution builds successfully
- [ ] No compilation errors
- [ ] No DI registration errors (service should already be registered)

---

## Phase 3: Testing

**Estimated Time**: 2 hours  
**Priority**: CRITICAL

### Step 3.1: Unit Test (Optional but Recommended)

**File**: Create `Magidesk.Application.Tests/Services/PrintToKitchenCommandHandlerTests.cs`

**Test Code**:
```csharp
using Xunit;
using Moq;
using Magidesk.Application.Services;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Tests.Services;

public class PrintToKitchenCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_SuccessfulRouting_CallsNotificationService()
    {
        // Arrange
        var mockTicketRepo = new Mock<ITicketRepository>();
        var mockPrintService = new Mock<IKitchenPrintService>();
        var mockRoutingService = new Mock<IKitchenRoutingService>();
        var mockAuditRepo = new Mock<IAuditEventRepository>();
        var mockNotificationService = new Mock<IOrderNotificationService>();
        var mockLogger = new Mock<ILogger<PrintToKitchenCommandHandler>>();

        var testTicketId = Guid.NewGuid();
        var testKitchenOrderId = Guid.NewGuid();
        
        // Setup mocks
        mockRoutingService
            .Setup(x => x.RouteToKitchenAsync(It.IsAny<TicketDto>(), null))
            .ReturnsAsync(new List<Guid> { testKitchenOrderId });

        var handler = new PrintToKitchenCommandHandler(
            mockTicketRepo.Object,
            mockPrintService.Object,
            mockRoutingService.Object,
            mockAuditRepo.Object,
            mockNotificationService.Object,
            mockLogger.Object);

        var command = new PrintToKitchenCommand { TicketId = testTicketId };

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockNotificationService.Verify(
            x => x.NotifyOrderCreatedAsync(
                testKitchenOrderId,
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once,
            "Notification service should be called once for the kitchen order");
    }

    [Fact]
    public async Task HandleAsync_NotificationFails_StillReturnsSuccess()
    {
        // Arrange
        var mockNotificationService = new Mock<IOrderNotificationService>();
        mockNotificationService
            .Setup(x => x.NotifyOrderCreatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("SignalR connection failed"));

        // ... setup other mocks ...

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success, "Order should still be marked as successful even if notification fails");
        Assert.Contains("KDS Notification Failed", result.Errors);
    }
}
```

**Run Tests**:
```bash
cd Magidesk
dotnet test
```

**Verification**:
- [ ] Tests compile
- [ ] Tests pass
- [ ] Notification service is called correctly

---

### Step 3.2: Integration Test (End-to-End)

**Prerequisites**:
- [ ] POS application can be launched
- [ ] KDS application can be launched
- [ ] SignalR hub is running (API server)
- [ ] Database is accessible

**Test Procedure**:

1. **Launch Applications**:
   ```bash
   # Terminal 1: Launch API (SignalR Hub)
   cd Magidesk/src/Magidesk.Api
   dotnet run

   # Terminal 2: Launch POS
   cd Magidesk/src/Magidesk.Presentation
   dotnet run

   # Terminal 3: Launch KDS
   cd Magidesk/src/Magidesk.Presentation
   dotnet run --launch-profile KDS
   ```

2. **Verify Initial State**:
   - [ ] POS application opens successfully
   - [ ] KDS application opens successfully
   - [ ] KDS shows "Connected" status (check logs or UI)

3. **Test Scenario 1: Single Order**:
   - [ ] In POS: Create new ticket
   - [ ] In POS: Add food item (e.g., "Cheeseburger")
   - [ ] In POS: Click "Send to Kitchen" button
   - [ ] **VERIFY**: KDS screen updates within 2 seconds
   - [ ] **VERIFY**: Order appears with correct table number
   - [ ] **VERIFY**: Order shows correct items
   - [ ] **VERIFY**: No manual refresh was required

4. **Test Scenario 2: Multiple Orders**:
   - [ ] In POS: Create 3 different tickets
   - [ ] In POS: Send all 3 to kitchen in rapid succession
   - [ ] **VERIFY**: All 3 orders appear on KDS within 2 seconds each
   - [ ] **VERIFY**: Orders appear in correct chronological order

5. **Test Scenario 3: Status Change (Regression Test)**:
   - [ ] In KDS: Bump an order (change status)
   - [ ] **VERIFY**: Status changes immediately
   - [ ] **VERIFY**: Existing functionality still works

6. **Test Scenario 4: SignalR Failure Resilience**:
   - [ ] Stop the API server (simulate network failure)
   - [ ] In POS: Send order to kitchen
   - [ ] **VERIFY**: Order still saves (check database or logs)
   - [ ] **VERIFY**: POS doesn't crash
   - [ ] **VERIFY**: Error is logged
   - [ ] Restart API server
   - [ ] Wait 60 seconds (polling interval)
   - [ ] **VERIFY**: KDS shows the order (via polling fallback)
   - [ ] In POS: Send another order
   - [ ] **VERIFY**: Real-time notification resumes (< 2 seconds)

**Verification**:
- [ ] All test scenarios pass
- [ ] No regressions in existing functionality
- [ ] System is resilient to failures

---

### Step 3.3: Log Verification

**Check Logs For**:

1. **Successful Notification**:
   ```
   [Information] New order notification: Kitchen Order {guid}, Table {number}, Server {name}
   [Information] Successfully notified KDS about kitchen order {guid} for table {number}
   ```

2. **Notification Failure** (if SignalR is down):
   ```
   [Error] Failed to notify KDS about kitchen order {guid}. Order was still saved to database.
   ```

3. **SignalR Connection** (in KDS logs):
   ```
   [Debug] KDS Connected to SignalR Hub. Stopping Polling.
   ```

**Verification**:
- [ ] Logs show successful notifications
- [ ] Logs show proper error handling
- [ ] No unexpected errors or warnings

---

## Phase 4: Code Cleanup (Optional)

**Estimated Time**: 30 minutes  
**Priority**: OPTIONAL (can defer to v1.1)

### Step 4.1: Remove Unused Service from OrderEntryViewModel

**File**: `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`

**Find and Remove**:
```csharp
// REMOVE THIS FIELD:
private readonly IOrderNotificationService _orderNotificationService;

// REMOVE THIS PARAMETER:
IOrderNotificationService orderNotificationService

// REMOVE THIS ASSIGNMENT:
_orderNotificationService = orderNotificationService;
```

**Verification**:
- [ ] Field removed
- [ ] Constructor parameter removed
- [ ] Assignment removed
- [ ] Code compiles
- [ ] Application runs without DI errors

---

## Phase 5: Final Verification

**Estimated Time**: 30 minutes

### Step 5.1: Release Gate Checklist

Review [release-gate.md](release-gate.md) and verify:

- [ ] **GATE-01: Data Persistence** - Still PASS ✅
- [ ] **GATE-02: Startup Stability** - Still PASS ✅
- [ ] **GATE-03: Real-Time Notification** - Now PASS ✅
- [ ] **GATE-04: SignalR Infrastructure** - Still PASS ✅
- [ ] **GATE-05: Notification Architecture** - Now PASS ✅
- [ ] **GATE-06: Code Quality** - PASS ✅ (if cleanup done)

### Step 5.2: Performance Verification

**Measure Latency**:
1. Create order in POS
2. Start timer
3. Wait for order to appear on KDS
4. Stop timer

**Acceptance**:
- [ ] Average latency < 2 seconds
- [ ] Maximum latency < 2 seconds (excluding network failures)

### Step 5.3: Documentation Update

**Update These Files**:
- [ ] [release-gate.md](release-gate.md) - Change decision to GO ✅
- [ ] [README.md](README.md) - Update status to COMPLETE
- [ ] Add implementation notes to [AUDIT-SUMMARY.md](AUDIT-SUMMARY.md)

---

## Rollback Plan

If issues are discovered:

### Immediate Rollback

**File**: `Magidesk.Application/Services/PrintToKitchenCommandHandler.cs`

**Comment Out Notification Section**:
```csharp
// 1.5. Notify KDS (Real-Time) - TEMPORARILY DISABLED
/*
if (kitchenOrderIds.Any())
{
    // ... notification logic ...
}
*/
```

**Rebuild and Deploy**:
```bash
dotnet build
# Deploy to production
```

**Result**: System reverts to polling mode (60-second updates)

---

## Troubleshooting

### Issue: Notification Service Not Registered

**Error**: `Unable to resolve service for type 'IOrderNotificationService'`

**Solution**: Check DI registration in `ServiceCollectionExtensions.cs` or `Program.cs`:
```csharp
services.AddScoped<IOrderNotificationService, OrderNotificationService>();
```

### Issue: SignalR Connection Fails

**Error**: `SignalR Connection Failed: ...`

**Solution**:
1. Verify API server is running
2. Check SignalR hub URL in KDS settings
3. Verify network connectivity
4. Check firewall settings

### Issue: Orders Still Not Appearing

**Debugging Steps**:
1. Check POS logs for notification calls
2. Check API logs for SignalR broadcasts
3. Check KDS logs for received events
4. Verify database has the orders
5. Check polling fallback is working

---

## Success Criteria

**Implementation is complete when**:
- [ ] All code changes implemented
- [ ] Solution builds without errors
- [ ] Unit tests pass (if written)
- [ ] Integration tests pass
- [ ] Orders appear on KDS within 2 seconds
- [ ] No regressions in existing functionality
- [ ] System is resilient to SignalR failures
- [ ] Logs show proper operation
- [ ] Release gates pass

---

## Timeline

**Day 1**:
- Morning: Phase 1 (Interface) + Phase 2 (Handler)
- Afternoon: Phase 3 (Testing)
- Evening: Phase 4 (Cleanup) + Phase 5 (Verification)

**Day 2** (if needed):
- Code review
- Additional testing
- Documentation updates

---

## Next Steps

1. **Start Implementation**: Begin with Phase 1, Step 1.1
2. **Follow Steps in Order**: Don't skip steps
3. **Verify Each Step**: Check off verification items
4. **Test Thoroughly**: Don't skip testing phase
5. **Update Documentation**: Keep audit documents current

---

**Implementation Status**: READY TO START  
**Estimated Completion**: 1 day  
**Next Action**: Begin Phase 1, Step 1.1

---

**Good luck with the implementation!** 🚀
