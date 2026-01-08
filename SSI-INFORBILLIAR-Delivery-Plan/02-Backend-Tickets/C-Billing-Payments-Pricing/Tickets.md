# Backend Tickets: Category C - Billing, Payments & Pricing

> [!IMPORTANT]
> This category has 31.3% full parity. Critical time-based billing features depend on Category A tickets.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-C.1-01 | C.1 | Complete Ticket Creation with Session Link | P0 | NOT_STARTED |
| BE-C.2-01 | C.2 | Implement Time-Based Line Items | P0 | NOT_STARTED |
| BE-C.3-01 | C.3 | Complete Product Addition with Modifiers | P1 | NOT_STARTED |
| BE-C.5-01 | C.5 | Complete Split Payment Processing | P1 | NOT_STARTED |
| BE-C.6-01 | C.6 | Implement Gratuity Calculations | P1 | NOT_STARTED |
| BE-C.7-01 | C.7 | Complete Discount Application | P1 | NOT_STARTED |
| BE-C.9-01 | C.9 | Complete Refund Processing | P1 | NOT_STARTED |
| BE-C.10-01 | C.10 | Complete Void Ticket Processing | P1 | NOT_STARTED |
| BE-C.11-01 | C.11 | Implement Hold/Release Ticket | P2 | NOT_STARTED |
| BE-C.12-01 | C.12 | Complete Ticket Transfer | P2 | NOT_STARTED |
| BE-C.14-01 | C.14 | Add Customer to Ticket | P1 | NOT_STARTED |

---

## BE-C.1-01: Complete Ticket Creation with Session Link

**Ticket ID:** BE-C.1-01  
**Feature ID:** C.1  
**Type:** Backend  
**Title:** Complete Ticket Creation with Session Link  
**Priority:** P0

### Outcome (measurable, testable)
Ticket creation that properly links to table sessions for time-based billing.

### Scope
- Modify `CreateTicketCommand` to accept optional SessionId
- Link Ticket to TableSession
- Auto-populate customer from session if exists

### Current State (Partial)
- Ticket creation exists
- **Missing:** Session linkage, auto-customer

### Implementation Notes
```csharp
public record CreateTicketCommand(
    Guid TableId,
    Guid? SessionId,    // NEW: Link to table session
    Guid? CustomerId,   // Explicit or auto from session
    int GuestCount
);

// Handler modifications:
// 1. If SessionId provided, validate session
// 2. Auto-populate CustomerId from session.CustomerId if not provided
// 3. Set Ticket.SessionId
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |

### Acceptance Criteria
- [ ] Ticket links to session
- [ ] Customer auto-populated from session
- [ ] Works without session (walk-in)
- [ ] Tests pass

---

## BE-C.2-01: Implement Time-Based Line Items

**Ticket ID:** BE-C.2-01  
**Feature ID:** C.2  
**Type:** Backend  
**Title:** Implement Time-Based Line Items  
**Priority:** P0

### Outcome (measurable, testable)
Ability to add time-based charges as order lines when session ends.

### Scope
- Create dedicated `TimeChargeOrderLine` type or flag
- Generate line item from session end
- Display time duration and rate on ticket

### Current State (Partial)
- Order lines exist
- **Missing:** Time-specific line type

### Implementation Notes
```csharp
// Add to OrderLine or create subtype
public OrderLine CreateTimeChargeLine(
    Guid ticketId,
    TimeSpan duration,
    decimal hourlyRate,
    Money totalCharge,
    string description  // e.g., "Table Time: 2h 15m"
);

// Properties to add to OrderLine
public TimeSpan? Duration { get; private set; }
public decimal? HourlyRate { get; private set; }
public bool IsTimeCharge { get; private set; }
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | PricingService | BE-A.9-01 |
| HARD | EndTableSessionCommand | BE-A.2-01 |

### Acceptance Criteria
- [ ] Time charge line item created correctly
- [ ] Duration displayed on line
- [ ] Rate displayed on line
- [ ] Total calculated correctly
- [ ] Taxable status configurable

---

## BE-C.5-01: Complete Split Payment Processing

**Ticket ID:** BE-C.5-01  
**Feature ID:** C.5  
**Type:** Backend  
**Title:** Complete Split Payment Processing  
**Priority:** P1

### Outcome (measurable, testable)
Multiple payment methods/split amounts properly tracked.

### Scope
- Enhance `ProcessPaymentCommand` for split tracking
- Track multiple payments against one ticket
- Calculate remaining balance
- Handle partial payments

### Current State (Partial)
- Single payment works
- **Missing:** Split tracking, partial payment handling

### Implementation Notes
```csharp
public record ProcessSplitPaymentCommand(
    Guid TicketId,
    IEnumerable<SplitPaymentEntry> Payments
);

public record SplitPaymentEntry(
    PaymentType Type,
    Money Amount,
    string Reference // Card last 4, check #, etc.
);

// Handler:
// 1. Validate sum >= remaining balance
// 2. Create Payment records for each entry
// 3. Update ticket balance
// 4. Close ticket if fully paid
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Payment entity | Exists |
| SOFT | Ticket entity | Exists |

### Acceptance Criteria
- [ ] Multiple payments recorded
- [ ] Balance calculated correctly
- [ ] Ticket closes when fully paid
- [ ] Overpayment handled (change due)
- [ ] Transaction audit complete

---

## BE-C.6-01: Implement Gratuity Calculations

**Ticket ID:** BE-C.6-01  
**Feature ID:** C.6  
**Type:** Backend  
**Title:** Implement Gratuity Calculations  
**Priority:** P1

### Outcome (measurable, testable)
Proper tip/gratuity handling with suggested amounts.

### Scope
- Add gratuity to Ticket entity
- Create gratuity calculation service
- Support percentage and flat amount
- Track server gratuity assignment

### Current State (Partial)
- Basic tip concept exists
- **Missing:** Suggested amounts, proper calculation

### Implementation Notes
```csharp
public interface IGratuityService
{
    GratuitySuggestions GetSuggestions(Money subtotal);
    void ApplyGratuity(Ticket ticket, Money amount);
}

public record GratuitySuggestions(
    Money Percent15,
    Money Percent18,
    Money Percent20,
    Money Percent25
);

// Add to Ticket
public Money GratuityAmount { get; private set; }
public Guid? GratuityServerUserId { get; private set; }
```

### Acceptance Criteria
- [ ] Suggested tips calculated
- [ ] Gratuity added to ticket
- [ ] Server assignment tracked
- [ ] Reports show gratuity totals

---

## BE-C.7-01: Complete Discount Application

**Ticket ID:** BE-C.7-01  
**Feature ID:** C.7  
**Type:** Backend  
**Title:** Complete Discount Application  
**Priority:** P1

### Outcome (measurable, testable)
Full discount application including member, promotional, and manager discounts.

### Scope
- Integrate member discounts (from F.5)
- Complete percentage and fixed discounts
- Add manager override discounts
- Audit all discounts applied

### Current State (Partial)
- Basic discount exists
- **Missing:** Member integration, manager override

### Implementation Notes
```csharp
public enum DiscountType
{
    Percentage,
    FixedAmount,
    MemberDiscount,
    ManagerOverride,
    Promotional
}

public record ApplyDiscountCommand(
    Guid TicketId,
    DiscountType Type,
    decimal Value,           // Percent or amount
    string Reason,           // Required for manager override
    Guid? AuthorizingUserId  // Required for manager override
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | MemberDiscountService | BE-F.5-01 |

### Acceptance Criteria
- [ ] Percentage discounts work
- [ ] Fixed discounts work
- [ ] Member discounts auto-apply
- [ ] Manager override requires auth
- [ ] Discount audit trail complete

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 2 | NOT_STARTED |
| P1 | 7 | NOT_STARTED |
| P2 | 2 | NOT_STARTED |
| **Total** | **11** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
