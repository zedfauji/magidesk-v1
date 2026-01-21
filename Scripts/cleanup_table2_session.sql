-- Cleanup Table 2 Session
-- Date: 2026-01-18
-- Purpose: End orphaned session on Table 2 that has no ticket

-- This session has been running since 2026-01-15 (3 days ago) with no ticket
-- It's likely test data that should be cleaned up

BEGIN;

-- Step 1: End the session
UPDATE magidesk."TableSessions"
SET "Status" = 'Ended',
    "EndTime" = NOW(),
    "TotalChargeAmount" = 0.00,
    "UpdatedAt" = NOW()
WHERE "Id" = '85db63d9-b2e3-4353-84ac-7c3e28fb86e5'
AND "Status" = 'Active';

-- Step 2: Mark table as available
UPDATE magidesk."Tables"
SET "Status" = 'Available',
    "CurrentTicketId" = NULL
WHERE "Id" = 'c0cfb2bd-efb0-4794-b1e1-a68c83068762'
AND "TableNumber" = 2;

-- Step 3: Verify the changes
SELECT 
    ts."Id" as session_id,
    ts."Status" as session_status,
    ts."EndTime",
    ts."TotalChargeAmount",
    t."TableNumber",
    t."Status" as table_status,
    t."CurrentTicketId"
FROM magidesk."TableSessions" ts
JOIN magidesk."Tables" t ON ts."TableId" = t."Id"
WHERE ts."Id" = '85db63d9-b2e3-4353-84ac-7c3e28fb86e5';

-- If everything looks good, commit
COMMIT;

-- If something is wrong, rollback
-- ROLLBACK;
