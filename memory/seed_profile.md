# FULL POS Seed Profile

- Generated: `2026-01-02T21:59:19.0821114Z`
- Target DB: `magidesk_new`
- Random seed: `42`
- History days: `60`

## How to re-run / reset

- **Reset + full seed**:

```bash
dotnet run --project Magidesk.Migrations -- --reset
```

- **Safety guard**: reset is blocked unless the database name is `magidesk_new` (use `--force` only if you *really* mean it).

## Seeded credentials (for end-to-end workflows)

- **owner**: username: owner, pin: 9999
- **manager**: username: manager, pin: 1234
- **cashier**: username: cashier, pin: 1111
- **server**: username: server, pin: 2222
- **kitchen**: username: kitchen, pin: 3333

## Seeded data (counts)

- **AttendanceHistories**: 120
- **AuditEvents**: 60
- **CashDrops**: 10
- **CashSessions**: 11
- **Discounts**: 4
- **DrawerBleeds**: 10
- **InventoryItems**: 20
- **KitchenOrderItems**: 664
- **KitchenOrders**: 173
- **MenuCategories**: 8
- **MenuGroups**: 13
- **MenuItemModifierGroups**: 31
- **MenuItems**: 95
- **MenuModifiers**: 22
- **MerchantGatewayConfigurations**: 2
- **ModifierGroups**: 7
- **OrderLineModifiers**: 508
- **OrderLines**: 2307
- **OrderTypes**: 5
- **Payments**: 622
- **Payouts**: 10
- **PrinterGroups**: 5
- **PrinterMappings**: 6
- **Roles**: 5
- **Shifts**: 4
- **Tables**: 34
- **Terminals**: 3
- **Tickets**: 643
- **Users**: 5

## Coverage notes

- **Master data**: Restaurant config (singleton), terminals, users/roles, printer groups/mappings, order types, shifts, tables, inventory.
- **Menu**: ~100+ items across drinks/appetizers/salads/burgers/pizzas/desserts/combos + menu modifier groups and modifiers.
- **Tickets**:
  - Open dine-in tickets today (with notes/discount snapshots on some).
  - Scheduled pickup tickets (future).
  - Historical closed tickets across the last N days (cash/card/split tender mix).
  - Voided tickets (manager).
  - Split tickets (order-line split).
  - Partial refunds on a subset of payments.
- **Kitchen/KDS**: Kitchen orders are created for a subset of historical tickets with mixed statuses.
- **Reports/Labor**: Attendance history for server + cashier across history range.

## Known limitations (schema / implementation constraints)

- **Tax rules**: The current schema is primarily item/ticket-level and the Domain ticket totals use a simplified calculation; the seed uses `TaxRate = 0` on order lines to avoid double-taxing. If/when a full tax-rule engine exists, the seed should be updated to match it.
- **Discount enforcement**: Manager-only / conditional discount authorization is not represented as a first-class DB relationship; discount snapshots are applied directly to tickets where needed.
- **Happy hour / time-based pricing**: Not modeled as first-class entities; not seeded beyond basic items/modifiers.

