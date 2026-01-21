-- Delete All Invalid GUID Data
-- This script removes all records with empty GUIDs (00000000-0000-0000-0000-000000000000)
-- These are invalid dummy values that should not exist in the database

-- ============================================================================
-- STEP 1: AUDIT - Show what will be deleted
-- ============================================================================

\echo '========================================='
\echo 'AUDIT: Records with Empty GUIDs'
\echo '========================================='

-- AuditEvents with empty UserId
SELECT 
    'AuditEvents' as table_name,
    'UserId' as field_name,
    COUNT(*) as count_to_delete
FROM public."AuditEvents"
WHERE "UserId" = '00000000-0000-0000-0000-000000000000';

-- Tickets with empty TerminalId, ShiftId, or OrderTypeId
SELECT 
    'Tickets' as table_name,
    'TerminalId/ShiftId/OrderTypeId' as field_name,
    COUNT(*) as count_to_delete
FROM public."Tickets"
WHERE "TerminalId" = '00000000-0000-0000-0000-000000000000'
   OR "ShiftId" = '00000000-0000-0000-0000-000000000000'
   OR "OrderTypeId" = '00000000-0000-0000-0000-000000000000';

\echo ''
\echo 'Sample of Tickets to be deleted:'
SELECT 
    "TicketNumber",
    "CreatedAt",
    "Status",
    "TotalAmount"
FROM public."Tickets"
WHERE "TerminalId" = '00000000-0000-0000-0000-000000000000'
   OR "ShiftId" = '00000000-0000-0000-0000-000000000000'
   OR "OrderTypeId" = '00000000-0000-0000-0000-000000000000'
ORDER BY "CreatedAt" DESC
LIMIT 10;

-- ============================================================================
-- STEP 2: DELETE INVALID DATA
-- ============================================================================

\echo ''
\echo '========================================='
\echo 'DELETING INVALID DATA'
\echo '========================================='

-- Delete AuditEvents with empty UserId
DO $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM public."AuditEvents"
    WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RAISE NOTICE 'Deleted % AuditEvents with empty UserId', deleted_count;
END $$;

-- Delete Tickets with empty TerminalId, ShiftId, or OrderTypeId
-- IMPORTANT: This will cascade delete related OrderLines, Payments, etc.
DO $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM public."Tickets"
    WHERE "TerminalId" = '00000000-0000-0000-0000-000000000000'
       OR "ShiftId" = '00000000-0000-0000-0000-000000000000'
       OR "OrderTypeId" = '00000000-0000-0000-0000-000000000000';
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RAISE NOTICE 'Deleted % Tickets with empty TerminalId/ShiftId/OrderTypeId', deleted_count;
END $$;

-- ============================================================================
-- STEP 3: VERIFY DELETION
-- ============================================================================

\echo ''
\echo '========================================='
\echo 'VERIFICATION'
\echo '========================================='

DO $$
DECLARE
    remaining_audit INTEGER;
    remaining_tickets INTEGER;
BEGIN
    -- Check AuditEvents
    SELECT COUNT(*) INTO remaining_audit
    FROM public."AuditEvents"
    WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    
    -- Check Tickets
    SELECT COUNT(*) INTO remaining_tickets
    FROM public."Tickets"
    WHERE "TerminalId" = '00000000-0000-0000-0000-000000000000'
       OR "ShiftId" = '00000000-0000-0000-0000-000000000000'
       OR "OrderTypeId" = '00000000-0000-0000-0000-000000000000';
    
    IF remaining_audit = 0 AND remaining_tickets = 0 THEN
        RAISE NOTICE 'SUCCESS: All invalid records have been deleted!';
    ELSE
        RAISE WARNING 'WARNING: Still have % AuditEvents and % Tickets with empty GUIDs', 
            remaining_audit, remaining_tickets;
    END IF;
END $$;

-- ============================================================================
-- STEP 4: SUMMARY
-- ============================================================================

\echo ''
\echo '========================================='
\echo 'SUMMARY'
\echo '========================================='

SELECT 
    'AuditEvents' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN "UserId" = '00000000-0000-0000-0000-000000000000' THEN 1 END) as invalid_records
FROM public."AuditEvents"

UNION ALL

SELECT 
    'Tickets',
    COUNT(*),
    COUNT(CASE WHEN "TerminalId" = '00000000-0000-0000-0000-000000000000' 
               OR "ShiftId" = '00000000-0000-0000-0000-000000000000'
               OR "OrderTypeId" = '00000000-0000-0000-0000-000000000000' THEN 1 END)
FROM public."Tickets";

\echo ''
\echo 'Cleanup complete!'
