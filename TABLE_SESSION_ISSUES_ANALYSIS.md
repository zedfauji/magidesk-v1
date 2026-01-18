# Table Session Issues Analysis

**Date:** 2026-01-18  
**Status:** 🔴 CRITICAL - Table Sessions Feature Not Fully Implemented

---

## ISSUES IDENTIFIED

### Issue 1: Table 13 - "Session has already ended" Error

**Error Message:**
```
System.InvalidOperationException: Session 8da9a610-64c9-40a3-a157-d44f7f54fab4 has already ended.
at EndTableSessionCommandHandler.HandleAsync() line 48
```

**Database State:**
- Ticket #1672 on Table 13
- Status: 1 (Open)
- SessionId: `8da9a610-64c9-40a3-a157-d44f7f54fab4`
- Created: 2026-01-14 09:15:33
- Opened: 2026-01-14 09:54:09

**Root Cause:**
The `TableSessions` table **does not exist** in the database. The application code references `ITableSessionRepository` and tries to query `TableSessions`, but the table was never created via migrations.

**Evidence:**
```sql
SELECT tablename FROM pg_tables WHERE schemaname = 'public' AND tablename ILIKE '%session%';
-- Result: Only 'CashSessions' exists, no 'TableSessions'
```

### Issue 2: Table 3 - Shows Running Session But No Content

**Database State:**
- Ticket #1696 on Table 3
- Status: 0 (Draft)
- SessionId: NULL
- Created: 2026-01-15 16:37:43
- OpenedAt: NULL

**Root Cause:**
- Ticket is in Draft status (never opened)
- No SessionId assigned
- UI shows "running session" but there's no actual session data
- Likely a UI state synchronization issue

### Issue 3: Table 2 - Click Does Nothing

**Database State:**
- Ticket #1001 on Table 2
- Status: 2 (Held)
- SessionId: NULL
- Created: 2026-01-02 15:59:15
- Opened: 2026-01-02 15:59:15

**Root Cause:**
- Ticket is in Held status
- No SessionId assigned
- UI may not be handling Held status correctly
- Click handler might be checking for session existence

---

## ROOT CAUSE ANALYSIS

### Missing Database Table

The `TableSessions` table is **completely missing** from the database schema. This is a critical infrastructure gap.

**Expected Schema (based on code):**
```csharp
public class TableSession
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public TableSessionStatus Status { get; set; }  // Active, Paused, Ended
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? PausedAt { get; set; }
    public DateTime? ResumedAt { get; set; }
    public int GuestCount { get; set; }
    public Money HourlyRate { get; set; }
    public Money TotalCharge { get; set; }
    public Guid? TicketId { get; set; }
    public Guid? CustomerId { get; set; }
    // ... other properties
}
```

**Current Database State:**
- ❌ No `TableSessions` table
- ✅ `Tickets` table has `SessionId` column (UUID, nullable)
- ❌ No foreign key constraint between `Tickets.SessionId` and `TableSessions.Id`
- ❌ No `Tables` table found either

### Application Code vs Database Mismatch

**Code References:**
1. `EndTableSessionCommandHandler` (line 38): `await _sessionRepository.GetByIdAsync(command.SessionId)`
2. `ITableSessionRepository` interface exists
3. Domain entity `TableSession` exists
4. But no EF Core configuration or migration created the table

**This indicates:**
- Feature was designed and coded
- Domain models were created
- Application handlers were implemented
- **BUT** database migrations were never run or never created

---

## IMPACT ASSESSMENT

### Severity: 🔴 CRITICAL

**Affected Features:**
1. ❌ Table session management (start/pause/resume/end)
2. ❌ Time-based billing for table games
3. ❌ Session-to-ticket linking
4. ❌ Table status management
5. ❌ Customer session tracking

**User Impact:**
- Cannot end table sessions
- Cannot calculate time-based charges
- Tables show incorrect status
- Clicking on tables may not work
- Session data is lost (only SessionId stored in Tickets, but no actual session record)

---

## RECOMMENDED FIXES

### Fix 1: Create TableSessions Table Migration (CRITICAL)

**Priority:** P0 - Immediate  
**Effort:** Medium

**Action Required:**
1. Create EF Core migration for `TableSessions` table
2. Include all required columns based on domain model
3. Add foreign key constraints
4. Add indexes for performance
5. Run migration against database

**Migration Script Needed:**
```sql
CREATE TABLE "TableSessions" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "TableId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "StartTime" timestamp with time zone NOT NULL,
    "EndTime" timestamp with time zone NULL,
    "PausedAt" timestamp with time zone NULL,
    "ResumedAt" timestamp with time zone NULL,
    "GuestCount" integer NOT NULL DEFAULT 1,
    "HourlyRate" numeric(18,2) NOT NULL,
    "HourlyRateCurrency" varchar(3) NOT NULL DEFAULT 'USD',
    "TotalCharge" numeric(18,2) NOT NULL DEFAULT 0,
    "TotalChargeCurrency" varchar(3) NOT NULL DEFAULT 'USD',
    "TicketId" uuid NULL,
    "CustomerId" uuid NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

CREATE INDEX "IX_TableSessions_TableId" ON "TableSessions" ("TableId");
CREATE INDEX "IX_TableSessions_Status" ON "TableSessions" ("Status");
CREATE INDEX "IX_TableSessions_TicketId" ON "TableSessions" ("TicketId");
CREATE INDEX "IX_TableSessions_StartTime" ON "TableSessions" ("StartTime");

-- Add foreign key from Tickets to TableSessions
ALTER TABLE "Tickets" 
ADD CONSTRAINT "FK_Tickets_TableSessions_SessionId" 
FOREIGN KEY ("SessionId") REFERENCES "TableSessions"("Id") 
ON DELETE SET NULL;
```

### Fix 2: Create Tables Table Migration (CRITICAL)

**Priority:** P0 - Immediate  
**Effort:** Medium

The `Tables` table is also missing. This is required for table management.

**Migration Script Needed:**
```sql
CREATE TABLE "Tables" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "TableNumber" integer NOT NULL UNIQUE,
    "Name" varchar(100) NULL,
    "Status" integer NOT NULL DEFAULT 0,  -- Available, Occupied, Reserved, etc.
    "CurrentSessionId" uuid NULL,
    "CurrentTicketId" uuid NULL,
    "Capacity" integer NOT NULL DEFAULT 4,
    "FloorId" uuid NULL,
    "PositionX" double precision NOT NULL DEFAULT 0,
    "PositionY" double precision NOT NULL DEFAULT 0,
    "Width" double precision NOT NULL DEFAULT 100,
    "Height" double precision NOT NULL DEFAULT 100,
    "Shape" integer NOT NULL DEFAULT 0,  -- Rectangle, Circle, etc.
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

CREATE INDEX "IX_Tables_TableNumber" ON "Tables" ("TableNumber");
CREATE INDEX "IX_Tables_Status" ON "Tables" ("Status");
CREATE INDEX "IX_Tables_CurrentSessionId" ON "Tables" ("CurrentSessionId");
CREATE INDEX "IX_Tables_CurrentTicketId" ON "Tables" ("CurrentTicketId");
```

### Fix 3: Data Migration for Existing Tickets

**Priority:** P1 - High  
**Effort:** Low

**Action Required:**
1. Identify all tickets with non-null `SessionId`
2. Create corresponding `TableSession` records
3. Set appropriate status based on ticket status
4. Backfill missing data with reasonable defaults

**Query to identify affected tickets:**
```sql
SELECT "Id", "TicketNumber", "SessionId", "Status", "TableNumbers", "CreatedAt", "OpenedAt"
FROM "Tickets"
WHERE "SessionId" IS NOT NULL;
```

### Fix 4: UI State Synchronization

**Priority:** P2 - Medium  
**Effort:** Low

**Action Required:**
1. Fix UI to handle missing session data gracefully
2. Add null checks before accessing session properties
3. Show appropriate messages when session data is missing
4. Handle Draft and Held ticket statuses correctly

---

## IMMEDIATE ACTIONS

1. **Stop trying to end sessions** until TableSessions table is created
2. **Create database migrations** for TableSessions and Tables tables
3. **Run migrations** against the database
4. **Backfill session data** for existing tickets with SessionId
5. **Test session lifecycle** (start → pause → resume → end)
6. **Fix UI** to handle edge cases

---

## RELATED FILES

**Application Code:**
- `Magidesk.Application/Commands/TableSessions/EndTableSessionCommandHandler.cs` (line 48 - error location)
- `Magidesk.Application/Interfaces/ITableSessionRepository.cs`
- `Magidesk.Domain/Entities/TableSession.cs`
- `Magidesk.Domain/Entities/Table.cs`

**Database:**
- Missing: `Magidesk.Infrastructure/Data/Configurations/TableSessionConfiguration.cs`
- Missing: `Magidesk.Infrastructure/Data/Configurations/TableConfiguration.cs`
- Missing: Migration file for TableSessions
- Missing: Migration file for Tables

**UI:**
- `Magidesk.Presentation/ViewModels/Dialogs/EndSessionDialogViewModel.cs` (line 133 - error caught)
- Table click handlers (need investigation)

---

## CONCLUSION

The table sessions feature is **partially implemented**:
- ✅ Domain models exist
- ✅ Application handlers exist
- ✅ UI components exist
- ❌ **Database tables DO NOT exist**
- ❌ **EF Core configurations missing**
- ❌ **Migrations never created**

This is a **critical infrastructure gap** that must be resolved before the table sessions feature can function.

**Next Steps:**
1. Create EF Core entity configurations
2. Generate and run migrations
3. Backfill existing data
4. Test end-to-end workflow
5. Fix UI edge cases

**Estimated Effort:** 4-6 hours
**Risk:** High (data loss if not handled carefully)
**Priority:** P0 - Block all table session features until resolved
