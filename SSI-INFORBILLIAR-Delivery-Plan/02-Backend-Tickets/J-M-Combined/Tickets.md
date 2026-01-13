# Backend Tickets: Categories J-M - Security, Localization, Operations, System

> [!NOTE]
> These categories have good parity (40-60%). Tickets focus on completing partial features.

---

## Category J - Security, Users & Staff

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-J.1-01 | J.1 | Create Manager Authorization Dialog Flow | P0 | NOT_STARTED |
| BE-J.7-01 | J.7 | Complete Server Assignment | P1 | NOT_STARTED |
| BE-J.9-01 | J.9 | Create Staff Clock-In/Out Tracking | P1 | NOT_STARTED |
| BE-J.10-01 | J.10 | Implement Break Tracking | P2 | NOT_STARTED |

### BE-J.1-01: Create Manager Authorization Dialog Flow

**Ticket ID:** BE-J.1-01  
**Feature ID:** J.1  
**Type:** Backend  
**Title:** Create Manager Authorization Dialog Flow  
**Priority:** P0

### Outcome (measurable, testable)
A secure manager PIN authorization system for privileged operations.

### Scope
- Create `AuthorizeManagerCommand`
- Validate manager-level permissions
- Return authorization token for operation
- Audit all authorization attempts

### Implementation Notes
```csharp
public record AuthorizeManagerCommand(
    string Pin,
    string OperationType  // "Void", "Discount", "Override", etc.
);

public record AuthorizationResult(
    bool Authorized,
    Guid? AuthorizingUserId,
    string AuthorizingUserName,
    DateTime? ExpiresAt,  // Short-lived for specific operation
    string FailureReason
);

// Handler:
// 1. Validate PIN
// 2. Check user has manager role
// 3. Log authorization attempt
// 4. Return result
```

### Acceptance Criteria
- [ ] PIN validation works
- [ ] Only managers authorize
- [ ] All attempts logged
- [ ] Authorization token time-limited
- [ ] Failed attempts tracked

---

### BE-J.9-01: Create Staff Clock-In/Out Tracking

**Ticket ID:** BE-J.9-01  
**Feature ID:** J.9  
**Type:** Backend  
**Title:** Create Staff Clock-In/Out Tracking  
**Priority:** P1

### Outcome (measurable, testable)
Staff time tracking with clock-in/out functionality.

### Scope
- Create `TimeEntry` entity
- Create clock-in/out commands
- Calculate shift duration
- Generate timesheet reports

### Implementation Notes
```csharp
public class TimeEntry
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ClockInTime { get; private set; }
    public DateTime? ClockOutTime { get; private set; }
    public TimeSpan? Duration => ClockOutTime.HasValue ? ClockOutTime.Value - ClockInTime : null;
    public TimeEntryStatus Status { get; private set; }
}

public record ClockInCommand(Guid UserId, string Pin);
public record ClockOutCommand(Guid UserId, string Pin);
```

### Acceptance Criteria
- [ ] Clock in records time
- [ ] Clock out calculates duration
- [ ] PIN required
- [ ] Timesheet query works
- [ ] Cannot clock in twice

---

## Category K - Localization

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-K.3-01 | K.3 | Complete Currency Formatting | P2 | NOT_STARTED |
| BE-K.4-01 | K.4 | Complete Date/Time Formatting | P2 | NOT_STARTED |

### BE-K.3-01: Complete Currency Formatting

**Ticket ID:** BE-K.3-01  
**Feature ID:** K.3  
**Type:** Backend  
**Title:** Complete Currency Formatting  
**Priority:** P2

### Scope
- Ensure Money uses locale-aware formatting
- Support configurable currency symbol placement
- Handle decimal separator differences

### Acceptance Criteria
- [ ] Currency formatted per locale
- [ ] Symbol position configurable
- [ ] Decimal separators correct

---

## Category L - Operations, Deployment & Config

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-L.3-01 | L.3 | Complete Database Backup System | P1 | NOT_STARTED |
| BE-L.4-01 | L.4 | Complete Database Restore | P1 | NOT_STARTED |
| BE-L.5-01 | L.5 | Implement Automatic Backups | P2 | NOT_STARTED |

### BE-L.3-01: Complete Database Backup System

**Ticket ID:** BE-L.3-01  
**Feature ID:** L.3  
**Type:** Backend  
**Title:** Complete Database Backup System  
**Priority:** P1

### Scope
- Create `BackupDatabaseCommand`
- Support scheduled backups
- Compress backup files
- Validate backup integrity

### Implementation Notes
```csharp
public interface IDatabaseBackupService
{
    Task<BackupResult> CreateBackup(string destinationPath);
    Task<bool> ValidateBackup(string backupPath);
    Task<RestoreResult> RestoreBackup(string backupPath);
}
```

### Acceptance Criteria
- [ ] Backup creates valid file
- [ ] Backup can be restored
- [ ] Compression works
- [ ] Error handling for disk space

---

## Category M - System Safety, Diagnostics & Recovery

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-M.3-01 | M.3 | Create Transaction Journal | P1 | NOT_STARTED |
| BE-M.4-01 | M.4 | Implement System Health Monitoring | P2 | IN_PROGRESS |
| BE-M.9-01 | M.9 | Implement Automatic Error Reporting | P2 | DONE |
| BE-M.10-01 | M.10 | Create System Health Dashboard Backend | P2 | IN_PROGRESS |

### BE-M.3-01: Create Transaction Journal

**Ticket ID:** BE-M.3-01  
**Feature ID:** M.3  
**Type:** Backend  
**Title:** Create Transaction Journal  
**Priority:** P1

### Scope
- Create comprehensive transaction log
- Log all financial operations
- Support replay for reconciliation
- Immutable records

### Implementation Notes
```csharp
public class TransactionJournal
{
    public Guid Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public JournalEntryType Type { get; private set; }
    public Guid? TicketId { get; private set; }
    public Guid? SessionId { get; private set; }
    public Money Amount { get; private set; }
    public string Details { get; private set; }  // JSON
    public string Checksum { get; private set; }  // Integrity
}
```

### Acceptance Criteria
- [ ] All transactions logged
- [ ] Checksum validates integrity
- [ ] Cannot modify entries
- [ ] Query by date range works

---

## BE-J.7-01: Complete Server Assignment

**Ticket ID:** BE-J.7-01  
**Feature ID:** J.7  
**Type:** Backend  
**Title:** Complete Server Assignment  
**Priority:** P1

### Outcome
Servers can be assigned to tables for service tracking.

### Scope
- Add server assignment to TableSession
- Track server performance
- Link to tip distribution
- Query tables by server

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession | BE-A.1-01 |

### Acceptance Criteria
- [ ] Server assigned to session
- [ ] Performance tracking works
- [ ] Tips linked correctly
- [ ] Server query functional

---

## BE-J.10-01: Implement Break Tracking

**Ticket ID:** BE-J.10-01  
**Feature ID:** J.10  
**Type:** Backend  
**Title:** Implement Break Tracking  
**Priority:** P2

### Outcome
Track employee breaks for labor compliance.

### Scope
- Create BreakEntry entity
- Clock in/out for breaks
- Calculate break duration
- Labor law compliance reports

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | TimeEntry | BE-J.9-01 |

### Acceptance Criteria
- [ ] Break tracking works
- [ ] Duration calculated
- [ ] Reports generated
- [ ] Compliance rules enforced

---

## BE-K.4-01: Complete Date/Time Formatting

**Ticket ID:** BE-K.4-01  
**Feature ID:** K.4  
**Type:** Backend  
**Title:** Complete Date/Time Formatting  
**Priority:** P2

### Outcome
Locale-aware date and time formatting.

### Scope
- Support multiple date formats
- Time zone handling
- 12/24 hour formats
- Regional preferences

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Restaurant config | Exists |

### Acceptance Criteria
- [ ] Date format configurable
- [ ] Time zones correct
- [ ] Regional formats supported
- [ ] Display consistent

---

## BE-L.4-01: Complete Database Restore

**Ticket ID:** BE-L.4-01  
**Feature ID:** L.4  
**Type:** Backend  
**Title:** Complete Database Restore  
**Priority:** P1

### Outcome
Restore database from backup files.

### Scope
- Create RestoreDatabaseCommand
- Validate backup files
- Stop services during restore
- Verify integrity post-restore

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Database backup | BE-L.3-01 |

### Acceptance Criteria
- [ ] Restore works correctly
- [ ] Data integrity verified
- [ ] Services restart properly
- [ ] Error handling robust

---

## BE-L.5-01: Implement Automatic Backups

**Ticket ID:** BE-L.5-01  
**Feature ID:** L.5  
**Type:** Backend  
**Title:** Implement Automatic Backups  
**Priority:** P2

### Outcome
Automatic scheduled database backups.

### Scope
- Create backup scheduler service
- Configurable backup schedule
- Retention policy
- Email notifications

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Database backup | BE-L.3-01 |

### Acceptance Criteria
- [ ] Scheduled backups run
- [ ] Retention policy works
- [ ] Notifications sent
- [ ] Error handling complete

---

## BE-M.6-01: Complete Health Check System

**Ticket ID:** BE-M.6-01  
**Feature ID:** M.6  
**Type:** Backend  
**Title:** Complete Health Check System  
**Priority:** P2

### Outcome
System health monitoring and diagnostics.

### Scope
- Create health check service
- Database connectivity check
- Peripheral device status
- Performance metrics

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | System services | Various |

### Acceptance Criteria
- [ ] Health checks run
- [ ] All systems monitored
- [ ] Alerts on failures
- [ ] Dashboard displays status

---

### BE-M.9-01: Implement Automatic Error Reporting

**Ticket ID:** BE-M.9-01  
**Feature ID:** M.9  
**Type:** Backend  
**Title:** Implement Automatic Error Reporting  
**Priority:** P2  
**Status:** DONE

### Outcome
Comprehensive error reporting system with management dashboard integration.

### Scope
- ✅ ErrorReportingService implementation
- ✅ Error categorization (Network, Hardware, Data, User, System, etc.)
- ✅ Error aggregation and deduplication
- ✅ Management dashboard integration
- ✅ Error statistics and trends

### Implementation Notes
```csharp
// Implemented in ErrorReportingService.cs
public class ErrorReportingService : IErrorReportingService
{
    // Error reporting with categorization
    // Deduplication logic for similar errors
    // Statistics and trend analysis
    // Management dashboard integration
}
```

### Acceptance Criteria
- [x] All errors automatically reported to management
- [x] Error categorization works correctly
- [x] Duplicate errors are aggregated
- [x] Error statistics available for analysis
- [x] Management dashboard displays error information

**Completed:** 2026-01-12

---

## Summary - All Categories

| Category | Tickets | Priority Distribution |
|----------|---------|----------------------|
| J | 4 | P0: 1, P1: 2, P2: 1 |
| K | 2 | P2: 2 |
| L | 3 | P1: 2, P2: 1 |
| M | 3 | P1: 1, P2: 2 |
| **Total** | **12** | **P0: 1, P1: 5, P2: 6** |

---

*Last Updated: 2026-01-12*
