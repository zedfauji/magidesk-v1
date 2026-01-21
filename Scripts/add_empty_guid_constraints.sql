-- Migration: Add Empty GUID Constraints
-- Date: 2026-01-15
-- Purpose: Prevent insertion of records with empty GUIDs (00000000-0000-0000-0000-000000000000)
-- 
-- This migration adds CHECK constraints to ensure data integrity at the database level.
-- These constraints complement the domain-level validation in the UserId value object.

-- ============================================================================
-- GUARDRAILS: Prevent Empty GUIDs in Critical Fields
-- ============================================================================

-- 1. AuditEvents.UserId
-- Ensures all audit events have a valid user
ALTER TABLE public."AuditEvents"
ADD CONSTRAINT "CK_AuditEvents_UserId_NotEmpty"
CHECK ("UserId" != '00000000-0000-0000-0000-000000000000');

-- 2. Tickets.TerminalId
-- Ensures all tickets are associated with a valid terminal
ALTER TABLE public."Tickets"
ADD CONSTRAINT "CK_Tickets_TerminalId_NotEmpty"
CHECK ("TerminalId" != '00000000-0000-0000-0000-000000000000');

-- 3. Tickets.ShiftId
-- Ensures all tickets are associated with a valid shift
ALTER TABLE public."Tickets"
ADD CONSTRAINT "CK_Tickets_ShiftId_NotEmpty"
CHECK ("ShiftId" != '00000000-0000-0000-0000-000000000000');

-- 4. Tickets.OrderTypeId
-- Ensures all tickets have a valid order type
ALTER TABLE public."Tickets"
ADD CONSTRAINT "CK_Tickets_OrderTypeId_NotEmpty"
CHECK ("OrderTypeId" != '00000000-0000-0000-0000-000000000000');

-- 5. Tickets.CreatedBy
-- Ensures all tickets have a valid creator
ALTER TABLE public."Tickets"
ADD CONSTRAINT "CK_Tickets_CreatedBy_NotEmpty"
CHECK ("CreatedBy" != '00000000-0000-0000-0000-000000000000');

-- 6. Payments.ProcessedBy
-- Ensures all payments have a valid processor
ALTER TABLE public."Payments"
ADD CONSTRAINT "CK_Payments_ProcessedBy_NotEmpty"
CHECK ("ProcessedBy" != '00000000-0000-0000-0000-000000000000');

-- 7. CashSessions.UserId
-- Ensures all cash sessions are associated with a valid user
ALTER TABLE public."CashSessions"
ADD CONSTRAINT "CK_CashSessions_UserId_NotEmpty"
CHECK ("UserId" != '00000000-0000-0000-0000-000000000000');

-- 8. AttendanceHistories.UserId
-- Ensures all attendance records are associated with a valid user
ALTER TABLE public."AttendanceHistories"
ADD CONSTRAINT "CK_AttendanceHistories_UserId_NotEmpty"
CHECK ("UserId" != '00000000-0000-0000-0000-000000000000');

-- ============================================================================
-- VERIFICATION
-- ============================================================================

-- Verify all constraints were added successfully
SELECT 
    conrelid::regclass AS table_name,
    conname AS constraint_name,
    pg_get_constraintdef(oid) AS constraint_definition
FROM pg_constraint
WHERE conname LIKE 'CK_%_NotEmpty'
ORDER BY table_name, constraint_name;

-- ============================================================================
-- ROLLBACK (if needed)
-- ============================================================================

-- Uncomment the following lines to remove all constraints:

-- ALTER TABLE public."AuditEvents" DROP CONSTRAINT IF EXISTS "CK_AuditEvents_UserId_NotEmpty";
-- ALTER TABLE public."Tickets" DROP CONSTRAINT IF EXISTS "CK_Tickets_TerminalId_NotEmpty";
-- ALTER TABLE public."Tickets" DROP CONSTRAINT IF EXISTS "CK_Tickets_ShiftId_NotEmpty";
-- ALTER TABLE public."Tickets" DROP CONSTRAINT IF EXISTS "CK_Tickets_OrderTypeId_NotEmpty";
-- ALTER TABLE public."Tickets" DROP CONSTRAINT IF EXISTS "CK_Tickets_CreatedBy_NotEmpty";
-- ALTER TABLE public."Payments" DROP CONSTRAINT IF EXISTS "CK_Payments_ProcessedBy_NotEmpty";
-- ALTER TABLE public."CashSessions" DROP CONSTRAINT IF EXISTS "CK_CashSessions_UserId_NotEmpty";
-- ALTER TABLE public."AttendanceHistories" DROP CONSTRAINT IF EXISTS "CK_AttendanceHistories_UserId_NotEmpty";
