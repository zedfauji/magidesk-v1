-- Migration: Add Hold Ticket Support (C.2)
-- Date: 2026-01-14
-- Description: Adds columns to support holding tickets for later payment

-- Add HeldAt column
ALTER TABLE magidesk."Tickets"
ADD COLUMN IF NOT EXISTS "HeldAt" timestamp with time zone NULL;

-- Add HoldReason column
ALTER TABLE magidesk."Tickets"
ADD COLUMN IF NOT EXISTS "HoldReason" character varying(500) NULL;

-- Add HeldBy column
ALTER TABLE magidesk."Tickets"
ADD COLUMN IF NOT EXISTS "HeldBy" uuid NULL;

-- Add index for held tickets query performance
CREATE INDEX IF NOT EXISTS "IX_Tickets_HeldAt_Held"
ON magidesk."Tickets" ("HeldAt")
WHERE "Status" = 2;

-- Verify columns were added
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'magidesk' 
  AND table_name = 'Tickets'
  AND column_name IN ('HeldAt', 'HoldReason', 'HeldBy')
ORDER BY column_name;
