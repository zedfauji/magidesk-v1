-- Fix Empty UserId Data (PostgreSQL)
-- This script updates all AuditEvents records with empty GUIDs
-- to use a default "System" user GUID

-- Step 1: Check current state
DO $$
DECLARE
    empty_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO empty_count
    FROM public."AuditEvents"
    WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    
    RAISE NOTICE 'Found % AuditEvents with empty UserId', empty_count;
END $$;

-- Step 2: Create a default "System" user if it doesn't exist
-- Using a well-known GUID: 00000000-0000-0000-0000-000000000001
DO $$
DECLARE
    system_user_id UUID := '00000000-0000-0000-0000-000000000001';
    system_user_exists BOOLEAN;
    admin_role_id UUID;
BEGIN
    -- Check if system user exists
    SELECT EXISTS(SELECT 1 FROM public."Users" WHERE "Id" = system_user_id) INTO system_user_exists;
    
    IF NOT system_user_exists THEN
        RAISE NOTICE 'Creating System user...';
        
        -- Get the Administrator role ID (or first role if Admin doesn't exist)
        SELECT "Id" INTO admin_role_id
        FROM public."Roles"
        WHERE "Name" = 'Administrator'
        LIMIT 1;
        
        -- If no Administrator role, get any role
        IF admin_role_id IS NULL THEN
            SELECT "Id" INTO admin_role_id
            FROM public."Roles"
            LIMIT 1;
        END IF;
        
        -- Insert system user
        INSERT INTO public."Users" (
            "Id",
            "Username",
            "FirstName",
            "LastName",
            "Pin",
            "RoleId",
            "IsActive",
            "CreatedAt"
        ) VALUES (
            system_user_id,
            'SYSTEM',
            'System',
            'User',
            '', -- Empty PIN
            admin_role_id,
            true,
            NOW()
        );
        
        RAISE NOTICE 'System user created successfully';
    ELSE
        RAISE NOTICE 'System user already exists';
    END IF;
END $$;

-- Step 3: Update AuditEvents with empty UserId
DO $$
DECLARE
    system_user_id UUID := '00000000-0000-0000-0000-000000000001';
    empty_guid UUID := '00000000-0000-0000-0000-000000000000';
    updated_count INTEGER;
BEGIN
    RAISE NOTICE 'Updating AuditEvents with empty UserId...';
    
    UPDATE public."AuditEvents"
    SET "UserId" = system_user_id
    WHERE "UserId" = empty_guid;
    
    GET DIAGNOSTICS updated_count = ROW_COUNT;
    RAISE NOTICE 'Updated % AuditEvents records', updated_count;
END $$;

-- Step 4: Verify the fix
DO $$
DECLARE
    remaining_empty INTEGER;
BEGIN
    SELECT COUNT(*) INTO remaining_empty
    FROM public."AuditEvents"
    WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    
    IF remaining_empty = 0 THEN
        RAISE NOTICE 'SUCCESS: All empty UserId fields have been fixed!';
    ELSE
        RAISE WARNING 'WARNING: Still have % records with empty UserId', remaining_empty;
    END IF;
END $$;

-- Step 5: Summary
SELECT 
    'AuditEvents' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN "UserId" = '00000000-0000-0000-0000-000000000001' THEN 1 END) as system_user_records,
    COUNT(CASE WHEN "UserId" = '00000000-0000-0000-0000-000000000000' THEN 1 END) as empty_guid_records
FROM public."AuditEvents";
