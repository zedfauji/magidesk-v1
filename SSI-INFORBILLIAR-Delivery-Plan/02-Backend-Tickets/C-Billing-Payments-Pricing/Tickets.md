# Backend Tickets: Category C - Billing, Payments & Pricing

> [!IMPORTANT]
> This category definition has been **NORMALIZED** to match Feature Universe v2. Previous ID conflicts (C.9 Refund, C.12 Transfer) have been resolved.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-C.1-01 | C.1 | Complete Ticket Creation with Session Link | P0 | COMPLETED |
| BE-C.1-02 | C.1 | Link Customer to Transaction | P1 | NOT_STARTED |
| BE-C.2-01 | C.2 | Implement Hold Ticket (Charge Later) | P2 | NOT_STARTED |
| BE-C.4-01 | C.4 | Complete Split Payment Processing | P1 | NOT_STARTED |
| BE-C.6-01 | C.6 | Implement Gratuity Calculations | P1 | NOT_STARTED |
| BE-C.7-01 | C.7 | Complete Discount Application | P1 | NOT_STARTED |
| BE-C.9-01 | C.9 | Implement Happy Hour Scheduling | P1 | NOT_STARTED |
| BE-C.12-01 | C.12 | Implement Price Override | P1 | NOT_STARTED |
| BE-C.15-01 | C.15 | Complete Void Ticket Processing | P1 | NOT_STARTED |
| BE-C.15-02 | C.15 | Complete Refund Processing | P1 | NOT_STARTED |

---

## BE-C.1-01: Complete Ticket Creation with Session Link

**Ticket ID:** BE-C.1-01  
**Feature ID:** C.1  
**Title:** Complete Ticket Creation with Session Link  
**Priority:** P0

### Outcome
Ticket creation that properly links to table sessions for time-based billing.

### Scope
- Modify `CreateTicketCommand` to accept optional SessionId
- Link Ticket to TableSession
- Auto-populate customer from session if exists

### Acceptance Criteria
- [ ] Ticket links to session
- [ ] Customer auto-populated from session
- [ ] Works without session (walk-in)

---

## BE-C.1-02: Link Customer to Transaction

**Ticket ID:** BE-C.1-02  
**Feature ID:** C.1  
**Title:** Link Customer to Transaction  
**Priority:** P1

### Outcome
Link customers to tickets for tracking purchase history and enabling member benefits.

### Scope
- Add CustomerId to Ticket entity (if not in C.1)
- Create `AssignCustomerToTicketCommand`
- Auto-apply member discounts when customer assigned
- Update customer stats

### Acceptance Criteria
- [ ] Customer assigned to ticket
- [ ] Member discounts auto-apply
- [ ] Purchase history updated

---

## BE-C.2-01: Implement Hold Ticket (Charge Later)

**Ticket ID:** BE-C.2-01  
**Feature ID:** C.2  
**Title:** Implement Hold Ticket (Charge Later)  
**Priority:** P2

### Outcome
Ability to hold tickets for later completion (deferred payment/tab).

### Scope
- Add ticket status: Held
- Create `HoldTicketCommand` and `ReleaseTicketCommand`
- Store hold reason and timestamp
- List all held tickets
- Auto-release table when held

### Implementation Notes
```csharp
public enum TicketStatus
{
    Open,
    Held,      // NEW
    Closed,
    Voided
}

public record HoldTicketCommand(Guid TicketId, string Reason);
public record ReleaseHeldTicketCommand(Guid TicketId);
```

### Acceptance Criteria
- [ ] Tickets can be held
- [ ] Held tickets listed
- [ ] Tickets can be released
- [ ] Table released when held

---

## BE-C.4-01: Complete Split Payment Processing

**Ticket ID:** BE-C.4-01  
**Feature ID:** C.4  
**Title:** Complete Split Payment Processing  
**Priority:** P1

### Outcome
Multiple payment methods/split amounts properly tracked.

### Scope
- Enhance `ProcessPaymentCommand` for split tracking
- Track multiple payments
- Handle partial payments

### Implementation Notes
```csharp
public record ProcessSplitPaymentCommand(
    Guid TicketId,
    IEnumerable<SplitPaymentEntry> Payments
);
```

### Acceptance Criteria
- [ ] Multiple payments recorded
- [ ] Balance calculated correctly
- [ ] Ticket closes when fully paid
- [ ] Overpayment handled

---

## BE-C.6-01: Implement Gratuity Calculations

**Ticket ID:** BE-C.6-01  
**Feature ID:** C.6  
**Title:** Implement Gratuity Calculations  
**Priority:** P1

### Outcome
Proper tip/gratuity handling with suggested amounts.

### Scope
- Add gratuity to Ticket entity
- Create gratuity calculation service
- Support percentage and flat amount

### Acceptance Criteria
- [ ] Suggested tips calculated
- [ ] Gratuity added to ticket
- [ ] Server assignment tracked

---

## BE-C.7-01: Complete Discount Application

**Ticket ID:** BE-C.7-01  
**Feature ID:** C.7  
**Title:** Complete Discount Application  
**Priority:** P1

### Outcome
Full discount application including member, promotional, and manager discounts.

### Scope
- Integrate member discounts
- Complete percentage and fixed discounts
- Add manager override discounts
- Audit applied discounts

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
```

### Acceptance Criteria
- [ ] Percentage discounts work
- [ ] Fixed discounts work
- [ ] Member discounts auto-apply
- [ ] Manager override requires auth
- [ ] Discount audit trail complete

---

## BE-C.9-01: Implement Happy Hour Scheduling

**Ticket ID:** BE-C.9-01  
**Feature ID:** C.9  
**Title:** Implement Happy Hour Scheduling  
**Priority:** P1

### Outcome
Automatic application of promotional pricing based on time/day.

### Scope
- Create `PromotionSchedule` entity (Days, StartTime, EndTime, DiscountId)
- Create `ApplyScheduledPromotions` service (or integrate into Pricing/Discount service)
- Validate Happy Hour applicability during billing

### Implementation Notes
```csharp
public class PromotionSchedule {
    public DayOfWeek Day { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public Guid DiscountId { get; set; }
}
```

### Acceptance Criteria
- [ ] Can define time-based promotion rules
- [ ] Promotions apply automatically during configured window
- [ ] Promotions do NOT apply outside window

---

## BE-C.12-01: Implement Price Override

**Ticket ID:** BE-C.12-01  
**Feature ID:** C.12  
**Title:** Implement Price Override  
**Priority:** P1

### Outcome
Manager can override line item prices with permission.

### Scope
- Create `OverrideLinePriceCommand`
- Require Manager Permission
- Log override Reason and User
- Update OrderLine.UnitPrice (or apply as 100% discount/adjustment)

### Implementation Notes
```csharp
public record OverrideLinePriceCommand(
    Guid TicketId, 
    Guid OrderLineId, 
    Money NewPrice, 
    string Reason, 
    Guid AuthorizingUserId
);
```

### Acceptance Criteria
- [ ] Line item price updated
- [ ] Requires manager auth
- [ ] Audit event created for price change
- [ ] Totals recalculated

---

## BE-C.14-01: Refund Preview Calculation

**Ticket ID:** BE-C.14-01
**Feature ID:** C.14
**Title:** Refund Preview Calculation
**Priority:** P2

### Outcome
Ability to calculate and preview the result of a refund before committing.

### Scope
- Create `CalculateRefundPreviewQuery`
- Return `RefundPreviewDto` containing:
  - Original Totals
  - Projected Totals (Paid, Due)
  - List of Payments to be Refunded
  - List of New Debit Payments to be Created
- **ReadOnly** operation - no state changes

### Acceptance Criteria
- [ ] Returns correct projected totals for full refund
- [ ] Returns correct projected totals for partial refund
- [ ] Returns correct list of affected payments
- [ ] No database changes occur

---

## BE-C.14-02: Partial Refund Command

**Ticket ID:** BE-C.14-02
**Feature ID:** C.14
**Title:** Partial Refund Command
**Priority:** P2

### Outcome
Support for arbitrary partial refund amounts.

### Scope
- Update `RefundTicketCommand` or create new `ProcessPartialRefundCommand`
- Support `RefundAmount` parameter
- Logic rule: If Amount < Total, process as partial
- Refund logic: Credit card refund amount passed to gateway
- Entity logic: Create debit payment for partial amount

### Dependencies
- BE-C.15-02 (Refund Core)

### Acceptance Criteria
- [ ] Can refund specific amount (less than total)
- [ ] Ticket status remains `Closed` if not fully refunded (or Paid < Total?) -> *Check domain rules: Likely remains Closed but PaidAmount decreases. If Paid < Total, is it Closed? No, it becomes Open/Unpaid details.*
  - *Correction: If Paid < Total, ticket is effectively Due. Status should revert to Open? Or PartialRefund state?*
  - **Decision:** Ticket status reverts to `Open` if DueAmount > 0.
- [ ] Payment record created for exact partial amount

---

## BE-C.14-03: Payment-Level Refund Validation

**Ticket ID:** BE-C.14-03
**Feature ID:** C.14
**Title:** Payment-Level Refund Validation
**Priority:** P2

### Outcome
Validate refunds against specific individual payments.

### Scope
- Logic to ensure Refund Amount <= Payment.Amount
- Prevent refunding same payment twice (track RemainingAmount?)
- Support "Include/Exclude" payment logic in RefundCommand
- Validate total refund <= Total Paid

### Acceptance Criteria
- [ ] Cannot refund more than original payment amount
- [ ] Cannot refund already refunded payment
- [ ] Sum of refunds <= Total Ticket Paid amount

---

## BE-C.14-04: Audit & Permission Enforcement

**Ticket ID:** BE-C.14-04
**Feature ID:** C.14
**Title:** Audit & Permission Enforcement
**Priority:** P2

### Outcome
Comprehensive audit trail for advanced refund actions.

### Scope
- Enhance `RefundProcessed` audit event
- Include "Before/After" snapshot in log
- Log specific refund mode (Full vs Partial vs Specific)
- Enforce `UserPermission.RefundTicket` (Existing)

### Acceptance Criteria
- [ ] Audit log clearly shows Partial vs Full
- [ ] "Before" and "After" amounts captured in logs
- [ ] Permission check enforces Manager PIN

---

## BE-C.15-01: Complete Void Ticket Processing

**Ticket ID:** BE-C.15-01  
**Feature ID:** C.15  
**Title:** Complete Void Ticket Processing  
**Priority:** P1

### Outcome
Proper ticket voiding with authorization and audit.

### Scope
- Enhance `VoidTicketCommand`
- Require manager authorization
- Track void reason
- Update table status

### Acceptance Criteria
- [ ] Void requires manager auth
- [ ] Void reason required
- [ ] Paid tickets cannot be voided
- [ ] Table released on void

---

## BE-C.15-02: Complete Refund Processing

**Ticket ID:** BE-C.15-02  
**Feature ID:** C.15  
**Title:** Complete Refund Processing  
**Priority:** P1

### Outcome
Full refund processing with audit trail.

### Scope
- Create `RefundTicketCommand`
- Support full and partial refunds
- Track refund reason and authorization
- Update payment records
- Generate refund receipt

### Implementation Notes
```csharp
public record RefundTicketCommand(
    Guid TicketId,
    Money RefundAmount,
    string Reason,
    Guid AuthorizingUserId,
    RefundMethod Method
);
```

### Acceptance Criteria
- [ ] Full refunds work
- [ ] Partial refunds work
- [ ] Refund reason required
- [ ] Audit trail complete
