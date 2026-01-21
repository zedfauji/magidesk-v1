# Magidesk POS - Domain Model (Full POS Scope)

## Overview
This domain model supports a **complete POS system** with all features: split payments, multiple payment types, tips, refunds, discounts, tax complexity, and more. Architecture supports all features from the start.

## Core Entities

### Ticket (Order)
**Purpose**: Represents a customer order/transaction

**Properties**:
- `Id`: Guid
- `TicketNumber`: Sequential number (int or string)
- `GlobalId`: String (for multi-terminal sync)
- `CreatedAt`: DateTime
- `OpenedAt`: DateTime? (when first item added)
- `ClosedAt`: DateTime? (when finalized)
- `ActiveDate`: DateTime (last activity)
- `DeliveryDate`: DateTime? (for delivery orders)
- `Status`: TicketStatus enum (see below)
- `KitchenStatus`: KitchenStatus enum (Waiting, Ready, NotSent, Driving, Void)
- `CreatedBy`: UserId
- `ClosedBy`: UserId?
- `VoidedBy`: UserId?
- `TerminalId`: Guid
- `ShiftId`: Guid
- `OrderTypeId`: Guid
- `CustomerId`: Guid?
- `AssignedDriverId`: Guid? (for delivery)
- `TableNumbers`: List<int> (can have multiple tables)
- `NumberOfGuests`: int (default 1)
- `SubtotalAmount`: Money
- `DiscountAmount`: Money
- `TaxAmount`: Money
- `ServiceChargeAmount`: Money
- `DeliveryChargeAmount`: Money
- `AdjustmentAmount`: Money
- `TotalAmount`: Money
- `PaidAmount`: Money (sum of all payments)
- `DueAmount`: Money (TotalAmount - PaidAmount)
- `AdvanceAmount`: Money (prepayments)
- `IsTaxExempt`: bool
- `IsBarTab`: bool
- `IsReOpened`: bool
- `DeliveryAddress`: string?
- `ExtraDeliveryInfo`: string?
- `CustomerWillPickup`: bool
- `OrderLines`: Collection<OrderLine>
- `Payments`: Collection<Payment>
- `Discounts`: Collection<TicketDiscount>
- `Gratuity`: Gratuity? (tips)
- `Version`: int (optimistic concurrency)
- `Properties`: Dictionary<string, string> (flexible metadata)

**TicketStatus Enum**:
- `Draft`: Being created, no items yet
- `Open`: Has items, can be modified
- `Paid`: All payments received (PaidAmount >= TotalAmount)
- `Closed`: Finalized and settled
- `Voided`: Cancelled before payment
- `Refunded`: Closed ticket that was refunded

**Key Behaviors**:
- `CalculateTotals()`: Recalculates all amounts
- `CanAddPayment(Payment)`: Validates if payment can be added
- `CanClose()`: Validates if ticket can be closed
- `CanVoid()`: Validates if ticket can be voided
- `CanRefund()`: Validates if ticket can be refunded
- `CanSplit()`: Validates if ticket can be split
- `GetRemainingDue()`: Returns amount still owed

**Invariants**:
- Cannot add items to Closed/Voided/Refunded ticket
- Cannot void ticket with payments (must refund instead)
- TotalAmount = Subtotal + Tax + ServiceCharge + DeliveryCharge + Adjustment - Discount + Gratuity
- Cannot close ticket with DueAmount > 0
- PaidAmount = Sum of all payment amounts
- DueAmount = TotalAmount - PaidAmount
- Cannot modify Closed/Refunded ticket

**Domain Events**:
- `TicketCreated`
- `TicketOpened`
- `OrderLineAdded`
- `OrderLineRemoved`
- `OrderLineModified`
- `DiscountApplied`
- `PaymentAdded`
- `TicketPaid`
- `TicketClosed`
- `TicketVoided`
- `TicketRefunded`
- `TicketReopened`

### OrderLine (TicketItem)
**Purpose**: Represents a single item in an order

**Properties**:
- `Id`: Guid
- `TicketId`: Guid
- `MenuItemId`: Guid
- `MenuItemName`: string (snapshot)
- `CategoryName`: string? (snapshot)
- `GroupName`: string? (snapshot)
- `Quantity`: decimal (for fractional units like weight)
- `ItemCount`: int (for discrete items)
- `ItemUnitName`: string? (e.g., "lb", "kg")
- `IsFractionalUnit`: bool
- `UnitPrice`: Money (snapshot at time of order)
- `SubtotalAmount`: Money (Quantity * UnitPrice + modifiers)
- `SubtotalAmountWithoutModifiers`: Money
- `DiscountAmount`: Money
- `TaxRate`: decimal (snapshot)
- `TaxAmount`: Money
- `TaxAmountWithoutModifiers`: Money
- `TotalAmount`: Money
- `TotalAmountWithoutModifiers`: Money
- `IsBeverage`: bool
- `ShouldPrintToKitchen`: bool
- `PrintedToKitchen`: bool
- `SeatNumber`: int? (for seat-based ordering)
- `TreatAsSeat`: bool
- `Modifiers`: Collection<OrderLineModifier>
- `AddOns`: Collection<OrderLineModifier> (extra modifiers)
- `Discounts`: Collection<OrderLineDiscount>
- `CookingInstructions`: Collection<CookingInstruction>
- `SizeModifier`: OrderLineModifier? (for pizza sizes, etc.)
- `PrinterGroupId`: Guid?
- `CreatedAt`: DateTime

**Key Behaviors**:
- `CalculatePrice()`: Recalculates line totals
- `CanMerge(OrderLine)`: Checks if items can be merged
- `Merge(OrderLine)`: Merges identical items
- `CanAddModifier()`: Validates modifier can be added
- `CanAddDiscount()`: Validates discount can be added

**Invariants**:
- Quantity > 0 OR ItemCount > 0
- UnitPrice >= 0
- DiscountAmount <= SubtotalAmount
- TotalAmount >= 0
- Cannot modify if parent ticket is Closed/Voided/Refunded
- Modifiers and add-ons must have valid MenuModifierId

### Payment (PosTransaction)
**Purpose**: Represents a payment transaction (can be multiple per ticket)

**Properties**:
- `Id`: Guid
- `GlobalId`: string
- `TicketId`: Guid
- `TransactionType`: TransactionType enum (Credit, Debit)
- `PaymentType`: PaymentType enum (see below)
- `Amount`: Money (payment amount, must be > 0)
- `TipsAmount`: Money (tips/gratuity on this payment)
- `TipsExceedAmount`: Money (tips exceeding payment amount)
- `TenderAmount`: Money (amount tendered, for cash)
- `ChangeAmount`: Money (calculated: TenderAmount - Amount, for cash)
- `TransactionTime`: DateTime
- `ProcessedBy`: UserId
- `TerminalId`: Guid
- `IsCaptured`: bool (for card pre-auth/capture)
- `IsVoided`: bool
- `IsAuthorizable`: bool (for card transactions)
- `CashSessionId`: Guid? (if cash payment)
- `Note`: string?

**Payment-Specific Fields**:
- **Cash**: TenderAmount, ChangeAmount
- **Credit/Debit Card**: CardHolderName, CardNumber (masked), CardAuthCode, CardType, CardTransactionId, CardMerchantGateway, CardReader, CardAID, CardARQC, CardExtData
- **Gift Certificate**: GiftCertNumber, GiftCertFaceValue, GiftCertPaidAmount, GiftCertCashBackAmount
- **Custom Payment**: CustomPaymentName, CustomPaymentRef, CustomPaymentFieldName

**PaymentType Enum**:
- `Cash`
- `CreditCard`, `CreditVisa`, `CreditMasterCard`, `CreditAmex`, `CreditDiscover`
- `DebitCard`, `DebitVisa`, `DebitMasterCard`
- `GiftCertificate`
- `CustomPayment`

**TransactionType Enum**:
- `Credit`: Money coming in (payments)
- `Debit`: Money going out (refunds, payouts, cash drops)

**Key Behaviors**:
- `CalculateChange()`: For cash payments
- `CanVoid()`: Validates if payment can be voided
- `CanRefund()`: Validates if payment can be refunded
- `CanCapture()`: For authorized card transactions

**Invariants**:
- Amount > 0
- For cash: TenderAmount >= Amount
- For cash: ChangeAmount = TenderAmount - Amount
- Cannot void if already voided
- Cannot refund if not captured/completed
- TipsAmount >= 0
- Gift cert cash back <= (FaceValue - PaidAmount)

**Domain Events**:
- `PaymentProcessed`
- `PaymentAuthorized` (for cards)
- `PaymentCaptured` (for cards)
- `PaymentVoided`
- `PaymentRefunded`

### Gratuity (Tips)
**Purpose**: Represents tips/gratuity on a ticket

**Properties**:
- `Id`: Guid
- `TicketId`: Guid
- `Amount`: Money
- `Paid`: bool
- `Refunded`: bool
- `TerminalId`: Guid
- `OwnerId`: UserId (who receives tips)
- `CreatedAt`: DateTime

**Invariants**:
- Amount >= 0
- Can be paid separately from ticket
- If ticket refunded, gratuity may be refunded

### Discount
**Purpose**: Discount definition (reference data)

**Properties**:
- `Id`: Guid
- `Name`: string
- `Type`: DiscountType enum (Amount, Percentage, RePrice, AltPrice)
- `Value`: decimal
- `MinimumBuy`: Money? (minimum purchase required)
- `MinimumQuantity`: int? (minimum item quantity)
- `QualificationType`: QualificationType enum (Item, Order)
- `ApplicationType`: ApplicationType enum (see below)
- `AutoApply`: bool
- `IsActive`: bool

**DiscountType Enum**:
- `Amount`: Fixed dollar amount
- `Percentage`: Percentage discount
- `RePrice`: Reprice item
- `AltPrice`: Alternative price

**ApplicationType Enum**:
- `FreeAmount`: Free amount off
- `FixedPerCategory`: Fixed amount per category
- `FixedPerItem`: Fixed amount per item
- `FixedPerOrder`: Fixed amount per order
- `PercentagePerCategory`: Percentage per category
- `PercentagePerItem`: Percentage per item
- `PercentagePerOrder`: Percentage per order

**Key Behaviors**:
- `CalculateDiscount(ITicketItem)`: Calculates discount amount
- `IsEligible(Ticket)`: Checks if discount applies to ticket
- `IsEligible(OrderLine)`: Checks if discount applies to item

### TicketDiscount
**Purpose**: Discount applied to entire ticket (snapshot)

**Properties**:
- `Id`: Guid
- `TicketId`: Guid
- `DiscountId`: Guid (reference to Discount)
- `Name`: string (snapshot)
- `Type`: DiscountType (snapshot)
- `Value`: decimal (snapshot)
- `MinimumAmount`: Money? (snapshot)
- `Amount`: Money (calculated discount amount)
- `AppliedAt`: DateTime

**Invariants**:
- Immutable once created (snapshot)
- Only one ticket discount applies (max discount selected)

### OrderLineDiscount
**Purpose**: Discount applied to order line (snapshot)

**Properties**:
- `Id`: Guid
- `OrderLineId`: Guid
- `DiscountId`: Guid (reference to Discount)
- `Name`: string (snapshot)
- `Type`: DiscountType (snapshot)
- `Value`: decimal (snapshot)
- `MinimumQuantity`: int? (snapshot)
- `Amount`: Money (calculated discount amount)
- `AutoApply`: bool (snapshot)
- `AppliedAt`: DateTime

**Invariants**:
- Immutable once created (snapshot)
- Only one discount per line applies (max discount selected)

### OrderLineModifier
**Purpose**: Modifier or add-on for an order line

**Properties**:
- `Id`: Guid
- `OrderLineId`: Guid
- `ModifierId`: Guid (reference to MenuModifier)
- `MenuItemModifierGroupId`: Guid?
- `Name`: string (snapshot)
- `ModifierType`: ModifierType enum (Normal, Extra, InfoOnly)
- `ItemCount`: int
- `UnitPrice`: Money
- `TaxRate`: decimal
- `TaxAmount`: Money
- `SubtotalAmount`: Money
- `TotalAmount`: Money
- `ShouldPrintToKitchen`: bool
- `PrintedToKitchen`: bool
- `MultiplierName`: string? (for pizza modifiers)
- `SectionName`: string? (for pizza sections)
- `IsSectionWisePrice`: bool
- `CreatedAt`: DateTime

**ModifierType Enum**:
- `Normal`: Standard modifier
- `Extra`: Add-on (extra charge)
- `InfoOnly`: Informational only (no charge)

**Invariants**:
- ItemCount > 0
- UnitPrice >= 0
- TotalAmount >= 0

### CashSession
**Purpose**: Represents a cash drawer session (explicit open/close)

**Properties**:
- `Id`: Guid
- `UserId`: Guid (who opened)
- `TerminalId`: Guid
- `ShiftId`: Guid
- `OpenedAt`: DateTime
- `ClosedAt`: DateTime?
- `ClosedBy`: UserId?
- `OpeningBalance`: Money (cash in drawer at open)
- `ExpectedCash`: Money (calculated)
- `ActualCash`: Money? (counted at close)
- `Difference`: Money? (ActualCash - ExpectedCash)
- `Status`: CashSessionStatus (Open, Closed)
- `Payments`: Collection<Payment> (cash payments in session)
- `Payouts`: Collection<Payout> (money taken out)
- `CashDrops`: Collection<CashDrop> (money deposited)
- `DrawerBleeds`: Collection<DrawerBleed> (money removed)
- `Version`: int

**Key Behaviors**:
- `CalculateExpectedCash()`: OpeningBalance + CashReceipts - CashRefunds - Payouts - CashDrops - Bleeds
- `CanClose()`: Validates if session can be closed
- `Close(actualCash)`: Closes session with actual count

**Invariants**:
- OpeningBalance >= 0
- Cannot close with open tickets for user
- ExpectedCash = OpeningBalance + Sum(CashPayments) - Sum(CashRefunds) - Sum(Payouts) - Sum(CashDrops) - Sum(Bleeds)
- Cannot modify once Closed

**Domain Events**:
- `CashSessionOpened`
- `CashSessionClosed`

### RefundTransaction
**Purpose**: Represents a refund transaction

**Properties**:
- Inherits from Payment
- `RefundedPaymentId`: Guid? (original payment being refunded)
- `RefundReason`: string?

**Invariants**:
- Can only refund paid/closed tickets
- Refund amount <= original payment amount
- Creates debit transaction

### Shift
**Purpose**: Represents a work shift

**Properties**:
- `Id`: Guid
- `Name`: string
- `StartTime`: TimeSpan
- `EndTime`: TimeSpan
- `IsActive`: bool

### OrderType
**Purpose**: Defines order types (Dine In, Take Out, etc.)

**Properties**:
- `Id`: Guid
- `Name`: string
- `CloseOnPaid`: bool
- `AllowSeatBasedOrder`: bool
- `AllowToAddTipsLater`: bool
- `IsBarTab`: bool
- `Properties`: Dictionary<string, string>

### Table (ShopTable)
**Purpose**: Represents a restaurant table

**Properties**:
- `Id`: Guid (table number)
- `FloorId`: Guid?
- `Capacity`: int
- `X`: int (position)
- `Y`: int (position)
- `Status`: TableStatus enum
- `CurrentTicketId`: Guid?

**TableStatus Enum**:
- `Available`
- `Seat` (occupied)
- `Booked`
- `Dirty`
- `Disable`

### AuditEvent
**Purpose**: Immutable audit record

**Properties**:
- `Id`: Guid
- `EventType`: AuditEventType enum
- `EntityType`: string
- `EntityId`: Guid
- `UserId`: Guid
- `Timestamp`: DateTime
- `BeforeState`: string? (JSON)
- `AfterState`: string (JSON)
- `Description`: string
- `CorrelationId`: Guid?

**Invariants**:
- Immutable once created
- Complete state snapshots

## Value Objects

### Money
**Properties**:
- `Amount`: decimal (2 decimal places)
- `Currency`: string (ISO 4217)

**Operations**: Add, Subtract, Multiply, Divide, Compare, Round

### UserId
**Properties**:
- `Value`: Guid

## Domain Services

### TicketDomainService
- `CalculateTotals(Ticket)`: Recalculates all ticket amounts
- `CanAddPayment(Ticket, Payment)`: Validates payment
- `CanCloseTicket(Ticket)`: Validates closing
- `CanVoidTicket(Ticket)`: Validates voiding
- `CanRefundTicket(Ticket, Money)`: Validates refund
- `CanSplitTicket(Ticket)`: Validates splitting
- `GetRemainingDue(Ticket)`: Calculates amount owed

### PaymentDomainService
- `CalculateChange(Payment)`: For cash payments
- `CanVoidPayment(Payment)`: Validates voiding
- `CanRefundPayment(Payment, Money)`: Validates refund
- `CanCapturePayment(Payment)`: For card transactions

### DiscountDomainService
- `GetMaxDiscount(List<Discount>, Money)`: Selects best discount
- `CalculateDiscountAmount(Discount, ITicketItem)`: Calculates discount
- `IsEligible(Discount, Ticket)`: Checks eligibility

### CashSessionDomainService
- `CalculateExpectedCash(CashSession)`: Calculates expected amount
- `CanCloseSession(CashSession)`: Validates closing

## Relationships

```
Ticket (1) ──→ (*) OrderLine
Ticket (1) ──→ (*) Payment
Ticket (1) ──→ (*) TicketDiscount
Ticket (1) ──→ (0..1) Gratuity
OrderLine (1) ──→ (*) OrderLineModifier
OrderLine (1) ──→ (*) OrderLineDiscount
Payment (*) ──→ (1) CashSession (if cash)
CashSession (1) ──→ (*) Payment (cash only)
Shift (1) ──→ (*) Ticket
OrderType (1) ──→ (*) Ticket
Table (*) ──→ (0..1) Ticket
All Entities ──→ (*) AuditEvent
```

## Design Principles

1. **Rich Domain Model**: Entities contain behavior
2. **Aggregate Roots**: Ticket, CashSession, Shift
3. **Value Objects**: Money, UserId
4. **Domain Events**: For audit and eventual consistency
5. **Invariants**: Enforced at domain level
6. **Immutability**: Financial records immutable once finalized
7. **Snapshots**: Discounts and prices snapshotted at application time

