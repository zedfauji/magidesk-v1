-- Reset Database Script
-- Deletes all transactional data in dependency order while preserving configuration data
-- This script is executed before each E2E test to ensure clean state

-- Delete transactional data in dependency order (child tables first)

-- Kitchen orders (check if tables exist first)
DELETE FROM kitchen_order_items WHERE EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'kitchen_order_items');
DELETE FROM kitchen_orders WHERE EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'kitchen_orders');

-- Payments and related
DELETE FROM gratuities;
DELETE FROM payments;

-- Order lines and related
DELETE FROM order_line_modifiers;
DELETE FROM order_line_discounts;
DELETE FROM order_lines;

-- Tickets and related
DELETE FROM ticket_discounts;
DELETE FROM tickets;

-- Cash sessions and related
DELETE FROM drawer_bleeds;
DELETE FROM cash_drops;
DELETE FROM payouts;
DELETE FROM cash_sessions;

-- Table sessions
DELETE FROM table_sessions;

-- Game history
DELETE FROM game_history;

-- Server assignments
DELETE FROM server_assignments;

-- Terminal transactions
DELETE FROM terminal_transactions;

-- Payment batches
DELETE FROM payment_batches;

-- Group settlements
DELETE FROM group_settlements;

-- Shifts
DELETE FROM shifts;

-- Attendance history
DELETE FROM attendance_histories;

-- Purchase orders
DELETE FROM purchase_order_lines;
DELETE FROM purchase_orders;

-- Inventory adjustments and stock movements
DELETE FROM inventory_adjustments;
DELETE FROM stock_movements;

-- Audit events (only transactional entity types)
DELETE FROM audit_events 
WHERE entity_type IN (
    'Ticket', 'Payment', 'CashSession', 'OrderLine', 
    'KitchenOrder', 'TableSession', 'Shift', 'PurchaseOrder'
);

-- Override audit entries
DELETE FROM override_audit_entries;

-- Session audit entries
DELETE FROM session_audit_entries;

-- Alerts
DELETE FROM alerts;

-- Performance metrics
DELETE FROM performance_metrics;
