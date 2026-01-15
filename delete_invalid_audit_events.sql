-- Delete Invalid AuditEvents with Empty UserId
-- These are invalid records that should not exist in the database

-- Step 1: Check what we're about to delete
DO $$
DECLARE
    invalid_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO invalid_count
    FROM public."AuditEvents"
    WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    
    RAISE NOTICE 'Found % invalid AuditEvents with empty UserId (will be deleted)', invalid_count;
END $$;

-- Step 2: Show sample of records to be deleted (for verification)
SELECT 
    "Id",
    "EntityType",
    "EventType",
    "Timestamp",
    "Description"
FROM public."AuditEvents"
WHERE "UserId" = '00000000-0000-0000-0000-000000000000'
ORDER BY "Timestamp" DESC
LIMIT 10;

-- Step 3: Delete the invalid records
DO $$
DECLARE
    empty_guid UUID := '00000000-0000-0000-0000-000000000000';
    deleted_count INTEGER;
BEGIN
    RAISE NOTICE 'Deleting invalid AuditEvents...';
    
    DELETE FROM public."AuditEvents"
    WHERE "UserId" = empty_guid;
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RAISE NOTICE 'Deleted % invalid AuditEvents records', deleted_count;
END $$;

-- Step 4: Verify deletion
DO $$
DECLARE
    remaining_invalid INTEGER;
BEGIN
    SELECT COUNT(*) INTO remaining_invalid
    FROM public."AuditEvents"
    WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    
    IF remaining_invalid = 0 THEN
        RAISE NOTICE 'SUCCESS: All invalid AuditEvents have been deleted!';
    ELSE
        RAISE WARNING 'WARNING: Still have % invalid records', remaining_invalid;
    END IF;
END $$;

-- Step 5: Summary
SELECT 
    COUNT(*) as total_audit_events,
    COUNT(CASE WHEN "UserId" = '00000000-0000-0000-0000-000000000000' THEN 1 END) as invalid_records_remaining
FROM public."AuditEvents";
