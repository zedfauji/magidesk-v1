-- Add RoundingRule column to TableTypes table
-- Database: magidesk_new
ALTER TABLE magidesk."TableTypes" 
ADD COLUMN IF NOT EXISTS "RoundingRule" text NOT NULL DEFAULT 'None';

-- Update existing records to have 'None' as default (if column already exists)
UPDATE magidesk."TableTypes" 
SET "RoundingRule" = 'None' 
WHERE "RoundingRule" IS NULL OR "RoundingRule" = '';
