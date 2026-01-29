# Database Contexts & Configuration

## Contexts
The application uses a single primary `DbContext`:

### `Magidesk.Infrastructure.Data.ApplicationDbContext`
*   **Database Provider:** PostgreSQL (via `Npgsql.EntityFrameworkCore.PostgreSQL`)
*   **Schema:** `public` (default)
*   **Migration History Table:** `__EFMigrationsHistory`

## Entity Configuration
Calls `modelBuilder.ApplyConfiguration` for the following entities (located in `Magidesk.Infrastructure.Data.Configurations`):

*   **Core POS:** `Ticket`, `OrderLine`, `Payment`, `Shift`, `Terminal`, `User`, `Role`
*   **Inventory:** `InventoryItem`, `StockMovement`, `Supplier` (Vendor), `PurchaseOrder`
*   **Menu:** `MenuItem`, `MenuCategory`, `Modifier`, `ComboDefinition`
*   **KDS:** `KitchenOrder`, `KitchenOrderItem`
*   **Hardware:** `PrinterGroup`, `PrinterMapping`

## Expected Schema (Code-First)
The Code-First model expects the following logical relationships:
*   **Tickets** 1:N **OrderLines**
*   **Tickets** 1:N **Payments**
*   **MenuItems** 1:N **RecipeLines** (`MenuItemRecipeLines` table via `OwnsMany`)
*   **MenuItems** 1:N **ModifierGroups**

## Observed Drift
*   **`MenuItemPrices`**: Table exists in database but is **NOT** present in `MenuItemConfiguration.cs` (which maps `Price` to columns on `MenuItems`). This suggests a legacy table or a feature branch remnant (potentially for Multi-Price/Price Levels).
