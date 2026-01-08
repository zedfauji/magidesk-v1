# Backend Tickets: Category E - Reservations & Scheduling

> [!CAUTION]
> **CRITICAL GAP**: This entire module is NOT IMPLEMENTED (0% parity). All 12 features require full implementation from scratch. This is a P0 blocker for billiard club operations.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-E.1-01 | E.1 | Create Reservation Entity and Repository | P0 | NOT_STARTED |
| BE-E.1-02 | E.1 | Create CreateReservationCommand | P0 | NOT_STARTED |
| BE-E.2-01 | E.2 | Create GetReservationsQuery with Calendar View | P0 | NOT_STARTED |
| BE-E.3-01 | E.3 | Create UpdateReservationCommand | P0 | NOT_STARTED |
| BE-E.4-01 | E.4 | Create CancelReservationCommand | P0 | NOT_STARTED |
| BE-E.5-01 | E.5 | Implement Availability Check Service | P0 | NOT_STARTED |
| BE-E.6-01 | E.6 | Create ConvertReservationToSessionCommand | P0 | NOT_STARTED |
| BE-E.7-01 | E.7 | Implement Reservation Conflict Detection | P1 | NOT_STARTED |
| BE-E.8-01 | E.8 | Associate Customer with Reservation | P1 | NOT_STARTED |
| BE-E.9-01 | E.9 | Create ClubSchedule Entity | P1 | NOT_STARTED |
| BE-E.10-01 | E.10 | Implement Recurring Reservation Support | P2 | NOT_STARTED |
| BE-E.11-01 | E.11 | Create Reservation Reminder System | P2 | NOT_STARTED |
| BE-E.12-01 | E.12 | Create WaitingListCommand | P2 | NOT_STARTED |

---

## BE-E.1-01: Create Reservation Entity and Repository

**Ticket ID:** BE-E.1-01  
**Feature ID:** E.1  
**Type:** Backend  
**Title:** Create Reservation Entity and Repository  
**Priority:** P0

### Outcome (measurable, testable)
A fully implemented `Reservation` domain entity with repository support for pre-booking tables.

### Scope
- Create `Reservation.cs` entity in `Magidesk.Domain/Entities/`
- Create `ReservationStatus` enumeration (Pending, Confirmed, CheckedIn, Completed, Cancelled, NoShow)
- Create `IReservationRepository` interface in `Magidesk.Application/Interfaces/`
- Create `ReservationRepository` implementation in `Magidesk.Infrastructure/Repositories/`
- Create EF Core configuration in `Magidesk.Infrastructure/Configurations/`
- Add migration for Reservations table

### Explicitly Out of Scope
- UI components
- Notification system
- Recurring reservations

### Implementation Notes
```csharp
public class Reservation
{
    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerPhone { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int PartySize { get; private set; }
    public ReservationStatus Status { get; private set; }
    public string Notes { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    
    // Domain methods
    public static Reservation Create(Guid tableId, DateTime date, TimeSpan start, TimeSpan end, string customerName, int partySize);
    public void Confirm();
    public void CheckIn();
    public void Cancel(string reason);
    public void MarkNoShow();
    public void Complete();
    public bool OverlapsWith(DateTime date, TimeSpan start, TimeSpan end);
}

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    CheckedIn = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5
}
```

### Quality & Guardrails
- **guardrails.md:** Rich domain model with behavior
- **domain-model.md:** State machine transitions enforced
- **testing-requirements.md:** Domain layer ≥90% coverage
- **exception-handling-contract.md:** Domain exceptions for invalid transitions

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Table entity | Exists |
| SOFT | Customer entity | BE-F.1-01 |

### Acceptance Criteria
- [ ] Entity created with all properties
- [ ] All status transitions validated in entity
- [ ] Cannot confirm cancelled reservation
- [ ] Cannot check-in unconfirmed reservation
- [ ] OverlapsWith() calculates correctly
- [ ] Repository interface created
- [ ] Repository implementation created
- [ ] EF Core configuration created
- [ ] Migration runs successfully
- [ ] Unit tests ≥90% coverage

### Failure Modes to Guard Against
- Invalid status transitions
- Overlapping reservation detection failure
- Missing customer validation

---

## BE-E.1-02: Create CreateReservationCommand

**Ticket ID:** BE-E.1-02  
**Feature ID:** E.1  
**Type:** Backend  
**Title:** Create CreateReservationCommand  
**Priority:** P0

### Outcome (measurable, testable)
A command handler that creates new reservations with availability validation.

### Scope
- Create `CreateReservationCommand` record
- Create `CreateReservationCommandValidator`
- Create `CreateReservationCommandHandler`
- Create `CreateReservationResult` DTO
- Integrate with availability check

### Explicitly Out of Scope
- UI components
- Email notifications
- Recurring reservations

### Implementation Notes
```csharp
public record CreateReservationCommand(
    Guid TableId,
    Guid? CustomerId,
    string CustomerName,
    string CustomerPhone,
    DateTime ReservationDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int PartySize,
    string Notes
);

public class CreateReservationCommandHandler
{
    // 1. Validate table exists
    // 2. Check availability using IAvailabilityService
    // 3. Create reservation entity
    // 4. Persist via repository
    // 5. Return result with reservation ID
}
```

### Quality & Guardrails
- **guardrails.md:** One use case per file
- **exception-handling-contract.md:** Return Result object
- **no-silent-failure.md:** All conflicts reported

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reservation entity | BE-E.1-01 |
| HARD | Availability service | BE-E.5-01 |

### Acceptance Criteria
- [ ] Command creates reservation successfully
- [ ] Validator checks all required fields
- [ ] Availability checked before creation
- [ ] Conflict prevents creation with clear error
- [ ] Result contains reservation ID on success
- [ ] Unit tests ≥80% coverage

### Failure Modes to Guard Against
- Creating overlapping reservations
- Past date reservations
- Invalid time ranges (end before start)

---

## BE-E.2-01: Create GetReservationsQuery with Calendar View

**Ticket ID:** BE-E.2-01  
**Feature ID:** E.2  
**Type:** Backend  
**Title:** Create GetReservationsQuery with Calendar View  
**Priority:** P0

### Outcome (measurable, testable)
A query that retrieves reservations for display in calendar and list views.

### Scope
- Create `GetReservationsQuery` with date range filter
- Create `GetReservationsQueryHandler`
- Create `ReservationDto` for UI
- Support filtering by table, date range, status

### Implementation Notes
```csharp
public record GetReservationsQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? TableId,
    ReservationStatus? Status
);

public record ReservationDto(
    Guid Id,
    Guid TableId,
    string TableName,
    string CustomerName,
    string CustomerPhone,
    DateTime ReservationDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int PartySize,
    ReservationStatus Status,
    string Notes
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reservation entity | BE-E.1-01 |

### Acceptance Criteria
- [ ] Query retrieves reservations correctly
- [ ] Date range filter works
- [ ] Table filter works
- [ ] Status filter works
- [ ] DTO contains all needed UI fields
- [ ] Performance acceptable for 1000+ reservations

---

## BE-E.5-01: Implement Availability Check Service

**Ticket ID:** BE-E.5-01  
**Feature ID:** E.5  
**Type:** Backend  
**Title:** Implement Availability Check Service  
**Priority:** P0

### Outcome (measurable, testable)
A service that checks table availability considering existing reservations and current sessions.

### Scope
- Create `IAvailabilityService` interface
- Create `AvailabilityService` implementation
- Check against existing reservations
- Check against current active sessions
- Support buffer time between reservations

### Implementation Notes
```csharp
public interface IAvailabilityService
{
    Task<bool> IsTableAvailable(Guid tableId, DateTime date, TimeSpan startTime, TimeSpan endTime);
    Task<IEnumerable<TimeSlot>> GetAvailableSlots(Guid tableId, DateTime date);
    Task<IEnumerable<Guid>> GetAvailableTables(DateTime date, TimeSpan startTime, TimeSpan endTime);
}

public class AvailabilityService : IAvailabilityService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ITableSessionRepository _tableSessionRepository;
    private readonly TimeSpan _bufferTime = TimeSpan.FromMinutes(15);
    
    public async Task<bool> IsTableAvailable(...)
    {
        // Check existing reservations
        // Check active sessions
        // Consider buffer time
        // Return availability
    }
}
```

### Quality & Guardrails
- **guardrails.md:** Domain service in Application layer
- **testing-requirements.md:** All scenarios tested

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reservation entity | BE-E.1-01 |
| HARD | TableSession entity | BE-A.1-01 |

### Acceptance Criteria
- [ ] Correctly identifies conflicts
- [ ] Handles edge cases (midnight crossover)
- [ ] Buffer time respected
- [ ] Active sessions considered
- [ ] GetAvailableSlots returns correct slots
- [ ] GetAvailableTables works correctly
- [ ] Unit tests cover all scenarios

---

## BE-E.6-01: Create ConvertReservationToSessionCommand

**Ticket ID:** BE-E.6-01  
**Feature ID:** E.6  
**Type:** Backend  
**Title:** Create ConvertReservationToSessionCommand  
**Priority:** P0

### Outcome (measurable, testable)
A command that converts a checked-in reservation into an active table session.

### Scope
- Create `ConvertReservationToSessionCommand`
- Create handler that:
  - Updates reservation status to CheckedIn
  - Creates TableSession
  - Links session to reservation
  - Links customer if exists

### Implementation Notes
```csharp
public record ConvertReservationToSessionCommand(
    Guid ReservationId
);

// Handler:
// 1. Get reservation
// 2. Validate status is Confirmed
// 3. Create TableSession with reservation's customer/table
// 4. Update reservation status to CheckedIn
// 5. Return session ID
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reservation entity | BE-E.1-01 |
| HARD | TableSession entity | BE-A.1-01 |
| HARD | StartTableSessionCommand | BE-A.1-02 |

### Acceptance Criteria
- [ ] Converts confirmed reservation to session
- [ ] Links customer to session
- [ ] Updates reservation status
- [ ] Fails if reservation not confirmed
- [ ] Fails if table already in use

---

## BE-E.3-01: Create UpdateReservationCommand

**Ticket ID:** BE-E.3-01  
**Feature ID:** E.3  
**Type:** Backend  
**Title:** Create UpdateReservationCommand  
**Priority:** P0

### Outcome (measurable, testable)
A command to modify existing reservation details.

### Scope
- Create `UpdateReservationCommand`
- Allow updating: date, time, party size, notes, table
- Re-validate availability on time/table changes
- Cannot update checked-in or completed reservations

### Implementation Notes
```csharp
public record UpdateReservationCommand(
    Guid ReservationId,
    Guid? TableId,
    DateTime? ReservationDate,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    int? PartySize,
    string Notes
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reservation entity | BE-E.1-01 |
| HARD | Availability service | BE-E.5-01 |

### Acceptance Criteria
- [ ] Updates allowed fields
- [ ] Re-validates availability on time change
- [ ] Prevents update of checked-in reservations
- [ ] Returns clear error on conflict

---

## BE-E.4-01: Create CancelReservationCommand

**Ticket ID:** BE-E.4-01  
**Feature ID:** E.4  
**Type:** Backend  
**Title:** Create CancelReservationCommand  
**Priority:** P0

### Outcome (measurable, testable)
A command to cancel reservations with reason tracking.

### Scope
- Create `CancelReservationCommand`
- Track cancellation reason
- Track who cancelled
- Prevent cancellation of completed reservations

### Implementation Notes
```csharp
public record CancelReservationCommand(
    Guid ReservationId,
    string Reason
);

// Handler updates status to Cancelled, stores reason
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reservation entity | BE-E.1-01 |

### Acceptance Criteria
- [ ] Cancels pending/confirmed reservations
- [ ] Stores cancellation reason
- [ ] Cannot cancel completed reservations
- [ ] Returns success/failure result

---

## BE-E.9-01: Create ClubSchedule Entity

**Ticket ID:** BE-E.9-01  
**Feature ID:** E.9  
**Type:** Backend  
**Title:** Create ClubSchedule Entity  
**Priority:** P1

### Outcome (measurable, testable)
An entity to manage club operating hours and special dates.

### Scope
- Create `ClubSchedule` entity with operating hours
- Create `SpecialDate` entity for holidays/events
- Integrate with availability service

### Implementation Notes
```csharp
public class ClubSchedule
{
    public Guid Id { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan OpenTime { get; private set; }
    public TimeSpan CloseTime { get; private set; }
    public bool IsClosed { get; private set; }
}

public class SpecialDate
{
    public Guid Id { get; private set; }
    public DateTime Date { get; private set; }
    public bool IsClosed { get; private set; }
    public TimeSpan? OpenTime { get; private set; }
    public TimeSpan? CloseTime { get; private set; }
    public string Description { get; private set; }
}
```

### Acceptance Criteria
- [ ] Regular schedule entity works
- [ ] Special dates override regular schedule
- [ ] Availability service respects schedule
- [ ] Migration creates tables

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 7 | NOT_STARTED |
| P1 | 3 | NOT_STARTED |
| P2 | 3 | NOT_STARTED |
| **Total** | **13** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
