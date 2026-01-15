-- Migration: Create Discount Tables
-- Date: 2026-01-14
-- Task: 2.1.10 - Create database tables for discounts
-- Description: Creates Discounts and TicketDiscounts tables with all fields, foreign keys, and indexes

-- ============================================================================
-- Create Discounts Table (Reference Data)
-- ============================================================================

CREATE TABLE IF NOT EXISTS "Discounts" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "Name" varchar(255) NOT NULL,
    "Type" integer NOT NULL,
    "Value" numeric(18,4) NOT NULL,
    "MinimumBuy" numeric(18,2) NULL,
    "MinimumBuyCurrency" varchar(3) NULL,
    "MinimumQuantity" integer NULL,
    "QualificationType" integer NOT NULL,
    "ApplicationType" integer NOT NULL,
    "AutoApply" boolean NOT NULL DEFAULT false,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CouponCode" varchar(50) NULL,
    "ExpirationDate" timestamp NULL,
    "RequiresAuthorization" boolean NOT NULL DEFAULT false
);

-- Indexes for Discounts table
CREATE INDEX IF NOT EXISTS "IX_Discounts_IsActive" 
ON "Discounts" ("IsActive");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Discounts_CouponCode" 
ON "Discounts" ("CouponCode") 
WHERE "CouponCode" IS NOT NULL;

-- Comments for Discounts table
COMMENT ON TABLE "Discounts" IS 'Reference data for discount definitions';
COMMENT ON COLUMN "Discounts"."Id" IS 'Unique identifier for the discount';
COMMENT ON COLUMN "Discounts"."Name" IS 'Display name of the discount';
COMMENT ON COLUMN "Discounts"."Type" IS 'Discount type: 0=Percentage, 1=FixedAmount, 2=Amount';
COMMENT ON COLUMN "Discounts"."Value" IS 'Discount value (percentage or fixed amount)';
COMMENT ON COLUMN "Discounts"."MinimumBuy" IS 'Minimum purchase amount required to qualify';
COMMENT ON COLUMN "Discounts"."MinimumQuantity" IS 'Minimum quantity required to qualify';
COMMENT ON COLUMN "Discounts"."QualificationType" IS 'How discount qualifies: 0=Order, 1=Item, 2=Category';
COMMENT ON COLUMN "Discounts"."ApplicationType" IS 'How discount applies: 0=FreeAmount, 1=FixedPerCategory, 2=FixedPerItem, 3=FixedPerOrder, 4=PercentagePerCategory, 5=PercentagePerItem, 6=PercentagePerOrder';
COMMENT ON COLUMN "Discounts"."AutoApply" IS 'Whether discount is automatically applied';
COMMENT ON COLUMN "Discounts"."IsActive" IS 'Whether discount is currently active';
COMMENT ON COLUMN "Discounts"."CouponCode" IS 'Optional coupon code for manual application';
COMMENT ON COLUMN "Discounts"."ExpirationDate" IS 'Optional expiration date for the discount';
COMMENT ON COLUMN "Discounts"."RequiresAuthorization" IS 'Whether discount requires manager authorization (e.g., >50%)';

-- ============================================================================
-- Create TicketDiscounts Table (Junction/Application Table)
-- ============================================================================

CREATE TABLE IF NOT EXISTS "TicketDiscounts" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "TicketId" uuid NOT NULL,
    "DiscountId" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Type" integer NOT NULL,
    "Value" numeric(18,4) NOT NULL,
    "MinimumAmount" numeric(18,2) NULL,
    "MinimumAmountCurrency" varchar(3) NULL,
    "DiscountAmount" numeric(18,2) NOT NULL,
    "DiscountAmountCurrency" varchar(3) NOT NULL,
    "AppliedAt" timestamp NOT NULL,
    "AppliedBy" uuid NOT NULL,
    "AuthorizedBy" uuid NULL,
    CONSTRAINT "FK_TicketDiscounts_Tickets_TicketId" 
        FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TicketDiscounts_Discounts_DiscountId" 
        FOREIGN KEY ("DiscountId") REFERENCES "Discounts" ("Id") ON DELETE RESTRICT
);

-- Indexes for TicketDiscounts table
CREATE INDEX IF NOT EXISTS "IX_TicketDiscounts_TicketId" 
ON "TicketDiscounts" ("TicketId");

CREATE INDEX IF NOT EXISTS "IX_TicketDiscounts_DiscountId" 
ON "TicketDiscounts" ("DiscountId");

CREATE INDEX IF NOT EXISTS "IX_TicketDiscounts_AppliedAt" 
ON "TicketDiscounts" ("AppliedAt");

-- Comments for TicketDiscounts table
COMMENT ON TABLE "TicketDiscounts" IS 'Tracks discounts applied to tickets (immutable snapshot)';
COMMENT ON COLUMN "TicketDiscounts"."Id" IS 'Unique identifier for the ticket discount application';
COMMENT ON COLUMN "TicketDiscounts"."TicketId" IS 'Foreign key to the ticket';
COMMENT ON COLUMN "TicketDiscounts"."DiscountId" IS 'Foreign key to the discount definition';
COMMENT ON COLUMN "TicketDiscounts"."Name" IS 'Snapshot of discount name at time of application';
COMMENT ON COLUMN "TicketDiscounts"."Type" IS 'Snapshot of discount type at time of application';
COMMENT ON COLUMN "TicketDiscounts"."Value" IS 'Snapshot of discount value at time of application';
COMMENT ON COLUMN "TicketDiscounts"."MinimumAmount" IS 'Snapshot of minimum amount at time of application';
COMMENT ON COLUMN "TicketDiscounts"."DiscountAmount" IS 'Calculated discount amount applied to ticket';
COMMENT ON COLUMN "TicketDiscounts"."AppliedAt" IS 'Timestamp when discount was applied';
COMMENT ON COLUMN "TicketDiscounts"."AppliedBy" IS 'User ID who applied the discount';
COMMENT ON COLUMN "TicketDiscounts"."AuthorizedBy" IS 'Manager user ID who authorized the discount (if required)';

-- ============================================================================
-- Verification Queries
-- ============================================================================

-- Verify tables were created
SELECT 
    table_name,
    (SELECT COUNT(*) FROM information_schema.columns WHERE table_name = t.table_name) as column_count
FROM information_schema.tables t
WHERE table_schema = 'public' 
    AND table_name IN ('Discounts', 'TicketDiscounts')
ORDER BY table_name;

-- Verify indexes were created
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'public' 
    AND tablename IN ('Discounts', 'TicketDiscounts')
ORDER BY tablename, indexname;
