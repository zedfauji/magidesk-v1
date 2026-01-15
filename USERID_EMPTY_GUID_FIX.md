# UserId Empty Guid Fix

## Problem

**Error:** `UserId cannot be empty Guid` thrown when loading entities from database.

**Root Cause:**
- The `UserId` value object has validation that rejects `Guid.Empty` (00000000-0000-0000-0000-000000000000)
- Database contains records with empty GUIDs for required `UserId` fields
- When EF Core materializes entities, it tries to create `UserId` value objects with empty GUIDs
- The `UserId` constructor throws `ArgumentException`

**Affected Entities:**
- `Ticket.CreatedBy` (required)
- `Payment.ProcessedBy` (required)
- `CashSession.UserId` (required)
- `CashDrop.ProcessedBy` (required)
- `DrawerBleed.ProcessedBy` (required)
- `Payout.ProcessedBy` (required)
- `AttendanceHistory.UserId` (required)
- `AuditEvent.UserId` (required)
- `SessionAudit.UserId` (required)

## Solution

### Option 1: Fix Database Data (Recommended)

Run the SQL script to update all empty GUIDs to a default "System" user:

```bash
# Execute the fix script
sqlcmd -S localhost -d MagideskDB -i fix_empty_userid_data.sql
```

**What it does:**
1. Creates a "System" user with GUID `00000000-0000-0000-0000-000000000001`
2. Updates all records with empty `UserId` fields to use the System user GUID
3. Preserves data integrity while fixing the validation issue

### Option 2: Temporary Workaround (Quick Fix)

If you need to run the app immediately before fixing the database:

**Modify `UserId.cs` temporarily:**

```csharp
public UserId(Guid value)
{
    // TEMPORARY: Allow empty GUID for legacy data
    // TODO: Remove this after running fix_empty_userid_data.sql
    if (value == Guid.Empty)
    {
        // Use a well-known "System" user GUID
        value = new Guid("00000000-0000-0000-0000-000000000001");
    }

    Value = value;
}
```

**⚠️ Warning:** This is a temporary workaround. You should fix the database data properly.

### Option 3: Make CreatedBy Nullable (Not Recommended)

This would require changing the domain model and is not recommended because:
- Every ticket should have a creator
- It weakens the domain invariants
- It doesn't fix the underlying data quality issue

## Verification

After applying the fix, verify:

```sql
-- Check for any remaining empty GUIDs
SELECT 'Tickets' AS TableName, COUNT(*) AS EmptyCount
FROM Tickets
WHERE CreatedBy = '00000000-0000-0000-0000-000000000000'

UNION ALL

SELECT 'Payments', COUNT(*)
FROM Payments
WHERE ProcessedBy = '00000000-0000-0000-0000-000000000000'

UNION ALL

SELECT 'CashSessions', COUNT(*)
FROM CashSessions
WHERE UserId = '00000000-0000-0000-0000-000000000000'

-- Add more checks for other tables...
```

Expected result: All counts should be 0.

## Prevention

To prevent this issue in the future:

1. **Database Constraints:**
   ```sql
   -- Add CHECK constraints to prevent empty GUIDs
   ALTER TABLE Tickets
   ADD CONSTRAINT CK_Tickets_CreatedBy_NotEmpty
   CHECK (CreatedBy != '00000000-0000-0000-0000-000000000000');
   ```

2. **Application Layer:**
   - Always set `CreatedBy` when creating tickets
   - Use the current user from `IUserService`
   - Fallback to System user if no user context available

3. **Seed Data:**
   - Ensure System user exists in all environments
   - Use consistent GUID: `00000000-0000-0000-0000-000000000001`

## Testing

After applying the fix:

1. **Run the application**
   ```bash
   dotnet run --project Magidesk.Presentation
   ```

2. **Test table click flow**
   - Click on a table in TableMapPage
   - Verify no `UserId` exceptions
   - Verify ticket creation works

3. **Test discount application**
   - Open SettlePage
   - Apply a discount
   - Verify no concurrency or UserId exceptions

## Related Issues

- **Concurrency Fix:** This fix works alongside the concurrency exception fix in `TASK_2_1_15_CONCURRENCY_FIX.md`
- **Table Click Confirmation:** Enables the new table click confirmation flow in `TABLE_CLICK_CONFIRMATION_IMPLEMENTATION.md`

## Files

- **SQL Fix Script:** `fix_empty_userid_data.sql`
- **Value Object:** `Magidesk.Domain/ValueObjects/UserId.cs`
- **Entity Configurations:** `Magidesk.Infrastructure/Data/Configurations/*Configuration.cs`

## Timeline

- **SQL Script Execution:** 1-2 minutes
- **Verification:** 5 minutes
- **Testing:** 10 minutes
- **Total:** ~15-20 minutes

## Status

- [ ] Run SQL fix script
- [ ] Verify no empty GUIDs remain
- [ ] Test application startup
- [ ] Test table click flow
- [ ] Test discount application
- [ ] Add database constraints (optional)
- [ ] Document System user in deployment guide

---

**Created:** January 15, 2026  
**Priority:** HIGH (Blocking application startup)  
**Estimated Fix Time:** 15-20 minutes
