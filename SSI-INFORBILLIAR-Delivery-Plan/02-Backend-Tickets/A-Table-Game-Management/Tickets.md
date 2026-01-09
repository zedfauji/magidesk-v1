# Backend Tickets: Category A - Table & Game Management

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-A.1-01 | A.1 | Create TableSession Entity | P0 | DONE |
| BE-A.1-02 | A.1 | Create StartTableSessionCommand | P0 | DONE |
| BE-A.1-03 | A.1 | Add Table Status Management Methods | P0 | NOT_STARTED |
| BE-A.2-01 | A.2 | Create EndTableSessionCommand | P0 | NOT_STARTED |
| BE-A.3-01 | A.3 | Create GetActiveSessionsQuery | P0 | NOT_STARTED |
| BE-A.4-01 | A.4 | Add Session Status to Table Query | P0 | NOT_STARTED |
| BE-A.5-01 | A.5 | Create TableType Entity | P0 | NOT_STARTED |
| BE-A.6-01 | A.6 | Add TableTypeId to Table Entity | P0 | NOT_STARTED |
| BE-A.9-01 | A.9 | Create PricingService for Time-Based Billing | P0 | NOT_STARTED |
| BE-A.10-01 | A.10 | Implement First-Hour Pricing Rules | P1 | NOT_STARTED |
| BE-A.11-01 | A.11 | Implement Time Rounding Rules | P1 | NOT_STARTED |
| BE-A.12-01 | A.12 | Implement Minimum Charge Rules | P1 | NOT_STARTED |
| BE-A.16-01 | A.16 | Create PauseTableSessionCommand | P0 | NOT_STARTED |
| BE-A.17-01 | A.17 | Create AdjustSessionTimeCommand | P0 | NOT_STARTED |
| BE-A.7-01 | A.7 | Link Game Equipment to Table | P2 | NOT_STARTED |
| BE-A.8-01 | A.8 | Create Game History Query | P2 | NOT_STARTED |
| BE-A.13-01 | A.13 | Complete Server Assignment Logic | P2 | NOT_STARTED |
| BE-A.14-01 | A.14 | Create MergeTablesCommand | P2 | NOT_STARTED |
| BE-A.15-01 | A.15 | Create SplitTableCommand | P2 | NOT_STARTED |
| BE-A.18-01 | A.18 | Create TransferSessionCommand | P2 | NOT_STARTED |
| BE-A.19-01 | A.19 | Add GuestCount to TableSession | P1 | NOT_STARTED |

---

## BE-A.1-01: Create TableSession Entity

**Ticket ID:** BE-A.1-01  
**Feature ID:** A.1  
**Type:** Backend  
**Title:** Create TableSession Entity  
**Priority:** P0

### Outcome (measurable, testable)
A fully implemented `TableSession` domain entity that tracks table usage time with support for pausing, and integrates with the billing system.

### Scope
- Create `TableSession.cs` entity in `Magidesk.Domain/Entities/`
- Create `TableSessionStatus` enumeration
- Create `ITableSessionRepository` interface in `Magidesk.Application/Interfaces/`
- Create `TableSessionRepository` implementation in `Magidesk.Infrastructure/Repositories/`
- Create EF Core configuration in `Magidesk.Infrastructure/Configurations/`
- Add migration for TableSessions table

### Explicitly Out of Scope
- UI components
- Query handlers
- Command handlers (separate ticket)
- Pricing calculation (separate ticket)

### Implementation Notes
```csharp
public class TableSession
{
    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? TicketId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public TimeSpan TotalPausedDuration { get; private set; }
    public TableSessionStatus Status { get; private set; }
    public Guid TableTypeId { get; private set; }
    public decimal HourlyRate { get; private set; }
    public Money TotalCharge { get; private set; }
    public int GuestCount { get; private set; }
    
    // Domain methods
    public void Start(Guid tableId, Guid tableTypeId, decimal hourlyRate);
    public void Pause();
    public void Resume();
    public void End(Money calculatedCharge);
    public TimeSpan GetBillableTime();
}
```

### Quality & Guardrails
- **guardrails.md:** Rich domain model - entity MUST have behavior, not just data
- **domain-model.md:** Aggregate root with invariant enforcement
- **testing-requirements.md:** Domain layer ≥90% coverage required
- **exception-handling-contract.md:** Throw domain exceptions for invalid operations

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableType entity | BE-A.5-01 |
| SOFT | Money value object | Exists |
| SOFT | Table entity | Exists |

### Acceptance Criteria
- [ ] Entity created with all properties
- [ ] All invariants enforced in entity
- [ ] Status transitions validated (cannot end paused session)
- [ ] GetBillableTime() excludes paused duration
- [ ] Repository interface created
- [ ] Repository implementation created
- [ ] EF Core configuration created
- [ ] Migration runs successfully
- [ ] Unit tests ≥90% coverage
- [ ] All invariant violation tests pass

### Failure Modes to Guard Against
- Entity without behavior (anemic model) - VIOLATION
- Missing invariant validation - VIOLATION
- Direct property setters - VIOLATION
- Missing repository interface - VIOLATION

---

## BE-A.1-02: Create StartTableSessionCommand

**Ticket ID:** BE-A.1-02  
**Feature ID:** A.1  
**Type:** Backend  
**Title:** Create StartTableSessionCommand  
**Priority:** P0

### Outcome (measurable, testable)
A command handler that creates a new table session, validates preconditions, and updates table status.

### Scope
- Create `StartTableSessionCommand` in `Magidesk.Application/Commands/`
- Create `StartTableSessionCommandValidator` using FluentValidation
- Create `StartTableSessionCommandHandler`
- Create `StartTableSessionResult` DTO

### Explicitly Out of Scope
- UI components
- Timer UI
- Pricing calculation at start (only at end)

### Implementation Notes
```csharp
public record StartTableSessionCommand(
    Guid TableId,
    Guid? CustomerId,
    int GuestCount
);

public class StartTableSessionCommandHandler : ICommandHandler<StartTableSessionCommand, StartTableSessionResult>
{
    // Validate table is available
    // Create TableSession
    // Update Table status to InUse
    // Return session ID and start time
}
```

### Quality & Guardrails
- **guardrails.md:** One use case per file
- **guardrails.md:** Use FluentValidation for validation
- **exception-handling-contract.md:** Return Result object with error message
- **no-silent-failure.md:** All errors must be reportable to UI

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |
| HARD | TableType entity | BE-A.5-01 |
| SOFT | Table entity | Exists |

### Acceptance Criteria
- [ ] Command record created
- [ ] Validator validates TableId exists
- [ ] Validator validates table is available
- [ ] Handler creates TableSession
- [ ] Handler updates Table status
- [ ] Handler returns success result with session ID
- [ ] Handler returns error result if table unavailable
- [ ] Unit tests for handler ≥80%
- [ ] Validation tests pass

### Failure Modes to Guard Against
- Starting session on occupied table
- Starting session with invalid TableId
- Silent failure on database error

---

## BE-A.1-03: Add Table Status Management Methods

**Ticket ID:** BE-A.1-03  
**Feature ID:** A.1  
**Type:** Backend  
**Title:** Add Table Status Management Methods  
**Priority:** P0

### Outcome (measurable, testable)
Table entity can change status to InUse/Available without requiring ticket assignment, enabling proper session lifecycle management.

### Scope
- Add `MarkInUse()` method to Table entity
- Add `MarkAvailable()` method to Table entity
- Add invariant validation (cannot mark in-use if already in-use)
- Update `StartTableSessionCommandHandler` to call `MarkInUse()`
- Update `EndTableSessionCommandHandler` to call `MarkAvailable()`
- Add unit tests for new methods

### Explicitly Out of Scope
- Changing existing `AssignTicket()` behavior
- Modifying ticket-related status transitions

### Implementation Notes
```csharp
public class Table
{
    // Existing code...
    
    /// <summary>
    /// Marks table as in-use (for sessions without tickets).
    /// </summary>
    public void MarkInUse()
    {
        if (Status == TableStatus.InUse)
        {
            throw new InvalidOperationException("Table is already in use.");
        }
        
        if (Status != TableStatus.Available)
        {
            throw new InvalidOperationException($"Cannot mark table in-use from status {Status}.");
        }
        
        Status = TableStatus.InUse;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks table as available (releases from session).
    /// </summary>
    public void MarkAvailable()
    {
        if (CurrentTicketId.HasValue)
        {
            throw new InvalidOperationException("Cannot mark table available while ticket is assigned. Release ticket first.");
        }
        
        Status = TableStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Quality & Guardrails
- **domain-model.md:** Add behavior to entity, not anemic
- **testing-requirements.md:** Domain layer ≥90% coverage
- **exception-handling-contract.md:** Throw domain exceptions for invalid state transitions

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Table entity | Exists |
| SOFT | StartTableSessionCommand | BE-A.1-02 |
| SOFT | EndTableSessionCommand | BE-A.2-01 |

### Acceptance Criteria
- [ ] `MarkInUse()` method added to Table entity
- [ ] `MarkAvailable()` method added to Table entity
- [ ] Cannot mark in-use if already in-use (invariant)
- [ ] Cannot mark available if ticket assigned (invariant)
- [ ] `StartTableSessionCommandHandler` updated to call `MarkInUse()`
- [ ] Unit tests for new methods ≥90%
- [ ] All invariant tests pass

### Failure Modes to Guard Against
- Marking table in-use when already in-use
- Marking table available while ticket still assigned
- Status transition from invalid states

---

## BE-A.2-01: Create EndTableSessionCommand

**Ticket ID:** BE-A.2-01  
**Feature ID:** A.2  
**Type:** Backend  
**Title:** Create EndTableSessionCommand  
**Priority:** P0

### Outcome (measurable, testable)
A command handler that ends a table session, calculates time-based charges, creates/updates ticket, and releases table.

### Scope
- Create `EndTableSessionCommand`
- Create `EndTableSessionCommandValidator`
- Create `EndTableSessionCommandHandler`
- Integrate with PricingService for charge calculation
- Create or update Ticket with time charges

### Explicitly Out of Scope
- Payment processing (happens after)
- Timer UI

### Implementation Notes
```csharp
public record EndTableSessionCommand(
    Guid SessionId,
    bool CreateTicket = true
);

// Handler:
// 1. Get session
// 2. Calculate billable time
// 3. Calculate charge using PricingService
// 4. End session (session.End(calculatedCharge))
// 5. Create/update ticket with time line item
// 6. Update table status to Available
// 7. Return result with ticket info
```

### Quality & Guardrails
- **domain-model.md:** Session.End() enforces invariants
- **exception-handling-contract.md:** Return Result with clear error message
- **guardrails.md:** Business logic in domain, orchestration in handler

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |
| HARD | PricingService | BE-A.9-01 |
| HARD | Ticket entity | Exists |

### Acceptance Criteria
- [ ] Command ends session correctly
- [ ] Billable time calculated correctly (excludes pauses)
- [ ] Time charge calculated using PricingService
- [ ] Ticket created with time line item
- [ ] Table status updated to Available
- [ ] Error returned if session not found
- [ ] Error returned if session already ended
- [ ] Unit tests ≥80% coverage

### Failure Modes to Guard Against
- Ending already-ended session
- Ending paused session without resuming
- Incorrect billable time calculation
- Missing ticket creation

---

## BE-A.5-01: Create TableType Entity

**Ticket ID:** BE-A.5-01  
**Feature ID:** A.5  
**Type:** Backend  
**Title:** Create TableType Entity  
**Priority:** P0

### Outcome (measurable, testable)
A `TableType` entity that defines different table categories with their pricing rules.

### Scope
- Create `TableType.cs` entity
- Create `ITableTypeRepository` interface
- Create `TableTypeRepository` implementation
- Create EF Core configuration
- Add migration
- Seed default table types

### Explicitly Out of Scope
- UI for managing table types
- Complex pricing rules (separate tickets)

### Implementation Notes
```csharp
public class TableType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal HourlyRate { get; private set; }
    public decimal? FirstHourRate { get; private set; }
    public int MinimumMinutes { get; private set; }
    public int RoundingMinutes { get; private set; }
    public bool IsActive { get; private set; }
    
    // Invariants
    private TableType() { } // EF
    public static TableType Create(string name, decimal hourlyRate);
    public void UpdateRates(decimal hourlyRate, decimal? firstHourRate);
    public void SetRounding(int minimumMinutes, int roundingMinutes);
}
```

### Quality & Guardrails
- **domain-model.md:** Rich entity with behavior
- **guardrails.md:** No anemic domain model
- **testing-requirements.md:** ≥90% coverage

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| NONE | - | - |

### Acceptance Criteria
- [ ] Entity created with invariants
- [ ] HourlyRate > 0 enforced
- [ ] MinimumMinutes >= 0 enforced
- [ ] RoundingMinutes >= 1 enforced
- [ ] Repository created
- [ ] Migration runs
- [ ] Default types seeded
- [ ] Tests ≥90%

### Failure Modes to Guard Against
- HourlyRate = 0 or negative
- Invalid rounding values
- Missing seed data

---

## BE-A.6-01: Add TableTypeId to Table Entity

**Ticket ID:** BE-A.6-01  
**Feature ID:** A.6  
**Type:** Backend  
**Title:** Add TableTypeId to Table Entity  
**Priority:** P0

### Outcome (measurable, testable)
Each table is linked to a `TableType` to determine its pricing rules for time-based billing.

### Scope
- Add `TableTypeId` property to `Table` entity
- Add `SetTableType()` method to `Table` entity
- Update `Table.Create()` to accept optional `tableTypeId`
- Update `TableConfiguration` EF Core mapping
- Create migration to add `TableTypeId` column
- Add foreign key constraint to `TableTypes` table
- Update existing table seeding to assign default table types
- Add unit tests for new functionality

### Explicitly Out of Scope
- UI for assigning table types
- Changing existing table type assignments (handled by UI later)
- Validation of table type pricing rules (handled in TableType entity)

### Implementation Notes
```csharp
public class Table
{
    // Existing properties...
    public Guid? TableTypeId { get; private set; }
    
    // Navigation property (optional, for EF Core)
    public TableType? TableType { get; private set; }
    
    // Update Create method signature
    public static Table Create(
        int tableNumber,
        int capacity,
        double x = 0,
        double y = 0,
        Guid? floorId = null,
        Guid? layoutId = null,
        Guid? tableTypeId = null,  // NEW PARAMETER
        bool isActive = true,
        TableShapeType shape = TableShapeType.Rectangle,
        double width = 100,
        double height = 100)
    {
        // Validation and creation logic
        TableTypeId = tableTypeId; // Can be null initially
    }
    
    // New method to assign/change table type
    public void SetTableType(Guid tableTypeId)
    {
        if (tableTypeId == Guid.Empty)
        {
            throw new ArgumentException("Table type ID cannot be empty.", nameof(tableTypeId));
        }
        
        TableTypeId = tableTypeId;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Method to clear table type (make it nullable)
    public void ClearTableType()
    {
        TableTypeId = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### EF Core Configuration
```csharp
public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        // Existing configuration...
        
        // Add TableTypeId foreign key
        builder.Property(t => t.TableTypeId)
            .IsRequired(false); // Nullable - tables can exist without a type initially
        
        builder.HasOne(t => t.TableType)
            .WithMany()
            .HasForeignKey(t => t.TableTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting table types in use
    }
}
```

### Migration Strategy
```csharp
// Migration Up():
// 1. Add TableTypeId column (nullable)
// 2. Add foreign key constraint to TableTypes
// 3. Update existing tables to assign default table type (Standard)

// Migration Down():
// 1. Drop foreign key constraint
// 2. Drop TableTypeId column
```

### Quality & Guardrails
- **domain-model.md:** Rich entity with behavior methods
- **guardrails.md:** Maintain encapsulation with private setters
- **testing-requirements.md:** ≥90% coverage for new methods
- **database-rules.md:** Migration must be reversible
- **exception-handling-contract.md:** Validate table type ID

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableType entity | BE-A.5-01 ✅ |
| SOFT | Table entity | Exists ✅ |

### Acceptance Criteria
- [ ] `TableTypeId` property added to Table entity
- [ ] `SetTableType()` method validates and sets table type
- [ ] `ClearTableType()` method allows removing table type
- [ ] `Create()` method accepts optional `tableTypeId` parameter
- [ ] EF Core configuration includes foreign key
- [ ] Migration created and runs successfully (Up and Down)
- [ ] Existing tables assigned default table type in migration
- [ ] Foreign key constraint prevents deleting table types in use
- [ ] Unit tests for `SetTableType()` and `ClearTableType()`
- [ ] Unit tests for validation (empty GUID throws exception)
- [ ] Tests ≥90% coverage

### Failure Modes to Guard Against
- Empty GUID for table type ID
- Deleting a table type that's assigned to tables
- Migration fails on existing data
- Null reference when accessing TableType navigation property
- Breaking existing table creation code

---

## BE-A.9-01: Create PricingService for Time-Based Billing

**Ticket ID:** BE-A.9-01  
**Feature ID:** A.9  
**Type:** Backend  
**Title:** Create PricingService for Time-Based Billing  
**Priority:** P0

### Outcome (measurable, testable)
A domain service that calculates time-based charges with support for first-hour rates, rounding, and minimum charges.

### Scope
- Create `IPricingService` interface in Domain
- Create `PricingService` implementation
- Handle first-hour pricing
- Handle time rounding
- Handle minimum charges
- Handle member discounts (if member passed)

### Explicitly Out of Scope
- Happy hour pricing (separate ticket)
- Promotional pricing (separate ticket)

### Implementation Notes
```csharp
public interface IPricingService
{
    Money CalculateTimeCharge(
        TimeSpan billableTime,
        TableType tableType,
        Member? member = null
    );
}

public class PricingService : IPricingService
{
    public Money CalculateTimeCharge(...)
    {
        // 1. Round time per tableType.RoundingMinutes
        // 2. Apply minimum if < tableType.MinimumMinutes
        // 3. Calculate first hour if tableType.FirstHourRate
        // 4. Calculate remaining hours at tableType.HourlyRate
        // 5. Apply member discount if member provided
        // 6. Return total charge
    }
}
```

### Quality & Guardrails
- **domain-model.md:** Domain service is stateless
- **guardrails.md:** Business logic in domain layer
- **testing-requirements.md:** All calculation paths tested

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableType entity | BE-A.5-01 |
| SOFT | Money value object | Exists |
| SOFT | Member entity | BE-F.3-01 (future) |

### Acceptance Criteria
- [ ] Service interface created
- [ ] Implementation handles all pricing rules
- [ ] First-hour rate applies in first 60 minutes
- [ ] Rounding applies correctly
- [ ] Minimum charge enforced
- [ ] Member discount applies
- [ ] Registered in DI container
- [ ] Unit tests cover all scenarios
- [ ] Edge cases tested (0 time, >24 hours, etc.)

### Failure Modes to Guard Against
- Incorrect rounding calculation
- Missing first-hour rule application
- Negative charge returned
- Overflow with long sessions

---

## BE-A.16-01: Create PauseTableSessionCommand

**Ticket ID:** BE-A.16-01  
**Feature ID:** A.16  
**Type:** Backend  
**Title:** Create PauseTableSessionCommand  
**Priority:** P0

### Outcome (measurable, testable)
Commands to pause and resume table sessions with accurate time tracking.

### Scope
- Create `PauseTableSessionCommand`
- Create `ResumeTableSessionCommand`
- Create handlers for both
- Track paused duration accurately

### Explicitly Out of Scope
- UI components
- Auto-pause features

### Implementation Notes
```csharp
// Pause sets PausedAt = DateTime.UtcNow
// Resume calculates pause duration and adds to TotalPausedDuration
// GetBillableTime() = (EndTime ?? Now) - StartTime - TotalPausedDuration
```

### Quality & Guardrails
- **domain-model.md:** State transitions validated in entity
- **exception-handling-contract.md:** Return Result with error

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |

### Acceptance Criteria
- [ ] Pause command sets PausedAt
- [ ] Resume command calculates duration
- [ ] Cannot pause already-paused session
- [ ] Cannot resume non-paused session
- [ ] Billable time excludes paused duration
- [ ] Tests cover all transitions

### Failure Modes to Guard Against
- Double-pause
- Resume without pause
- Incorrect duration calculation

---

## BE-A.17-01: Create AdjustSessionTimeCommand

**Ticket ID:** BE-A.17-01  
**Feature ID:** A.17  
**Type:** Backend  
**Title:** Create AdjustSessionTimeCommand  
**Priority:** P0

### Outcome (measurable, testable)
A manager-only command to override session time for corrections.

### Scope
- Create `AdjustSessionTimeCommand`
- Require manager permission
- Log adjustment with reason
- Recalculate charges after adjustment

### Explicitly Out of Scope
- UI for adjustment
- Automatic adjustments

### Implementation Notes
```csharp
public record AdjustSessionTimeCommand(
    Guid SessionId,
    TimeSpan AdjustmentAmount, // Positive = add time, Negative = subtract
    string Reason
);

// Handler:
// 1. Verify manager permission
// 2. Apply adjustment to session
// 3. Log audit event
// 4. Recalculate charges if session ended
```

### Quality & Guardrails
- **security-policies.md:** Permission check required
- **domain-model.md:** Audit logging for sensitive actions

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |
| HARD | Permission system | Exists |

### Acceptance Criteria
- [ ] Only managers can execute
- [ ] Adjustment applied correctly
- [ ] Audit log created
- [ ] Charges recalculated
- [ ] Error if non-manager
- [ ] Tests pass

### Failure Modes to Guard Against
- Unauthorized adjustment
- Missing audit trail
- Incorrect charge recalculation

---

*Remaining tickets (BE-A.7-01 through BE-A.19-01) follow same format - lower priority, created as needed.*

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 9 | NOT_STARTED |
| P1 | 4 | NOT_STARTED |
| P2 | 7 | NOT_STARTED |
| **Total** | **20** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
