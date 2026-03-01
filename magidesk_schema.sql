CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    CREATE TABLE "PrinterGroups" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Type" integer NOT NULL,
        CONSTRAINT "PK_PrinterGroups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    CREATE TABLE "PrinterMappings" (
        "Id" uuid NOT NULL,
        "TerminalId" uuid NOT NULL,
        "PrinterGroupId" uuid NOT NULL,
        "PhysicalPrinterName" character varying(255) NOT NULL,
        "Format" integer NOT NULL,
        "CutEnabled" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_PrinterMappings" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PrinterMappings_PrinterGroups_PrinterGroupId" FOREIGN KEY ("PrinterGroupId") REFERENCES "PrinterGroups" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    CREATE INDEX "IX_PrinterMappings_PrinterGroupId" ON "PrinterMappings" ("PrinterGroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    ALTER TABLE "MenuItems" ADD "PrinterGroupId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    CREATE INDEX "IX_MenuItems_PrinterGroupId" ON "MenuItems" ("PrinterGroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    ALTER TABLE "MenuItems" ADD CONSTRAINT "FK_MenuItems_PrinterGroups_PrinterGroupId" FOREIGN KEY ("PrinterGroupId") REFERENCES "PrinterGroups" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104154305_AddPrinterSupportColumns') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260104154305_AddPrinterSupportColumns', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterMappings" ADD "Dpi" integer NOT NULL DEFAULT 203;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterMappings" ADD "PaperWidthMm" integer NOT NULL DEFAULT 80;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterMappings" ADD "PrintableWidthChars" integer NOT NULL DEFAULT 48;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterMappings" ADD "SupportsCashDrawer" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterMappings" ADD "SupportsImages" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterMappings" ADD "SupportsQr" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterGroups" ADD "AllowReprint" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterGroups" ADD "CutBehavior" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterGroups" ADD "FallbackPrinterGroupId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterGroups" ADD "RetryCount" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterGroups" ADD "RetryDelayMs" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    ALTER TABLE "PrinterGroups" ADD "ShowPrices" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260104171112_AddPrinterDetailedConfiguration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260104171112_AddPrinterDetailedConfiguration', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260106150426_AddIsDraftCol') THEN
    ALTER TABLE magidesk."Tables" ALTER COLUMN "Y" TYPE double precision;
    ALTER TABLE magidesk."Tables" ALTER COLUMN "Y" SET DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260106150426_AddIsDraftCol') THEN
    ALTER TABLE magidesk."Tables" ALTER COLUMN "X" TYPE double precision;
    ALTER TABLE magidesk."Tables" ALTER COLUMN "X" SET DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260106150426_AddIsDraftCol') THEN
    ALTER TABLE magidesk."Tables" ALTER COLUMN "Width" TYPE double precision;
    ALTER TABLE magidesk."Tables" ALTER COLUMN "Width" SET DEFAULT 100.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260106150426_AddIsDraftCol') THEN
    ALTER TABLE magidesk."Tables" ALTER COLUMN "Height" TYPE double precision;
    ALTER TABLE magidesk."Tables" ALTER COLUMN "Height" SET DEFAULT 100.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260106150426_AddIsDraftCol') THEN
    ALTER TABLE magidesk."TableLayouts" ADD "IsDraft" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260106150426_AddIsDraftCol') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260106150426_AddIsDraftCol', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    ALTER TABLE "Users" ADD "PreferredLanguage" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    ALTER TABLE magidesk."Tables" ADD "TableTypeId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE TABLE magidesk."TableSessions" (
        "Id" uuid NOT NULL,
        "TableId" uuid NOT NULL,
        "CustomerId" uuid,
        "TicketId" uuid,
        "StartTime" timestamp with time zone NOT NULL,
        "EndTime" timestamp with time zone,
        "PausedAt" timestamp with time zone,
        "TotalPausedDuration" interval NOT NULL,
        "Status" text NOT NULL,
        "TableTypeId" uuid NOT NULL,
        "HourlyRate" numeric(10,2) NOT NULL,
        "TotalChargeAmount" numeric(10,2) NOT NULL,
        "TotalChargeCurrency" character varying(3) NOT NULL,
        "GuestCount" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        "ManualAdjustment" interval NOT NULL,
        CONSTRAINT "PK_TableSessions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE TABLE magidesk."TableTypes" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "HourlyRate" numeric(10,2) NOT NULL,
        "FirstHourRate" numeric(10,2),
        "MinimumMinutes" integer NOT NULL DEFAULT 0,
        "RoundingMinutes" integer NOT NULL DEFAULT 1,
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_TableTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_Tables_TableTypeId" ON magidesk."Tables" ("TableTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_TableSessions_CustomerId" ON magidesk."TableSessions" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_TableSessions_StartTime" ON magidesk."TableSessions" ("StartTime");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_TableSessions_Status" ON magidesk."TableSessions" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_TableSessions_TableId" ON magidesk."TableSessions" ("TableId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_TableSessions_TicketId" ON magidesk."TableSessions" ("TicketId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE INDEX "IX_TableTypes_IsActive" ON magidesk."TableTypes" ("IsActive");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    CREATE UNIQUE INDEX "IX_TableTypes_Name" ON magidesk."TableTypes" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    ALTER TABLE magidesk."Tables" ADD CONSTRAINT "FK_Tables_TableTypes_TableTypeId" FOREIGN KEY ("TableTypeId") REFERENCES magidesk."TableTypes" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109232333_AddSessionManualAdjustment') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260109232333_AddSessionManualAdjustment', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109235836_AddTicketSessionLink') THEN
    ALTER TABLE "Tickets" ADD "SessionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109235836_AddTicketSessionLink') THEN
    CREATE INDEX "IX_Tickets_SessionId" ON "Tickets" ("SessionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260109235836_AddTicketSessionLink') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260109235836_AddTicketSessionLink', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110000507_AddTimeChargesToOrderLine') THEN
    ALTER TABLE "OrderLines" ADD "Duration" interval;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110000507_AddTimeChargesToOrderLine') THEN
    ALTER TABLE "OrderLines" ADD "HourlyRate" numeric(18,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110000507_AddTimeChargesToOrderLine') THEN
    ALTER TABLE "OrderLines" ADD "IsTimeCharge" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110000507_AddTimeChargesToOrderLine') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260110000507_AddTimeChargesToOrderLine', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110004722_AddCustomersTable') THEN
    CREATE TABLE magidesk."Customers" (
        "Id" uuid NOT NULL,
        "FirstName" character varying(100) NOT NULL,
        "LastName" character varying(100) NOT NULL,
        "Email" character varying(150),
        "Phone" character varying(20) NOT NULL,
        "DateOfBirth" timestamp with time zone,
        "Address" character varying(250),
        "City" character varying(100),
        "PostalCode" character varying(20),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastVisitAt" timestamp with time zone,
        "TotalVisits" integer NOT NULL DEFAULT 0,
        "TotalSpentAmount" numeric(18,2) NOT NULL,
        "TotalSpentCurrency" character varying(3) NOT NULL DEFAULT 'USD',
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110004722_AddCustomersTable') THEN
    CREATE INDEX "IX_Customers_Email" ON magidesk."Customers" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110004722_AddCustomersTable') THEN
    CREATE INDEX "IX_Customers_FirstName" ON magidesk."Customers" ("FirstName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110004722_AddCustomersTable') THEN
    CREATE INDEX "IX_Customers_LastName" ON magidesk."Customers" ("LastName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110004722_AddCustomersTable') THEN
    CREATE UNIQUE INDEX "IX_Customers_Phone" ON magidesk."Customers" ("Phone");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110004722_AddCustomersTable') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260110004722_AddCustomersTable', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110014436_AddMemberAndTier') THEN
    CREATE TABLE "MembershipTiers" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" text NOT NULL,
        "DiscountPercent" numeric NOT NULL,
        "HourlyRateDiscount" numeric,
        "IncludesFreeGuests" boolean NOT NULL,
        "FreeGuestsPerVisit" integer NOT NULL,
        "MonthlyFeeAmount" numeric NOT NULL,
        "MonthlyFeeCurrency" text NOT NULL,
        "AnnualFeeAmount" numeric NOT NULL,
        "AnnualFeeCurrency" text NOT NULL,
        "SortOrder" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_MembershipTiers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110014436_AddMemberAndTier') THEN
    CREATE TABLE "Members" (
        "Id" uuid NOT NULL,
        "CustomerId" uuid NOT NULL,
        "TierId" uuid NOT NULL,
        "MemberNumber" character varying(50) NOT NULL,
        "JoinDate" timestamp with time zone NOT NULL,
        "ExpirationDate" timestamp with time zone,
        "Status" integer NOT NULL,
        "PrepaidBalanceAmount" numeric NOT NULL,
        "PrepaidBalanceCurrency" text NOT NULL,
        CONSTRAINT "PK_Members" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Members_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES magidesk."Customers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Members_MembershipTiers_TierId" FOREIGN KEY ("TierId") REFERENCES "MembershipTiers" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110014436_AddMemberAndTier') THEN
    CREATE UNIQUE INDEX "IX_Members_CustomerId" ON "Members" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110014436_AddMemberAndTier') THEN
    CREATE UNIQUE INDEX "IX_Members_MemberNumber" ON "Members" ("MemberNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110014436_AddMemberAndTier') THEN
    CREATE INDEX "IX_Members_TierId" ON "Members" ("TierId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260110014436_AddMemberAndTier') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260110014436_AddMemberAndTier', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111012900_AddStockTracking') THEN
    ALTER TABLE "MenuItems" ADD "MinimumStockLevel" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111012900_AddStockTracking') THEN
    ALTER TABLE "MenuItems" ADD "StockQuantity" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111012900_AddStockTracking') THEN
    ALTER TABLE "MenuItems" ADD "TrackStock" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111012900_AddStockTracking') THEN
    CREATE TABLE "StockMovements" (
        "Id" uuid NOT NULL,
        "MenuItemId" uuid NOT NULL,
        "QuantityChange" integer NOT NULL,
        "Type" integer NOT NULL,
        "Reference" text NOT NULL,
        "Timestamp" timestamp with time zone NOT NULL,
        "UserId" uuid,
        CONSTRAINT "PK_StockMovements" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111012900_AddStockTracking') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260111012900_AddStockTracking', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111164112_AddCategoryHierarchy') THEN
    ALTER TABLE "MenuCategories" ADD "ParentCategoryId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111164112_AddCategoryHierarchy') THEN
    CREATE INDEX "IX_MenuCategories_ParentCategoryId" ON "MenuCategories" ("ParentCategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111164112_AddCategoryHierarchy') THEN
    ALTER TABLE "MenuCategories" ADD CONSTRAINT "FK_MenuCategories_MenuCategories_ParentCategoryId" FOREIGN KEY ("ParentCategoryId") REFERENCES "MenuCategories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111164112_AddCategoryHierarchy') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260111164112_AddCategoryHierarchy', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111171228_AddModifierGroupPricing') THEN
    ALTER TABLE magidesk."ModifierGroups" ADD "ExtraModifierPrice" numeric NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111171228_AddModifierGroupPricing') THEN
    ALTER TABLE magidesk."ModifierGroups" ADD "FreeModifiers" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260111171228_AddModifierGroupPricing') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260111171228_AddModifierGroupPricing', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260128191850_AddPrinterGroupToKitchenOrder') THEN
    ALTER TABLE "KitchenOrders" ADD "PrinterGroupId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260128191850_AddPrinterGroupToKitchenOrder') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260128191850_AddPrinterGroupToKitchenOrder', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260129032507_AddKitchenOrderLifecycleTimestamps') THEN
    ALTER TABLE "OrderLines" ADD "DeliveredAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260129032507_AddKitchenOrderLifecycleTimestamps') THEN
    ALTER TABLE "OrderLines" ADD "SentToKitchenAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260129032507_AddKitchenOrderLifecycleTimestamps') THEN
    ALTER TABLE "KitchenOrders" ADD "DeliveredAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260129032507_AddKitchenOrderLifecycleTimestamps') THEN
    ALTER TABLE "KitchenOrders" ADD "SentToKitchenAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260129032507_AddKitchenOrderLifecycleTimestamps') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260129032507_AddKitchenOrderLifecycleTimestamps', '8.0.0');
    END IF;
END $EF$;
COMMIT;

