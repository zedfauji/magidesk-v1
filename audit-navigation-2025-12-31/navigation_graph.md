# Navigation Graph (Current App)

**Source of truth**: derived from explicit navigation calls and XAML command bindings captured in `navigation_entries.md`.

## Mermaid graph
See `navigation_graph.mmd`.

## High-level flows
## Startup / authentication
- **Startup → LoginPage** (App launch)
- **LoginPage → SwitchboardPage** (successful login)

## Root navigation (MainWindow sidebar)
- **Sidebar → SwitchboardPage** (Home)
- **Sidebar → CashSessionPage**
- **Sidebar → TicketPage**
- **Sidebar → PaymentPage**
- **Sidebar → DiscountTaxPage**
- **Sidebar → PrintPage**
- **Sidebar → TicketManagementPage**
- **Sidebar → DrawerPullReportDialog** (dialog)
- **Sidebar → SalesReportsPage**
- **Sidebar → UserManagementPage**
- **Sidebar → SettingsPage**

## Switchboard (primary operational hub)
- **SwitchboardPage → OrderEntryPage** via `NewTicketCommand` flow:
  - OrderTypeSelectionDialog (dialog)
  - GuestCountDialog (dialog, conditional)
  - ShiftStartDialog (dialog, conditional)
  - then navigation to OrderEntryPage
- **SwitchboardPage → OrderEntryPage** via Edit Selected
- **SwitchboardPage → TicketManagementPage** (fallback when Edit Selected has no selection)
- **SwitchboardPage → TableMapPage**
- **SwitchboardPage → KitchenDisplayPage**
- **SwitchboardPage → ManagerFunctionsDialog**
- **SwitchboardPage → LoginPage** (logout)

## Manager Functions dialog
- **ManagerFunctionsDialog → DrawerPullReportDialog**
- **ManagerFunctionsDialog → CashDropManagementDialog → CashEntryDialog**
- **ManagerFunctionsDialog → OpenTicketsListDialog**
- **ManagerFunctionsDialog → GroupSettleTicketSelectionWindow → GroupSettleTicketDialog**
- **ManagerFunctionsDialog → BackOfficePage**
- **ManagerFunctionsDialog → SystemConfigPage**
- **ManagerFunctionsDialog → ShiftEndDialog**

## Open Tickets List dialog
- **OpenTicketsListDialog → OrderEntryPage** (Resume)
- **OpenTicketsListDialog → VoidTicketDialog** (Void)
- **OpenTicketsListDialog → inline ContentDialog** (Transfer user selection)
- **OpenTicketsListDialog → SplitTicketDialog** (exists in ViewModel; no binding in XAML)

## TicketPage (Ticket Entry)
- **TicketPage → SplitTicketDialog**
- **TicketPage → TableMapPage** (Move table)
- **TicketPage → SettlePage**
- **TicketPage → ModifierSelectionDialog (Presentation)** during Add Order Line

## TableMapPage
- **TableMapPage → OrderEntryPage** (select table: existing ticket or create new)

## OrderEntryPage
- **OrderEntryPage → SettlePage**
- **OrderEntryPage → SplitTicketDialog**
- **OrderEntryPage → MergeTicketsDialog**
- **OrderEntryPage → CustomerSelectionDialog**
- **OrderEntryPage → TableSelectionDialog**
- **OrderEntryPage → CookingInstructionDialog**
- **OrderEntryPage → ModifierSelectionDialog (Dialogs)**
- **OrderEntryPage → AddOnSelectionDialog**
- **OrderEntryPage → ComboSelectionDialog**
- **OrderEntryPage → PizzaModifierDialog**
- **OrderEntryPage → TicketFeeDialog**
- **OrderEntryPage → SeatSelectionDialog**
- **OrderEntryPage → MiscItemDialog**
- **OrderEntryPage → SizeSelectionDialog**
- **OrderEntryPage → ItemSearchDialog**
- **OrderEntryPage → PriceEntryDialog**
- **OrderEntryPage → QuantityDialog**

## SettlePage
- **SettlePage → LoginPage** (logout)
- **SettlePage → SwipeCardDialog → AuthorizationCodeDialog/PaymentProcessWaitDialog** (payment flows)

## BackOfficePage (internal frame navigation)
- **BackOfficePage → (MenuEditorPage | ModifierEditorPage | InventoryPage | TableMapPage | UserManagementPage | DiscountTaxPage | OrderTypeExplorerPage | ShiftExplorerPage | SalesReportsPage | SettingsPage | SystemConfigPage)**
- **BackOfficePage → AuthorizationCaptureBatchDialog**

## Notes / UNKNOWN
- Many flows return using `GoBack()` (root frame back stack). Exact return destination depends on runtime navigation history and is not encoded as explicit edges.
