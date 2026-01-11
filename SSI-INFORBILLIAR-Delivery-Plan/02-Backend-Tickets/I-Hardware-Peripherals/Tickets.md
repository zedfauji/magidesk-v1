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

## BE-I.5-01: Complete Barcode Scanner Support

**Ticket ID:** BE-I.5-01  
**Feature ID:** I.5  
**Type:** Backend  
**Title:** Complete Barcode Scanner Support  
**Priority:** P2

### Outcome
Barcode scanning integrated for product lookup.

### Scope
- Create barcode scanning service
- Product lookup by barcode
- Quantity entry shortcuts
- Error handling for invalid codes

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | SKU/Barcode support | BE-G.7-01 |

### Acceptance Criteria
- [ ] Barcode scanned correctly
- [ ] Product lookup instant
- [ ] Invalid codes handled
- [ ] Multiple scanner types supported

---

## BE-I.6-01: Complete Customer Display Support

**Ticket ID:** BE-I.6-01  
**Feature ID:** I.6  
**Type:** Backend  
**Title:** Complete Customer Display Support  
**Priority:** P2

### Outcome
Pole display shows pricing to customers.

### Scope
- Create customer display service
- Show item name and price
- Update on line item changes
- Support multiple display types

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | OrderLine entity | Exists |

### Acceptance Criteria
- [ ] Display updates correctly
- [ ] Formatting readable
- [ ] Multiple displays supported
- [ ] Error handling robust

---

## BE-I.8-01: Complete Card Reader Integration

**Ticket ID:** BE-I.8-01  
**Feature ID:** I.8  
**Type:** Backend  
**Title:** Complete Card Reader Integration  
**Priority:** P2

### Outcome
Integrated card payment processing.

### Scope
- Create payment gateway service
- Process card transactions
- Handle EMV chip cards
- Support contactless payments

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Payment processing | Exists |

### Acceptance Criteria
- [ ] Card transactions work
- [ ] EMV supported
- [ ] Contactless works
- [ ] Error handling complete
- [ ] PCI compliance verified

---

## BE-I.10-01: Implement Caller ID Integration

**Ticket ID:** BE-I.10-01  
**Feature ID:** I.10  
**Type:** Backend  
**Title:** Implement Caller ID Integration  
**Priority:** P2

### Outcome
Automatic customer lookup from caller ID.

### Scope
- Create caller ID service
- Phone number normalization
- Customer auto-lookup
- Popup notification

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Customer search | BE-F.2-01 |

### Acceptance Criteria
- [ ] Caller ID captured
- [ ] Customer found automatically
- [ ] Phone normalization works
- [ ] Popup displays customer info

---

## BE-I.11-01: Implement Kitchen Display System

**Ticket ID:** BE-I.11-01  
**Feature ID:** I.11  
**Type:** Backend  
**Title:** Implement Kitchen Display System  
**Priority:** P2

### Outcome
Kitchen display integration for order management.

### Scope
- Create KDS service
- Send orders to kitchen
- Track order status
- Support bump/recall

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | OrderLine entity | Exists |

### Acceptance Criteria
- [ ] Orders sent to KDS
- [ ] Status tracking works
- [ ] Bump functionality works
- [ ] Multiple displays supported

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 2 | NOT_STARTED |
| P2 | 5 | NOT_STARTED |
| **Total** | **7** | **NOT_STARTED** |

---

*Last Updated: 2026-01-10*
