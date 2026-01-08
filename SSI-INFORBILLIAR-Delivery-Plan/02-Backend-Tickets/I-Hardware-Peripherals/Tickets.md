# Backend Tickets: Category I - Hardware & Peripherals

> [!NOTE]
> This category has 44.4% parity (4 full, 4 partial, 1 not implemented). Work focuses on completing integrations.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-I.2-01 | I.2 | Complete Cash Drawer Auto-Open | P1 | NOT_STARTED |
| BE-I.4-01 | I.4 | Implement Lamp Control Integration | P1 | NOT_STARTED |
| BE-I.5-01 | I.5 | Complete Barcode Scanner Support | P2 | NOT_STARTED |
| BE-I.6-01 | I.6 | Complete Display Customer Support | P2 | NOT_STARTED |
| BE-I.8-01 | I.8 | Complete Card Reader Integration | P2 | NOT_STARTED |

---

## BE-I.4-01: Implement Lamp Control Integration

**Ticket ID:** BE-I.4-01  
**Feature ID:** I.4  
**Type:** Backend  
**Title:** Implement Lamp Control Integration  
**Priority:** P1

### Outcome (measurable, testable)
Integration with billiard table lamp control systems.

### Scope
- Create `ILampControlService` interface
- Implement control for common protocols (serial, relay)
- Auto-on at session start
- Auto-off at session end
- Manual override support

### Current State (Not Implemented)
- No lamp control exists

### Implementation Notes
```csharp
public interface ILampControlService
{
    Task TurnOnLamp(Guid tableId);
    Task TurnOffLamp(Guid tableId);
    Task<bool> IsLampOn(Guid tableId);
    Task SetAllLamps(bool on);
}

// Configuration
public class LampControlConfig
{
    public string ProtocolType { get; set; }  // Serial, Relay, HTTP
    public string ConnectionString { get; set; }
    public Dictionary<Guid, int> TableLampMapping { get; set; }
}
```

### Acceptance Criteria
- [ ] Service interface created
- [ ] Configuration system works
- [ ] Auto-on at session start
- [ ] Auto-off at session end
- [ ] Manual override available
- [ ] Error handling for hardware failures

---

## BE-I.2-01: Complete Cash Drawer Auto-Open

**Ticket ID:** BE-I.2-01  
**Feature ID:** I.2  
**Type:** Backend  
**Title:** Complete Cash Drawer Auto-Open  
**Priority:** P1

### Outcome (measurable, testable)
Cash drawer automatically opens on cash transactions.

### Scope
- Enhance existing drawer service
- Trigger on cash payment
- Trigger on cash refund
- Manual open command
- Audit drawer operations

### Current State (Partial)
- Drawer service exists
- **Missing:** Auto-trigger, audit

### Implementation Notes
```csharp
public interface ICashDrawerService
{
    Task OpenDrawer();
    Task<bool> IsDrawerOpen();
    Task LogDrawerOperation(DrawerOperationType type, Guid userId, string reason);
}

public enum DrawerOperationType
{
    CashPayment,
    CashRefund,
    NoSaleOpen,
    PayIn,
    PayOut,
    CashDrop
}
```

### Acceptance Criteria
- [ ] Drawer opens on cash payment
- [ ] Drawer opens on cash refund
- [ ] Manual open works
- [ ] Operations logged
- [ ] Error handling for hardware

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 2 | NOT_STARTED |
| P2 | 3 | NOT_STARTED |
| **Total** | **5** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
