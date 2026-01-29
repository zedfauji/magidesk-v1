# Database Table Structure
**Source:** Live Database Inspection (PostgreSQL)
**Schema:** `public`

## Core Tables
| Table Name | Description |
|------------|-------------|
| `Tickets` | Headers for Orders/Transactions |
| `OrderLines` | Items within a Ticket |
| `Payments` | Payment records linked to Tickets |
| `AuditEvents` | Security and Operation logs |
| `Shifts` | Employee work periods |
| `Users` | System users/employees |

## Menu & Catalog
| Table Name | Description |
|------------|-------------|
| `MenuItems` | Product definitions |
| `MenuCategories` | Grouping for items |
| `MenuGroups` | Top-level groups (e.g. Food, Bar) |
| `MenuItemRecipeLines` | Inventory consumption rules (Owned Collection) |
| `ModifierGroups` | Collections of modifiers |
| `MenuModifiers` | Individual modifier options |

## Kitchen Display (KDS)
| Table Name | Description |
|------------|-------------|
| `KitchenOrders` | Grouped items sent to kitchen |
| `KitchenOrderItems` | Individual items in a kitchen order |

## Inventory
| Table Name | Description |
|------------|-------------|
| `InventoryItems` | Raw materials / Stock items |
| `StockMovements` | Ledger of stock changes |
| `Vendors` | Suppliers |
| `PurchaseOrders` | Orders to suppliers |

## Hardware & Config
| Table Name | Description |
|------------|-------------|
| `Terminals` | Register identifiers |
| `PrinterGroups` | Logical Printers (e.g. "Hot Kitchen") |
| `PrinterMappings` | Physical Printer mappings per Terminal |

## Legacy / Unmapped Tables (Drift)
| Table Name | Status |
|------------|--------|
| `MenuItemPrices` | **Unmapped** in current `MenuItemConfiguration`. Likely deprecated. |
| `PriceLevels` | **Unmapped**. |
| `MenuItemModifierGroups` | Join table (Many-to-Many). |
