# Backend Overview

## Introduction

The Magidesk POS backend is built using Clean Architecture principles with a focus on business logic separation, testability, and maintainability. This document provides a comprehensive overview of the backend architecture, components, and operational patterns.

## Architecture Philosophy

### Core Principles

#### 1. Business Logic Centric
- **Domain First**: Business rules and logic reside in the Domain layer
- **Rich Domain Model**: Entities contain behavior, not just data
- **Invariant Enforcement**: Business rules enforced at the domain level
- **Ubiquitous Language**: Consistent terminology across all layers

#### 2. Clean Architecture
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Separation of Concerns**: Each layer has a single, well-defined responsibility
- **Testability**: Each layer can be tested in isolation
- **Flexibility**: Easy to modify and extend without affecting other layers

#### 3. CQRS Pattern
- **Command Query Separation**: Separate read and write operations
- **Optimized Models**: Different models for reads and writes
- **Clear Intent**: Commands change state, queries read state
- **Performance**: Tailored data access patterns

## Backend Architecture

### Layer Structure

```
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │    Commands     │  │     Queries      │  │   DTOs &      │ │
│  │ (Write Model)   │  │  (Read Model)    │  │ Interfaces    │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      DOMAIN LAYER                           │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │    Entities     │  │  Value Objects  │  │ Domain       │ │
│  │ (Aggregate Roots)│  │   (Immutable)   │  │ Services     │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 INFRASTRUCTURE LAYER                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   Repositories  │  │  External       │  │   Database    │ │
│  │ (Data Access)   │  │   Services      │  │ (PostgreSQL)  │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Dependency Rules

#### Allowed Dependencies
1. **Application → Domain**: Use cases can use domain logic
2. **Infrastructure → Application**: Data access implements application interfaces
3. **Infrastructure → Domain**: Repositories work with domain entities

#### Prohibited Dependencies
1. **Domain → Infrastructure**: Domain must not depend on technical details
2. **Domain → Application**: Domain must not depend on use cases
3. **Application → Infrastructure**: Application must not depend on implementations

## Application Layer

### Purpose

The Application layer orchestrates business operations, defines use cases, and manages transactions. It serves as the bridge between external interfaces (Presentation, API) and the Domain layer.

### Components

#### Commands (Write Model)

Commands represent intentions to change the system state. Each command encapsulates all data needed to perform a specific operation.

**Structure**:
```csharp
public class CreateTicketCommand
{
    public int TicketNumber { get; set; }
    public UserId CreatedBy { get; set; }
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid OrderTypeId { get; set; }
    public string? GlobalId { get; set; }
}

public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, TicketDto>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IEventPublisher _eventPublisher;

    public async Task<TicketDto> Handle(CreateTicketCommand command)
    {
        // Business logic orchestration
        var ticket = Ticket.Create(
            command.TicketNumber,
            command.CreatedBy,
            command.TerminalId,
            command.ShiftId,
            command.OrderTypeId,
            command.GlobalId);

        await _ticketRepository.AddAsync(ticket);
        await _eventPublisher.PublishAsync(new TicketCreatedEvent(ticket.Id));

        return ticket.ToDto();
    }
}
```

**Key Characteristics**:
- **Immutable**: Commands cannot be modified after creation
- **Validated**: Input validation before processing
- **Transactional**: Database transaction management
- **Event Publishing**: Domain events for state changes

#### Queries (Read Model)

Queries represent requests for information without changing state. Each query is optimized for specific read scenarios.

**Structure**:
```csharp
public class GetTicketQuery
{
    public Guid TicketId { get; set; }
}

public class GetTicketQueryHandler : IQueryHandler<GetTicketQuery, TicketDto>
{
    private readonly ITicketRepository _ticketRepository;

    public async Task<TicketDto> Handle(GetTicketQuery query)
    {
        var ticket = await _ticketRepository.GetByIdAsync(query.TicketId);
        if (ticket == null)
            throw new NotFoundException($"Ticket {query.TicketId} not found");

        return ticket.ToDto();
    }
}
```

**Key Characteristics**:
- **Read-Only**: Queries do not modify state
- **Optimized**: Tailored for specific read scenarios
- **Cached**: Frequently accessed data cached
- **Projected**: Return DTOs, not domain entities

#### DTOs (Data Transfer Objects)

DTOs provide data contracts for communication between layers and external systems.

**Structure**:
```csharp
public class TicketDto
{
    public Guid Id { get; set; }
    public int TicketNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public TicketStatus Status { get; set; }
    public UserId CreatedBy { get; set; }
    public Money TotalAmount { get; set; }
    public Money PaidAmount { get; set; }
    public Money DueAmount { get; set; }
    public IReadOnlyList<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();
    public IReadOnlyList<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
}
```

**Key Characteristics**:
- **Data Containers**: No business logic
- **Serializable**: Support JSON/XML serialization
- **Validatable**: Input validation attributes
- **Immutable**: Read-only where appropriate

### Application Services

Application services provide coordination for complex operations and cross-cutting concerns.

#### Event Publisher
```csharp
public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent) where T : IDomainEvent;
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
}

public class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventPublisher> _logger;

    public async Task PublishAsync<T>(T domainEvent) where T : IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<T>>();
        
        foreach (var handler in handlers)
        {
            try
            {
                await handler.Handle(domainEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling domain event {EventType}", typeof(T).Name);
            }
        }
    }
}
```

#### Validation Service
```csharp
public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T instance);
}

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public async Task<ValidationResult> ValidateAsync<T>(T instance)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        if (validator == null)
            return ValidationResult.Success();

        var result = await validator.ValidateAsync(instance);
        return result.IsValid ? ValidationResult.Success() : ValidationResult.Failure(result.Errors);
    }
}
```

## Domain Layer

### Purpose

The Domain layer contains the core business logic, entities, and rules. It is the heart of the system and has no dependencies on external frameworks or systems.

### Components

#### Entities (Aggregate Roots)

Entities represent core business concepts with identity, behavior, and invariant enforcement.

**Ticket Entity Example**:
```csharp
public class Ticket : AggregateRoot
{
    private readonly List<OrderLine> _orderLines = new();
    private readonly List<Payment> _payments = new();
    private readonly List<TicketDiscount> _discounts = new();

    // Core Properties
    public Guid Id { get; private set; }
    public int TicketNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public TicketStatus Status { get; private set; }
    public UserId CreatedBy { get; private set; }
    
    // Financial Amounts
    public Money SubtotalAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money PaidAmount { get; private set; }
    public Money DueAmount { get; private set; }

    // Collections
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    // Business Methods
    public void AddOrderLine(OrderLine orderLine)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided)
            throw new DomainInvalidOperationException($"Cannot add items to ticket in {Status} status.");

        _orderLines.Add(orderLine);
        CalculateTotals();
        AddDomainEvent(new OrderLineAddedEvent(Id, orderLine.Id));
    }

    public void AddPayment(Payment payment)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided)
            throw new DomainInvalidOperationException($"Cannot add payment to ticket in {Status} status.");

        _payments.Add(payment);
        RecalculatePaidAmount();
        
        if (PaidAmount >= TotalAmount && Status == TicketStatus.Open)
        {
            Status = TicketStatus.Paid;
            AddDomainEvent(new TicketPaidEvent(Id));
        }
    }

    private void CalculateTotals()
    {
        SubtotalAmount = _orderLines.Aggregate(Money.Zero(), (sum, line) => sum + line.TotalAmount);
        TaxAmount = IsTaxExempt ? Money.Zero() : SubtotalAmount * 0.10m;
        
        DiscountAmount = _discounts.Aggregate(Money.Zero(), (sum, d) => sum + d.Amount);
        
        TotalAmount = SubtotalAmount + TaxAmount - DiscountAmount;
        DueAmount = TotalAmount - PaidAmount;
    }
}
```

**Key Characteristics**:
- **Rich Behavior**: Contain business logic
- **Invariant Enforcement**: Protect business rules
- **Aggregate Roots**: Control consistency boundaries
- **Event Publishing**: Notify state changes

#### Value Objects

Value objects represent immutable concepts defined by their attributes rather than identity.

**Money Value Object**:
```csharp
public sealed record Money : IComparable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new BusinessRuleViolationException("Money amount cannot be negative.");
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new BusinessRuleViolationException("Currency is required.");

        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new Money(0, currency);
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new BusinessRuleViolationException("Cannot add money with different currencies.");
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new BusinessRuleViolationException("Cannot subtract money with different currencies.");
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }
}
```

**Key Characteristics**:
- **Immutable**: Cannot be modified after creation
- **Value Equality**: Equality based on values, not identity
- **Validation**: Ensure valid state on creation
- **Self-Validation**: Contain validation logic

#### Domain Services

Domain services provide complex business logic that doesn't naturally fit in a single entity.

**Ticket Domain Service**:
```csharp
public class TicketDomainService
{
    private readonly ITaxCalculator _taxCalculator;
    private readonly IDiscountCalculator _discountCalculator;

    public TicketDomainService(ITaxCalculator taxCalculator, IDiscountCalculator discountCalculator)
    {
        _taxCalculator = taxCalculator;
        _discountCalculator = discountCalculator;
    }

    public void CalculateTotals(Ticket ticket)
    {
        // Calculate subtotal
        var subtotal = ticket.OrderLines.Aggregate(Money.Zero(), (sum, line) => sum + line.TotalAmount);
        
        // Calculate tax using domain service
        var tax = _taxCalculator.CalculateTax(subtotal, ticket.IsTaxExempt, ticket.PriceIncludesTax);
        
        // Calculate discounts
        var discount = _discountCalculator.CalculateDiscount(ticket.Discounts, subtotal);
        
        // Update ticket totals
        ticket.UpdateTotals(subtotal, tax, discount);
    }

    public bool CanSplitTicket(Ticket ticket, SplitType splitType)
    {
        return ticket.Status switch
        {
            TicketStatus.Open => true,
            TicketStatus.Paid => splitType == SplitType.ByPayment,
            _ => false
        };
    }

    public IReadOnlyList<Ticket> SplitTicket(Ticket ticket, SplitType splitType, SplitParameters parameters)
    {
        if (!CanSplitTicket(ticket, splitType))
            throw new DomainInvalidOperationException($"Cannot split ticket in {ticket.Status} status.");

        return splitType switch
        {
            SplitType.BySeat => SplitBySeat(ticket, parameters.SeatNumbers),
            SplitType.ByItem => SplitByItem(ticket, parameters.ItemIds),
            SplitType.ByAmount => SplitByAmount(ticket, parameters.Amounts),
            _ => throw new NotSupportedException($"Split type {splitType} not supported.")
        };
    }
}
```

**Key Characteristics**:
- **Stateless**: No persistent state
- **Business Logic**: Complex business rules
- **Coordination**: Orchestrate multiple entities
- **Calculation**: Complex mathematical operations

#### Domain Events

Domain events represent important business state changes that other parts of the system may need to react to.

**Event Examples**:
```csharp
public record TicketCreatedEvent(Guid TicketId, int TicketNumber, UserId CreatedBy) : IDomainEvent;

public record PaymentProcessedEvent(Guid TicketId, Guid PaymentId, Money Amount, PaymentType PaymentType) : IDomainEvent;

public record OrderModifiedEvent(Guid TicketId, Guid OrderLineId, OrderLineModificationType ModificationType) : IDomainEvent;

public record TicketClosedEvent(Guid TicketId, UserId ClosedBy, DateTime ClosedAt) : IDomainEvent;
```

**Event Handlers**:
```csharp
public class TicketCreatedEventHandler : IDomainEventHandler<TicketCreatedEvent>
{
    private readonly IKitchenRoutingService _kitchenRouting;
    private readonly INotificationService _notificationService;

    public async Task Handle(TicketCreatedEvent domainEvent)
    {
        // Route to kitchen if applicable
        await _kitchenRouting.RouteTicketAsync(domainEvent.TicketId);
        
        // Send notification to manager
        await _notificationService.NotifyManagerAsync($"New ticket #{domainEvent.TicketNumber} created");
    }
}
```

**Key Characteristics**:
- **Immutable**: Event data cannot change
- **Serializable**: Support persistence and transmission
- **Timestamped**: Include occurrence time
- **Contextual**: Include relevant context information

## Infrastructure Layer

### Purpose

The Infrastructure layer implements technical concerns and external integrations. It contains all code that interacts with external systems and services.

### Components

#### Repositories

Repositories implement data access patterns for domain entities using Entity Framework Core.

**Ticket Repository Example**:
```csharp
public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TicketRepository> _logger;

    public TicketRepository(ApplicationDbContext context, ILogger<TicketRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Include(t => t.OrderLines)
                .ThenInclude(ol => ol.Modifiers)
            .Include(t => t.Payments)
            .Include(t => t.Discounts)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(t => t.Status == TicketStatus.Open)
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
        _logger.LogInformation("Added ticket {TicketId} with number {TicketNumber}", ticket.Id, ticket.TicketNumber);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _context.Tickets.Update(ticket);
        _logger.LogInformation("Updated ticket {TicketId}", ticket.Id);
    }

    public async Task DeleteAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _context.Tickets.Remove(ticket);
        _logger.LogInformation("Deleted ticket {TicketId}", ticket.Id);
    }
}
```

**Key Characteristics**:
- **Interface Implementation**: Implement repository interfaces
- **Entity Framework**: Use EF Core for data access
- **Optimization**: Query optimization and caching
- **Transaction Management**: Database transaction coordination

#### External Services

External services implement integrations with third-party systems and APIs.

**Payment Gateway Service**:
```csharp
public interface IPaymentGatewayService
{
    Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizationRequest request);
    Task<PaymentCaptureResult> CaptureAsync(PaymentCaptureRequest request);
    Task<PaymentVoidResult> VoidAsync(PaymentVoidRequest request);
    Task<PaymentRefundResult> RefundAsync(PaymentRefundRequest request);
}

public class MockPaymentGatewayService : IPaymentGatewayService
{
    private readonly ILogger<MockPaymentGatewayService> _logger;

    public async Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizationRequest request)
    {
        _logger.LogInformation("Authorizing payment {Amount} for card {LastFour}", 
            request.Amount, request.CardNumber.LastFour());

        // Simulate processing delay
        await Task.Delay(Random.Shared.Next(1000, 3000));

        // Simulate approval/denial
        var isApproved = Random.Shared.NextDouble() > 0.1; // 90% approval rate

        return new PaymentAuthorizationResult
        {
            IsApproved = isApproved,
            AuthorizationCode = isApproved ? GenerateAuthCode() : null,
            ErrorMessage = isApproved ? null : "Insufficient funds",
            TransactionId = Guid.NewGuid()
        };
    }

    private string GenerateAuthCode() => 
        $"{Random.Shared.Next(100000, 999999)}";
}
```

**Key Characteristics**:
- **Interface Implementation**: Implement service interfaces
- **Error Handling**: Robust error handling and retry logic
- **Configuration**: External service configuration
- **Logging**: Comprehensive operation logging

#### Database Configuration

Entity Framework Core configuration for database mapping and behavior.

**Entity Configuration Example**:
```csharp
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets", "magidesk");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.TicketNumber)
            .IsRequired()
            .HasComment("Unique ticket number for the day");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasComment("Current status of the ticket");

        builder.Property(t => t.SubtotalAmount)
            .HasConversion(
                money => money.Amount,
                amount => new Money(amount))
            .HasComment("Subtotal amount before tax and discounts");

        builder.Property(t => t.TotalAmount)
            .HasConversion(
                money => money.Amount,
                amount => new Money(amount))
            .HasComment("Total amount including tax");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("When the ticket was created");

        builder.Property(t => t.Version)
            .IsRowVersion()
            .HasComment("Optimistic concurrency version");

        // Indexes
        builder.HasIndex(t => t.TicketNumber)
            .IsUnique()
            .HasDatabaseName("IX_Tickets_TicketNumber");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tickets_Status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tickets_CreatedAt");

        // Relationships
        builder.HasOne(t => t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.OrderLines)
            .WithOne(ol => ol.Ticket)
            .HasForeignKey(ol => ol.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Payments)
            .WithOne(p => p.Ticket)
            .HasForeignKey(p => p.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Key Characteristics**:
- **Code-First**: Schema defined in code
- **Migrations**: Automatic schema evolution
- **Optimization**: Performance-tuned configurations
- **Versioning**: Tracked schema versions

## Business Rules and Logic

### Ticket State Machine

The ticket entity follows a strict state machine with enforced transitions:

```
Draft → Open → Paid → Closed
  │        │        │
  └──► Voided  ◄────┘
```

**State Transition Rules**:
```csharp
public enum TicketStatus
{
    Draft,      // Initial state, no items yet
    Open,       // Items added, active order
    Paid,       // Fully paid, awaiting closure
    Closed,     // Finalized, no modifications allowed
    Voided,     // Cancelled, no payments processed
    Refunded    // Closed and refunded
}

public class Ticket
{
    public void Open()
    {
        if (Status != TicketStatus.Draft)
            throw new DomainInvalidOperationException($"Cannot open ticket in {Status} status.");

        Status = TicketStatus.Open;
        OpenedAt = DateTime.UtcNow;
    }

    public void Close(UserId closedBy)
    {
        if (!CanClose())
            throw new DomainInvalidOperationException($"Cannot close ticket in {Status} status.");

        Status = TicketStatus.Closed;
        ClosedBy = closedBy;
        ClosedAt = DateTime.UtcNow;
    }

    public bool CanClose()
    {
        return Status == TicketStatus.Paid && DueAmount <= Money.Zero();
    }
}
```

### Payment Processing Rules

**Payment Validation**:
```csharp
public class Payment
{
    public void ValidateForTicket(Ticket ticket)
    {
        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Voided)
            throw new DomainInvalidOperationException($"Cannot add payment to ticket in {ticket.Status} status.");

        if (Amount > ticket.DueAmount)
            throw new BusinessRuleViolationException("Payment amount cannot exceed due amount.");

        if (Amount <= Money.Zero())
            throw new BusinessRuleViolationException("Payment amount must be positive.");
    }
}
```

**Card Payment Processing**:
```csharp
public class CreditCardPayment : Payment
{
    public async Task<PaymentResult> ProcessAsync(IPaymentGatewayService gatewayService)
    {
        // Step 1: Authorize
        var authResult = await gatewayService.AuthorizeAsync(new PaymentAuthorizationRequest
        {
            Amount = Amount,
            CardNumber = CardNumber,
            ExpiryDate = ExpiryDate,
            Cvv = Cvv
        });

        if (!authResult.IsApproved)
        {
            Status = PaymentStatus.Declined;
            return PaymentResult.Failed(authResult.ErrorMessage);
        }

        // Step 2: Capture
        var captureResult = await gatewayService.CaptureAsync(new PaymentCaptureRequest
        {
            AuthorizationCode = authResult.AuthorizationCode,
            Amount = Amount
        });

        if (captureResult.IsSuccessful)
        {
            Status = PaymentStatus.Completed;
            TransactionId = captureResult.TransactionId;
            return PaymentResult.Successful();
        }
        else
        {
            Status = PaymentStatus.Failed;
            return PaymentResult.Failed(captureResult.ErrorMessage);
        }
    }
}
```

### Inventory Management Rules

**Stock Depletion**:
```csharp
public class InventoryService
{
    public async Task DepleteInventoryAsync(OrderLine orderLine)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(orderLine.MenuItemId);
        if (menuItem == null)
            throw new NotFoundException($"Menu item {orderLine.MenuItemId} not found.");

        foreach (var recipeLine in menuItem.RecipeLines)
        {
            var inventoryItem = await _inventoryRepository.GetByIdAsync(recipeLine.InventoryItemId);
            if (inventoryItem == null)
                continue;

            var requiredQuantity = recipeLine.Quantity * orderLine.Quantity;
            
            if (inventoryItem.CurrentStock < requiredQuantity)
            {
                throw new BusinessRuleViolationException(
                    $"Insufficient stock for {inventoryItem.Name}. Required: {requiredQuantity}, Available: {inventoryItem.CurrentStock}");
            }

            inventoryItem.DepleteStock(requiredQuantity);
            await _inventoryRepository.UpdateAsync(inventoryItem);

            // Check if stock is low and alert
            if (inventoryItem.IsBelowReorderLevel)
            {
                await _alertService.SendLowStockAlertAsync(inventoryItem);
            }
        }
    }
}
```

## Error Handling

### Exception Hierarchy

```csharp
// Base domain exception
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

// Business rule violations
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message) { }
}

// Invalid operations in current state
public class DomainInvalidOperationException : DomainException
{
    public DomainInvalidOperationException(string message) : base(message) { }
}

// Concurrency conflicts
public class ConcurrencyException : DomainException
{
    public ConcurrencyException(string message) : base(message) { }
}

// Entity not found
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}
```

### Error Handling Strategy

**Global Error Handler**:
```csharp
public class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async Task HandleExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, "An error occurred");

        switch (exception)
        {
            case BusinessRuleViolationException bre:
                await HandleBusinessRuleViolation(bre);
                break;
            case ConcurrencyException ce:
                await HandleConcurrencyError(ce);
                break;
            case NotFoundException nfe:
                await HandleNotFoundError(nfe);
                break;
            default:
                await HandleGenericError(exception);
                break;
        }
    }

    private async Task HandleBusinessRuleViolation(BusinessRuleViolationException exception)
    {
        // Return user-friendly error message
        // Log for monitoring
        // Possibly notify administrators
    }
}
```

## Performance Optimization

### Caching Strategy

**Multi-Level Caching**:
```csharp
public class CachedMenuRepository : IMenuRepository
{
    private readonly IMenuRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public async Task<IReadOnlyList<MenuItem>> GetActiveItemsAsync()
    {
        const string cacheKey = "active_menu_items";
        
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<MenuItem>? cachedItems))
        {
            return cachedItems!;
        }

        var items = await _innerRepository.GetActiveItemsAsync();
        _cache.Set(cacheKey, items, _cacheDuration);
        
        return items;
    }
}
```

### Query Optimization

**Optimized Repository Queries**:
```csharp
public class TicketRepository
{
    public async Task<IReadOnlyList<Ticket>> GetOpenTicketsForTerminalAsync(Guid terminalId)
    {
        return await _context.Tickets
            .Where(t => t.Status == TicketStatus.Open && t.TerminalId == terminalId)
            .Select(t => new TicketDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                CreatedAt = t.CreatedAt,
                TotalAmount = t.TotalAmount,
                DueAmount = t.DueAmount,
                OrderLineCount = t.OrderLines.Count
            })
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }
}
```

## Testing Strategy

### Unit Testing

**Domain Entity Tests**:
```csharp
public class TicketTests
{
    [Fact]
    public void AddOrderLine_ShouldCalculateTotalsCorrectly()
    {
        // Arrange
        var ticket = Ticket.Create(1, UserId.Empty, Guid.Empty, Guid.Empty, Guid.Empty);
        var orderLine = new OrderLine(Guid.NewGuid, ticket.Id, menuItemId, 1, Money.Of(10.00m));

        // Act
        ticket.AddOrderLine(orderLine);

        // Assert
        ticket.SubtotalAmount.ShouldBe(Money.Of(10.00m));
        ticket.TaxAmount.ShouldBe(Money.Of(1.00m)); // 10% tax
        ticket.TotalAmount.ShouldBe(Money.Of(11.00m));
    }

    [Fact]
    public void CloseTicket_ShouldFailWhenNotPaid()
    {
        // Arrange
        var ticket = Ticket.Create(1, UserId.Empty, Guid.Empty, Guid.Empty, Guid.Empty);
        ticket.AddOrderLine(new OrderLine(Guid.NewGuid, ticket.Id, menuItemId, 1, Money.Of(10.00m)));

        // Act & Assert
        Should.Throw<DomainInvalidOperationException>(() => ticket.Close(UserId.Empty));
    }
}
```

### Integration Testing

**Repository Integration Tests**:
```csharp
public class TicketRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly TicketRepository _repository;
    private readonly ApplicationDbContext _context;

    public TicketRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _repository = new TicketRepository(_context, NullLogger<TicketRepository>.Instance);
    }

    [Fact]
    public async Task AddAndGetTicket_ShouldPersistCorrectly()
    {
        // Arrange
        var ticket = Ticket.Create(123, UserId.Empty, Guid.Empty, Guid.Empty, Guid.Empty);

        // Act
        await _repository.AddAsync(ticket);
        await _context.SaveChangesAsync();

        var retrieved = await _repository.GetByIdAsync(ticket.Id);

        // Assert
        retrieved.ShouldNotBeNull();
        retrieved.TicketNumber.ShouldBe(123);
        retrieved.Status.ShouldBe(TicketStatus.Draft);
    }
}
```

## Conclusion

The Magidesk POS backend architecture provides a solid foundation for a scalable, maintainable, and testable point-of-sale system. The Clean Architecture approach ensures that business logic remains independent of technical concerns, making the system easier to evolve and maintain.

Key strengths of the backend architecture include:

- **Business Logic Centric**: Core business rules are protected and enforced
- **Testability**: Each layer can be tested in isolation
- **Flexibility**: Easy to modify and extend without affecting other layers
- **Performance**: Optimized data access and caching strategies
- **Reliability**: Robust error handling and transaction management

The backend will continue to evolve based on changing business requirements, technological advances, and performance needs, while maintaining the core principles of clean architecture and domain-driven design.

---

*This backend overview provides the comprehensive foundation for understanding all server-side aspects of the Magidesk POS system.*