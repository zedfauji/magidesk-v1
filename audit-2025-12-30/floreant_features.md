# FloreantPOS Feature Inventory

## Scope and method
- **Source**: `C:\Users\giris\Documents\Code\Redesign-POS\floreantpos`
- **Inventory basis**: Observable UI surfaces and workflow entry points in code (Swing views/dialogs/actions) plus major back-office/report surfaces.
- **No comparison**: This file only lists FloreantPOS features.
- **Evidence style**: File paths to concrete classes.

## Application entry and shell
- **Feature Name**: Application bootstrap / startup
  - **Evidence**: `floreantpos/src/com/floreantpos/main/Main.java`
  - **Observed**: CLI option `developmentMode`; sets default locale from `TerminalConfig`; starts `Application`.

- **Feature Name**: Application runtime initialization (DB, terminal, order types, printers, plugins)
  - **Evidence**: `floreantpos/src/com/floreantpos/main/Application.java`
  - **Observed**:
    - `DatabaseUtil.checkConnection(DatabaseUtil.initialize())`
    - `initTerminal`, `initOrderTypes`, `initPrintConfig`, `refreshRestaurant`, `loadCurrency`, `loadGlobalConfig`, `loadPrinters`, `initPlugins`
    - Initializes views: `RootView.getInstance().initializeViews()`, `LoginView.getInstance().initializeOrderButtonPanel()`

- **Feature Name**: Main window shell with status bar and timers
  - **Evidence**: `floreantpos/src/com/floreantpos/main/PosWindow.java`
  - **Observed**:
    - Status fields: terminal, user, DB, "Tax included".
    - Clock timer; auto logoff timer placeholders.
    - Fullscreen mode (`TerminalConfig.isFullscreenMode()` via `Application.start()`).

## Core navigation / root view
- **Feature Name**: Root view and view switching (CardLayout)
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/RootView.java`
  - **Observed**:
    - `LoginView` as initial view.
    - Default view selection based on `TerminalConfig.getDefaultView()`.
    - Supports switching between:
      - `SwitchboardView`
      - `SwitchboardOtherFunctionsView`
      - `KitchenDisplayView` (demo)
      - `OrderView` (order entry)
      - `TableMapView` (table selection)
      - `CustomerView` / plugin-driven delivery dispatch view
    - Back office launch: `BackOfficeWindow`.

## Front-of-house: switchboard / ticket list / order start
- **Feature Name**: Switchboard (open tickets + order type actions)
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/SwitchboardView.java`
  - **Observed**:
    - Ticket list panel with activity.
    - Buttons wired in constructor: edit ticket, group settle, order info, reopen ticket, settle ticket, split ticket, void ticket.
    - Order type buttons dynamically rendered from `Application.getInstance().getOrderTypes()`.
    - Bar tab button created when floor layout plugin exists (`FloorLayoutPlugin`).

- **Feature Name**: Order type selection dialogs (for multi-choice)
  - **Evidence**:
    - `floreantpos/src/com/floreantpos/ui/views/order/OrderTypeSelectionDialog.java`
    - `floreantpos/src/com/floreantpos/ui/views/order/OrderTypeSelectionDialog2.java`

- **Feature Name**: Open tickets list dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/OpenTicketsListDialog.java`

- **Feature Name**: Ticket selection dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/TicketSelectionDialog.java`

## Front-of-house: order entry
- **Feature Name**: Order entry main view
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/OrderView.java`
  - **Observed**:
    - Panels: categories, groups, menu items, ticket view.
    - Action buttons (partial list visible in header section): Hold, Done/Save, Send to kitchen, Cancel, Guest No, Seat No, Misc, Order type, Table number, Customer selection, Cooking instruction, Discount, Delivery info.
    - Integrations referenced in imports:
      - Customer selection (`CustomerSelectorDialog`)
      - Table selection (`TableSelectorDialog`)
      - Seat selection (`SeatSelectionDialog`)
      - Misc item (`MiscTicketItemDialog`)
      - Payment integration (`PaymentView`, `SettleTicketProcessor`)
      - Drawer utilities (`DrawerUtil`)

- **Feature Name**: Category browsing
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/CategoryView.java`

- **Feature Name**: Group browsing
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/GroupView.java`

- **Feature Name**: Menu item browsing
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/MenuItemView.java`

- **Feature Name**: Ticket view / ticket item list
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/order/TicketView.java`

- **Feature Name**: Cooking instruction selection view
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/CookingInstructionSelectionView.java`

- **Feature Name**: Item search dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/ItemSearchDialog.java`

- **Feature Name**: Misc ticket item dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/MiscTicketItemDialog.java`

- **Feature Name**: Quantity / number selection dialog (keypad)
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/NumberSelectionDialog2.java`

- **Feature Name**: Seat selection dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/SeatSelectionDialog.java`

- **Feature Name**: Table selector dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/tableselection/TableSelectorDialog.java`

- **Feature Name**: Table map view
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/TableMapView.java`

- **Feature Name**: Split ticket dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/SplitTicketDialog.java`

## Front-of-house: payment / settlement
- **Feature Name**: Settle ticket dialog (payment container)
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/SettleTicketDialog.java`

- **Feature Name**: Payment UI (tender keypad + tender types)
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/PaymentView.java`
  - **Observed (from class fields)**:
    - Tender types: Cash, Credit Card, Debit Card, Gift, Other.
    - Actions: gratuity, print, cancel.
    - Keypad: digits, dot, clear, 00, "Next Amount".
    - Quick cash amounts and exact amount.
    - No Sale button.
    - Supports multi-currency tender dialog and drawer adjustment.

- **Feature Name**: Swipe card dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/SwipeCardDialog.java`

- **Feature Name**: Manual card entry dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/ManualCardEntryDialog.java`

- **Feature Name**: Authorization code dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/AuthorizationCodeDialog.java`

- **Feature Name**: Payment process wait dialogs
  - **Evidence**:
    - `floreantpos/src/com/floreantpos/ui/views/payment/PaymentProcessWaitDialog.java`
    - `floreantpos/src/com/floreantpos/ui/views/payment/PosPaymentWaitDialog.java`

- **Feature Name**: Group settle ticket dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/GroupSettleTicketDialog.java`

- **Feature Name**: Split ticket selection dialog (payment context)
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/SplitedTicketSelectionDialog.java`

- **Feature Name**: Gift certificate dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/GiftCertDialog.java`

- **Feature Name**: Gratuity input dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/GratuityInputDialog.java`

- **Feature Name**: Custom payment selection dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/CustomPaymentSelectionDialog.java`

- **Feature Name**: Payment reference entry dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/PaymentReferenceEntryDialog.java`

- **Feature Name**: Confirm pay dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/views/payment/ConfirmPayDialog.java`

## Cash management (drawer)
- **Feature Name**: Cash drop dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/CashDropDialog.java`

- **Feature Name**: Payout dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/PayoutDialog.java`

- **Feature Name**: Multi-currency tender dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/MultiCurrencyTenderDialog.java`

- **Feature Name**: Multi-currency assign drawer dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/MultiCurrencyAssignDrawerDialog.java`

- **Feature Name**: Drawer pull report dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/DrawerPullReportDialog.java`

## Ticket management / void / refunds
- **Feature Name**: Void ticket dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/VoidTicketDialog.java`

- **Feature Name**: Ticket detail dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/TicketDetailDialog.java`

- **Feature Name**: Ticket item discount selection dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/TicketItemDiscountSelectionDialog.java`

- **Feature Name**: Coupon and discount dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/CouponAndDiscountDialog.java`

## Login / authentication / manager
- **Feature Name**: Password entry dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/PasswordEntryDialog.java`

- **Feature Name**: Manager dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/ManagerDialog.java`

- **Feature Name**: User list dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/ui/dialog/UserListDialog.java`

## Kitchen / KDS (demo surfaces visible)
- **Feature Name**: Kitchen display view (demo)
  - **Evidence**: `floreantpos/src/com/floreantpos/demo/KitchenDisplayView.java`

- **Feature Name**: Kitchen ticket view (demo)
  - **Evidence**: `floreantpos/src/com/floreantpos/demo/KitchenTicketView.java`

- **Feature Name**: Kitchen printer view (demo)
  - **Evidence**: `floreantpos/src/com/floreantpos/demo/KitchenPrinterView.java`

## Back office
- **Feature Name**: Back office window
  - **Evidence**: `floreantpos/src/com/floreantpos/bo/ui/BackOfficeWindow.java`
  - **Observed**:
    - Menus include Admin / Explorer / Reports / Floor.
    - Permission gating via `UserPermission`.
    - Reports include sales, open ticket summary, labor/payroll, attendance, key statistics, sales analysis, credit card report, custom payment report, menu usage, productivity, journal, balance, exceptions, etc.

## Configuration / setup
- **Feature Name**: Database configuration dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/config/ui/DatabaseConfigurationDialog.java`

- **Feature Name**: Terminal setup dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/config/ui/TerminalSetupDialog.java`

- **Feature Name**: Configuration dialog
  - **Evidence**: `floreantpos/src/com/floreantpos/config/ui/ConfigurationDialog.java`

- **Feature Name**: Printer configuration dialogs
  - **Evidence**:
    - `floreantpos/src/com/floreantpos/config/ui/AddPrinterDialog.java`
    - `floreantpos/src/com/floreantpos/config/ui/AddPrinterGroupDialog.java`
    - `floreantpos/src/com/floreantpos/config/ui/PrinterTypeSelectionDialog.java`
    - `floreantpos/src/com/floreantpos/config/ui/VirtualPrinterConfigDialog.java`

## Reports (views)
- **Feature Name**: Report viewer surfaces
  - **Evidence**: `floreantpos/src/com/floreantpos/report/ReportViewer.java`

- **Feature Name**: Sales and operational reports (views)
  - **Evidence**:
    - `floreantpos/src/com/floreantpos/report/SalesSummaryReportView.java`
    - `floreantpos/src/com/floreantpos/report/SalesDetailReportView.java`
    - `floreantpos/src/com/floreantpos/report/SalesBalanceReportView.java`
    - `floreantpos/src/com/floreantpos/report/SalesExceptionReportView.java`
    - `floreantpos/src/com/floreantpos/report/CreditCardReportView.java`
    - `floreantpos/src/com/floreantpos/report/CustomPaymentReportView.java`
    - `floreantpos/src/com/floreantpos/report/MenuUsageReportView.java`
    - `floreantpos/src/com/floreantpos/report/ServerProductivityReportView.java`
    - `floreantpos/src/com/floreantpos/report/HourlyLaborReportView.java`
    - `floreantpos/src/com/floreantpos/report/PayrollReportView.java`
    - `floreantpos/src/com/floreantpos/report/AttendanceReportView.java`
    - `floreantpos/src/com/floreantpos/report/JournalReportView.java`

## Notes / uncertainty
- This inventory is grounded in observable classes found by pattern search (`*Dialog*.java`, `*View*.java`) and targeted reads of entry points.
- Additional FloreantPOS features likely exist outside the `ui/` tree (e.g., services, DAOs, extension plugins). These will be enumerated in subsequent passes when building the full mapping and parity matrix.
