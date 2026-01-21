-- Migration: Add Gratuities.OwnerId Empty GUID Constraint
-- Date: 2026-01-18
-- Purpose: Complete database-level protection against empty GUIDs in Gratuities.OwnerId
-- 
-- This constraint completes the set of 9 CHECK constraints that prevent empty GUIDs
-- in all UserId-related fields across the database.

-- ============================================================================
-- VERIFICATION: Check current state
-- ============================================================================

-- Check if constraint already exists
SELECT 
    conrelid::regclass AS table_name,
    conname AS constraint_name,
    pg_get_constraintdef(oid) AS constraint_definition
FROM pg_constraint
WHERE conrelid = '"Gratuities"'::regclass
  AND contype = 'c'
  AND conname = 'CK_Gratuities_OwnerId_NotEmpty';

-- Check for any existing empty GUIDs (should be 0)
SELECT COUNT(*) as empty_owner_count
FROM public."Gratuities"
WHERE "OwnerId" = '00000000-0000-0000-0000-000000000000';

-- ============================================================================
-- ADD CONSTRAINT
-- ============================================================================

-- Add CHECK constraint to prevent empty GUID in OwnerId
ALTER TABLE public."Gratuities"
ADD CONSTRAINT "CK_Gratuities_OwnerId_NotEmpty"
CHECK ("OwnerId" != '00000000-0000-0000-0000-000000000000');

-- ============================================================================
-- VERIFICATION: Confirm constraint was added
-- ============================================================================

-- Verify constraint exists
SELECT 
    conrelid::regclass AS table_name,
    conname AS constraint_name,
    pg_get_constraintdef(oid) AS constraint_definition
FROM pg_constraint
WHERE conrelid = '"Gratuities"'::regclass
  AND contype = 'c'
  AND conname = 'CK_Gratuities_OwnerId_NotEmpty';

-- ============================================================================
-- TEST: Verify constraint works
-- ============================================================================

-- This should FAIL with constraint violation error:
-- INSERT INTO public."Gratuities" 
-- ("Id", "TicketId", "Amount", "AmountCurrency", "Paid", "Refunded", "TerminalId", "OwnerId", "CreatedAt")
-- VALUES 
-- (gen_random_uuid(), gen_random_uuid(), 10.00, 'USD', false, false, gen_random_uuid(), '00000000-0000-0000-0000-000000000000', NOW());
-- 
-- Expected Error: new row for relation "Gratuities" violates check constraint "CK_Gratuities_OwnerId_NotEmpty"

-- ============================================================================
-- SUMMARY: All UserId Constraints
-- ============================================================================

-- List all empty GUID constraints in the database
SELECT 
    conrelid::regclass AS table_name,
    conname AS constraint_name,
    pg_get_constraintdef(oid) AS constraint_definition
FROM pg_constraint
WHERE conname LIKE 'CK_%_NotEmpty'
ORDER BY table_name, constraint_name;

-- Expected Result: 9 constraints
-- 1. CK_AttendanceHistories_UserId_NotEmpty
-- 2. CK_AuditEvents_UserId_NotEmpty
-- 3. CK_CashSessions_UserId_NotEmpty
-- 4. CK_Gratuities_OwnerId_NotEmpty ← NEW
-- 5. CK_Payments_ProcessedBy_NotEmpty
-- 6. CK_Tickets_CreatedBy_NotEmpty
-- 7. CK_Tickets_OrderTypeId_NotEmpty
-- 8. CK_Tickets_ShiftId_NotEmpty
-- 9. CK_Tickets_TerminalId_NotEmpty

-- ============================================================================
-- ROLLBACK (if needed)
-- ============================================================================

-- Uncomment to remove the constraint:
-- ALTER TABLE public."Gratuities" 
-- DROP CONSTRAINT IF EXISTS "CK_Gratuities_OwnerId_NotEmpty";

