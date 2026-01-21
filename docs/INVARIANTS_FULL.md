# Magidesk POS - Core Invariants (Full POS Scope)

## Definition
Invariants are business rules that MUST NEVER be violated. They are enforced at the Domain layer and cannot be bypassed. This document covers invariants for the full POS system.

## Financial Invariants

### F1: Money Precision
- **Rule**: All monetary amounts must be rounded to 2 decimal places
- **Enforcement**: Money value object
- **Violation**: Cannot create Money with more than 2 decimal places

### F2: Non-Negative Amounts
- **Rule**: Payment amounts, ticket totals, and cash balances cannot be negative
- **Enforcement**: Domain entity constructors and setters
- **Violation**: Throws `BusinessRuleViolationException`

### F3: Payment-Total Balance (Split Payments)
- **Rule**: Sum of all payments on a ticket must be >= ticket total before closing
- **Enforcement**: `TicketDomainService.CanCloseTicket()`
- **Violation**: Cannot close ticket, throws `InvalidOperationException`
- **Note**: Supports partial payments, but ticket closes only when PaidAmount >= TotalAmount

### F4: Immutable Finalized Records
- **Rule**: Closed tickets, completed payments, and closed cash sessions cannot be modified
- **Enforcement**: Entity state checks before mutations
- **Violation**: Throws `InvalidOperationException`

### F5: Cash Change Calculation
- **Rule**: For cash payments, ChangeAmount = TenderedAmount - Amount, and TenderedAmount >= Amount
- **Enforcement**: `PaymentDomainService.CalculateChange()`
- **Violation**: Cannot process cash payment with insufficient tender

### F6: Gift Certificate Cash Back
- **Rule**: Gift cert cash back <= (FaceValue - PaidAmount)
- **Enforcement**: Payment processing logic
- **Violation**: Cannot process gift cert payment, throws `BusinessRuleViolationException`

### F7: Tips Amount
- **Rule**: TipsAmount >= 0, TipsExceedAmount >= 0
- **Enforcement**: Payment entity
- **Violation**: Cannot create payment with negative tips

### F8: Refund Amount Limit
- **Rule**: Refund amount <= original payment amount
- **Enforcement**: `PaymentDomainService.CanRefundPayment()`
- **Violation**: Cannot refund, throws `BusinessRuleViolationException`

## Ticket Invariants

### T1: Non-Empty Ticket
- **Rule**: Ticket must have at least one OrderLine before opening
- **Enforcement**: `TicketDomainService.CanOpenTicket()`
- **Violation**: Cannot open ticket, throws `BusinessRuleViolationException`

### T2: No Modifications to Finalized Tickets
- **Rule**: Cannot add/remove/modify OrderLines on Closed, Voided, or Refunded tickets
- **Enforcement**: OrderLine operations check ticket status
- **Violation**: Throws `InvalidOperationException`

### T3: Void Before Payment
- **Rule**: Ticket can only be voided if it has no completed payments (PaidAmount == 0)
- **Enforcement**: `TicketDomainService.CanVoidTicket()`
- **Violation**: Cannot void ticket, throws `InvalidOperationException`
- **Note**: Tickets with payments must be refunded, not voided

### T4: Total Calculation Consistency
- **Rule**: TotalAmount = Subtotal + Tax + ServiceCharge + DeliveryCharge + Adjustment - Discount + Gratuity
- **Enforcement**: `TicketDomainService.CalculateTotals()`
- **Violation**: Throws `BusinessRuleViolationException` if calculation fails

### T5: Due Amount Calculation
- **Rule**: DueAmount = TotalAmount - PaidAmount
- **Enforcement**: `TicketDomainService.CalculateTotals()`
- **Violation**: Throws `BusinessRuleViolationException` if calculation fails

### T6: Cannot Close with Outstanding Balance
- **Rule**: Ticket cannot be closed if DueAmount > 0
- **Enforcement**: `TicketDomainService.CanCloseTicket()`
- **Violation**: Cannot close ticket, throws `InvalidOperationException`

### T7: Refund Eligibility
- **Rule**: Can only refund tickets that are Paid and Closed
- **Enforcement**: `TicketDomainService.CanRefundTicket()`
- **Violation**: Cannot refund, throws `InvalidOperationException`

### T8: Split Ticket Eligibility
- **Rule**: Can only split tickets that are Open (not paid, not closed, not voided)
- **Enforcement**: `TicketDomainService.CanSplitTicket()`
- **Violation**: Cannot split ticket, throws `InvalidOperationException`

### T9: Reopen Eligibility
- **Rule**: Can only reopen Closed tickets (not voided, not refunded)
- **Enforcement**: `TicketDomainService.CanReopenTicket()`
- **Violation**: Cannot reopen ticket, throws `InvalidOperationException`

## OrderLine Invariants

### OL1: Positive Quantity
- **Rule**: Quantity > 0 OR ItemCount > 0
- **Enforcement**: OrderLine constructor
- **Violation**: Cannot create OrderLine, throws `BusinessRuleViolationException`

### OL2: Non-Negative Unit Price
- **Rule**: UnitPrice >= 0
- **Enforcement**: OrderLine constructor and setter
- **Violation**: Throws `BusinessRuleViolationException`

### OL3: Discount Limit
- **Rule**: DiscountAmount <= SubtotalAmount
- **Enforcement**: OrderLine discount application
- **Violation**: Throws `BusinessRuleViolationException`

### OL4: Non-Negative Totals
- **Rule**: All calculated amounts (Subtotal, Tax, Total) >= 0
- **Enforcement**: OrderLine.CalculatePrice()
- **Violation**: Throws `BusinessRuleViolationException`

### OL5: Cannot Modify Finalized
- **Rule**: Cannot modify OrderLine if parent ticket is Closed/Voided/Refunded
- **Enforcement**: OrderLine mutation methods
- **Violation**: Throws `InvalidOperationException`

## Payment Invariants

### P1: Positive Payment Amount
- **Rule**: Payment.Amount must be > 0
- **Enforcement**: Payment constructor
- **Violation**: Cannot create payment, throws `BusinessRuleViolationException`

### P2: Split Payment Total
- **Rule**: Sum of all payments on ticket <= Ticket.TotalAmount (can be partial)
- **Enforcement**: `TicketDomainService.CanAddPayment()`
- **Violation**: Cannot add payment if would exceed total, throws `BusinessRuleViolationException`

### P3: Cash Tender Sufficiency
- **Rule**: For cash payments, TenderedAmount >= Amount
- **Enforcement**: Payment processing logic
- **Violation**: Cannot process payment, throws `BusinessRuleViolationException`

### P4: Immutable Completed Payments
- **Rule**: Payment that is Captured/Completed cannot be modified
- **Enforcement**: Payment mutation methods
- **Violation**: Throws `InvalidOperationException`

### P5: Void Eligibility
- **Rule**: Can only void payments that are not already voided
- **Enforcement**: `PaymentDomainService.CanVoidPayment()`
- **Violation**: Cannot void payment, throws `InvalidOperationException`

### P6: Refund Eligibility
- **Rule**: Can only refund payments that are Completed/Captured
- **Enforcement**: `PaymentDomainService.CanRefundPayment()`
- **Violation**: Cannot refund payment, throws `InvalidOperationException`

### P7: Card Authorization
- **Rule**: Card payments can be authorized then captured, or captured directly
- **Enforcement**: Payment processing workflow
- **Violation**: Cannot capture unauthorized payment, throws `InvalidOperationException`

### P8: Gift Certificate Validation
- **Rule**: Gift cert paid amount <= face value
- **Enforcement**: Gift cert payment processing
- **Violation**: Cannot process payment, throws `BusinessRuleViolationException`

## Discount Invariants

### D1: Single Discount Application
- **Rule**: Only one discount applies per ticket/item (max discount selected)
- **Enforcement**: `DiscountDomainService.GetMaxDiscount()`
- **Violation**: Multiple discounts not applied simultaneously

### D2: Discount Eligibility
- **Rule**: Discount can only apply if minimum buy/quantity requirements met
- **Enforcement**: `DiscountDomainService.IsEligible()`
- **Violation**: Discount not applied if requirements not met

### D3: Discount Amount Limit
- **Rule**: Discount amount cannot exceed item/ticket subtotal
- **Enforcement**: Discount calculation
- **Violation**: Discount capped at subtotal amount

### D4: Immutable Discount Snapshots
- **Rule**: TicketDiscount and OrderLineDiscount are immutable snapshots
- **Enforcement**: Discount entities are immutable
- **Violation**: Compile-time prevention (no setters after creation)

## Cash Session Invariants

### C1: Non-Negative Opening Balance
- **Rule**: CashSession.OpeningBalance >= 0
- **Enforcement**: CashSession constructor
- **Violation**: Cannot create session, throws `BusinessRuleViolationException`

### C2: No Close with Open Tickets
- **Rule**: Cannot close cash session if user has open (non-closed) tickets
- **Enforcement**: `CashSessionDomainService.CanCloseSession()`
- **Violation**: Cannot close session, throws `InvalidOperationException`

### C3: Expected Cash Calculation
- **Rule**: ExpectedCash = OpeningBalance + Sum(CashPayments) - Sum(CashRefunds) - Sum(Payouts) - Sum(CashDrops) - Sum(Bleeds)
- **Enforcement**: `CashSessionDomainService.CalculateExpectedCash()`
- **Violation**: Throws `BusinessRuleViolationException` if calculation fails

### C4: Immutable Closed Sessions
- **Rule**: Closed cash session cannot be modified
- **Enforcement**: CashSession mutation methods
- **Violation**: Throws `InvalidOperationException`

### C5: Single Open Session
- **Rule**: User can have only one open cash session at a time
- **Enforcement**: Cash session creation logic
- **Violation**: Cannot open new session, throws `InvalidOperationException`

## Gratuity Invariants

### G1: Non-Negative Gratuity
- **Rule**: Gratuity.Amount >= 0
- **Enforcement**: Gratuity entity
- **Violation**: Throws `BusinessRuleViolationException`

### G2: Gratuity on Ticket
- **Rule**: Gratuity belongs to a ticket
- **Enforcement**: Gratuity entity
- **Violation**: Cannot create gratuity without ticket

## Refund Invariants

### R1: Refund on Paid Ticket
- **Rule**: Can only refund tickets that are Paid and Closed
- **Enforcement**: `TicketDomainService.CanRefundTicket()`
- **Violation**: Cannot refund, throws `InvalidOperationException`

### R2: Refund Amount Limit
- **Rule**: Refund amount <= Ticket.PaidAmount
- **Enforcement**: Refund processing
- **Violation**: Cannot refund, throws `BusinessRuleViolationException`

### R3: Refund Creates Debit Transaction
- **Rule**: Refund creates TransactionType.Debit payment
- **Enforcement**: Refund processing
- **Violation**: System design ensures this

## Ticket Split Invariants

### S1: Split Eligibility
- **Rule**: Can only split Open tickets (not paid, not closed, not voided)
- **Enforcement**: `TicketDomainService.CanSplitTicket()`
- **Violation**: Cannot split ticket, throws `InvalidOperationException`

### S2: Split Items
- **Rule**: Split must include at least one item
- **Enforcement**: Split ticket logic
- **Violation**: Cannot split with no items, throws `BusinessRuleViolationException`

### S3: Original Ticket Remains Valid
- **Rule**: Original ticket must have at least one item after split
- **Enforcement**: Split ticket logic
- **Violation**: Cannot split all items, throws `BusinessRuleViolationException`

## Audit Invariants

### A1: Immutable Audit Events
- **Rule**: AuditEvent cannot be modified after creation
- **Enforcement**: AuditEvent is immutable class
- **Violation**: Compile-time prevention (no setters)

### A2: Complete State Snapshots
- **Rule**: AuditEvent must contain complete entity state (BeforeState and AfterState)
- **Enforcement**: Audit service implementation
- **Violation**: Throws `BusinessRuleViolationException`

### A3: All Financial Mutations Audited
- **Rule**: Every mutation to Ticket, Payment, or CashSession must emit AuditEvent
- **Enforcement**: Application layer ensures events are created
- **Violation**: System design prevents bypass (events are part of transaction)

## Concurrency Invariants

### CO1: Optimistic Concurrency
- **Rule**: Entity version must match database version for updates
- **Enforcement**: EF Core optimistic concurrency control
- **Violation**: Throws `ConcurrencyException`, requires retry

## User Interface Invariants

### UI1: No Business Logic in UI
- **Rule**: Views and ViewModels contain zero business logic
- **Enforcement**: Code review, architecture validation
- **Violation**: Architectural violation (not runtime)

### UI2: No Direct Database Access
- **Rule**: UI layer cannot access database directly
- **Enforcement**: Dependency injection, no EF Core in Presentation layer
- **Violation**: Compile-time prevention (no DbContext in Presentation)

## Legacy Behaviors REJECTED

### L1: Allow Negative Balances
- **Rejection**: System will NOT allow negative cash balances or negative payment amounts
- **Reason**: Financial correctness and auditability

### L2: Modify Closed Tickets
- **Rejection**: System will NOT allow editing closed tickets
- **Reason**: Immutability ensures audit trail integrity
- **Alternative**: Create refund/credit ticket instead, or use reopen if business requires

### L3: Skip Payment Validation
- **Rejection**: System will NOT allow closing tickets without payment validation
- **Reason**: Prevents revenue loss and ensures data integrity

### L4: Manual Total Override
- **Rejection**: System will NOT allow manual override of calculated totals
- **Reason**: Totals must be calculated from line items for correctness
- **Alternative**: Use adjustment amount (audited) for corrections

### L5: Delete Financial Records
- **Rejection**: System will NOT allow hard deletion of tickets, payments, or cash sessions
- **Reason**: Audit trail requires all records
- **Alternative**: Use void/refund status instead

### L6: Void Tickets with Payments
- **Rejection**: System will NOT allow voiding tickets that have payments
- **Reason**: Payments must be refunded, not voided
- **Alternative**: Use refund workflow

### L7: Bypass Cash Session Requirements
- **Rejection**: System will NOT allow transactions without an open cash session (for cash payments)
- **Reason**: Ensures proper cash accountability
- **Alternative**: Force cash session open before any cash operations

### L8: String-Based Status
- **Rejection**: System will NOT use string-based status (like FloreantPOS)
- **Reason**: Type safety and validation
- **Alternative**: Use strongly-typed enums

### L9: Multiple Boolean Flags for State
- **Rejection**: System will NOT use multiple boolean flags (paid, voided, closed, refunded)
- **Reason**: State machine pattern is clearer and prevents invalid states
- **Alternative**: Use state machine with proper transitions

## Enforcement Strategy

1. **Domain Layer**: Invariants enforced in entity constructors, setters, and domain services
2. **Application Layer**: Use case validators check preconditions
3. **Infrastructure Layer**: Database constraints as last line of defense
4. **Tests**: Comprehensive unit tests for each invariant

## Testing Requirements

Every invariant must have:
- Unit test proving it cannot be violated
- Integration test showing enforcement in full stack
- Negative test showing proper exception thrown

