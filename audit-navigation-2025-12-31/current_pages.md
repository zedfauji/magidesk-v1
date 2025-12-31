# Current App UI Surface Inventory (Magidesk)

**Scope**: This inventory is derived from the current codebase XAML files under `Magidesk/` excluding build output (`obj/`, `bin/`, `.vs/`). Classification is based on each XAML root element (`Window`, `Page`, `ContentDialog`) and its `x:Class`.

## Windows
- **Main Window**
  - **File**: `MainWindow.xaml`
  - **Root**: `Window`
  - **x:Class**: `Magidesk.Presentation.MainWindow`
  - **Notes**: Hosts a `NavigationView` sidebar and a root `Frame` (`ContentFrame`).

## Pages (`<Page ...>`)
- **Login**
  - **File**: `Views/LoginPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.LoginPage`

- **Switchboard (Quick Actions / Open Tickets list)**
  - **File**: `Views/SwitchboardPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.SwitchboardPage`

- **Cash Session**
  - **File**: `Views/CashSessionPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.CashSessionPage`

- **Ticket Entry (debug/utility surface)**
  - **File**: `Views/TicketPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.TicketPage`

- **Payments (debug/utility surface)**
  - **File**: `Views/PaymentPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.PaymentPage`

- **Discount & Tax**
  - **File**: `Views/DiscountTaxPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.DiscountTaxPage`

- **Printing**
  - **File**: `Views/PrintPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.PrintPage`

- **Ticket Management (debug/utility surface)**
  - **File**: `Views/TicketManagementPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.TicketManagementPage`

- **Drawer Pull Report (hosted as dialog; not a page)**
  - **Not applicable**

- **Sales Reports**
  - **File**: `Views/SalesReportsPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.SalesReportsPage`

- **User Management (placeholder page)**
  - **File**: `Views/UserManagementPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.UserManagementPage`

- **Settings (general app settings)**
  - **File**: `Views/SettingsPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.SettingsPage`

- **System Configuration**
  - **File**: `Views/SystemConfigPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.SystemConfigPage`

- **Back Office (admin hub page; contains its own `NavigationView` + `Frame`)**
  - **File**: `Views/BackOfficePage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.BackOfficePage`

- **Menu Editor (Back Office sub-page)**
  - **File**: `Views/MenuEditorPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.MenuEditorPage`

- **Modifier Editor (Back Office sub-page)**
  - **File**: `Views/ModifierEditorPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.ModifierEditorPage`

- **Inventory (Back Office sub-page)**
  - **File**: `Views/InventoryPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.InventoryPage`

- **Order Type Explorer (Back Office sub-page)**
  - **File**: `Views/OrderTypeExplorerPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.OrderTypeExplorerPage`

- **Shift Explorer (Back Office sub-page)**
  - **File**: `Views/ShiftExplorerPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.ShiftExplorerPage`

- **Table Map**
  - **File**: `Views/TableMapPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.TableMapPage`

- **Kitchen Display**
  - **File**: `Views/KitchenDisplayPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.KitchenDisplayPage`

- **Order Entry**
  - **File**: `Views/OrderEntryPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.OrderEntryPage`

- **Settle**
  - **File**: `Views/SettlePage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.SettlePage`

- **MainPage (template/Hello World)**
  - **File**: `Views/MainPage.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.MainPage`

## ContentDialogs (`<ContentDialog ...>`)
### High-level dialogs (under `Views/`)
- **Manager Functions**
  - **File**: `Views/ManagerFunctionsDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.ManagerFunctionsDialog`

- **Open Tickets List**
  - **File**: `Views/OpenTicketsListDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.OpenTicketsListDialog`

- **Cash Drop Management**
  - **File**: `Views/CashDropManagementDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.CashDropManagementDialog`

- **Cash Entry (drawer ops input)**
  - **File**: `Views/CashEntryDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.CashEntryDialog`

- **Drawer Pull Report**
  - **File**: `Views/DrawerPullReportDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.DrawerPullReportDialog`

- **Void Ticket**
  - **File**: `Views/VoidTicketDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.VoidTicketDialog`

- **Split Ticket**
  - **File**: `Views/SplitTicketDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.SplitTicketDialog`

- **Split Ticket (variants present in repo)**
  - **File**: `Views/SplitTicketDialog_Backup.xaml`
  - **Root**: `ContentDialog`
  - **Notes**: variant file; no inbound navigation discovered in scanned call sites.
  - **File**: `Views/SplitTicketDialog_Fixed.xaml`
  - **Root**: `ContentDialog`
  - **Notes**: variant file; no inbound navigation discovered in scanned call sites.
  - **File**: `Views/SplitTicketDialog_Minimal.xaml`
  - **Root**: `ContentDialog`
  - **Notes**: variant file; no inbound navigation discovered in scanned call sites.

- **Group Settle Ticket Selection**
  - **File**: `Views/GroupSettleTicketSelectionWindow.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Presentation.Views.GroupSettleTicketSelectionWindow`

- **Group Settle Ticket**
  - **File**: `Views/GroupSettleTicketDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Presentation.Views.GroupSettleTicketDialog`

- **Modifier Selection (menu-item based; Presentation namespace)**
  - **File**: `Views/ModifierSelectionDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Presentation.Views.ModifierSelectionDialog`

- **Order Type Selection**
  - **File**: `Views/OrderTypeSelectionDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Presentation.Views.OrderTypeSelectionDialog`

- **Payment Process Wait**
  - **File**: `Views/PaymentProcessWaitDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Views.PaymentProcessWaitDialog`

- **Swipe Card**
  - **File**: `Views/SwipeCardDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Views.SwipeCardDialog`

- **Authorization Code**
  - **File**: `Views/AuthorizationCodeDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Views.AuthorizationCodeDialog`

- **Authorization Capture Batch**
  - **File**: `Views/AuthorizationCaptureBatchDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Views.AuthorizationCaptureBatchDialog`

- **Password Entry**
  - **File**: `Views/PasswordEntryDialog.xaml`
  - **Root**: `ContentDialog`
  - **x:Class**: `Magidesk.Presentation.Views.PasswordEntryDialog`

### Order-entry related dialogs (under `Views/Dialogs/`)
- **Add-On Selection**
  - **File**: `Views/Dialogs/AddOnSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.AddOnSelectionDialog`

- **Combo Selection**
  - **File**: `Views/Dialogs/ComboSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.ComboSelectionDialog`

- **Cooking Instruction**
  - **File**: `Views/Dialogs/CookingInstructionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.CookingInstructionDialog`

- **Customer Selection**
  - **File**: `Views/Dialogs/CustomerSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.CustomerSelectionDialog`

- **Guest Count**
  - **File**: `Views/Dialogs/GuestCountDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.Dialogs.GuestCountDialog`

- **Item Search**
  - **File**: `Views/Dialogs/ItemSearchDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.ItemSearchDialog`

- **Merge Tickets**
  - **File**: `Views/Dialogs/MergeTicketsDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.MergeTicketsDialog`

- **Misc Item**
  - **File**: `Views/Dialogs/MiscItemDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.MiscItemDialog`

- **Modifier Selection (view-model driven; Magidesk.Views.Dialogs namespace)**
  - **File**: `Views/Dialogs/ModifierSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.ModifierSelectionDialog`

- **Pizza Modifier**
  - **File**: `Views/Dialogs/PizzaModifierDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.PizzaModifierDialog`

- **Price Entry**
  - **File**: `Views/Dialogs/PriceEntryDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.PriceEntryDialog`

- **Quantity**
  - **File**: `Views/Dialogs/QuantityDialog.xaml`
  - **x:Class**: `Magidesk.Views.QuantityDialog`

- **Seat Selection**
  - **File**: `Views/Dialogs/SeatSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.SeatSelectionDialog`

- **Shift Start**
  - **File**: `Views/Dialogs/ShiftStartDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.Dialogs.ShiftStartDialog`

- **Shift End**
  - **File**: `Views/Dialogs/ShiftEndDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.Dialogs.ShiftEndDialog`

- **Size Selection**
  - **File**: `Views/Dialogs/SizeSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.SizeSelectionDialog`

- **Table Selection**
  - **File**: `Views/Dialogs/TableSelectionDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.TableSelectionDialog`

- **Ticket Fee**
  - **File**: `Views/Dialogs/TicketFeeDialog.xaml`
  - **x:Class**: `Magidesk.Views.Dialogs.TicketFeeDialog`

- **Cash Entry (Dialogs folder)**
  - **File**: `Views/Dialogs/CashEntryDialog.xaml`
  - **x:Class**: `Magidesk.Presentation.Views.Dialogs.CashEntryDialog`
  - **Notes**: no inbound navigation discovered in scanned call sites.
