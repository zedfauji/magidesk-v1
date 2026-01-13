# Design Document

## Overview

The Table Session Management system enables time-based billing for billiard club operations by tracking when customers start and end table sessions. The system integrates with the existing Clean Architecture POS system, leveraging the current domain model while adding new entities and services for session management.

This design builds upon the existing `Table`, `Ticket`, and `Payment` entities, adding `TableSession` and `TableType` entities to support time-based billing workflows. The system follows the established MVVM pattern for UI components and maintains strict separation between domain logic and presentation concerns.

## Architecture

The Table Session Management system follows the existing Clean Architecture pattern:

### Domain Layer
- **TableSession Entity**: Tracks session lifecycle, timing, and billing
- **TableType Entity**: Defines pricing rules and table categories  
- **PricingService**: Calculates time-based charges with complex pricing rules
- **SessionDomainService**: Orchestrates session operations and state transitions

### Application Layer
- **Commands**: StartTableSessionCommand, EndTableSessionCommand, PauseTableSessionCommand, etc.
- **Queries**: GetActiveSessionsQuery, GetSessionHistoryQuery, GetTableTypesQuery
- **DTOs**: TableSessionDto, TableTypeDto, ActiveSessionDto
- **Validators**: FluentValidation for all command inputs

### Infrastructure Layer
- **Repositories**: TableSessionRepository, TableTypeRepository (already implemented)
- **EF Core Configurations**: Entity mappings and database constraints
- **Migrations**: Database schema updates for new entities

### Presentation Layer
- **Dialogs**: StartSessionDialog, EndSessionDialog, TimeAdjustmentDialog
- **ViewModels**: Session management ViewModels following MVVM pattern
- **Controls**: Live timer displays, status indicators, session panels

## Components and Interfaces

### Core Entities

#### TableSession Entity (Already Implemented)
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
    public TimeSpan ManualAdjustment { get; private set; }
    
    // Domain methods for session lifecycle
    public static TableSession Start(Guid tableId, Guid tableTypeId, decimal hourlyRate, int guestCount, Guid? customerId = null, Guid? ticketId = null);
    public void Pause();
    public void Resume();
    public void End(Money calculatedCharge);
    public void AdjustTime(TimeSpan adjustment);
    public TimeSpan GetBillableTime();
    public void LinkToTicket(Guid ticketId);
    public void UpdateGuestCount(int guestCount);
}
```

#### TableType Entity (Already Implemented)
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
    
    // Domain methods for configuration
    public static TableType Create(string name, decimal hourlyRate, string description = "");
    public void UpdateRates(decimal hourlyRate, decimal? firstHourRate = null);
    public void SetRounding(int minimumMinutes, int roundingMinutes);
    public void UpdateDetails(string name, string description = "");
}
```

#### Enhanced Table Entity
The existing `Table` entity already includes the necessary methods for session management:
- `MarkInUse()` and `MarkAvailable()` for session lifecycle
- `SetTableType()` and `ClearTableType()` for pricing configuration
- Status management with proper invariant enforcement

### Domain Services

#### PricingService (To Be Implemented)
```csharp
public interface IPricingService
{
    Money CalculateTimeCharge(TimeSpan billableTime, TableType tableType, Member? member = null);
    Money CalculateRunningCharge(TableSession session, TableType tableType);
    TimeSpan ApplyRounding(TimeSpan duration, int roundingMinutes);
    TimeSpan ApplyMinimum(TimeSpan duration, int minimumMinutes);
}

public class PricingService : IPricingService
{
    public Money CalculateTimeCharge(TimeSpan billableTime, TableType tableType, Member? member = null)
    {
        // 1. Apply minimum duration if configured
        var adjustedTime = ApplyMinimum(billableTime, tableType.MinimumMinutes);
        
        // 2. Apply rounding rules
        var roundedTime = ApplyRounding(adjustedTime, tableType.RoundingMinutes);
        
        // 3. Calculate first hour charge if configured
        var firstHourCharge = Money.Zero();
        var remainingTime = roundedTime;
        
        if (tableType.FirstHourRate.HasValue && roundedTime > TimeSpan.Zero)
        {
            var firstHourTime = TimeSpan.FromHours(1);
            if (roundedTime >= firstHourTime)
            {
                firstHourCharge = new Money(tableType.FirstHourRate.Value);
                remainingTime = roundedTime - firstHourTime;
            }
            else
            {
                // Partial first hour - prorate the first hour rate
                var fraction = roundedTime.TotalHours;
                firstHourCharge = new Money(tableType.FirstHourRate.Value * (decimal)fraction);
                remainingTime = TimeSpan.Zero;
            }
        }
        
        // 4. Calculate remaining hours at standard rate
        var remainingCharge = Money.Zero();
        if (remainingTime > TimeSpan.Zero)
        {
            var remainingHours = (decimal)remainingTime.TotalHours;
            remainingCharge = new Money(tableType.HourlyRate * remainingHours);
        }
        
        // 5. Apply member discount if applicable
        var totalCharge = firstHourCharge + remainingCharge;
        if (member != null)
        {
            // Apply member discount logic (to be implemented with member system)
            // totalCharge = ApplyMemberDiscount(totalCharge, member);
        }
        
        return totalCharge;
    }
}
```

### Application Layer Commands

#### StartTableSessionCommand
```csharp
public record StartTableSessionCommand(
    Guid TableId,
    Guid? CustomerId,
    int GuestCount
) : ICommand<StartTableSessionResult>;

public class StartTableSessionCommandHandler : ICommandHandler<StartTableSessionCommand, StartTableSessionResult>
{
    public async Task<Result<StartTableSessionResult>> Handle(StartTableSessionCommand command, CancellationToken cancellationToken)
    {
        // 1. Validate table exists and is available
        // 2. Get table type and pricing information
        // 3. Create TableSession entity
        // 4. Mark table as in-use
        // 5. Save changes and return session info
    }
}
```

#### EndTableSessionCommand
```csharp
public record EndTableSessionCommand(
    Guid SessionId,
    bool CreateNewTicket = true,
    Guid? ExistingTicketId = null
) : ICommand<EndTableSessionResult>;

public class EndTableSessionCommandHandler : ICommandHandler<EndTableSessionCommand, EndTableSessionResult>
{
    public async Task<Result<EndTableSessionResult>> Handle(EndTableSessionCommand command, CancellationToken cancellationToken)
    {
        // 1. Get session and validate it can be ended
        // 2. Calculate final charge using PricingService
        // 3. End session with calculated charge
        // 4. Create or update ticket with time charge line item
        // 5. Mark table as available
        // 6. Return ticket information for payment processing
    }
}
```

## Data Models

### Database Schema Extensions

The system extends the existing database schema with new tables:

#### TableSessions Table
```sql
CREATE TABLE magidesk.TableSessions (
    Id UUID PRIMARY KEY,
    TableId UUID NOT NULL REFERENCES magidesk.Tables(Id),
    CustomerId UUID NULL REFERENCES magidesk.Customers(Id),
    TicketId UUID NULL REFERENCES magidesk.Tickets(Id),
    StartTime TIMESTAMP WITH TIME ZONE NOT NULL,
    EndTime TIMESTAMP WITH TIME ZONE NULL,
    PausedAt TIMESTAMP WITH TIME ZONE NULL,
    TotalPausedDuration INTERVAL NOT NULL DEFAULT '0',
    Status INTEGER NOT NULL, -- TableSessionStatus enum
    TableTypeId UUID NOT NULL REFERENCES magidesk.TableTypes(Id),
    HourlyRate DECIMAL(18,2) NOT NULL,
    TotalCharge_Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    TotalCharge_Currency VARCHAR(3) NOT NULL DEFAULT 'USD',
    GuestCount INTEGER NOT NULL CHECK (GuestCount >= 1 AND GuestCount <= 20),
    ManualAdjustment INTERVAL NOT NULL DEFAULT '0',
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);
```

#### TableTypes Table
```sql
CREATE TABLE magidesk.TableTypes (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL DEFAULT '',
    HourlyRate DECIMAL(18,2) NOT NULL CHECK (HourlyRate > 0),
    FirstHourRate DECIMAL(18,2) NULL CHECK (FirstHourRate > 0),
    MinimumMinutes INTEGER NOT NULL DEFAULT 0 CHECK (MinimumMinutes >= 0),
    RoundingMinutes INTEGER NOT NULL DEFAULT 1 CHECK (RoundingMinutes >= 1),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);
```

### DTOs for Application Layer

#### TableSessionDto
```csharp
public class TableSessionDto
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? TicketId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TableSessionStatus Status { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public TimeSpan BillableTime { get; set; }
    public Money RunningCharge { get; set; } = Money.Zero();
    public Money TotalCharge { get; set; } = Money.Zero();
    public int GuestCount { get; set; }
    public bool IsPaused { get; set; }
    public DateTime? PausedAt { get; set; }
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Session Lifecycle State Transitions
*For any* table session, state transitions must follow the valid sequence: Active → Paused → Active → Ended, where sessions can only be ended from Active state and cannot transition backwards from Ended state.
**Validates: Requirements 1.3, 1.4, 1.6**

### Property 2: Table Status Synchronization
*For any* table session operation (start/pause/resume/end), the associated table's status must be updated to reflect the current session state (Available/In-Use/Paused).
**Validates: Requirements 5.2, 5.3, 5.4**

### Property 3: Billable Time Calculation Accuracy
*For any* table session with pause periods, the billable time must equal total elapsed time minus all paused durations plus any manual adjustments, and must never be negative.
**Validates: Requirements 1.4, 4.5, 7.4**

### Property 4: Pricing Rule Application
*For any* table session and table type, the calculated charge must correctly apply first-hour pricing, time rounding, and minimum charge rules according to the table type configuration.
**Validates: Requirements 4.1, 4.2, 4.3, 4.4**

### Property 5: Session Start Validation
*For any* table, a new session can only be started if the table status is Available, and starting a session must create a valid TableSession entity with correct initial state.
**Validates: Requirements 1.1, 1.2, 5.5**

### Property 6: Time Charge Line Item Creation
*For any* ended session, creating a time charge line item must include accurate duration, hourly rate, and total charge information that matches the session's calculated values.
**Validates: Requirements 4.6, 9.3, 9.4**

### Property 7: Guest Count Validation
*For any* session operation involving guest count, the value must be between 1 and 20 inclusive, and updates must only be allowed on non-ended sessions.
**Validates: Requirements 6.2, 6.4**

### Property 8: Manager Override Audit Trail
*For any* time adjustment operation, if the user has manager permissions, the adjustment must be applied and logged with complete audit information; if not, the operation must be rejected.
**Validates: Requirements 7.1, 7.2, 7.3, 7.5**

### Property 9: Session Transfer Continuity
*For any* session transfer between tables, the billable time must be preserved exactly, and both source and destination table statuses must be updated correctly.
**Validates: Requirements 8.1, 8.2, 8.3**

### Property 10: Ticket Integration Consistency
*For any* session ending, if creating a new ticket, it must contain the time charge; if adding to existing ticket, the time charge must be appended without affecting existing line items.
**Validates: Requirements 9.1, 9.2, 9.5**

### Property 11: Referential Integrity Preservation
*For any* session operation, all references between sessions, tables, and tickets must remain valid, and no orphaned sessions can exist without valid table assignments.
**Validates: Requirements 10.4, 10.5**

### Property 12: State Transition Invariant Enforcement
*For any* attempted invalid state transition (e.g., ending a paused session, pausing an ended session), the system must reject the operation and maintain the current valid state.
**Validates: Requirements 10.2**

## Error Handling

### Domain Exceptions
- **BusinessRuleViolationException**: Thrown for invariant violations (negative rates, invalid guest counts)
- **InvalidOperationException**: Thrown for invalid state transitions (ending paused session, starting on occupied table)
- **ArgumentException**: Thrown for invalid method parameters (empty GUIDs, null values)

### Application Layer Error Handling
- **Result Pattern**: All command handlers return `Result<T>` with success/failure indication
- **Validation Errors**: FluentValidation provides detailed error messages for UI display
- **Concurrency Handling**: Optimistic concurrency conflicts handled gracefully with retry logic

### UI Error Display
- **Dialog Error Messages**: Clear, user-friendly error messages in session dialogs
- **Status Indicators**: Visual feedback for error states (red borders, warning icons)
- **Toast Notifications**: Non-blocking notifications for background operations

## Testing Strategy

### Unit Testing Approach
The testing strategy follows a dual approach combining unit tests for specific scenarios and property-based tests for comprehensive coverage:

**Unit Tests Focus:**
- Specific examples demonstrating correct behavior
- Edge cases and boundary conditions
- Error scenarios and exception handling
- Integration points between components

**Property-Based Tests Focus:**
- Universal properties that hold for all valid inputs
- Comprehensive input coverage through randomization
- State transition validation across all possible sequences
- Invariant enforcement under all conditions

### Property-Based Testing Configuration
- **Framework**: Use appropriate PBT library for C# (FsCheck or similar)
- **Iterations**: Minimum 100 iterations per property test
- **Test Tagging**: Each property test tagged with format: **Feature: table-session-management, Property {number}: {property_text}**
- **Coverage Requirements**: Domain layer ≥90%, Application layer ≥80%

### Test Organization
```
Tests/
├── Domain.Tests/
│   ├── Entities/
│   │   ├── TableSessionTests.cs (unit tests)
│   │   ├── TableSessionPropertyTests.cs (property tests)
│   │   └── TableTypeTests.cs
│   └── Services/
│       └── PricingServiceTests.cs
├── Application.Tests/
│   ├── Commands/
│   │   ├── StartTableSessionCommandTests.cs
│   │   └── EndTableSessionCommandTests.cs
│   └── Queries/
│       └── GetActiveSessionsQueryTests.cs
└── Integration.Tests/
    ├── SessionWorkflowTests.cs
    └── DatabaseIntegrationTests.cs
```

### Example Property Test Structure
```csharp
[Property]
public Property SessionLifecycleStateTransitions()
{
    return Prop.ForAll(
        GenerateValidTableSession(),
        session =>
        {
            // Test valid state transitions
            session.Pause();
            Assert.Equal(TableSessionStatus.Paused, session.Status);
            
            session.Resume();
            Assert.Equal(TableSessionStatus.Active, session.Status);
            
            session.End(Money.Zero());
            Assert.Equal(TableSessionStatus.Ended, session.Status);
            
            // Test invalid transitions throw exceptions
            Assert.Throws<InvalidOperationException>(() => session.Pause());
        })
        .Label("Feature: table-session-management, Property 1: Session Lifecycle State Transitions");
}
```