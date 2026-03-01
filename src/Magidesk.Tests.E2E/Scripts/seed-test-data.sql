-- Seed Test Data Script
-- Seeds minimum required data for E2E tests
-- This script is executed after database reset to ensure baseline configuration exists

-- Seed default roles if not exists
INSERT INTO "Roles" ("Id", "Name")
VALUES ('00000000-0000-0000-0000-000000000001'::uuid, 'Manager')
ON CONFLICT DO NOTHING;

-- Seed manager user with PIN 1234 if not exists
INSERT INTO "Users" (
    "Id",
    "Username",
    "FirstName",
    "LastName",
    "EncryptedPin",
    "RoleId",
    "IsActive"
)
VALUES (
    '00000000-0000-0000-0000-000000000002'::uuid,
    'manager',
    'Manager',
    'User',
    '1234',  -- For testing, using plain text PIN
    '00000000-0000-0000-0000-000000000001'::uuid,
    true
)
ON CONFLICT DO NOTHING;

-- Seed default terminal if not exists
INSERT INTO "Terminals" (
    "Id",
    "TerminalNumber",
    "Name",
    "IsActive"
)
VALUES (
    '00000000-0000-0000-0000-000000000003'::uuid,
    1,
    'Terminal 1',
    true
)
ON CONFLICT DO NOTHING;

-- Seed restaurant configuration if not exists
INSERT INTO "RestaurantConfigurations" (
    "Id",
    "RestaurantName",
    "TaxRate",
    "ReducedTaxRate",
    "CurrencyCode"
)
VALUES (
    1,
    'Test Restaurant',
    0.10,
    0.05,
    'USD'
)
ON CONFLICT DO NOTHING;
