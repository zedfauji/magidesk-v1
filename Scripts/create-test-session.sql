-- Test Script: Create a TableSession to verify Feature A.4 visual indicators
-- Run this in your PostgreSQL database to create a test session

-- Step 1: Get a table ID (using Table 1 as example)
DO $$
DECLARE
    v_table_id UUID;
    v_table_type_id UUID;
    v_session_id UUID;
BEGIN
    -- Get Table 1's ID
    SELECT "Id" INTO v_table_id 
    FROM magidesk."Tables" 
    WHERE "TableNumber" = 1 
    LIMIT 1;
    
    -- Get or create a default TableType
    SELECT "Id" INTO v_table_type_id 
    FROM magidesk."TableTypes" 
    LIMIT 1;
    
    IF v_table_type_id IS NULL THEN
        -- Create a default table type if none exists
        v_table_type_id := gen_random_uuid();
        INSERT INTO magidesk."TableTypes" 
            ("Id", "Name", "Description", "HourlyRate", "MinimumMinutes", "RoundingMinutes", "IsActive", "CreatedAt", "UpdatedAt")
        VALUES 
            (v_table_type_id, 'Standard', 'Standard table type', 15.00, 15, 15, true, NOW(), NOW());
    END IF;
    
    -- Create a new session
    v_session_id := gen_random_uuid();
    INSERT INTO magidesk."TableSessions"
        ("Id", "TableId", "TableTypeId", "StartTime", "Status", "HourlyRate", "GuestCount", "TotalPausedDuration", "CreatedAt", "UpdatedAt")
    VALUES
        (v_session_id, v_table_id, v_table_type_id, NOW() - INTERVAL '15 minutes', 0, 15.00, 2, INTERVAL '0 seconds', NOW(), NOW());
    
    -- Mark table as in use
    UPDATE magidesk."Tables"
    SET "Status" = 1  -- Seat status
    WHERE "Id" = v_table_id;
    
    RAISE NOTICE 'Created test session % for Table 1', v_session_id;
END $$;

-- Verify the session was created
SELECT 
    t."TableNumber",
    t."Status",
    ts."Id" as "SessionId",
    ts."StartTime",
    ts."Status" as "SessionStatus",
    ts."HourlyRate",
    ts."GuestCount"
FROM magidesk."Tables" t
LEFT JOIN magidesk."TableSessions" ts ON ts."TableId" = t."Id" AND ts."Status" = 0
WHERE t."TableNumber" = 1;
