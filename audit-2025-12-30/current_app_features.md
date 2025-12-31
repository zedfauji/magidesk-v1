# Current Application Feature Inventory

## Scope and method
- **Source**: `C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk`
- **Inventory basis**: Observable UI surfaces (WinUI 3 pages/dialogs), ViewModels, API controllers, and Application-layer commands/queries.
- **Classification**:
  - **Implemented**: code path and UI surface exist with non-placeholder behavior.
  - **Partial**: surface exists but contains explicit TODOs, hardcoded IDs, mock behavior, or missing integration.
  - **Stubbed**: surface exists but is largely placeholder (debug prints, minimal wiring).
  - **Non-existent**: no corresponding surface/handler found (not exhaustively asserted yet; used when a referenced feature is clearly missing).
- **No comparison**: This file only lists current app features.

## Application entry and shell
- **Feature Name**: App bootstrap and dependency injection composition root
  - **Status**: Implemented
  - **Evidence**: `App.xaml.cs`
  - **Observed**:
    - Uses `Host.CreateDefaultBuilder()` and registers `AddApplication()` + `AddInfrastructure()`.
    - Registers many ViewModels and some command handlers.
    - Launch flow: creates `MainWindow`, runs `ISystemInitializationService.InitializeSystemAsync()`, navigates to `Views.LoginPage` on success.

- **Feature Name**: Main window shell + navigation + loading overlay
  - **Status**: Implemented (with some TODO notes)
  - **Evidence**: `MainWindow.xaml.cs`
  - **Observed**:
    - Initializes `NavigationService` with `ContentFrame`.
    - Status bar clock updates every second.
    - NavigationView menu routes to pages (home/switchboard, cash session, ticket, payments, discount/tax, printing, ticket management, sales reports, user mgmt, settings).
    - Drawer pull is opened as a dialog via `Views.DrawerPullReportDialog`.

- **Feature Name**: Navigation service abstraction
  - **Status**: Implemented
  - **Evidence**: `Services/NavigationService.cs`
  - **Observed**: Wraps `Frame.Navigate`, `GoBack`, and `ShowDialogAsync(ContentDialog)` (requires `Frame.XamlRoot`).

## Authentication / login
- **Feature Name**: Login via PIN (encrypt + lookup)
  - **Status**: Implemented (behavior exists; overall auth model may be partial)
  - **Evidence**: `ViewModels/LoginViewModel.cs`
  - **Observed**:
    - Encrypts PIN via `IAesEncryptionService` and fetches user via `ISecurityService.GetUserByPinAsync`.
    - Sets `_userService.CurrentUser`.
    - Updates main window user display via `MainWindow.SetUser(...)`.
    - Navigates to `Views.SwitchboardPage`.

- **Feature Name**: Clock In / Clock Out from login
  - **Status**: Implemented/Partial
  - **Evidence**: `ViewModels/LoginViewModel.cs`
  - **Observed**:
    - Fetches open attendance via `IAttendanceRepository.GetOpenByUserIdAsync`.
    - Calls `_clockInHandler` / `_clockOutHandler`.
    - Shows a `ContentDialog` for feedback.

## Switchboard (front-of-house landing)
- **Feature Name**: Switchboard page and command surface
  - **Status**: Partial
  - **Evidence**: `ViewModels/SwitchboardViewModel.cs`, `Views/SwitchboardPage.xaml`
  - **Observed**:
    - Loads open tickets via `GetOpenTicketsQuery`.
    - New ticket flow includes Order Type selection and guest count for Dine In.
    - Explicit TODO/partial integrations:
      - Table selection not linked when order type requires table.
      - Customer selection not linked when order type requires customer.
      - Uses hardcoded GUIDs for terminal/user/shift fallback.

- **Feature Name**: Open tickets list (manager/settle entry)
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/OpenTicketsListDialog.xaml`, `ViewModels/OpenTicketsListViewModel.cs`, `SwitchboardViewModel.Settle()`
  - **Observed**: Launched via `SwitchboardViewModel.Settle()`.

- **Feature Name**: Drawer pull report
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/DrawerPullReportDialog.xaml`, `ViewModels/DrawerPullReportViewModel.cs`, `Magidesk.Application/Queries/GetDrawerPullReportQuery.cs`
  - **Observed**: Launchable from switchboard and main menu; report generation parity not assessed here.

- **Feature Name**: Cash drawer operations from switchboard (cash drop, payout, drawer bleed, open drawer, drawer balance)
  - **Status**: Partial
  - **Evidence**: `ViewModels/SwitchboardViewModel.cs`, dialogs: `Views/CashDropManagementDialog.xaml`, `Views/CashEntryDialog.xaml`
  - **Observed**:
    - Cash drop management dialog is used.
    - Payout/bleed/drop implemented via `PerformDrawerOperationAsync` creating domain entities and calling `_cashSessionRepository.UpdateAsync(session)`.
    - Open drawer logs a `TerminalTransactionType.NoSale`.
    - Uses hardcoded terminal/user IDs.

- **Feature Name**: Tables entry point
  - **Status**: Stubbed
  - **Evidence**: `ViewModels/SwitchboardViewModel.cs` (`TablesCommand = Debug.WriteLine("Tables Not Implemented")`)

- **Feature Name**: Manager functions dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/ManagerFunctionsDialog.xaml`, `ViewModels/ManagerFunctionsViewModel.cs`, `SwitchboardViewModel.ManagerFunctionsAsync()`

## Order entry
- **Feature Name**: Order entry page (ticket context)
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/OrderEntryPage.xaml`, `ViewModels/OrderEntryViewModel.cs`
  - **Observed (non-exhaustive, based on file scan)**:
    - Category/group/item browsing via repositories.
    - Add item uses `AddOrderLineCommand`.
    - Modifier workflows:
      - Size selection dialog (heuristic group name contains "Size")
      - Generic modifier selection dialog
      - Pizza modifier dialog
      - Add-on selection dialog
      - Combo selection dialog
    - Quantity dialog (pending quantity), increment/decrement, remove item with kitchen-printed guards.
    - Cooking instruction dialog.
    - Print ticket (`IPrintingService.PrintTicketAsync`).
    - Send to kitchen (`PrintToKitchenCommand`).
    - Pay now (quick pay) via `PayNowCommand` with tender type "CASH".
    - Split ticket dialog invoked.
    - Multiple explicit TODOs and hardcoded IDs suggest partial parity and incomplete integration.

- **Feature Name**: Item search
  - **Status**: Implemented
  - **Evidence**: `Views/Dialogs/ItemSearchDialog.xaml`, `ViewModels/Dialogs/ItemSearchViewModel.cs`, usage in `OrderEntryViewModel.SearchItemAsync()`

- **Feature Name**: Misc item
  - **Status**: Implemented
  - **Evidence**: `Views/Dialogs/MiscItemDialog.xaml`, `ViewModels/MiscItemViewModel.cs`, usage in `OrderEntryViewModel.AddMiscItemAsync()`

- **Feature Name**: Ticket fee dialog (service charge / delivery charge / adjustment)
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/Dialogs/TicketFeeDialog.xaml`, `ViewModels/TicketFeeViewModel.cs`, commands `SetServiceChargeCommand`, `SetDeliveryChargeCommand`, `SetAdjustmentCommand`.

## Settlement / payments
- **Feature Name**: Settle page (keypad + tender)
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/SettlePage.xaml`, `ViewModels/SettleViewModel.cs`
  - **Observed**:
    - Loads ticket via `GetTicketQuery`.
    - Processes payments via `ProcessPaymentCommand`.
    - Tax exempt toggle via `SetTaxExemptCommand`.
    - Quick cash, exact due, next amount implemented in VM.
    - No sale is currently simulated with delay and sets `Error` string (explicit TODO indicates missing backend/hardware integration).
    - Logout uses hardcoded userId (partial).

- **Feature Name**: Swipe card dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/SwipeCardDialog.xaml`, `ViewModels/SwipeCardViewModel.cs`

- **Feature Name**: Authorization code dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/AuthorizationCodeDialog.xaml`, `ViewModels/AuthorizationCodeViewModel.cs`

- **Feature Name**: Authorization capture batch dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/AuthorizationCaptureBatchDialog.xaml`, `ViewModels/AuthorizationCaptureBatchViewModel.cs`

- **Feature Name**: Payment process wait dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/PaymentProcessWaitDialog.xaml`, `ViewModels/PaymentProcessWaitViewModel.cs`

- **Feature Name**: Group settle (UI)
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/GroupSettleTicketDialog.xaml`, `Views/GroupSettleTicketSelectionWindow.xaml`, `ViewModels/GroupSettleTicketViewModel.cs`, `ViewModels/GroupSettleTicketSelectionViewModel.cs`, plus application commands `GroupSettleCommand`.

## Ticket management
- **Feature Name**: Ticket management page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/TicketManagementPage.xaml`, `ViewModels/TicketManagementViewModel.cs`

- **Feature Name**: Split ticket dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/SplitTicketDialog.xaml` and variants (`_Backup`, `_Fixed`, `_Minimal`), `ViewModels/SplitTicketViewModel.cs`, application commands `SplitTicketCommand`, `SplitBySeatCommand` (+ handler).

- **Feature Name**: Void ticket dialog
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/VoidTicketDialog.xaml`, `ViewModels/VoidTicketViewModel.cs`, application command `VoidTicketCommand`.

- **Feature Name**: Transfer ticket
  - **Status**: Partial
  - **Evidence**: `Magidesk.Application/Commands/TransferTicketCommand.cs`, UI evidence not yet confirmed in this inventory pass.

## Kitchen
- **Feature Name**: Kitchen display page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/KitchenDisplayPage.xaml`, `ViewModels/KitchenDisplayViewModel.cs`

## Back office / configuration
- **Feature Name**: Back office page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/BackOfficePage.xaml`, `ViewModels/BackOfficeViewModel.cs`

- **Feature Name**: System config page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/SystemConfigPage.xaml`, `ViewModels/SystemConfigViewModel.cs`

- **Feature Name**: Menu editor page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/MenuEditorPage.xaml`, `ViewModels/MenuEditorViewModel.cs`

- **Feature Name**: Modifier editor page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/ModifierEditorPage.xaml`, `ViewModels/ModifierEditorViewModel.cs`

- **Feature Name**: Inventory page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/InventoryPage.xaml`, `ViewModels/InventoryViewModel.cs`

- **Feature Name**: Sales reports page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/SalesReportsPage.xaml`, `ViewModels/SalesReportsViewModel.cs`

- **Feature Name**: User management page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/UserManagementPage.xaml`, `ViewModels/UserManagementViewModel.cs`

- **Feature Name**: Settings page
  - **Status**: Implemented/Partial
  - **Evidence**: `Views/SettingsPage.xaml`, `ViewModels/SettingsViewModel.cs`

## API surfaces (backend)
- **Feature Name**: Cash session API
  - **Status**: Implemented/Partial
  - **Evidence**: `Magidesk.Api/Controllers/CashController.cs`
  - **Observed**:
    - Start session, close session, get active session by user.
    - Hardcoded currency "USD".

- **Feature Name**: Kitchen API
  - **Status**: Implemented/Partial
  - **Evidence**: `Magidesk.Api/Controllers/KitchenController.cs`
  - **Observed**:
    - Get active orders, bump order, void order.

- **Feature Name**: Reports API
  - **Status**: Implemented/Partial
  - **Evidence**: `Magidesk.Api/Controllers/ReportsController.cs`
  - **Observed**:
    - Sales balance, sales summary, exceptions, journal, productivity, labor, delivery.

- **Feature Name**: Menu category/group APIs
  - **Status**: Implemented/Partial
  - **Evidence**: `Magidesk.Api/Controllers/MenuCategoriesController.cs`, `Magidesk.Api/Controllers/MenuGroupsController.cs`

- **Feature Name**: System API
  - **Status**: Implemented/Partial
  - **Evidence**: `Magidesk.Api/Controllers/SystemController.cs`

## Documentation corpus (intent reference)
- The `/docs` folder contains extensive feature audits and parity reports (backend and UI) under:
  - `docs/forensic-backend-audit/**`
  - `docs/forensic-ui-audit/**`
  - plus consolidated parity reports and matrices.

## Notes / uncertainty
- This inventory is derived from file enumeration and targeted reads; it is **not** a runtime-verified feature list.
- Some features may be present but not yet captured here (e.g., table selection flows, customer explorer/editor, refunds) and will be refined during Step 2 mapping and Step 3 matrix creation.
