Ticket: TICKET-S002
Title
Inventory Sidebar UX Overhaul — Categorization, Search, Virtualization, Bulk Edit, Quick Filters
Branch
feature/inventory-ux-overhaul
Description
The current inventory sidebar renders a flat list of 2000+ items causing performance issues and poor usability. This ticket delivers five coordinated improvements that together make the inventory sidebar production-ready for high-volume use.
The five features are tightly coupled — they share the same data pipeline, the same ViewModel state, and the same XAML control. They must be implemented as a single coherent unit, not five independent changes.
Feature 1: Category Hierarchy
Add a Category property to inventory items. Replace the flat list with a grouped TreeView or grouped ItemsControl that allows drill-down by category. Users should see 10–20 items at a time, not 2000. Categories must be expandable and collapsible.
Feature 2: Real-Time Search with Fuzzy Filtering
Add a search TextBox above the inventory list. Filter the collection in real-time as the user types. Search must be case-insensitive and match against both item name and SKU/Code. Filtering must be asynchronous so the UI remains responsive with 2000+ items.
Feature 3: UI Virtualization
The inventory list must only render visible items. Implement UI virtualization using the appropriate WinUI 3 panel virtualization settings. Data fetching from the repository must use pagination — fetch in batches, not all at once. Initial load time must remain under 1 second for 2000+ items.
Feature 4: Bulk Edit Mode
Add a checkbox to each inventory item row. When multiple items are selected, show a bulk action bar. When confirmed, open a DataGrid (CommunityToolkit.WinUI DataGrid) populated with only the selected items, allowing inline editing of quantity and reorder point in a spreadsheet-style interface. Changes must persist via the Application layer — not direct DB calls.
Feature 5: Quick Filter Ribbon
Add a filter ribbon above the list with these filter chips: All, Low Stock (StockQuantity <= ReorderPoint), Out of Stock (StockQuantity == 0), Recently Added. Selecting a filter instantly narrows the visible list. Filters compose with the search box — both active at the same time must work correctly.
Acceptance Criteria

 Inventory items have a Category property persisted in the database
 Sidebar displays items grouped by category with expand/collapse
 Search box filters by name and SKU in real-time without UI freeze
 List is virtualized — memory usage does not scale with total item count
 Repository fetches inventory in pages, not all at once
 Bulk edit mode activates when 2+ items are checked
 Bulk edit DataGrid allows inline quantity and reorder point edits
 Bulk edits persist correctly via Application layer commands
 Filter ribbon chips work: All, Low Stock, Out of Stock, Recently Added
 Search and filter chips compose correctly when both are active
 dotnet build passes with 0 errors
 All new behavior covered by Domain and Application tests
 Existing 144/156 passing tests remain passing
 XAML compiles cleanly in Visual Studio Insider

Technical Constraints

Category is a Domain concept — it must be defined in the Domain layer as a value object or entity, not a string hardcoded in the ViewModel
All data access goes through the Application layer — no repository calls from ViewModels
Bulk edit persistence must use a Command + Handler — not direct EF calls from the UI
Virtualization is a pure UI concern — no changes to Domain or Application for this feature
Search filtering is a ViewModel concern — filter logic lives in the ViewModel, not in a repository query unless the dataset requires server-side filtering
Any new ViewModel files must be under 300 lines — extract partials if needed
Do not modify OrderPageViewModel.cs, Ticket.cs, or SalesReportRepository.cs unless directly required by this ticket

Out of Scope

Authentication or permission changes
Changes to any other page or sidebar outside inventory
Supabase integration (project uses PostgreSQL via EF Core)
Any changes to the financial, payment, or ticket subsystems