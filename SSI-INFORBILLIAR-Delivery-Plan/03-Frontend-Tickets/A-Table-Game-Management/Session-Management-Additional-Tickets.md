# Session Management - Additional Tickets

## Backend Tickets

### BE-A.15-02: Fix Session Validation Logic
**Status**: TODO  
**Priority**: Critical  
**Effort**: 1 hour  
**Assignee**: TBD  
**Epic**: A - Table & Game Management  

**Description**:
Fix the validation logic in `StartTableSessionCommandHandler` to allow sessions to be started on tables with `Seat` status (tables that have tickets but no active sessions).

**Current Behavior**:
- Validation only allows sessions on tables with `Available` status
- Clicking a table creates a ticket and changes status to `Seat`
- Session can never be started due to circular dependency

**Expected Behavior**:
- Allow sessions on tables with `Seat` status
- Check for existing active session instead of table status
- Only block if table already has an active session

**Acceptance Criteria**:
- [ ] Can start session on table with `Seat` status
- [ ] Cannot start session if active session already exists
- [ ] Cannot start session on tables with other statuses (Dirty, Reserved, etc.)
- [ ] Error messages are clear and specific

**Technical Details**:
```csharp
// Current (Line 42-45)
if (table.Status != TableStatus.Available)
{
    throw new InvalidOperationException($"Table {table.TableNumber} is not available. Current status: {table.Status}");
}

// Proposed
if (table.Status != TableStatus.Available && table.Status != TableStatus.Seat)
{
    throw new InvalidOperationException($"Table {table.TableNumber} is not available for sessions. Current status: {table.Status}");
}

// Check for existing session (already exists at line 48-52)
var existingSession = await _sessionRepository.GetActiveSessionByTableIdAsync(command.TableId);
if (existingSession != null)
{
    throw new InvalidOperationException($"Table {table.TableNumber} already has an active session.");
}
```

**Files to Modify**:
- `Magidesk.Application/Commands/TableSessions/StartTableSessionCommandHandler.cs`

**Testing**:
1. Click available table → Creates ticket
2. Click "Start Session" → Should succeed
3. Try to start another session → Should fail with "already has an active session"

---

### BE-A.15-03: Add Session State to TicketDto
**Status**: TODO  
**Priority**: High  
**Effort**: 2 hours  
**Assignee**: TBD  
**Epic**: A - Table & Game Management  

**Description**:
Add session-related properties to `TicketDto` to enable proper session state tracking and UI state management.

**Properties to Add**:
```csharp
public Guid? SessionId { get; set; }
public DateTime? SessionStartTime { get; set; }
public decimal? SessionHourlyRate { get; set; }
public TimeSpan? SessionElapsedTime { get; set; }
public decimal? SessionRunningCharge { get; set; }
public bool HasActiveSession => SessionId.HasValue;
```

**Acceptance Criteria**:
- [ ] TicketDto includes all session properties
- [ ] Properties are nullable (backward compatible)
- [ ] `HasActiveSession` computed property works correctly
- [ ] GetTicketQueryHandler populates session data
- [ ] Session data updates when session starts/ends

**Files to Modify**:
- `Magidesk.Application/DTOs/TicketDto.cs`
- `Magidesk.Application/Queries/GetTicketQueryHandler.cs`

**Dependencies**:
- Requires session-ticket linking in database (BE-A.15-04)

---

### BE-A.15-04: Link Sessions to Tickets in Database
**Status**: TODO  
**Priority**: High  
**Effort**: 3 hours  
**Assignee**: TBD  
**Epic**: A - Table & Game Management  

**Description**:
Update database schema and domain model to link table sessions to tickets, enabling proper session tracking.

**Schema Changes**:
```sql
-- Add TicketId to TableSessions
ALTER TABLE TableSessions
ADD TicketId uniqueidentifier NULL
    CONSTRAINT FK_TableSessions_Tickets FOREIGN KEY REFERENCES Tickets(Id);

-- Add index for performance
CREATE INDEX IX_TableSessions_TicketId ON TableSessions(TicketId);
```

**Domain Model Changes**:
- Add `TicketId` property to `TableSession` entity
- Update `TableSession.Start()` to accept optional `TicketId`
- Update `StartTableSessionCommandHandler` to link session to ticket

**Acceptance Criteria**:
- [ ] Database migration created and tested
- [ ] TableSession entity has TicketId property
- [ ] Sessions are linked to tickets when created
- [ ] Queries can retrieve session by ticket ID
- [ ] Existing sessions (without tickets) still work

**Files to Modify**:
- `Magidesk.Domain/Entities/TableSession.cs`
- `Magidesk.Infrastructure/Data/Migrations/` (new migration)
- `Magidesk.Application/Commands/TableSessions/StartTableSessionCommandHandler.cs`

---

## Frontend Tickets

### FE-A.15-01: Implement Session Button State Management
**Status**: TODO  
**Priority**: High  
**Effort**: 2 hours  
**Assignee**: TBD  
**Epic**: A - Table & Game Management  

**Description**:
Add properties to `OrderEntryViewModel` to control the enabled/disabled state of Start/End Session buttons based on actual session state.

**Properties to Add**:
```csharp
public bool HasActiveSession => Ticket?.HasActiveSession ?? false;
public bool CanStartSession => Ticket != null && !HasActiveSession;
public bool CanEndSession => Ticket != null && HasActiveSession;
```

**Acceptance Criteria**:
- [ ] Properties update when ticket loads
- [ ] Properties update after session start/end
- [ ] "Start Session" button disabled when session exists
- [ ] "End Session" button disabled when no session exists
- [ ] Buttons re-enable after session state changes

**Files to Modify**:
- `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`
- `Magidesk.Presentation/Views/OrderEntryPage.xaml`

**XAML Changes**:
```xml
<Button Content="Start Session" 
        Command="{Binding StartSessionCommand}"
        IsEnabled="{Binding CanStartSession}"/>

<Button Content="End Session" 
        Command="{Binding EndSessionCommand}"
        IsEnabled="{Binding CanEndSession}"/>
```

**Dependencies**:
- Requires BE-A.15-03 (session state in TicketDto)

---

### FE-A.15-02: Add Session Visual Indicators
**Status**: TODO  
**Priority**: Medium  
**Effort**: 1 hour  
**Assignee**: TBD  
**Epic**: A - Table & Game Management  

**Description**:
Add visual indicators on the Order Entry page to show active session state, including timer and running charges.

**UI Elements to Add**:
- Session status badge (Active/No Session)
- Session timer (HH:MM:SS)
- Running charge display ($X.XX)
- Session start time

**Acceptance Criteria**:
- [ ] Session badge visible when session active
- [ ] Timer updates every second
- [ ] Running charge calculates correctly
- [ ] UI updates when session starts/ends
- [ ] Design matches existing UI patterns

**Files to Modify**:
- `Magidesk.Presentation/Views/OrderEntryPage.xaml`
- `Magidesk.Presentation/ViewModels/OrderEntryViewModel.cs`

**Dependencies**:
- Requires BE-A.15-03 (session data in TicketDto)

---

### FE-A.15-03: Update Table Map Session Indicators
**Status**: TODO  
**Priority**: Medium  
**Effort**: 1 hour  
**Assignee**: TBD  
**Epic**: A - Table & Game Management  

**Description**:
Ensure table map visual indicators (Feature A.4) correctly show session state for tables accessed via Order Entry.

**Acceptance Criteria**:
- [ ] Tables with active sessions show timer icon
- [ ] Timer displays correctly
- [ ] Running charges display
- [ ] Visual indicators update after session start/end from Order Entry
- [ ] Real-time updates work (if enabled)

**Files to Modify**:
- `Magidesk.Presentation/ViewModels/TableMapViewModel.cs`
- May need to refresh table map after session changes

**Dependencies**:
- Feature A.4 (already implemented)
- BE-A.15-03 (session data in TicketDto)

---

## Summary

**Total Tickets**: 6 (3 Backend, 3 Frontend)

**Critical Path**:
1. BE-A.15-02 (Quick fix - unblocks testing)
2. BE-A.15-04 (Database changes)
3. BE-A.15-03 (DTO changes)
4. FE-A.15-01 (Button state management)
5. FE-A.15-02 (Visual indicators)
6. FE-A.15-03 (Table map updates)

**Estimated Total Effort**: 10 hours

**Priority Order**:
1. **Critical**: BE-A.15-02 (1 hour) - Unblocks testing
2. **High**: BE-A.15-04 (3 hours) - Database foundation
3. **High**: BE-A.15-03 (2 hours) - DTO updates
4. **High**: FE-A.15-01 (2 hours) - Button state
5. **Medium**: FE-A.15-02 (1 hour) - Visual feedback
6. **Medium**: FE-A.15-03 (1 hour) - Table map sync
