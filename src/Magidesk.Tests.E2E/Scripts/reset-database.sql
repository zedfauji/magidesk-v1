-- Reset Database Script
-- Deletes all transactional data in dependency order while preserving configuration data
-- This script is executed before each E2E test to ensure clean state

-- Delete transactional data in dependency order (child tables first)

-- Kitchen orders
DELETE FROM "KitchenOrderItems";
DELETE FROM "KitchenOrders";

-- Payments and related
DELETE FROM "Gratuities";
DELETE FROM "Payments";

-- Order lines and related
DELETE FROM "OrderLineModifiers";
DELETE FROM "OrderLineDiscounts";
DELETE FROM "OrderLines";

-- Tickets and related
DELETE FROM "TicketDiscounts";
DELETE FROM "Tickets";

-- Cash sessions and related
DELETE FROM "DrawerBleeds";
DELETE FROM "CashDrops";
DELETE FROM "Payouts";
DELETE FROM "CashSessions";

-- Terminal transactions
DELETE FROM "TerminalTransactions";

-- Payment batches
DELETE FROM "PaymentBatches";

-- Group settlements
DELETE FROM "GroupSettlements";

-- Shifts
DELETE FROM "Shifts";

-- Attendance history
DELETE FROM "AttendanceHistories";

-- Purchase orders
DELETE FROM "PurchaseOrderLines";
DELETE FROM "PurchaseOrders";

-- Inventory adjustments and stock movements
DELETE FROM "InventoryAdjustments";
DELETE FROM "StockMovements";

-- Audit events (only transactional entity types)
DELETE FROM "AuditEvents" 
WHERE "EntityType" IN (
    'Ticket', 'Payment', 'CashSession', 'OrderLine', 
    'KitchenOrder', 'TableSession', 'Shift', 'PurchaseOrder'
);
