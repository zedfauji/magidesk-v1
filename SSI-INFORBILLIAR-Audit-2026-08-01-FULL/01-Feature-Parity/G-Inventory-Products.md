# Category G: Inventory & Products

## G.1 Product catalog

**Feature ID:** G.1  
**Feature Name:** Product catalog  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuItems table exists
- Domain entities: `MenuItem.cs` - full implementation
- Services: `IMenuItemRepository`, CRUD operations
- APIs / handlers: Menu queries and commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `OrderEntryPage.xaml` displays menu items
- ViewModels: `OrderEntryViewModel.cs` loads menu
- Navigation path: Order Entry → Menu Grid
- User-visible workflow: Grid of items for selection

**Notes:**
- MenuItem entity has Name, Description, Price, Barcode, CategoryId
- Supports modifiers via MenuItemModifierGroup

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Maintain current implementation

---

## G.2 Product categories

**Feature ID:** G.2  
**Feature Name:** Product categories  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuCategories table
- Domain entities: `MenuCategory.cs` with Name, DisplayIndex, IsActive
- Services: `IMenuCategoryRepository`
- APIs / handlers: Category queries
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Category tabs/buttons in Order Entry
- ViewModels: Categories loaded in OrderEntryViewModel
- Navigation path: Order Entry → Category Selection
- User-visible workflow: Category filtering

**Notes:**
- Hierarchical with MenuGroup for sub-categories

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Maintain current implementation

---

## G.3 Enable / disable products

**Feature ID:** G.3  
**Feature Name:** Enable / disable products  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuItem.IsActive column
- Domain entities: `MenuItem.cs` - IsActive property
- Services: Activate/Deactivate methods in repository
- APIs / handlers: Commands exist
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Admin menu editor (partial)
- ViewModels: Toggle logic exists
- Navigation path: Back Office → Menu
- User-visible workflow: Toggle switch for items

**Notes:**
- Disabled items not shown in Order Entry

**Risks / Gaps:**
- UI for enabling/disabling incomplete

**Recommendation:** VERIFY - Ensure admin UI exists for toggles

---

## G.4 Premium product types

**Feature ID:** G.4  
**Feature Name:** Premium product types  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuItem has properties
- Domain entities: No explicit PremiumType field
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Could be represented via categories
- No special pricing logic for premium types

**Risks / Gaps:**
- Cannot differentiate premium items visually
- No special handling in billing

**Recommendation:** EXTEND - Add visual indicators for premium items

---

## G.5 Inventory tracking

**Feature ID:** G.5  
**Feature Name:** Inventory tracking  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: InventoryItems table, StockQuantity
- Domain entities: `InventoryItem.cs` with StockQuantity, ReorderPoint
- Services: `AdjustStock` method exists
- APIs / handlers: `InventoryAdjustment` entity
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND for inventory UI
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Backend entities exist but UI missing
- RecipeLine links MenuItem to InventoryItem

**Risks / Gaps:**
- Cannot manage inventory via UI
- Stock not auto-deducted on sale

**Recommendation:** IMPLEMENT - Add inventory management UI, auto-deduction

---

## G.6 Minimum stock alerts

**Feature ID:** G.6  
**Feature Name:** Minimum stock alerts  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: InventoryItem.ReorderPoint
- Domain entities: `InventoryItem.cs` has ReorderPoint
- Services: NO EVIDENCE FOUND for alert generation
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Data field exists but no alerting logic

**Risks / Gaps:**
- Staff won't know when to reorder
- Manual tracking required

**Recommendation:** IMPLEMENT - Add alert notification when below reorder point

---

## G.7 Inventory physical count mode

**Feature ID:** G.7  
**Feature Name:** Inventory physical count mode  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Would freeze normal operations
- Count entry UI needed

**Risks / Gaps:**
- Cannot do formal inventory counts

**Recommendation:** IMPLEMENT - Add count mode with variance reporting

---

## G.8 Stock reconciliation

**Feature ID:** G.8  
**Feature Name:** Stock reconciliation  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Requires count mode first
- Compare counted vs expected

**Risks / Gaps:**
- Cannot identify shrinkage

**Recommendation:** IMPLEMENT - Add reconciliation workflow

---

## G.9 Theft prevention indicators

**Feature ID:** G.9  
**Feature Name:** Theft prevention indicators  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Variance tracking, anomaly detection
- Requires inventory tracking first

**Risks / Gaps:**
- No theft detection

**Recommendation:** DEFER - Implement after inventory tracking complete

---

## G.10 Barcode scanning for products

**Feature ID:** G.10  
**Feature Name:** Barcode scanning for products  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuItem.Barcode exists
- Domain entities: `MenuItem.cs` has Barcode property
- Services: Search by barcode in queries
- APIs / handlers: Item search includes barcode
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `ItemSearchDialog.xaml` - "Search by Name or Barcode"
- ViewModels: `ItemSearchViewModel.cs` filters by Barcode
- Navigation path: Order Entry → Search → Scan/Type barcode
- User-visible workflow: Type barcode, item found

**Notes:**
- Keyboard-wedge scanner supported
- Barcode input in search box

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Works as expected

---

## G.11 Supplier association

**Feature ID:** G.11  
**Feature Name:** Supplier association  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: PurchaseOrder references supplier-like data
- Domain entities: `PurchaseOrder.cs` exists
- Services: NO EVIDENCE FOUND for supplier management
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Purchase orders exist but no Supplier entity
- Basic structure present

**Risks / Gaps:**
- Cannot manage vendor relationships
- Manual reordering

**Recommendation:** IMPLEMENT - Add Supplier entity and management UI

---

## G.12 Cost vs sale price tracking

**Feature ID:** G.12  
**Feature Name:** Cost vs sale price tracking  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuItem.Price exists (sale price)
- Domain entities: No explicit Cost field on MenuItem
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Sale price tracked, cost price missing
- Cannot calculate margins

**Risks / Gaps:**
- No profitability analysis
- Cannot price items properly

**Recommendation:** IMPLEMENT - Add Cost field to MenuItem, margin reports

---

## Category G COMPLETE

- Features audited: 12
- Fully implemented: 4
- Partially implemented: 5
- Not implemented: 3
