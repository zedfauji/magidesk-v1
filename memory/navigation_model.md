# System Patterns: Magidesk Navigation Model (FloreantPOS-aligned)

## System Architecture
Magidesk navigation is organized into two primary domains mirroring FloreantPOS:
- **Operational Domain**: Primary operator workflows (order entry, table service, kitchen display, cash management)
- **BackOffice Domain**: Administrative, reporting, configuration, and system management

## Key Technical Decisions
- **Root Navigation**: MainWindow sidebar should expose only Operational domain at root level
- **BackOffice Entry**: Should be guarded and separated (analogous to FloreantPOS BackOfficeWindow)
- **Default View Routing**: Implement FloreantPOS-like default view selection based on terminal/order type configuration
- **Permission Guards**: BackOffice entry and manager functions require role-based permissions

## Design Patterns in Use
- **Domain Separation**: Clear boundary between Operational and BackOffice domains
- **Dialog-First for Blocking Actions**: Short-lived dialogs for selections/confirmations
- **Hub-and-Spoke for BackOffice**: BackOfficePage as hub with internal NavigationView to sub-pages
- **Canonical Home**: SwitchboardPage as the single operator home (analogous to Floreant SwitchboardView/HomeScreen)

## Component Relationships
- **Operational Flow**: SwitchboardPage → OrderEntryPage/SettlePage/TableMapPage/KitchenDisplayPage
- **Manager Functions**: SwitchboardPage → ManagerFunctionsDialog → (OpenTicketsListDialog, CashDropManagementDialog, DrawerPullReportDialog, BackOfficePage)
- **BackOffice Navigation**: BackOfficePage internal NavigationView → (MenuEditorPage, ModifierEditorPage, InventoryPage, UserManagementPage, DiscountTaxPage, OrderTypeExplorerPage, ShiftExplorerPage, SalesReportsPage, SettingsPage, SystemConfigPage)

## Critical Implementation Paths
- **Startup Routing**: App startup → LoginPage → SwitchboardPage (Operational home)
- **Default View Policy**: SwitchboardPage (default) with overrides for KDS/TableMap/OrderEntry based on configuration
- **New Ticket Flow**: OrderTypeSelectionDialog → conditional GuestCount/ShiftStart → OrderEntryPage
- **Resume Ticket Flow**: SwitchboardPage → OrderEntryPage (with TicketId) OR OpenTicketsListDialog → OrderEntryPage
- **BackOffice Entry**: ManagerFunctionsDialog → BackOfficePage (permission-gated)
- **Logout**: Any domain → LoginPage

## Domain Assignments

### Operational Domain
**Core Pages**: SwitchboardPage, OrderEntryPage, TableMapPage, KitchenDisplayPage, SettlePage

**Operational Dialogs**: OrderTypeSelectionDialog, GuestCountDialog, ShiftStartDialog, CustomerSelectionDialog, TableSelectionDialog, CookingInstructionDialog, ModifierSelectionDialog (Presentation), AddOnSelectionDialog, ComboSelectionDialog, PizzaModifierDialog, TicketFeeDialog, SeatSelectionDialog, MiscItemDialog, SizeSelectionDialog, ItemSearchDialog, PriceEntryDialog, QuantityDialog, MergeTicketsDialog, SplitTicketDialog, VoidTicketDialog, SwipeCardDialog, AuthorizationCodeDialog, PaymentProcessWaitDialog

**Manager-Level Operational Dialogs**: OpenTicketsListDialog, CashDropManagementDialog, CashEntryDialog, DrawerPullReportDialog, ManagerFunctionsDialog, GroupSettleTicketSelectionWindow, GroupSettleTicketDialog, ShiftEndDialog

### BackOffice Domain
**Hub Page**: BackOfficePage

**Sub-Pages**: MenuEditorPage, ModifierEditorPage, InventoryPage, UserManagementPage, DiscountTaxPage, OrderTypeExplorerPage, ShiftExplorerPage, SalesReportsPage, SettingsPage, SystemConfigPage

**Dialogs**: AuthorizationCaptureBatchDialog

### Excluded from Primary Domains
**Authentication**: LoginPage

**Utility/Debug**: CashSessionPage, TicketPage, PaymentPage, TicketManagementPage, PrintPage

**Orphans**: MainPage, PasswordEntryDialog

## Usage Guidelines
- Use this model as the source of truth for navigation refactoring
- Do not expose BackOffice pages directly at root navigation level
- Ensure permission guards exist for BackOffice entry and manager functions
- Maintain SwitchboardPage as the single operator home
- Keep operational flows within the Operational domain
- Use dialogs for short-lived blocking interactions
- Reserve BackOffice for administrative/configuration tasks