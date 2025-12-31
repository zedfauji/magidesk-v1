# Navigation Entry Points (Current App)

**Scope**: Entry points are derived from explicit, code-level evidence in:
- `App.xaml.cs` (startup)
- `MainWindow.xaml(.cs)` (root sidebar `NavigationView`)
- `Views/*.xaml` (buttons/menus bound to commands)
- `Views/*.xaml.cs` (code-behind navigation)
- `ViewModels/*.cs` (commands calling `Navigate`, `GoBack`, `ShowDialogAsync`, or `ContentDialog.ShowAsync`)

For each entry, this file captures:
- **Source UI element** (and file)
- **Target surface**
- **Mechanism** (`NavigationService.Navigate`, `NavigationService.ShowDialogAsync`, `ContentDialog.ShowAsync`, `Frame.Navigate`)

## Startup
- **[Startup → Login]**
  - **Source**: `App.xaml.cs` `OnLaunched`
  - **Target**: `Views.LoginPage`
  - **Mechanism**: `NavigationService.Navigate(typeof(Views.LoginPage))`

## Root shell navigation (MainWindow)
### Sidebar menu items (`MainWindow.xaml` / `MainWindow.xaml.cs`)
- **[Home]**
  - **Source**: `NavigationViewItem Tag="home"`
  - **Target**: `Views.SwitchboardPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.SwitchboardPage))`

- **[Cash Session]**
  - **Source**: `NavigationViewItem Tag="cashSession"`
  - **Target**: `Views.CashSessionPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.CashSessionPage))`

- **[Ticket Entry]**
  - **Source**: `NavigationViewItem Tag="ticket"`
  - **Target**: `Views.TicketPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.TicketPage))`

- **[Payments]**
  - **Source**: `NavigationViewItem Tag="payments"`
  - **Target**: `Views.PaymentPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.PaymentPage))`

- **[Discount & Tax]**
  - **Source**: `NavigationViewItem Tag="discountTax"`
  - **Target**: `Views.DiscountTaxPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.DiscountTaxPage))`

- **[Printing]**
  - **Source**: `NavigationViewItem Tag="printing"`
  - **Target**: `Views.PrintPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.PrintPage))`

- **[Ticket Management]**
  - **Source**: `NavigationViewItem Tag="ticketMgmt"`
  - **Target**: `Views.TicketManagementPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.TicketManagementPage))`

- **[Drawer Pull Report]**
  - **Source**: `NavigationViewItem Tag="drawerPull"`
  - **Target**: `Views.DrawerPullReportDialog`
  - **Mechanism**: `_navigation.ShowDialogAsync(new Views.DrawerPullReportDialog())`

- **[Sales Reports]**
  - **Source**: `NavigationViewItem Tag="salesReports"`
  - **Target**: `Views.SalesReportsPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.SalesReportsPage))`

- **[User Management]**
  - **Source**: `NavigationViewItem Tag="userMgmt"`
  - **Target**: `Views.UserManagementPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.UserManagementPage))`

- **[Settings]**
  - **Source**: `NavigationViewItem Tag="settings"`
  - **Target**: `Views.SettingsPage`
  - **Mechanism**: `_navigation.Navigate(typeof(Views.SettingsPage))`

### Back navigation (global)
- **[Back button → Frame.GoBack]**
  - **Source**: `MainWindow.xaml` `NavigationView` back button (`BackRequested="OnBackRequested"`)
  - **Target**: previous page in `Frame` back stack
  - **Mechanism**: `_navigation.GoBack()`

## Login surface (`LoginViewModel`)
- **[Login success → Switchboard]**
  - **Source**: `LoginViewModel.LoginAsync()`
  - **Target**: `Views.SwitchboardPage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.SwitchboardPage))`

- **[Clock In/Out result dialog]**
  - **Source**: `LoginViewModel.ClockInOutAsync()`
  - **Target**: inline `ContentDialog` (attendance message)
  - **Mechanism**: `_navigationService.ShowDialogAsync(dialog)`

## Switchboard (`SwitchboardPage.xaml` / `SwitchboardViewModel`)
- **[New Ticket]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.NewTicketCommand}"`
  - **Targets / flow** (as implemented in `SwitchboardViewModel.NewTicketAsync`):
    - `Views.OrderTypeSelectionDialog` (dialog)
    - `Views.Dialogs.GuestCountDialog` (dialog, conditional)
    - `Views.Dialogs.ShiftStartDialog` (dialog, conditional)
    - `Views.OrderEntryPage` (page)
  - **Mechanism**: `_navigationService.ShowDialogAsync(...)` then `_navigationService.Navigate(typeof(Views.OrderEntryPage), result.TicketId)`

- **[Edit Selected]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.EditTicketCommand}"`
  - **Target**:
    - `Views.OrderEntryPage` (if `SelectedTicket != null`)
    - `Views.TicketManagementPage` (fallback when no selection)
  - **Mechanism**: `_navigationService.Navigate(...)`

- **[Table Map]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.TablesCommand}"`
  - **Target**: `Views.TableMapPage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.TableMapPage))`

- **[Kitchen Display]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.KitchenCommand}"`
  - **Target**: `Views.KitchenDisplayPage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.KitchenDisplayPage))`

- **[Manager]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.ManagerFunctionsCommand}"`
  - **Target**: `Views.ManagerFunctionsDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.ManagerFunctionsDialog())`

- **[Logout]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.LogoutCommand}"`
  - **Target**: `Views.LoginPage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.LoginPage))`

- **[Shutdown]**
  - **Source UI**: `SwitchboardPage.xaml` button `Command="{x:Bind ViewModel.ShutdownCommand}"`
  - **Target**: application exit
  - **Mechanism**: `Microsoft.UI.Xaml.Application.Current.Exit()`

- **[Settle / Drawer Pull / Cash Drop / Open Tickets etc. (commands exist)]**
  - **Source**: `SwitchboardViewModel` defines commands like `SettleCommand`, `DrawerPullCommand`, `PerformCashDropCommand`, `OpenTicketsListCommand`.
  - **UI binding evidence**: No occurrences of these command names in `SwitchboardPage.xaml` (no bindings found).

## Manager Functions dialog (`ManagerFunctionsDialog.xaml` / `ManagerFunctionsViewModel`)
- **[Drawer Pull]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding DrawerPullCommand}"`
  - **Target**: `Views.DrawerPullReportDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.DrawerPullReportDialog())`

- **[Cash Drop]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding CashDropCommand}"`
  - **Target**: `Views.CashDropManagementDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.CashDropManagementDialog())`

- **[Open Tickets]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding OpenTicketsCommand}"`
  - **Target**: `Views.OpenTicketsListDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.OpenTicketsListDialog())`

- **[Group Settle]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding GroupSettleCommand}"`
  - **Targets**:
    - `Views.GroupSettleTicketSelectionWindow` (dialog)
    - then `Views.GroupSettleTicketDialog` (dialog)
  - **Mechanism**: `_navigationService.ShowDialogAsync(...)` on both dialogs

- **[Reports]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Content="Tips Report"` bound to `ReportsCommand`
  - **Target**: `Views.BackOfficePage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.BackOfficePage))`

- **[System Config]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding SettingsCommand}"`
  - **Target**: `Views.SystemConfigPage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.SystemConfigPage))`

- **[End Shift]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding EndShiftCommand}"`
  - **Target**: `Views.Dialogs.ShiftEndDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.Dialogs.ShiftEndDialog(vm))`

- **[Clock In/Out]**
  - **Source UI**: `ManagerFunctionsDialog.xaml` button `Command="{Binding ClockInOutCommand}"`
  - **Target**: inline `ContentDialog` (Clock In/Clock Out choice) + inline success/error dialogs
  - **Mechanism**: `_navigationService.ShowDialogAsync(...)`

## Open Tickets List dialog (`OpenTicketsListDialog.xaml` / `OpenTicketsListViewModel`)
- **[Resume]**
  - **Source UI**: `OpenTicketsListDialog.xaml` button `Command="{Binding ResumeCommand}"`
  - **Target**: `Views.OrderEntryPage` (parameter: `SelectedTicket.Id`)
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.OrderEntryPage), SelectedTicket.Id)`

- **[Transfer]**
  - **Source UI**: `OpenTicketsListDialog.xaml` button `Command="{Binding TransferCommand}"`
  - **Target**: inline `ContentDialog` (user selection)
  - **Mechanism**: `_navigationService.ShowDialogAsync(userDialog)`

- **[Void]**
  - **Source UI**: `OpenTicketsListDialog.xaml` button `Command="{Binding VoidCommand}"`
  - **Target**: `Views.VoidTicketDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.VoidTicketDialog())`

- **[Split (reachable via ViewModel only)]**
  - **Source (code)**: `OpenTicketsListViewModel.SplitAsync()`
  - **Target**: `Views.SplitTicketDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new SplitTicketDialog())`
  - **UI binding evidence**: No `Split` button exists in `OpenTicketsListDialog.xaml` header (only Resume/Transfer/Void).

## Cash Drop Management dialog (`CashDropManagementDialog.xaml` / `CashDropManagementViewModel`)
- **[New Cash Drop]**
  - **Source UI**: button binding `AddCashDropCommand`
  - **Target**: `Views.CashEntryDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.CashEntryDialog(title, message))`

- **[New Drawer Bleed]**
  - **Source UI**: button binding `AddDrawerBleedCommand`
  - **Target**: `Views.CashEntryDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.CashEntryDialog(title, message))`

## Back Office (`BackOfficePage.xaml(.cs)` / `BackOfficeViewModel`)
- **[Back]**
  - **Source UI**: `BackOfficePage.xaml` CommandBar back button `Command="{x:Bind ViewModel.GoBackCommand}"`
  - **Target**: previous root-frame page
  - **Mechanism**: `_navigationService.GoBack()`

- **[Capture Batch]**
  - **Source UI**: `BackOfficePage.xaml` CommandBar button `Command="{x:Bind ViewModel.CaptureBatchCommand}"`
  - **Target**: `Magidesk.Views.AuthorizationCaptureBatchDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Magidesk.Views.AuthorizationCaptureBatchDialog())`

- **[Back Office internal navigation]**
  - **Source**: `BackOfficePage.xaml.cs` `NavigationView_SelectionChanged`
  - **Target**: One of:
    - `MenuEditorPage`, `ModifierEditorPage`, `InventoryPage`, `TableMapPage`, `UserManagementPage`, `DiscountTaxPage`, `OrderTypeExplorerPage`, `ShiftExplorerPage`, `SalesReportsPage`, `SettingsPage`, `SystemConfigPage`
  - **Mechanism**: `ContentFrame.Navigate(item.PageType)`

- **[Back Office stub dialog]**
  - **Source**: `BackOfficePage.xaml.cs` `ShowNotImplementedAsync(title)`
  - **Target**: inline `ContentDialog` ("Not implemented yet")
  - **Mechanism**: `await dialog.ShowAsync()`

## Ticket Entry page (`TicketPage.xaml` / `TicketViewModel`)
- **[Split Ticket]**
  - **Source UI**: `TicketPage.xaml` button `Command="{Binding SplitTicketUiCommand}"`
  - **Target**: `Views.SplitTicketDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.SplitTicketDialog())`

- **[Move Table]**
  - **Source UI**: `TicketPage.xaml` button `Command="{Binding MoveTableUiCommand}"`
  - **Target**: `Views.TableMapPage` (parameter: `Ticket.Id`)
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.TableMapPage), Ticket.Id)`

- **[Settle]**
  - **Source UI**: `TicketPage.xaml` button `Command="{Binding SettleUiCommand}"`
  - **Target**: `Views.SettlePage` (parameter: `Ticket.Id`)
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.SettlePage), Ticket.Id)`

- **[Modifier selection (during Add Order Line)]**
  - **Source (code)**: `TicketViewModel.AddOrderLineAsync()`
  - **Target**: `Views.ModifierSelectionDialog` (Presentation namespace)
  - **Mechanism**: `_navigationService.ShowDialogAsync(new Views.ModifierSelectionDialog(menuItem))`

## Table Map page (`TableMapViewModel`)
- **[Select table → Order Entry]**
  - **Source**: `TableMapViewModel.SelectTableAsync()`
  - **Target**: `Views.OrderEntryPage` (parameter: existing ticket id OR none for new)
  - **Mechanism**: `_navigationService.Navigate(typeof(OrderEntryPage), ...)`

## Order Entry page (`OrderEntryViewModel`)
- **[Settle ticket]**
  - **Source**: `OrderEntryViewModel.Settle()`
  - **Target**: `Views.SettlePage` (parameter: `Ticket.Id`)
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.SettlePage), Ticket.Id)`

- **[Split ticket]**
  - **Source**: `OrderEntryViewModel.SplitTicketAsync()`
  - **Target**: `Views.SplitTicketDialog`
  - **Mechanism**: `_navigationService.ShowDialogAsync(dialog)`

- **[Order-entry dialogs (selection/configuration flows)]**
  - **Source**: `OrderEntryViewModel` dialog methods (merge, assign customer, change table, instructions, modifiers, etc.)
  - **Targets**:
    - `Views.Dialogs.MergeTicketsDialog`
    - `Views.Dialogs.CustomerSelectionDialog`
    - `Views.Dialogs.TableSelectionDialog`
    - `Views.Dialogs.CookingInstructionDialog`
    - `Views.Dialogs.ModifierSelectionDialog` (Magidesk.Views.Dialogs)
    - `Views.Dialogs.AddOnSelectionDialog`
    - `Views.Dialogs.ComboSelectionDialog`
    - `Views.Dialogs.PizzaModifierDialog`
    - `Views.Dialogs.TicketFeeDialog`
    - `Views.Dialogs.SeatSelectionDialog`
    - `Views.Dialogs.MiscItemDialog`
    - `Views.Dialogs.SizeSelectionDialog`
    - `Views.Dialogs.ItemSearchDialog`
    - `Views.Dialogs.PriceEntryDialog`
    - `Views.Dialogs.QuantityDialog`
  - **Mechanism**: `await dialog.ShowAsync()` (direct) and/or `_navigationService.ShowDialogAsync(dialog)`

- **[GoBack (return to prior page)]**
  - **Source**: `OrderEntryViewModel` contains multiple `GoBack()` calls after merge / close / settle flows
  - **Target**: previous page in root frame back stack
  - **Mechanism**: `_navigationService.GoBack()`

## Settle page (`SettleViewModel`)
- **[Close settle]**
  - **Source**: `SettleViewModel.OnClose()`
  - **Target**: previous page
  - **Mechanism**: `_navigationService.GoBack()`

- **[Logout from settle]**
  - **Source**: `SettleViewModel.OnLogoutAsync()`
  - **Target**: `Views.LoginPage`
  - **Mechanism**: `_navigationService.Navigate(typeof(Views.LoginPage))`

- **[Swipe card flow]**
  - **Source**: `SettleViewModel.SwipeCardAsync()`
  - **Targets**:
    - `Magidesk.Views.SwipeCardDialog`
    - `Magidesk.Views.AuthorizationCodeDialog` (conditional)
    - `Magidesk.Views.PaymentProcessWaitDialog` (shown programmatically during processing)
  - **Mechanism**: `_navigationService.ShowDialogAsync(...)` + dialog `Hide()`
