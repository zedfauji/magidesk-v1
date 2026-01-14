# BE-C.2-01: Implement Hold Ticket (Charge Later) - Detailed Implementation

**Ticket ID:** BE-C.2-01  
**Feature ID:** C.2  
**Title:** Implement Hold Ticket (Charge Later)  
**Priority:** P0  
**Status:** READY FOR IMPLEMENTATION

---

## Overview

Implement the ability to hold tickets for later completion, allowing customers to defer payment while freeing up the table for other customers. This is essential for tab-style operations and "charge to room" scenarios.

---

## Technical Design

### 1. Domain Layer Changes

#### 1.1 Update TicketStatus Enum
**File**: `Magidesk.Domain/Enumerations/TicketStatus.cs`

```csharp
namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the lifecycle status of a ticket.
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket is open and accepting orders.
    /// </summary>
    Open = 0,
    
    /// <summary>
    /// Ticket is held for later payment (tab/deferred payment).
    /// Table is released but ticket remains unpaid.
    /// </summary>
    Held = 1,
    
    /// <summary>
    /// Ticket is closed and fully paid.
    /// </summary>
    Closed = 2,
    
    /// <summary>
    /// Ticket has been voided (cancelled before payment).
    /// </summary>
    Voided = 3
}
```

#### 1.2 Enhance Ticket Entity
**File**: `Magidesk.Domain/Entities/Ticket.cs`

Add new properties and methods:

```csharp
public class Ticket
{
    // Existing properties...
    
    /// <summary>
    /// Timestamp when ticket was held (if applicable).
    /// </summary>
    public DateTime? HeldAt { get; private set; }
    
    /// <summary>
    /// Reason for holding the ticket.
    /// </summary>
    public string? HoldReason { get; private set; }
    
    /// <summary>
    /// User who held the ticket.
    /// </summary>
    public Guid? HeldBy { get; private set; }
    
    /// <summary>
    /// Holds the ticket for later payment.
    /// </summary>
    /// <param name="reason">Reason for holding</param>
    /// <param name="userId">User performing the hold</param>
    /// <exception cref="InvalidOperationException">Thrown if ticket cannot be held</exception>
    public void Hold(string reason, Guid userId)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException("Cannot hold a closed ticket.");
        }
        
        if (Status == TicketStatus.Voided)
        {
            throw new InvalidOperationException("Cannot hold a voided ticket.");
        }
        
        if (Status == TicketStatus.Held)
        {
            throw new InvalidOperationException("Ticket is already held.");
        }
        
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Hold reason is required.", nameof(reason));
        }
        
        Status = TicketStatus.Held;
        HeldAt = DateTime.UtcNow;
        HoldReason = reason;
        HeldBy = userId;
        UpdatedAt = DateTime.UtcNow;
        
        // Domain event
        AddDomainEvent(new TicketHeldEvent(Id, reason, userId));
    }
    
    /// <summary>
    /// Releases a held ticket back to open status.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if ticket is not held</exception>
    public void Release()
    {
        if (Status != TicketStatus.Held)
        {
            throw new InvalidOperationException("Only held tickets can be released.");
        }
        
        Status = TicketStatus.Open;
        UpdatedAt = DateTime.UtcNow;
        
        // Domain event
        AddDomainEvent(new TicketReleasedEvent(Id));
    }
}
```

#### 1.3 Create Domain Events
**File**: `Magidesk.Domain/Events/TicketHeldEvent.cs`

```csharp
namespace Magidesk.Domain.Events;

public record TicketHeldEvent(
    Guid TicketId,
    string Reason,
    Guid HeldBy
) : IDomainEvent;
```

**File**: `Magidesk.Domain/Events/TicketReleasedEvent.cs`

```csharp
namespace Magidesk.Domain.Events;

public record TicketReleasedEvent(
    Guid TicketId
) : IDomainEvent;
```

---

### 2. Application Layer

#### 2.1 Hold Ticket Command
**File**: `Magidesk.Application/Tickets/Commands/HoldTicket/HoldTicketCommand.cs`

```csharp
namespace Magidesk.Application.Tickets.Commands.HoldTicket;

public record HoldTicketCommand(
    Guid TicketId,
    string Reason,
    Guid UserId
) : IRequest<Result>;
```

#### 2.2 Hold Ticket Command Handler
**File**: `Magidesk.Application/Tickets/Commands/HoldTicket/HoldTicketCommandHandler.cs`

```csharp
namespace Magidesk.Application.Tickets.Commands.HoldTicket;

public class HoldTicketCommandHandler : IRequestHandler<HoldTicketCommand, Result>
{
    private the ITicketRepository _ticketRepository;
    private readonly ITableSessionRepository _sessionRepository;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public HoldTicketCommandHandler(
        ITicketRepository ticketRepository,
        ITableSessionRepository sessionRepository,
        IAuditService auditService,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _sessionRepository = sessionRepository;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(HoldTicketCommand request, CancellationToken cancellationToken)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            return Result.Failure("Ticket not found.");
        }

        // Hold the ticket
        try
        {
            ticket.Hold(request.Reason, request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        // If ticket is linked to a session, end the session
        if (ticket.TableSessionId.HasValue)
        {
            var session = await _sessionRepository.GetByIdAsync(ticket.TableSessionId.Value, cancellationToken);
            if (session != null && session.Status == TableSessionStatus.Active)
            {
                session.End(request.UserId);
            }
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Audit
        await _auditService.LogAsync(
            AuditEventType.TicketHeld,
            ticket.Id,
            "Ticket",
            request.UserId,
            $"Ticket held: {request.Reason}",
            cancellationToken);

        return Result.Success();
    }
}
```

#### 2.3 Release Held Ticket Command
**File**: `Magidesk.Application/Tickets/Commands/ReleaseHeldTicket/ReleaseHeldTicketCommand.cs`

```csharp
namespace Magidesk.Application.Tickets.Commands.ReleaseHeldTicket;

public record ReleaseHeldTicketCommand(
    Guid TicketId,
    Guid UserId
) : IRequest<Result>;
```

#### 2.4 Release Held Ticket Command Handler
**File**: `Magidesk.Application/Tickets/Commands/ReleaseHeldTicket/ReleaseHeldTicketCommandHandler.cs`

```csharp
namespace Magidesk.Application.Tickets.Commands.ReleaseHeldTicket;

public class ReleaseHeldTicketCommandHandler : IRequestHandler<ReleaseHeldTicketCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseHeldTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditService auditService,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReleaseHeldTicketCommand request, CancellationToken cancellationToken)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            return Result.Failure("Ticket not found.");
        }

        // Release the ticket
        try
        {
            ticket.Release();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Audit
        await _auditService.LogAsync(
            AuditEventType.TicketReleased,
            ticket.Id,
            "Ticket",
            request.UserId,
            "Held ticket released for payment",
            cancellationToken);

        return Result.Success();
    }
}
```

#### 2.5 Get Held Tickets Query
**File**: `Magidesk.Application/Tickets/Queries/GetHeldTickets/GetHeldTicketsQuery.cs`

```csharp
namespace Magidesk.Application.Tickets.Queries.GetHeldTickets;

public record GetHeldTicketsQuery : IRequest<Result<List<HeldTicketDto>>>;

public record HeldTicketDto(
    Guid Id,
    string TicketNumber,
    DateTime HeldAt,
    string HoldReason,
    string HeldByUserName,
    decimal TotalAmount,
    string? CustomerName,
    string? TableName
);
```

#### 2.6 Get Held Tickets Query Handler
**File**: `Magidesk.Application/Tickets/Queries/GetHeldTickets/GetHeldTicketsQueryHandler.cs`

```csharp
namespace Magidesk.Application.Tickets.Queries.GetHeldTickets;

public class GetHeldTicketsQueryHandler : IRequestHandler<GetHeldTicketsQuery, Result<List<HeldTicketDto>>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetHeldTicketsQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<List<HeldTicketDto>>> Handle(GetHeldTicketsQuery request, CancellationToken cancellationToken)
    {
        var heldTickets = await _ticketRepository.GetHeldTicketsAsync(cancellationToken);
        
        var dtos = heldTickets.Select(t => new HeldTicketDto(
            t.Id,
            t.TicketNumber,
            t.HeldAt!.Value,
            t.HoldReason!,
            t.HeldByUser?.FullName ?? "Unknown",
            t.TotalAmount.Amount,
            t.Customer?.FullName,
            t.Table?.Name
        )).ToList();

        return Result.Success(dtos);
    }
}
```

---

### 3. Infrastructure Layer

#### 3.1 Update Ticket Repository
**File**: `Magidesk.Infrastructure/Repositories/TicketRepository.cs`

Add new method:

```csharp
public async Task<List<Ticket>> GetHeldTicketsAsync(CancellationToken cancellationToken = default)
{
    return await _context.Tickets
        .Include(t => t.Customer)
        .Include(t => t.Table)
        .Include(t => t.HeldByUser)
        .Where(t => t.Status == TicketStatus.Held)
        .OrderByDescending(t => t.HeldAt)
        .ToListAsync(cancellationToken);
}
```

#### 3.2 Update EF Core Configuration
**File**: `Magidesk.Infrastructure/Data/Configurations/TicketConfiguration.cs`

Add new property mappings:

```csharp
builder.Property(t => t.HeldAt)
    .HasColumnType("timestamp with time zone");

builder.Property(t => t.HoldReason)
    .HasMaxLength(500);

builder.Property(t => t.HeldBy)
    .HasColumnType("uuid");

// Add index for held tickets
builder.HasIndex(t => t.Status)
    .HasFilter($"\"{nameof(Ticket.Status)}\" = {(int)TicketStatus.Held}");
```

---

### 4. Database Migration

Create migration:

```bash
dotnet ef migrations add AddHoldTicketSupport --project Magidesk.Migrations
```

Expected migration:

```csharp
migrationBuilder.AddColumn<DateTime>(
    name: "HeldAt",
    schema: "magidesk",
    table: "Tickets",
    type: "timestamp with time zone",
    nullable: true);

migrationBuilder.AddColumn<string>(
    name: "HoldReason",
    schema: "magidesk",
    table: "Tickets",
    type: "character varying(500)",
    maxLength: 500,
    nullable: true);

migrationBuilder.AddColumn<Guid>(
    name: "HeldBy",
    schema: "magidesk",
    table: "Tickets",
    type: "uuid",
    nullable: true);

migrationBuilder.CreateIndex(
    name: "IX_Tickets_Status_Held",
    schema: "magidesk",
    table: "Tickets",
    column: "Status",
    filter: "\"Status\" = 1");
```

---

## Testing Strategy

### Unit Tests

**File**: `Magidesk.Domain.Tests/Entities/TicketTests_Hold.cs`

```csharp
[Fact]
public void Hold_ValidTicket_SetsStatusToHeld()
{
    // Arrange
    var ticket = CreateOpenTicket();
    var userId = Guid.NewGuid();
    
    // Act
    ticket.Hold("Customer tab", userId);
    
    // Assert
    ticket.Status.Should().Be(TicketStatus.Held);
    ticket.HeldAt.Should().NotBeNull();
    ticket.HoldReason.Should().Be("Customer tab");
    ticket.HeldBy.Should().Be(userId);
}

[Fact]
public void Hold_ClosedTicket_ThrowsInvalidOperationException()
{
    // Arrange
    var ticket = CreateClosedTicket();
    
    // Act & Assert
    var act = () => ticket.Hold("Test", Guid.NewGuid());
    act.Should().Throw<InvalidOperationException>()
        .WithMessage("Cannot hold a closed ticket.");
}

[Fact]
public void Release_HeldTicket_SetsStatusToOpen()
{
    // Arrange
    var ticket = CreateHeldTicket();
    
    // Act
    ticket.Release();
    
    // Assert
    ticket.Status.Should().Be(TicketStatus.Open);
}
```

### Integration Tests

**File**: `Magidesk.Application.Tests/Tickets/Commands/HoldTicketCommandTests.cs`

```csharp
[Fact]
public async Task Handle_ValidTicket_HoldsTicketAndEndsSession()
{
    // Arrange
    var ticket = await CreateTicketWithSession();
    var command = new HoldTicketCommand(ticket.Id, "Customer tab", _testUserId);
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    
    var updatedTicket = await _ticketRepository.GetByIdAsync(ticket.Id);
    updatedTicket.Status.Should().Be(TicketStatus.Held);
    
    var session = await _sessionRepository.GetByIdAsync(ticket.TableSessionId.Value);
    session.Status.Should().Be(TableSessionStatus.Ended);
}
```

---

## Acceptance Criteria

- [ ] Ticket can be held with reason
- [ ] Held tickets listed in query
- [ ] Held ticket can be released
- [ ] Table released when ticket held
- [ ] Cannot hold closed/voided tickets
- [ ] Audit trail created for hold/release
- [ ] Unit tests pass
- [ ] Integration tests pass

---

## Implementation Checklist

### Domain Layer
- [ ] Update `TicketStatus` enum
- [ ] Add hold properties to `Ticket` entity
- [ ] Implement `Hold()` method
- [ ] Implement `Release()` method
- [ ] Create `TicketHeldEvent`
- [ ] Create `TicketReleasedEvent`

### Application Layer
- [ ] Create `HoldTicketCommand`
- [ ] Create `HoldTicketCommandHandler`
- [ ] Create `ReleaseHeldTicketCommand`
- [ ] Create `ReleaseHeldTicketCommandHandler`
- [ ] Create `GetHeldTicketsQuery`
- [ ] Create `GetHeldTicketsQueryHandler`

### Infrastructure Layer
- [ ] Update `TicketConfiguration`
- [ ] Add `GetHeldTicketsAsync()` to repository
- [ ] Create and apply migration

### Testing
- [ ] Write unit tests for `Ticket.Hold()`
- [ ] Write unit tests for `Ticket.Release()`
- [ ] Write integration tests for commands
- [ ] Write integration tests for queries

---

*Ready for implementation - January 14, 2026*
