-- Migration: Add Split Payment Support to Payments table
-- Date: 2026-01-14
-- Description: Adds columns for split payment tracking and refund support

-- Add SplitGroupId column (nullable, groups related split payments)
ALTER TABLE "Payments" 
ADD COLUMN IF NOT EXISTS "SplitGroupId" uuid NULL;

-- Add SplitSequence column (nullable, orders payments within a split group)
ALTER TABLE "Payments" 
ADD COLUMN IF NOT EXISTS "SplitSequence" integer NULL;

-- Add RefundedAmount column (tracks how much has been refunded)
ALTER TABLE "Payments" 
ADD COLUMN IF NOT EXISTS "RefundedAmount" numeric(18,2) NOT NULL DEFAULT 0.00;

-- Add RefundedCurrency column
ALTER TABLE "Payments" 
ADD COLUMN IF NOT EXISTS "RefundedCurrency" varchar(3) NOT NULL DEFAULT 'USD';

-- Add IsRefunded column (boolean flag for refund status)
ALTER TABLE "Payments" 
ADD COLUMN IF NOT EXISTS "IsRefunded" boolean NOT NULL DEFAULT false;

-- Create filtered index on SplitGroupId for efficient split payment queries
CREATE INDEX IF NOT EXISTS "IX_Payments_SplitGroupId" 
ON "Payments" ("SplitGroupId") 
WHERE "SplitGroupId" IS NOT NULL;

-- Add comment for documentation
COMMENT ON COLUMN "Payments"."SplitGroupId" IS 'Groups multiple payments that are part of a split payment transaction';
COMMENT ON COLUMN "Payments"."SplitSequence" IS 'Sequence number of this payment within its split group (1-based)';
COMMENT ON COLUMN "Payments"."RefundedAmount" IS 'Amount that has been refunded from this payment';
COMMENT ON COLUMN "Payments"."IsRefunded" IS 'Indicates if this payment has been fully or partially refunded';
