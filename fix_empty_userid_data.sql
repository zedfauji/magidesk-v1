-- Fix Empty UserId Data
-- This script updates all records with empty GUIDs (00000000-0000-0000-0000-000000000000)
-- to use a default "System" user GUID

-- Step 1: Create a default "System" user if it doesn't exist
-- Using a well-known GUID: 00000000-0000-0000-0000-000000000001
DECLARE @SystemUserId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @EmptyGuid UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000';

-- Check if we need to create the system user
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @SystemUserId)
BEGIN
    PRINT 'Creating System user...';
    
    -- You'll need to adjust this based on your Users table structure
    -- This is a placeholder - adjust columns as needed
    INSERT INTO Users (Id, Username, FirstName, LastName, Pin, RoleId, IsActive, CreatedAt)
    VALUES (
        @SystemUserId,
        'SYSTEM',
        'System',
        'User',
        '', -- Empty PIN or encrypted default
        (SELECT TOP 1 Id FROM Roles WHERE Name = 'Administrator'), -- Adjust as needed
        1,
        GETUTCDATE()
    );
    
    PRINT 'System user created.';
END
ELSE
BEGIN
    PRINT 'System user already exists.';
END

-- Step 2: Update Tickets with empty CreatedBy
PRINT 'Updating Tickets with empty CreatedBy...';
UPDATE Tickets
SET CreatedBy = @SystemUserId
WHERE CreatedBy = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' Tickets.');

-- Step 3: Update Payments with empty ProcessedBy
PRINT 'Updating Payments with empty ProcessedBy...';
UPDATE Payments
SET ProcessedBy = @SystemUserId
WHERE ProcessedBy = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' Payments.');

-- Step 4: Update CashSessions with empty UserId
PRINT 'Updating CashSessions with empty UserId...';
UPDATE CashSessions
SET UserId = @SystemUserId
WHERE UserId = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' CashSessions.');

-- Step 5: Update CashDrops with empty ProcessedBy
PRINT 'Updating CashDrops with empty ProcessedBy...';
UPDATE CashDrops
SET ProcessedBy = @SystemUserId
WHERE ProcessedBy = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' CashDrops.');

-- Step 6: Update DrawerBleeds with empty ProcessedBy
PRINT 'Updating DrawerBleeds with empty ProcessedBy...';
UPDATE DrawerBleeds
SET ProcessedBy = @SystemUserId
WHERE ProcessedBy = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' DrawerBleeds.');

-- Step 7: Update Payouts with empty ProcessedBy
PRINT 'Updating Payouts with empty ProcessedBy...';
UPDATE Payouts
SET ProcessedBy = @SystemUserId
WHERE ProcessedBy = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' Payouts.');

-- Step 8: Update AttendanceHistory with empty UserId
PRINT 'Updating AttendanceHistory with empty UserId...';
UPDATE AttendanceHistory
SET UserId = @SystemUserId
WHERE UserId = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' AttendanceHistory records.');

-- Step 9: Update AuditEvents with empty UserId
PRINT 'Updating AuditEvents with empty UserId...';
UPDATE AuditEvents
SET UserId = @SystemUserId
WHERE UserId = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' AuditEvents.');

-- Step 10: Update SessionAudit with empty UserId
PRINT 'Updating SessionAudit with empty UserId...';
UPDATE SessionAudit
SET UserId = @SystemUserId
WHERE UserId = @EmptyGuid;
PRINT CONCAT('Updated ', @@ROWCOUNT, ' SessionAudit records.');

PRINT 'All empty UserId fields have been updated to System user.';
PRINT 'Script completed successfully.';
