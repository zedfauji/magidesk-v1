# Design Document

## Overview

This document describes the technical design for Category C: Billing, Payments & Pricing features. The system builds upon the existing payment infrastructure to provide comprehensive billing capabilities including deferred payments, split payments, discounts, promotional pricing, group billing, and audit trails.

The design follows a layered architecture with clear separation between domain logic, application services, infrastructure, and presentation layers. All payment calculations and state transitions are designed to be testable through property-based testing to ensure correctness.

## Architecture

### System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  (ViewModels, Views, Dialogs, Pages)                        │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                   Application Layer                          │
│  (Commands, Queries, Handlers, DTOs)                        │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                     Domain Layer                             │
│  (Entities, Value Objects, Domain Events, Business Rules)   │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                 Infrastructure Layer                         │
│  (Repositories, Database, External Services)                │
└─────────────────────────────────────────────────────────────┘
```

### Key Design Principles

1. **Domain-Driven Design**: Business logic encapsulated in domain entities
2. **CQRS Pattern**: Separate commands (writes) from queries (reads)
3. **Event Sourcing**: Domain events for audit trail
4. **Immutability**: Value objects are immutable
5. **Testability**: All calculations testable via property-based tests

## Components and Interfaces

### Domain Layer Components

#### Entities

**Ticket** (Enhanced)
```csharp
public class Ticket : AggregateRoot
{
    public TicketId Id { get; private set; }
    public int TicketNumber { get; private set; }
    public TicketStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money PaidAmount { get; private set; }
    public Money DueAmount { get; private set; }
    
    // Hold Ticket Support
    public DateTime? HeldAt { get; private set; }
    public string? HoldReason { get; private set; }
    public UserId? HeldBy { get; private set; }
    
    // Collections
    public IReadOnlyList<OrderLine> OrderLines { get; private set; }
    public IReadOnlyList<Payment> Payments { get; private set; }
    public IReadOnlyList<TicketDiscount> Discounts { get; private set; }
    
    // Methods
    public void Hold(string reason, UserId userId);
    public void Release();
    public void AddPayment(Payment payment);
    public void ApplyDiscount(Discount discount, UserId appliedBy, UserId? authorizedBy = null);
    public void RemoveDiscount(DiscountId discountId);
    public void Void(string reason, UserId voidedBy);
    public void Refund(Money amount, string reason, UserId refundedBy);
}
```

**Payment** (Enhanced)
```csharp
public class Payment : Entity
{
    public PaymentId Id { get; private set; }
    public TicketId TicketId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public Money Amount { get; private set; }
    public DateTime ProcessedAt { get; private set; }
    public UserId ProcessedBy { get; private set; }
    public PaymentStatus Status { get; private set; }
    
    // Split Payment Support
    public Guid? SplitGroupId { get; private set; }
    public int? SplitSequence { get; private set; }
    
    // Refund Support
    public Money RefundedAmount { get; private set; }
    public bool IsRefunded { get; private set; }
}
```

**Discount** (New)
```csharp
public class Discount : Entity
{
    public DiscountId Id { get; private set; }
    public string Name { get; private set; }
    public DiscountType Type { get; private set; } // Percentage, FixedAmount
    public decimal Value { get; private set; }
    public DiscountApplicationType ApplicationType { get; private set; } // Ticket, Item
    public bool RequiresAuthorization { get; private set; }
    public bool IsActive { get; private set; }
    
    public Money CalculateDiscount(Money amount);
}
```

**PromotionSchedule** (New)
```csharp
public class PromotionSchedule : Entity
{
    public PromotionScheduleId Id { get; private set; }
    public string Name { get; private set; }
    public DiscountId DiscountId { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public RecurrencePattern Recurrence { get; private set; }
    public IReadOnlyList<MenuItemId> QualifyingItems { get; private set; }
    public bool IsActive { get; private set; }
    
    public bool IsActiveAt(DateTime dateTime);
    public bool OverlapsWith(PromotionSchedule other);
}
```

**GroupSettlement** (New)
```csharp
public class GroupSettlement : Entity
{
    public GroupSettlementId Id { get; private set; }
    public IReadOnlyList<TicketId> TicketIds { get; private set; }
    public GroupSettlementStrategy Strategy { get; private set; }
    public PaymentId MasterPaymentId { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId CreatedBy { get; private set; }
    
    public void AddTicket(TicketId ticketId);
    public void RemoveTicket(TicketId ticketId);
    public Dictionary<TicketId, Money> CalculateDistribution(IReadOnlyList<Ticket> tickets);
}
```

**PriceOverride** (New)
```csharp
public class PriceOverride : Entity
{
    public PriceOverrideId Id { get; private set; }
    public OrderLineId OrderLineId { get; private set; }
    public Money OriginalPrice { get; private set; }
    public Money OverridePrice { get; private set; }
    public Money Variance { get; private set; }
    public string Reason { get; private set; }
    public UserId AppliedBy { get; private set; }
    public UserId AuthorizedBy { get; private set; }
    public DateTime AppliedAt { get; private set; }
}
```

#### Value Objects

**Money**
```csharp
public record Money(decimal Amount, string Currency)
{
    public static Money operator +(Money a, Money b);
    public static Money operator -(Money a, Money b);
    public static Money operator *(Money a, decimal multiplier);
    public Money ApplyPercentage(decimal percentage);
    public bool IsZero();
    public bool IsNegative();
}
```

#### Enumerations

```csharp
public enum TicketStatus
{
    Open = 0,
    Held = 2,
    Paid = 3,
    Voided = 4,
    Refunded = 5
}

public enum DiscountType
{
    Percentage,
    FixedAmount
}

public enum DiscountApplicationType
{
    Ticket,
    Item
}

public enum GroupSettlementStrategy
{
    EqualSplit,
    ByItem,
    Custom
}

public enum RecurrencePattern
{
    Daily,
    Weekly,
    Custom
}
```

### Application Layer Components

#### Commands

```csharp
// Hold Ticket
public record HoldTicketCommand(TicketId TicketId, string Reason, UserId UserId);
public record ReleaseHeldTicketCommand(TicketId TicketId, UserId UserId);

// Split Payment
public record ProcessSplitPaymentCommand(
    TicketId TicketId,
    IReadOnlyList<PaymentEntry> Payments,
    UserId ProcessedBy);

// Discount
public record ApplyDiscountCommand(
    TicketId TicketId,
    DiscountId DiscountId,
    UserId AppliedBy,
    UserId? AuthorizedBy = null);

public record RemoveDiscountCommand(
    TicketId TicketId,
    DiscountId DiscountId,
    UserId RemovedBy);

// Price Override
public record OverrideLinePriceCommand(
    OrderLineId OrderLineId,
    Money NewPrice,
    string Reason,
    UserId AppliedBy,
    UserId AuthorizedBy);

// Void/Refund
public record VoidTicketCommand(
    TicketId TicketId,
    string Reason,
    UserId VoidedBy,
    UserId AuthorizedBy);

public record RefundTicketCommand(
    TicketId TicketId,
    Money Amount,
    string Reason,
    UserId RefundedBy,
    UserId AuthorizedBy,
    bool IsPartial);

// Promotion
public record CreatePromotionScheduleCommand(
    string Name,
    DiscountId DiscountId,
    TimeSpan StartTime,
    TimeSpan EndTime,
    RecurrencePattern Recurrence,
    IReadOnlyList<MenuItemId> QualifyingItems);

public record DisablePromotionCommand(
    TicketId TicketId,
    PromotionScheduleId PromotionId,
    string Reason,
    UserId DisabledBy,
    UserId AuthorizedBy);

// Group Settlement
public record CreateGroupSettlementCommand(
    IReadOnlyList<TicketId> TicketIds,
    GroupSettlementStrategy Strategy,
    UserId CreatedBy);
```

#### Queries

```csharp
public record GetHeldTicketsQuery();
public record GetPriceOverridesQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    UserId? StaffMember = null,
    UserId? Manager = null,
    decimal? MinVariance = null);
public record GetActivePromotionsQuery(DateTime ForDateTime);
public record GetAuditEventsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? EntityType = null,
    AuditEventType? EventType = null,
    UserId? UserId = null);
```

#### DTOs

```csharp
public record HeldTicketDto(
    TicketId Id,
    int TicketNumber,
    DateTime HeldAt,
    string HoldReason,
    string HeldByUserName,
    Money TotalAmount,
    string? CustomerName,
    int? TableNumber);

public record PriceOverrideDto(
    PriceOverrideId Id,
    OrderLineId OrderLineId,
    string ItemName,
    Money OriginalPrice,
    Money OverridePrice,
    Money Variance,
    decimal VariancePercentage,
    string Reason,
    string AppliedByUserName,
    string AuthorizedByUserName,
    DateTime AppliedAt);

public record SplitPaymentEntry(
    PaymentMethod Method,
    Money Amount);
```

### Infrastructure Layer Components

#### Repositories

```csharp
public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(TicketId id);
    Task<IReadOnlyList<Ticket>> GetHeldTicketsAsync();
    Task SaveAsync(Ticket ticket);
}

public interface IDiscountRepository
{
    Task<Discount?> GetByIdAsync(DiscountId id);
    Task<IReadOnlyList<Discount>> GetActiveDiscountsAsync();
}

public interface IPromotionScheduleRepository
{
    Task<IReadOnlyList<PromotionSchedule>> GetActivePromotionsAsync(DateTime dateTime);
    Task<bool> HasOverlappingPromotionsAsync(PromotionSchedule schedule);
}

public interface IPriceOverrideRepository
{
    Task<IReadOnlyList<PriceOverride>> GetOverridesAsync(
        DateTime? startDate,
        DateTime? endDate,
        UserId? staffMember,
        UserId? manager,
        decimal? minVariance);
}

public interface IGroupSettlementRepository
{
    Task<GroupSettlement?> GetByIdAsync(GroupSettlementId id);
    Task SaveAsync(GroupSettlement settlement);
}
```

### Presentation Layer Components

#### ViewModels

```csharp
// Hold Ticket
public partial class HoldTicketDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _holdReason;
    [ObservableProperty] private string _selectedReasonCode;
    [RelayCommand] private async Task HoldTicketAsync();
}

public partial class HeldTicketsViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<HeldTicketDto> _heldTickets;
    [RelayCommand] private async Task ReleaseTicketAsync(HeldTicketDto ticket);
    [RelayCommand] private async Task RefreshAsync();
}

// Split Payment
public partial class SplitPaymentViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PaymentEntry> _payments;
    [ObservableProperty] private Money _remainingAmount;
    [RelayCommand] private void AddPayment();
    [RelayCommand] private async Task ProcessSplitPaymentAsync();
}

// Discount
public partial class DiscountSelectionViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Discount> _availableDiscounts;
    [ObservableProperty] private Discount? _selectedDiscount;
    [RelayCommand] private async Task ApplyDiscountAsync();
}

// Price Override
public partial class PriceOverrideDialogViewModel : ObservableObject
{
    [ObservableProperty] private Money _originalPrice;
    [ObservableProperty] private Money _newPrice;
    [ObservableProperty] private string _reason;
    [RelayCommand] private async Task OverridePriceAsync();
}

// Refund
public partial class RefundWizardViewModel : ObservableObject
{
    [ObservableProperty] private RefundMode _selectedMode; // Full, Partial, Specific
    [ObservableProperty] private Money _refundAmount;
    [ObservableProperty] private string _refundReason;
    [RelayCommand] private async Task ProcessRefundAsync();
}

// Promotion Management
public partial class PromotionScheduleViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PromotionSchedule> _schedules;
    [RelayCommand] private async Task CreateScheduleAsync();
    [RelayCommand] private async Task EditScheduleAsync(PromotionSchedule schedule);
}

// Audit
public partial class PriceOverrideAuditViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PriceOverrideDto> _overrides;
    [ObservableProperty] private DateTime? _startDate;
    [ObservableProperty] private DateTime? _endDate;
    [RelayCommand] private async Task FilterAsync();
    [RelayCommand] private async Task ExportAsync();
}
```

## Data Models

### Database Schema Changes

#### Tickets Table (Enhanced)
```sql
ALTER TABLE "Tickets" ADD COLUMN "HeldAt" timestamp with time zone NULL;
ALTER TABLE "Tickets" ADD COLUMN "HoldReason" varchar(500) NULL;
ALTER TABLE "Tickets" ADD COLUMN "HeldBy" uuid NULL;

CREATE INDEX "IX_Tickets_HeldAt_Held" ON "Tickets" ("HeldAt") 
WHERE "Status" = 2;
```

#### Payments Table (Enhanced)
```sql
ALTER TABLE "Payments" ADD COLUMN "SplitGroupId" uuid NULL;
ALTER TABLE "Payments" ADD COLUMN "SplitSequence" int NULL;
ALTER TABLE "Payments" ADD COLUMN "RefundedAmount" decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE "Payments" ADD COLUMN "IsRefunded" boolean NOT NULL DEFAULT false;

CREATE INDEX "IX_Payments_SplitGroupId" ON "Payments" ("SplitGroupId")
WHERE "SplitGroupId" IS NOT NULL;
```

#### PromotionSchedules Table (New)
```sql
CREATE TABLE "PromotionSchedules" (
    "Id" uuid PRIMARY KEY,
    "Name" varchar(200) NOT NULL,
    "DiscountId" uuid NOT NULL,
    "StartTime" time NOT NULL,
    "EndTime" time NOT NULL,
    "Recurrence" int NOT NULL,
    "QualifyingItems" uuid[] NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    FOREIGN KEY ("DiscountId") REFERENCES "Discounts"("Id")
);

CREATE INDEX "IX_PromotionSchedules_IsActive" ON "PromotionSchedules" ("IsActive");
CREATE INDEX "IX_PromotionSchedules_StartTime_EndTime" ON "PromotionSchedules" ("StartTime", "EndTime");
```

#### PriceOverrides Table (New)
```sql
CREATE TABLE "PriceOverrides" (
    "Id" uuid PRIMARY KEY,
    "OrderLineId" uuid NOT NULL,
    "OriginalPrice" decimal(18,2) NOT NULL,
    "OverridePrice" decimal(18,2) NOT NULL,
    "Variance" decimal(18,2) NOT NULL,
    "Reason" varchar(500) NOT NULL,
    "AppliedBy" uuid NOT NULL,
    "AuthorizedBy" uuid NOT NULL,
    "AppliedAt" timestamp with time zone NOT NULL,
    FOREIGN KEY ("OrderLineId") REFERENCES "OrderLines"("Id")
);

CREATE INDEX "IX_PriceOverrides_AppliedAt" ON "PriceOverrides" ("AppliedAt");
CREATE INDEX "IX_PriceOverrides_AppliedBy" ON "PriceOverrides" ("AppliedBy");
CREATE INDEX "IX_PriceOverrides_Variance" ON "PriceOverrides" ("Variance");
```

#### GroupSettlements Table (New)
```sql
CREATE TABLE "GroupSettlements" (
    "Id" uuid PRIMARY KEY,
    "TicketIds" uuid[] NOT NULL,
    "Strategy" int NOT NULL,
    "MasterPaymentId" uuid NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" uuid NOT NULL,
    FOREIGN KEY ("MasterPaymentId") REFERENCES "Payments"("Id")
);

CREATE INDEX "IX_GroupSettlements_CreatedAt" ON "GroupSettlements" ("CreatedAt");
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Hold Ticket State Transition
*For any* open ticket, when held with a valid reason and user ID, the ticket status should change to "Held" and the hold fields (HeldAt, HoldReason, HeldBy) should be populated.
**Validates: Requirements 1.1, 1.2**

### Property 2: Hold Ticket Table Release
*For any* ticket with an associated table session, when the ticket is held, the table session should be ended and the table should be available.
**Validates: Requirements 1.3**

### Property 3: Hold Ticket Invalid States
*For any* ticket with status Closed, Voided, or Refunded, attempting to hold the ticket should fail with an error.
**Validates: Requirements 1.4**

### Property 4: Hold Ticket Validation
*For any* ticket, attempting to hold it with an empty or null reason should fail with a validation error.
**Validates: Requirements 1.5**

### Property 5: Release Held Ticket Round Trip
*For any* held ticket, releasing it should change the status back to "Open" and clear the hold fields.
**Validates: Requirements 1.6**

### Property 6: Held Tickets Query Completeness
*For any* set of tickets in the system, querying held tickets should return exactly those tickets with status "Held" and no others.
**Validates: Requirements 1.7**

### Property 7: Hold/Release Audit Trail
*For any* ticket that is held or released, an audit event should be created with the operation details.
**Validates: Requirements 1.8**

### Property 8: Split Payment Sum Equals Total
*For any* ticket and collection of split payments, the sum of all payment amounts must equal the ticket total for the payment to be accepted.
**Validates: Requirements 2.2**

### Property 9: Split Payment Overpayment Change
*For any* ticket where the sum of split payments exceeds the total, the change amount should equal (sum of payments - ticket total).
**Validates: Requirements 2.3**

### Property 10: Split Payment Underpayment Rejection
*For any* ticket where the sum of split payments is less than the total, the payment should be rejected and the remaining amount should be displayed.
**Validates: Requirements 2.4**

### Property 11: Split Payment Record Count
*For any* completed split payment with N payment entries, exactly N payment records should be created in the database.
**Validates: Requirements 2.8**

### Property 12: Discount Calculation Correctness
*For any* ticket and discount, applying the discount should reduce the total by exactly the discount amount (fixed) or percentage (percentage type).
**Validates: Requirements 3.1**

### Property 13: Discount Non-Negative Total
*For any* ticket and discount, the total after applying the discount must be greater than or equal to zero.
**Validates: Requirements 3.8**

### Property 14: Member Discount Auto-Application
*For any* ticket associated with a member customer, the member discount should be automatically applied.
**Validates: Requirements 3.3**

### Property 15: Large Discount Authorization
*For any* discount that exceeds 50% of the ticket total, manager authorization must be provided for the discount to be applied.
**Validates: Requirements 3.5**

### Property 16: Discount Audit Trail
*For any* discount applied or removed, an audit event should be created with the discount details.
**Validates: Requirements 3.7**

### Property 17: Price Override Authorization Required
*For any* order line, attempting to override the price should require manager PIN authorization.
**Validates: Requirements 4.1**

### Property 18: Price Override Non-Negative
*For any* order line, the override price must be greater than or equal to zero.
**Validates: Requirements 4.6**

### Property 19: Price Override Data Integrity
*For any* price override, the system should record original price, new price, reason, and authorizing manager.
**Validates: Requirements 4.3**

### Property 20: Price Override Total Calculation
*For any* ticket with overridden order lines, the ticket total should be calculated using the override prices.
**Validates: Requirements 4.7**

### Property 21: Price Override Audit Trail
*For any* price override applied, an audit event should be created with the override details.
**Validates: Requirements 4.5**

### Property 22: Void Ticket State Transition
*For any* open ticket, when voided with authorization and reason, the ticket status should change to "Voided".
**Validates: Requirements 5.1**

### Property 23: Void Paid Ticket Rejection
*For any* ticket with status "Paid", attempting to void it should fail and suggest refund instead.
**Validates: Requirements 5.3**

### Property 24: Full Refund Processing
*For any* paid ticket, processing a full refund should refund all payments and change status to "Refunded".
**Validates: Requirements 5.4**

### Property 25: Refund Amount Constraint
*For any* ticket, the refund amount must be less than or equal to the paid amount.
**Validates: Requirements 5.9**

### Property 26: Void/Refund Authorization Required
*For any* ticket, voiding or refunding should require manager authorization and a reason.
**Validates: Requirements 5.2, 5.6**

### Property 27: Void/Refund Audit Trail
*For any* ticket that is voided or refunded, an audit event should be created with the operation details.
**Validates: Requirements 5.8**

### Property 28: Promotion Time-Based Application
*For any* order line with a qualifying item, if a promotion is active at the order time, promotional pricing should be applied.
**Validates: Requirements 6.1**

### Property 29: Promotion Best Discount Selection
*For any* order line with multiple applicable promotions, the promotion with the best discount for the customer should be applied.
**Validates: Requirements 6.6**

### Property 30: Promotion Schedule Overlap Detection
*For any* two promotion schedules with overlapping time ranges and shared qualifying items, the system should detect and prevent the overlap.
**Validates: Requirements 7.2**

### Property 31: Promotion Manual Override
*For any* ticket with active promotions, when a manager disables the promotion with authorization, standard pricing should be applied.
**Validates: Requirements 8.1**

### Property 32: Promotion Override Audit Trail
*For any* promotion manually disabled, an audit event should be created with the override details.
**Validates: Requirements 8.3, 8.5**

### Property 33: Group Settlement Total Calculation
*For any* group settlement with N tickets, the total should equal the sum of all ticket totals.
**Validates: Requirements 9.3**

### Property 34: Group Settlement Payment Distribution
*For any* group settlement, the sum of distributed payments across all tickets should equal the master payment amount.
**Validates: Requirements 9.4**

### Property 35: Group Settlement Ticket Closure
*For any* completed group settlement with N tickets, all N tickets should have status "Paid".
**Validates: Requirements 9.6**

### Property 36: Group Settlement Audit Trail
*For any* group settlement created, an audit event should be created with the settlement details.
**Validates: Requirements 9.7**

### Property 37: Price Override Variance Calculation
*For any* price override, the variance should equal (override price - original price).
**Validates: Requirements 10.4**

### Property 38: Price Override Query Completeness
*For any* query with filters, all price overrides matching the filter criteria should be returned.
**Validates: Requirements 10.1, 10.3**

### Property 39: Manager Authorization Failed Attempts
*For any* sequence of failed PIN attempts, the failed attempt counter should increment with each failure.
**Validates: Requirements 12.3**

### Property 40: Manager Authorization Lockout
*For any* authorization attempt where failed attempts exceed the threshold, the authorization should be locked for the configured period.
**Validates: Requirements 12.4**

### Property 41: Manager Authorization Audit Trail
*For any* successful manager authorization, the authorizing manager should be recorded in the audit trail.
**Validates: Requirements 12.5**

### Property 42: Comprehensive Audit Trail
*For any* billing operation (hold, release, payment, discount, override, void, refund, promotion), an audit event should be created.
**Validates: Requirements 13.1**

### Property 43: Audit Event Data Integrity
*For any* audit event, all required fields (entity type, entity ID, event type, user ID, timestamp, before state, after state) should be populated.
**Validates: Requirements 13.2**

### Property 44: Audit Event Query Filtering
*For any* audit query with filters, only audit events matching all filter criteria should be returned.
**Validates: Requirements 13.3**

## Error Handling

### Domain Exceptions

```csharp
public class TicketCannotBeHeldException : DomainException
{
    public TicketCannotBeHeldException(TicketStatus status)
        : base($"Ticket with status {status} cannot be held") { }
}

public class InvalidPaymentSumException : DomainException
{
    public InvalidPaymentSumException(Money sum, Money total)
        : base($"Payment sum {sum} does not equal ticket total {total}") { }
}

public class DiscountExceedsTotalException : DomainException
{
    public DiscountExceedsTotalException()
        : base("Discount would result in negative total") { }
}

public class UnauthorizedOperationException : DomainException
{
    public UnauthorizedOperationException(string operation)
        : base($"Operation {operation} requires manager authorization") { }
}

public class RefundExceedsPaidAmountException : DomainException
{
    public RefundExceedsPaidAmountException(Money refund, Money paid)
        : base($"Refund amount {refund} exceeds paid amount {paid}") { }
}

public class PromotionOverlapException : DomainException
{
    public PromotionOverlapException()
        : base("Promotion schedule overlaps with existing promotion") { }
}
```

### Error Recovery Strategies

1. **Validation Errors**: Display user-friendly messages with guidance
2. **Authorization Failures**: Prompt for manager PIN, track attempts
3. **Concurrency Conflicts**: Refresh data and retry operation
4. **Database Errors**: Log error, display generic message, retry with exponential backoff
5. **Calculation Errors**: Log error, prevent operation, alert support

## Testing Strategy

### Unit Tests

Unit tests will verify specific examples and edge cases:

- Hold ticket with various ticket statuses
- Split payment with exact total, overpayment, underpayment
- Discount application with various discount types
- Price override with authorization
- Void/refund with various scenarios
- Promotion scheduling with overlaps
- Group settlement with various strategies

### Property-Based Tests

Property-based tests will verify universal properties across all inputs:

- **Test Framework**: Use fast-check (TypeScript) or FsCheck (C#)
- **Minimum Iterations**: 100 per property test
- **Tag Format**: `Feature: category-c-billing-payments, Property {number}: {property_text}`

Each correctness property listed above will be implemented as a property-based test that:
1. Generates random valid inputs
2. Executes the operation
3. Verifies the property holds

### Integration Tests

Integration tests will verify end-to-end workflows:

- Hold ticket → Table released → Resume ticket → Payment
- Split payment → Multiple payment records created
- Apply discount → Manager authorization → Audit trail
- Price override → Authorization → Total recalculation
- Void ticket → Audit trail → Cannot be paid
- Refund ticket → Payment records updated → Receipt generated
- Promotion schedule → Auto-application → Manual override
- Group settlement → Payment distribution → All tickets closed

### Performance Tests

- Payment processing: < 2 seconds
- Query held tickets: < 1 second
- Discount calculation: < 500ms
- Settle page load: < 1 second

