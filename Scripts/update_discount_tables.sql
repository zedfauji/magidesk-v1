-- Migration: Update Discount Tables with Missing Columns
-- Date: 2026-01-14
-- Task: 2.1.10 - Add missing columns to existing discount tables
-- Description: Adds RequiresAuthorization to Discounts, AppliedBy and AuthorizedBy to TicketDiscounts

-- ============================================================================
-- Update Discounts Table
-- ============================================================================

-- Add RequiresAuthorization column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'public' 
        AND table_name = 'Discounts' 
        AND column_name = 'RequiresAuthorization'
    ) THEN
        ALTER TABLE "Discounts" 
        ADD COLUMN "RequiresAuthorization" boolean NOT NULL DEFAULT false;
        
        COMMENT ON COLUMN "Discounts"."RequiresAuthorization" 
        IS 'Whether discount requires manager authorization (e.g., >50%)';
        
        RAISE NOTICE 'Added RequiresAuthorization column to Discounts table';
    ELSE
        RAISE NOTICE 'RequiresAuthorization column already exists in Discounts table';
    END IF;
END $$;

-- ============================================================================
-- Update TicketDiscounts Table
-- ============================================================================

-- Add AppliedBy column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'public' 
        AND table_name = 'TicketDiscounts' 
        AND column_name = 'AppliedBy'
    ) THEN
        -- Add as nullable first to handle existing data
        ALTER TABLE "TicketDiscounts" 
        ADD COLUMN "AppliedBy" uuid NULL;
        
        -- Update existing rows with a default system user ID (you may need to adjust this)
        -- Using all zeros as a placeholder - should be updated to actual system user
        UPDATE "TicketDiscounts" 
        SET "AppliedBy" = '00000000-0000-0000-0000-000000000000'
        WHERE "AppliedBy" IS NULL;
        
        -- Now make it NOT NULL
        ALTER TABLE "TicketDiscounts" 
        ALTER COLUMN "AppliedBy" SET NOT NULL;
        
        COMMENT ON COLUMN "TicketDiscounts"."AppliedBy" 
        IS 'User ID who applied the discount';
        
        RAISE NOTICE 'Added AppliedBy column to TicketDiscounts table';
    ELSE
        RAISE NOTICE 'AppliedBy column already exists in TicketDiscounts table';
    END IF;
END $$;

-- Add AuthorizedBy column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'public' 
        AND table_name = 'TicketDiscounts' 
        AND column_name = 'AuthorizedBy'
    ) THEN
        ALTER TABLE "TicketDiscounts" 
        ADD COLUMN "AuthorizedBy" uuid NULL;
        
        COMMENT ON COLUMN "TicketDiscounts"."AuthorizedBy" 
        IS 'Manager user ID who authorized the discount (if required)';
        
        RAISE NOTICE 'Added AuthorizedBy column to TicketDiscounts table';
    ELSE
        RAISE NOTICE 'AuthorizedBy column already exists in TicketDiscounts table';
    END IF;
END $$;

-- ============================================================================
-- Add Missing Indexes
-- ============================================================================

-- Add index on DiscountId if it doesn't exist
CREATE INDEX IF NOT EXISTS "IX_TicketDiscounts_DiscountId" 
ON "TicketDiscounts" ("DiscountId");

-- Add index on AppliedAt if it doesn't exist
CREATE INDEX IF NOT EXISTS "IX_TicketDiscounts_AppliedAt" 
ON "TicketDiscounts" ("AppliedAt");

-- Add foreign key constraint to Discounts if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_schema = 'public' 
        AND table_name = 'TicketDiscounts' 
        AND constraint_name = 'FK_TicketDiscounts_Discounts_DiscountId'
    ) THEN
        ALTER TABLE "TicketDiscounts"
        ADD CONSTRAINT "FK_TicketDiscounts_Discounts_DiscountId" 
        FOREIGN KEY ("DiscountId") REFERENCES "Discounts" ("Id") ON DELETE RESTRICT;
        
        RAISE NOTICE 'Added foreign key constraint FK_TicketDiscounts_Discounts_DiscountId';
    ELSE
        RAISE NOTICE 'Foreign key constraint FK_TicketDiscounts_Discounts_DiscountId already exists';
    END IF;
END $$;

-- ============================================================================
-- Verification Queries
-- ============================================================================

-- Verify columns were added
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
    AND table_name IN ('Discounts', 'TicketDiscounts')
    AND column_name IN ('RequiresAuthorization', 'AppliedBy', 'AuthorizedBy')
ORDER BY table_name, column_name;

-- Verify indexes
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'public' 
    AND tablename = 'TicketDiscounts'
    AND indexname IN ('IX_TicketDiscounts_DiscountId', 'IX_TicketDiscounts_AppliedAt')
ORDER BY indexname;
