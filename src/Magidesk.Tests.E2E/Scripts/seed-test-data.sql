-- Seed Test Data Script
-- Seeds minimum required data for E2E tests
-- This script is executed after database reset to ensure baseline configuration exists

-- Seed default roles if not exists
INSERT INTO "Roles" ("Id", "Name", "Permissions")
VALUES ('00000000-0000-0000-0000-000000000001'::uuid, 'Manager', 0)
ON CONFLICT ("Name") DO NOTHING;

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
ON CONFLICT ("Username") DO NOTHING;

-- Seed default terminal if not exists
INSERT INTO "Terminals" (
    "Id",
    "Name",
    "TerminalKey",
    "Location",
    "HasCashDrawer",
    "OpeningBalance",
    "CurrentBalance",
    "AutoLogOut",
    "AutoLogOutTime",
    "ShowGuestSelection",
    "ShowTableSelection",
    "KitchenMode",
    "DefaultFontSize",
    "DefaultFontFamily"
)
VALUES (
    '00000000-0000-0000-0000-000000000003'::uuid,
    'Terminal 1',
    'TERM001',
    'Main Floor',
    true,
    0.00,
    0.00,
    false,
    0,
    false,
    false,
    false,
    '14',
    'Segoe UI'
)
ON CONFLICT ("Id") DO NOTHING;

-- Seed restaurant configuration if not exists
INSERT INTO "RestaurantConfigurations" (
    "Id",
    "Name",
    "Address",
    "Phone",
    "Email",
    "Website",
    "ReceiptFooterMessage",
    "TaxId",
    "Capacity",
    "CurrencySymbol",
    "DefaultGratuityPercentage",
    "IsKioskMode",
    "ServiceChargePercentage",
    "ZipCode"
)
VALUES (
    1,
    'Test Restaurant',
    '123 Test St',
    '555-1234',
    'test@restaurant.com',
    'www.testrestaurant.com',
    'Thank you for your visit!',
    'TAX123',
    100,
    '$',
    0.15,
    false,
    0.00,
    '12345'
)
ON CONFLICT ("Id") DO NOTHING;
