# Magidesk POS - Core Invariants

> **Note**: This is a summary. See [INVARIANTS_FULL.md](./INVARIANTS_FULL.md) for the complete invariant list covering full POS scope.

## Definition
Invariants are business rules that MUST NEVER be violated. They are enforced at the Domain layer and cannot be bypassed.

## Financial Invariants

### F1: Money Precision
- **Rule**: All monetary amounts must be rounded to 2 decimal places
- **Enforcement**: Money value object
- **Violation**: Cannot create Money with more than 2 decimal places

### F2: Non-Negative Amounts
- **Rule**: Payment amounts, ticket totals, and cash balances cannot be negative
- **Enforcement**: Domain entity constructors and setters
- **Violation**: Throws `BusinessRuleViolationException`

### F3: Payment-Total Balance
- **Rule**: Sum of all payments on a ticket must equal the ticket total before closing
- **Enforcement**: `TicketDomainService.CanCloseTicket()`
- **Violation**: Cannot close ticket, throws `InvalidOperationException`

### F4: Immutable Finalized Records
- **Rule**: Closed tickets, completed payments, and closed cash sessions cannot be modified
- **Enforcement**: Entity state checks before mutations
- **Violation**: Throws `InvalidOperationException`

### F5: Cash Change Calculation
- **Rule**: For cash payments, ChangeAmount = TenderedAmount - Amount, and TenderedAmount >= Amount
- **Enforcement**: `PaymentDomainService.CalculateChange()`
- **Violation**: Cannot process cash payment with insufficient tender

## Ticket Invariants

### T1: Non-Empty Ticket
- **Rule**: Ticket must have at least one OrderLine before opening
- **Enforcement**: `TicketDomainService.CanOpenTicket()`
- **Violation**: Cannot open ticket, throws `BusinessRuleViolationException`

### T2: No Modifications to Closed Tickets
- **Rule**: Cannot add/remove/modify OrderLines on Closed or Voided tickets
- **Enforcement**: OrderLine operations check ticket status
- **Violation**: Throws `InvalidOperationException`

### T3: Void Before Payment
- **Rule**: Ticket can only be voided if it has no completed payments
- **Enforcement**: `TicketDomainService.CanVoidTicket()`
- **Violation**: Cannot void ticket, throws `InvalidOperationException`

### T4: Total Calculation Consistency
- **Rule**: Ticket.TotalAmount = Subtotal + TaxAmount - DiscountAmount
- **Enforcement**: `TicketDomainService.CalculateTotals()`
- **Violation**: Throws `BusinessRuleViolationException` if calculation fails

### T5: OrderLine Quantity
- **Rule**: OrderLine.Quantity must be > 0
- **Enforcement**: OrderLine constructor
- **Violation**: Cannot create OrderLine, throws `BusinessRuleViolationException`

### T6: OrderLine Discount
- **Rule**: OrderLine.DiscountAmount <= (Quantity * UnitPrice)
- **Enforcement**: OrderLine setter
- **Violation**: Throws `BusinessRuleViolationException`

## Payment Invariants

### P1: Positive Payment Amount
- **Rule**: Payment.Amount must be > 0
- **Enforcement**: Payment constructor
- **Violation**: Cannot create payment, throws `BusinessRuleViolationException`

### P2: No Refund of Unpaid Ticket
- **Rule**: Cannot refund payment if ticket is not Closed
- **Enforcement**: `PaymentDomainService.CanRefundPayment()`
- **Violation**: Cannot refund, throws `InvalidOperationException`

### P3: Immutable Completed Payments
- **Rule**: Payment with Status = Completed cannot be modified
- **Enforcement**: Payment mutation methods
- **Violation**: Throws `InvalidOperationException`

### P4: Cash Tender Sufficiency
- **Rule**: For cash payments, TenderedAmount >= Amount
- **Enforcement**: Payment processing logic
- **Violation**: Cannot process payment, throws `BusinessRuleViolationException`

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
- **Rule**: ExpectedCash = OpeningBalance + Sum(CashPayments) - Sum(CashRefunds)
- **Enforcement**: `CashSessionDomainService.CalculateExpectedCash()`
- **Violation**: Throws `BusinessRuleViolationException` if calculation fails

### C4: Immutable Closed Sessions
- **Rule**: Closed cash session cannot be modified
- **Enforcement**: CashSession mutation methods
- **Violation**: Throws `InvalidOperationException`

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

## Legacy Behaviors to REJECT

### L1: Allow Negative Balances
- **Rejection**: System will NOT allow negative cash balances or negative payment amounts
- **Reason**: Financial correctness and auditability

### L2: Modify Closed Tickets
- **Rejection**: System will NOT allow editing closed tickets
- **Reason**: Immutability ensures audit trail integrity
- **Alternative**: Create refund/credit ticket instead

### L3: Skip Payment Validation
- **Rejection**: System will NOT allow closing tickets without payment validation
- **Reason**: Prevents revenue loss and ensures data integrity

### L4: Manual Total Override
- **Rejection**: System will NOT allow manual override of calculated totals
- **Reason**: Totals must be calculated from line items for correctness
- **Alternative**: Use discounts or adjustments (audited)

### L5: Delete Financial Records
- **Rejection**: System will NOT allow hard deletion of tickets, payments, or cash sessions
- **Reason**: Audit trail requires all records
- **Alternative**: Use void/refund status instead

### L6: Offline Payment Processing Without Validation
- **Rejection**: System will NOT process card payments offline without proper validation
- **Reason**: Prevents fraud and chargebacks
- **Alternative**: Queue for online processing, or require cash

### L7: Bypass Cash Session Requirements
- **Rejection**: System will NOT allow transactions without an open cash session
- **Reason**: Ensures proper cash accountability
- **Alternative**: Force cash session open before any operations

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

