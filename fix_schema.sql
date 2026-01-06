-- Fix missing columns for PrinterMappings if they don't exist
DO $$
BEGIN
    -- PrinterMappings.CutEnabled
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='CutEnabled') THEN
        ALTER TABLE "PrinterMappings" ADD COLUMN "CutEnabled" boolean NOT NULL DEFAULT TRUE;
    END IF;

    -- PrinterMappings.Format
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='Format') THEN
        ALTER TABLE "PrinterMappings" ADD COLUMN "Format" integer NOT NULL DEFAULT 0;
    END IF;

    -- PrinterGroups.AllowReprint (from the same migration era)
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterGroups' AND column_name='AllowReprint') THEN
        ALTER TABLE "PrinterGroups" ADD COLUMN "AllowReprint" boolean NOT NULL DEFAULT TRUE;
    END IF;

    -- PrinterGroups.CutBehavior
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterGroups' AND column_name='CutBehavior') THEN
        ALTER TABLE "PrinterGroups" ADD COLUMN "CutBehavior" integer NOT NULL DEFAULT 0;
    END IF;

    -- PrinterGroups.FallbackPrinterGroupId
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterGroups' AND column_name='FallbackPrinterGroupId') THEN
        ALTER TABLE "PrinterGroups" ADD COLUMN "FallbackPrinterGroupId" uuid NULL;
    END IF;

    -- PrinterGroups.RetryCount
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterGroups' AND column_name='RetryCount') THEN
         ALTER TABLE "PrinterGroups" ADD COLUMN "RetryCount" integer NOT NULL DEFAULT 0;
    END IF;

    -- PrinterGroups.RetryDelayMs
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterGroups' AND column_name='RetryDelayMs') THEN
         ALTER TABLE "PrinterGroups" ADD COLUMN "RetryDelayMs" integer NOT NULL DEFAULT 0;
    END IF;

    -- PrinterGroups.ShowPrices
     IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterGroups' AND column_name='ShowPrices') THEN
         ALTER TABLE "PrinterGroups" ADD COLUMN "ShowPrices" boolean NOT NULL DEFAULT TRUE;
    END IF;
    
    -- PrinterMappings.Dpi
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='Dpi') THEN
         ALTER TABLE "PrinterMappings" ADD COLUMN "Dpi" integer NOT NULL DEFAULT 203;
    END IF;

    -- PrinterMappings.PaperWidthMm
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='PaperWidthMm') THEN
         ALTER TABLE "PrinterMappings" ADD COLUMN "PaperWidthMm" integer NOT NULL DEFAULT 80;
    END IF;

    -- PrinterMappings.PrintableWidthChars
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='PrintableWidthChars') THEN
         ALTER TABLE "PrinterMappings" ADD COLUMN "PrintableWidthChars" integer NOT NULL DEFAULT 48;
    END IF;

     -- PrinterMappings.SupportsCashDrawer
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='SupportsCashDrawer') THEN
         ALTER TABLE "PrinterMappings" ADD COLUMN "SupportsCashDrawer" boolean NOT NULL DEFAULT TRUE;
    END IF;

     -- PrinterMappings.SupportsImages
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='SupportsImages') THEN
         ALTER TABLE "PrinterMappings" ADD COLUMN "SupportsImages" boolean NOT NULL DEFAULT TRUE;
    END IF;

     -- PrinterMappings.SupportsQr
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='PrinterMappings' AND column_name='SupportsQr') THEN
         ALTER TABLE "PrinterMappings" ADD COLUMN "SupportsQr" boolean NOT NULL DEFAULT TRUE;
    END IF;

END $$;
