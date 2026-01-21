-- Table Session Data Repair Script
-- Date: 2026-01-18
-- Purpose: Fix data inconsistencies identified in TABLE_SESSION_ROOT_CAUSE_ANALYSIS.md

-- ============================================================================
-- COMPLETED FIXES
-- ============================================================================

-- Fix 1: Table 3 - Link session to correct ticket
-- COMPLETED: Session now linked to Ticket #1696
UPDATE magidesk."TableSessions"
SET "TicketId" = '2c9d825f-b573-49ac-8e1d-bd58947f8813'
WHERE "Id" = 'f436c2ca-d8e5-4aec-8eec-da7560855835';

-- Fix 2: Add FK constraint to prevent orphaned references
-- COMPLETED: Constraint added successfully
ALTER TABLE magidesk."TableSessions"
ADD CONSTRAINT "FK_TableSessions_Tickets"
FOREIGN KEY ("TicketId")
REFERENCES public."Tickets"("Id")
ON DELETE SET NULL;

-- ============================================================================
-- PENDING: TABLE 2 - Manual Intervention Required
-- ============================================================================

-- Table 2 has an Active session with no ticket
-- Session ID: 85db63d9-b2e3-4353-84ac-7c3e28fb86e5
-- Started: 2026-01-15 15:08:34 (3 days ago)
-- HourlyRate: $15.00
-- GuestCount: 1

-- OPTION 1: End the session without creating a ticket (if test data)
-- This will mark the session as Ended and set TotalCharge to 0
-- UPDATE magidesk."TableSessions"
-- SET "Status" = 'Ended',
--     "EndTime" = NOW(),
--     "TotalChargeAmount" = 0.00
-- WHERE "Id" = '85db63d9-b2e3-4353-84ac-7c3e28fb86e5';

-- OPTION 2: Create a ticket and link it (if real session)
-- This requires creating a ticket through the application
-- because we need proper UserId, TerminalId, ShiftId, OrderTypeId
-- Then link the session:
-- UPDATE magidesk."TableSessions"
-- SET "TicketId" = '<new_ticket_id>'
-- WHERE "Id" = '85db63d9-b2e3-4353-84ac-7c3e28fb86e5';

-- OPTION 3: Delete the session (if invalid test data)
-- DELETE FROM magidesk."TableSessions"
-- WHERE "Id" = '85db63d9-b2e3-4353-84ac-7c3e28fb86e5';

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

-- Verify Table 3 fix
SELECT 
    ts."Id" as session_id,
    ts."Status" as session_status,
    ts."TicketId" as session_ticket,
    t."TableNumber",
    t."CurrentTicketId" as table_ticket,
    tk."TicketNumber",
    tk."Status" as ticket_status
FROM magidesk."TableSessions" ts
LEFT JOIN magidesk."Tables" t ON ts."TableId" = t."Id"
LEFT JOIN public."Tickets" tk ON ts."TicketId" = tk."Id"
WHERE t."TableNumber" = 3
AND ts."Status" = 'Active';

-- Check for orphaned session references (should return 0 rows after FK constraint)
SELECT 
    ts."Id",
    ts."TicketId",
    t."TableNumber"
FROM magidesk."TableSessions" ts
LEFT JOIN magidesk."Tables" t ON ts."TableId" = t."Id"
LEFT JOIN public."Tickets" tk ON ts."TicketId" = tk."Id"
WHERE ts."TicketId" IS NOT NULL
AND tk."Id" IS NULL;

-- Check all active sessions
SELECT 
    ts."Id" as session_id,
    ts."Status",
    ts."TicketId",
    ts."StartTime",
    t."TableNumber",
    t."Status" as table_status,
    tk."TicketNumber",
    tk."Status" as ticket_status
FROM magidesk."TableSessions" ts
LEFT JOIN magidesk."Tables" t ON ts."TableId" = t."Id"
LEFT JOIN public."Tickets" tk ON ts."TicketId" = tk."Id"
WHERE ts."Status" IN ('Active', 'Paused')
ORDER BY t."TableNumber";
