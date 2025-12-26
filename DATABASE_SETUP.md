# Magidesk POS - Database Setup

## PostgreSQL Configuration

### Current Status
- **Primary dev DB**: `magidesk_pos` (exists)
- **Integration-test DB**: `magidesk_test` (tests create/drop as needed)
- **User**: `postgres` (password-based in tests by default)
- **Connection**: Local PostgreSQL server

### Existing Tables
The database already contains some tables:
- `Orders`
- `OrderItems`
- `payments`
- `shifts`
- `tables`
- `Users`
- And others...

### Decision Required
**Question**: Should we:
1. **Option A**: Use existing tables and migrate them to our domain model?
2. **Option B**: Create new tables with our domain model and ignore existing ones?
3. **Option C**: Create new schema (e.g., `magidesk`) and use that?

**Recommendation**: Option C - Create new schema to avoid conflicts and maintain clean separation.

## Database Schema Plan

### Schema: `magidesk` (proposed)

#### Core Tables
- `Tickets` (replaces Orders)
- `OrderLines` (replaces OrderItems)
- `Payments` (enhances existing payments)
- `CashSessions` (new, replaces shifts concept)
- `Shifts` (reference data)
- `OrderTypes` (reference data)
- `Tables` (restaurant tables)
- `AuditEvents` (new)

#### Reference Data Tables
- `MenuItems`
- `MenuCategories`
- `MenuGroups`
- `MenuModifiers`
- `ModifierGroups`
- `Discounts`
- `TaxGroups`
- `Taxes`
- `Users`
- `UserTypes`

#### Payment Tables
- `CashTransactions`
- `CreditCardTransactions`
- `DebitCardTransactions`
- `GiftCertificateTransactions`
- `CustomPaymentTransactions`
- `RefundTransactions`

#### Cash Management Tables
- `Payouts`
- `CashDrops`
- `DrawerBleeds`

#### Other Tables
- `Gratuities`
- `TicketDiscounts`
- `OrderLineDiscounts`
- `OrderLineModifiers`
- `CookingInstructions`

## EF Core Migration Strategy

1. **Initial Migration**: Create all tables in `magidesk` schema
2. **Incremental Migrations**: Add tables/columns as features are implemented
3. **Data Migrations**: Seed initial data (order types, shifts, admin user)

## Connection String

### Integration Tests (Magidesk.Infrastructure.Tests)

The repository integration tests use a local Postgres database named `magidesk_test` and will recreate schema during runs.

```
Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres;
```

### Dev DB (local)

```
Host=localhost;Port=5432;Database=magidesk_pos;Username=postgres;Password=;
```

Or using Npgsql format:
```
Server=localhost;Port=5432;Database=magidesk_pos;User Id=postgres;Password=;
```

## Next Steps

1. **Decide on schema approach** (new schema recommended)
2. **Create initial EF Core DbContext**
3. **Create first migration** (will create schema and tables)
4. **Seed initial data**
5. **Test connection and migrations**

## Database Naming Conventions

- **Tables**: PascalCase (e.g., `Tickets`, `OrderLines`)
- **Columns**: PascalCase (e.g., `Id`, `TicketNumber`, `CreatedAt`)
- **Foreign Keys**: `{Entity}Id` (e.g., `TicketId`, `UserId`)
- **Indexes**: `IX_{Table}_{Columns}` (e.g., `IX_Tickets_TicketNumber`)
- **Primary Keys**: `PK_{Table}` (e.g., `PK_Tickets`)

## Constraints

- **Primary Keys**: All tables use `Guid` as primary key
- **Foreign Keys**: All foreign keys have proper constraints
- **Unique Constraints**: Ticket numbers, payment transaction IDs
- **Check Constraints**: Amounts >= 0, quantities > 0
- **Default Values**: Timestamps, status enums

