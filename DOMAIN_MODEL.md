# Magidesk POS - Domain Model (Full POS Scope)

> **Note**: This is the full domain model supporting complete POS functionality. See [DOMAIN_MODEL_FULL.md](./DOMAIN_MODEL_FULL.md) for the comprehensive version with all details.

## Core Entities

### Ticket (Order)
**Purpose**: Represents a customer order/transaction

**Properties**:
- `Id`: Unique identifier (Guid)
- `TicketNumber`: Human-readable sequential number
- `Status`: TicketStatus enum (Draft, Open, Closed, Voided, Refunded)
- `CreatedAt`: DateTime (immutable)
- `OpenedAt`: DateTime? (when moved from Draft to Open)
- `ClosedAt`: DateTime? (when finalized)
- `VoidedAt`: DateTime? (if voided)
- `CreatedBy`: UserId
- `TableNumber`: int? (for restaurant POS)
- `CustomerId`: Guid? (optional customer reference)
- `Subtotal`: Money (calculated, immutable)
- `TaxAmount`: Money (calculated, immutable)
- `DiscountAmount`: Money (calculated, immutable)
- `TotalAmount`: Money (calculated, immutable)
- `OrderLines`: Collection<OrderLine>
- `Payments`: Collection<Payment>
- `Version`: int (for optimistic concurrency)

**Invariants**:
- Cannot add items to Closed/Voided ticket
- Cannot void ticket with payments
- TotalAmount = Subtotal + TaxAmount - DiscountAmount
- Cannot close ticket with zero items
- All payments must sum to TotalAmount before closing

**Domain Events**:
- `TicketCreated`
- `TicketOpened`
- `OrderLineAdded`
- `OrderLineRemoved`
- `OrderLineModified`
- `TicketClosed`
- `TicketVoided`

### OrderLine
**Purpose**: Represents a single item in an order

**Properties**:
- `Id`: Guid
- `TicketId`: Guid (parent ticket)
- `MenuItemId`: Guid
- `MenuItemName`: string (snapshot at time of order)
- `Quantity`: decimal (must be > 0)
- `UnitPrice`: Money (snapshot at time of order)
- `DiscountAmount`: Money
- `TaxRate`: decimal (snapshot at time of order)
- `LineTotal`: Money (calculated: (Quantity * UnitPrice - DiscountAmount) * (1 + TaxRate))
- `Notes`: string? (special instructions)
- `CreatedAt`: DateTime

**Invariants**:
- Quantity > 0
- UnitPrice >= 0
- DiscountAmount <= (Quantity * UnitPrice)
- LineTotal >= 0
- Cannot modify if parent ticket is Closed/Voided

### Payment
**Purpose**: Represents a payment transaction

**Properties**:
- `Id`: Guid
- `TicketId`: Guid
- `PaymentType`: PaymentType enum (Cash, Card, GiftCard, etc.)
- `Amount`: Money (must be > 0)
- `TenderedAmount`: Money? (cash received, for change calculation)
- `ChangeAmount`: Money (calculated: TenderedAmount - Amount, if applicable)
- `ProcessedAt`: DateTime
- `ProcessedBy`: UserId
- `TransactionReference`: string? (external transaction ID)
- `Status`: PaymentStatus (Pending, Completed, Failed, Refunded)
- `CashSessionId`: Guid? (if cash payment, links to cash session)

**Invariants**:
- Amount > 0
- Cannot refund payment if ticket is not closed
- TenderedAmount >= Amount (for cash)
- ChangeAmount = TenderedAmount - Amount (for cash)
- Cannot modify once Status = Completed

**Domain Events**:
- `PaymentProcessed`
- `PaymentRefunded`

### CashSession
**Purpose**: Represents a cash drawer session (shift)

**Properties**:
- `Id`: Guid
- `UserId`: Guid (who opened the session)
- `OpenedAt`: DateTime
- `ClosedAt`: DateTime?
- `OpeningBalance`: Money (cash in drawer at open)
- `ExpectedCash`: Money (calculated: OpeningBalance + CashSales - CashRefunds)
- `ActualCash`: Money? (counted at close)
- `Difference`: Money? (ActualCash - ExpectedCash)
- `Status`: CashSessionStatus (Open, Closed)
- `Payments`: Collection<Payment> (cash payments in this session)

**Invariants**:
- Cannot close session with open tickets
- OpeningBalance >= 0
- ExpectedCash = OpeningBalance + Sum(CashPayments) - Sum(CashRefunds)
- Cannot modify once Closed

**Domain Events**:
- `CashSessionOpened`
- `CashSessionClosed`

### AuditEvent
**Purpose**: Immutable record of all financial mutations

**Properties**:
- `Id`: Guid
- `EventType`: AuditEventType enum
- `EntityType`: string (Ticket, Payment, CashSession, etc.)
- `EntityId`: Guid
- `UserId`: Guid
- `Timestamp`: DateTime
- `BeforeState`: string? (JSON snapshot of entity before change)
- `AfterState`: string (JSON snapshot of entity after change)
- `Description`: string (human-readable description)
- `CorrelationId`: Guid? (groups related events)

**Invariants**:
- Immutable once created
- Timestamp is server/client time (synchronized)
- BeforeState and AfterState are complete snapshots

## Value Objects

### Money
**Purpose**: Represents monetary amounts with currency

**Properties**:
- `Amount`: decimal
- `Currency`: string (ISO 4217, e.g., "USD")

**Invariants**:
- Amount precision: 2 decimal places
- Currency must be valid ISO 4217 code
- Immutable

**Operations**:
- Addition, subtraction, multiplication, division
- Comparison operators
- Rounding to 2 decimal places

### UserId
**Purpose**: Strongly-typed user identifier

**Properties**:
- `Value`: Guid

**Invariants**:
- Cannot be empty Guid

## Domain Services

### TicketDomainService
- `CalculateTotals(Ticket)`: Recalculates subtotal, tax, discount, total
- `CanAddPayment(Ticket, Payment)`: Validates if payment can be added
- `CanCloseTicket(Ticket)`: Validates if ticket can be closed
- `CanVoidTicket(Ticket)`: Validates if ticket can be voided

### PaymentDomainService
- `CalculateChange(Payment)`: Calculates change for cash payment
- `CanRefundPayment(Payment, Ticket)`: Validates refund eligibility

### CashSessionDomainService
- `CalculateExpectedCash(CashSession)`: Calculates expected cash from payments
- `CanCloseSession(CashSession)`: Validates if session can be closed

## Enumerations

### TicketStatus
- `Draft`: Being created, not yet finalized
- `Open`: Active order, can be modified
- `Closed`: Finalized, paid, cannot modify
- `Voided`: Cancelled before payment
- `Refunded`: Closed ticket that was refunded

### PaymentType
- `Cash`
- `CreditCard`
- `DebitCard`
- `GiftCard`
- `StoreCredit`
- `Other`

### PaymentStatus
- `Pending`: Initiated but not confirmed
- `Completed`: Successfully processed
- `Failed`: Processing failed
- `Refunded`: Refunded to customer

### CashSessionStatus
- `Open`: Active session
- `Closed`: Session ended

### AuditEventType
- `Created`
- `Modified`
- `Deleted`
- `StatusChanged`
- `PaymentProcessed`
- `RefundProcessed`
- `Voided`

## Domain Exceptions

### DomainException (base)
- `BusinessRuleViolationException`: Invariant violation
- `InvalidOperationException`: Operation not allowed in current state
- `ConcurrencyException`: Optimistic concurrency conflict

## Relationships

```
Ticket (1) ──→ (*) OrderLine
Ticket (1) ──→ (*) Payment
Payment (*) ──→ (1) CashSession (if cash)
CashSession (1) ──→ (*) Payment (cash payments only)
All Entities ──→ (*) AuditEvent
```

## Design Principles Applied

1. **Rich Domain Model**: Entities contain behavior, not just data
2. **Aggregate Roots**: Ticket and CashSession are aggregate roots
3. **Value Objects**: Money, UserId prevent primitive obsession
4. **Domain Events**: Enable audit trail and eventual consistency
5. **Invariants**: Enforced at domain level, cannot be bypassed
6. **Immutability**: Financial records immutable once finalized

