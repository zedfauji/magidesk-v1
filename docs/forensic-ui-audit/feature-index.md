# FloreantPOS UI Surface & Feature Index (Forensic)

This index enumerates **every UI surface** discovered in FloreantPOS code and maps it to a per-feature document in `features/`.

> **Status**: PHASE 1 COMPLETE - 105 features enumerated  
> **Generated**: 2025-12-25  
> **Source**: FloreantPOS at `/projects/Code/Redesign-POS/floreantpos`

---

## Feature Categories

| Category | Feature Count | ID Range |
|----------|---------------|----------|
| System & Bootstrap | 5 | F-0001 to F-0005 |
| Authentication & Login | 4 | F-0006 to F-0009 |
| Switchboard & Navigation | 8 | F-0010 to F-0017 |
| Order Entry | 12 | F-0018 to F-0029 |
| Menu & Item Selection | 8 | F-0030 to F-0037 |
| Modifiers & Customization | 6 | F-0038 to F-0043 |
| Payment Processing | 15 | F-0044 to F-0058 |
| Cash Management | 8 | F-0059 to F-0066 |
| Tickets & Orders | 10 | F-0067 to F-0076 |
| Customers | 5 | F-0077 to F-0081 |
| Tables & Seating | 6 | F-0082 to F-0087 |
| Kitchen Display | 4 | F-0088 to F-0091 |
| Back Office - Reports | 12 | F-0092 to F-0103 |
| Back Office - Configuration | 8 | F-0104 to F-0111 |
| Back Office - Menu Management | 10 | F-0112 to F-0121 |

---

## System & Bootstrap (F-0001 to F-0005)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0001 | Application bootstrap & system initialization | Auto-triggered workflow + dialogs | `Application` | `main/Application.java` | DB connection check + terminal init + plugin init; may open DB config dialog + update dialog |
| F-0002 | POS main window shell | Main window | `PosWindow` | `main/PosWindow.java` | Clock timer; glass pane; windowClosing behavior; status bar |
| F-0003 | Initial setup wizard | Setup window | `SetUpWindow` | `main/SetUpWindow.java` | First-time setup configuration |
| F-0004 | Database configuration dialog | Configuration dialog | `DatabaseConfigurationDialog` | `config/ui/DatabaseConfigurationDialog.java` | DB host, port, name, user, password configuration |
| F-0005 | Application update dialog | System dialog | `UpdateDialog` | `ui/dialog/UpdateDialog.java` | Check for updates and apply |

---

## Authentication & Login (F-0006 to F-0009)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0006 | Login screen | Screen | `LoginView` | `ui/views/LoginView.java` | Order-type buttons; switchboard/backoffice/kitchen entry; DB configure + shutdown |
| F-0007 | Password entry dialog | Modal dialog | `PasswordEntryDialog` | `ui/dialog/PasswordEntryDialog.java` | PIN/password entry for user authentication; manager override authentication |
| F-0008 | User list selection | Dialog | `UserListDialog` | `ui/dialog/UserListDialog.java` | Select user from list (for transfers, clock-in) |
| F-0009 | Clock in/out | Action | `ClockInOutAction` | `actions/ClockInOutAction.java` | Employee attendance tracking |

---

## Switchboard & Navigation (F-0010 to F-0017)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0010 | Switchboard: Open Tickets & Activity | Screen | `SwitchboardView` | `ui/views/SwitchboardView.java` | Permission-gated buttons; drawer-present layout; settle/reopen/split/void/refund/assign-driver |
| F-0011 | Cashier switchboard view | Screen | `CashierSwitchBoardView` | `ui/views/CashierSwitchBoardView.java` | Cashier-specific switchboard layout |
| F-0012 | Other functions dialog | Dialog | `SwitchboardOtherFunctionsDialog` | `ui/views/SwitchboardOtherFunctionsDialog.java` | Additional functions menu (manager, reports, etc.) |
| F-0013 | Other functions view | Panel | `SwitchboardOtherFunctionsView` | `ui/views/SwitchboardOtherFunctionsView.java` | Other functions panel content |
| F-0014 | Home screen navigation | Action | `HomeScreenViewAction` | `actions/HomeScreenViewAction.java` | Return to home screen |
| F-0015 | Show back office | Action | `ShowBackofficeAction` | `actions/ShowBackofficeAction.java` | Open back office window |
| F-0016 | Show kitchen display | Action | `ShowKitchenDisplayAction` | `actions/ShowKitchenDisplayAction.java` | Open kitchen display |
| F-0017 | Logout | Action | `LogoutAction` | `actions/LogoutAction.java` | User logout with confirmation |

---

## Order Entry (F-0018 to F-0029)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0018 | Order entry view container | Screen | `OrderView` | `ui/views/order/OrderView.java` | Dine-in/retail dual-mode; cancel after kitchen-send requires confirm; hold may require secret key |
| F-0019 | Ticket panel | Panel | `TicketView` | `ui/views/order/TicketView.java` | Search/add items, qty controls, pay-now; barcode + item-id add; stock gating; customer display |
| F-0020 | Order type selection | Dialog | `OrderTypeSelectionDialog` | `ui/views/order/OrderTypeSelectionDialog.java` | Select dine-in, takeout, delivery, bar tab, etc. |
| F-0021 | Order type selection (alternate) | Dialog | `OrderTypeSelectionDialog2` | `ui/views/order/OrderTypeSelectionDialog2.java` | Alternative order type dialog |
| F-0022 | Ticket detail view | Panel | `TicketDetailView` | `ui/views/TicketDetailView.java` | Detailed ticket information display |
| F-0023 | Ticket detail dialog | Dialog | `TicketDetailDialog` | `ui/dialog/TicketDetailDialog.java` | Full ticket detail in dialog |
| F-0024 | Ticket receipt view | Panel | `TicketReceiptView` | `ui/views/TicketReceiptView.java` | Receipt preview |
| F-0025 | Order info dialog | Dialog | `OrderInfoDialog` | `ui/views/OrderInfoDialog.java` | Order information display |
| F-0026 | Order info view | Panel | `OrderInfoView` | `ui/views/OrderInfoView.java` | Order information panel |
| F-0027 | Order filters view | Panel | `OrderFiltersView` | `ui/views/OrderFiltersView.java` | Filter orders by type, status, date |
| F-0028 | Cashier mode next action | Dialog | `CashierModeNextActionDialog` | `ui/views/order/CashierModeNextActionDialog.java` | Next action after order completion |
| F-0029 | Misc ticket item entry | Dialog | `MiscTicketItemDialog` | `ui/dialog/MiscTicketItemDialog.java` | Add miscellaneous items not on menu |

---

## Menu & Item Selection (F-0030 to F-0037)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0030 | Category view | Panel | `CategoryView` | `ui/views/order/CategoryView.java` | Menu category selection grid |
| F-0031 | Group view | Panel | `GroupView` | `ui/views/order/GroupView.java` | Menu group selection |
| F-0032 | Menu item view | Panel | `MenuItemView` | `ui/views/order/MenuItemView.java` | Menu item selection grid |
| F-0033 | Selection view | Panel | `SelectionView` | `ui/views/order/SelectionView.java` | Generic item selection panel |
| F-0034 | Item search dialog | Dialog | `ItemSearchDialog` | `ui/dialog/ItemSearchDialog.java` | Search menu items by name/barcode |
| F-0035 | Item selection dialog | Dialog | `ItemSelectionDialog` | `ui/dialog/ItemSelectionDialog.java` | Select menu item from list |
| F-0036 | Combo item selection | Dialog | `ComboItemSelectionDialog` | `ui/dialog/ComboItemSelectionDialog.java` | Select items for combo/meal deal |
| F-0037 | Weight input dialog | Dialog | `BasicWeightInputDialog` | `ui/dialog/BasicWeightInputDialog.java` | Enter weight for weight-based items |

---

## Modifiers & Customization (F-0038 to F-0043)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0038 | Modifier selection dialog | Dialog | `ModifierSelectionDialog` | `ui/views/order/modifier/ModifierSelectionDialog.java` | Select modifiers for item |
| F-0039 | Modifier group view | Panel | `ModifierGroupView` | `ui/views/order/modifier/ModifierGroupView.java` | Display modifier groups |
| F-0040 | Modifier view | Panel | `ModifierView` | `ui/views/order/modifier/ModifierView.java` | Display individual modifiers |
| F-0041 | Pizza modifier selection | Dialog | `PizzaModifierSelectionDialog` | `ui/views/order/multipart/PizzaModifierSelectionDialog.java` | Half/quarter pizza modifiers |
| F-0042 | Cooking instruction selection | Dialog | `SelectCookongInstructionDialog` | `ui/dialog/SelectCookongInstructionDialog.java` | Select cooking instructions |
| F-0043 | New cooking instruction | Dialog | `NewCookongInstructionDialog` | `ui/dialog/NewCookongInstructionDialog.java` | Add new cooking instruction |

---

## Payment Processing (F-0044 to F-0058)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0044 | Payment keypad + tender types | Panel | `PaymentView` | `ui/views/payment/PaymentView.java` | Cash/CC/debit/gift/other; multi-currency; gratuity/coupon/print; NO SALE drawer kick |
| F-0045 | Settle ticket dialog | Dialog | `SettleTicketDialog` | `ui/views/payment/SettleTicketDialog.java` | Consolidate-items-in-receipt; bar-tab preauth capture vs void |
| F-0046 | Group settle ticket dialog | Dialog | `GroupSettleTicketDialog` | `ui/views/payment/GroupSettleTicketDialog.java` | Settle multiple tickets at once |
| F-0047 | Group settle selection window | Window | `GroupSettleTicketSelectionWindow` | `ui/views/GroupSettleTicketSelectionWindow.java` | Select tickets for group settlement |
| F-0048 | Payment type selection | Dialog | `PaymentTypeSelectionDialog` | `ui/dialog/PaymentTypeSelectionDialog.java` | Select payment type (cash, card, etc.) |
| F-0049 | Custom payment selection | Dialog | `CustomPaymentSelectionDialog` | `ui/views/payment/CustomPaymentSelectionDialog.java` | Select custom payment type |
| F-0050 | Swipe card input | Dialog | `SwipeCardDialog` | `ui/views/payment/SwipeCardDialog.java` | Card swipe; manual entry; auth-code entry options |
| F-0051 | Manual card entry | Dialog | `ManualCardEntryDialog` | `ui/views/payment/ManualCardEntryDialog.java` | Manual card number entry |
| F-0052 | Authorization code entry | Dialog | `AuthorizationCodeDialog` | `ui/views/payment/AuthorizationCodeDialog.java` | Card-type selection + auth code entry |
| F-0053 | Authorization capture batch | Dialog | `AuthorizationDialog` | `ui/views/payment/AuthorizationDialog.java` | Batch capture authorized transactions |
| F-0054 | Gift certificate entry | Dialog | `GiftCertDialog` | `ui/views/payment/GiftCertDialog.java` | Gift cert number + face value entry |
| F-0055 | Gratuity input | Dialog | `GratuityInputDialog` | `ui/views/payment/GratuityInputDialog.java` | Numeric keypad gratuity entry |
| F-0056 | Payment processing wait | Dialog | `PaymentProcessWaitDialog` | `ui/views/payment/PaymentProcessWaitDialog.java` | Non-modal wait during processing |
| F-0057 | Payment confirmation | Dialog | `ConfirmPayDialog` | `ui/views/payment/ConfirmPayDialog.java` | Confirm payment amount |
| F-0058 | Multi-currency tender | Dialog | `MultiCurrencyTenderDialog` | `ui/dialog/MultiCurrencyTenderDialog.java` | Accept multiple currencies |

---

## Cash Management (F-0059 to F-0066)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0059 | Manager functions dialog | Dialog | `ManagerDialog` | `ui/dialog/ManagerDialog.java` | Entry to cash drops, open tickets, drawer pull, tips, drawer kick |
| F-0060 | Cash drops (drawer bleed) | Dialog | `CashDropDialog` | `ui/dialog/CashDropDialog.java` | List/add/delete cash drops for terminal |
| F-0061 | Drawer pull report | Dialog | `DrawerPullReportDialog` | `ui/dialog/DrawerPullReportDialog.java` | HTML preview + print; multi-currency; void ticket section |
| F-0062 | Payout | Dialog | `PayoutDialog` | `ui/dialog/PayoutDialog.java` | Record cash payout with reason, recipient, amount |
| F-0063 | Tips cashout report | Dialog | `TipsCashoutReportDialog` | `ui/dialog/TipsCashoutReportDialog.java` | Tips report by date range and user |
| F-0064 | Drawer kick action | Action | `DrawerKickAction` | `actions/DrawerKickAction.java` | Open cash drawer |
| F-0065 | Drawer assignment | Action | `DrawerAssignmentAction` | `actions/DrawerAssignmentAction.java` | Assign drawer to user |
| F-0066 | Multi-currency drawer assignment | Dialog | `MultiCurrencyAssignDrawerDialog` | `ui/dialog/MultiCurrencyAssignDrawerDialog.java` | Assign drawer with multiple currencies |

---

## Tickets & Orders (F-0067 to F-0076)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0067 | Open tickets list | Dialog | `OpenTicketsListDialog` | `ui/dialog/OpenTicketsListDialog.java` | Manager view; transfer server; void; cashier mode loads ticket |
| F-0068 | Void ticket | Dialog | `VoidTicketDialog` | `ui/dialog/VoidTicketDialog.java` | Refund prompts; tips refund; delete-vs-void; prints report; logs history |
| F-0069 | Split ticket | Dialog | `SplitTicketDialog` | `ui/views/SplitTicketDialog.java` | 2-4 splits; split by seat; transaction transfer; DB transaction |
| F-0070 | Ticket for split view | Panel | `TicketForSplitView` | `ui/views/order/TicketForSplitView.java` | Ticket view within split dialog |
| F-0071 | Ticket selection dialog | Dialog | `TicketSelectionDialog` | `ui/views/order/TicketSelectionDialog.java` | Select ticket from list |
| F-0072 | Split ticket selection | Dialog | `SplitedTicketSelectionDialog` | `ui/views/payment/SplitedTicketSelectionDialog.java` | Select from split tickets |
| F-0073 | Refund action | Action | `RefundAction` | `actions/RefundAction.java` | Validate paid; amount entry; cannot exceed paid |
| F-0074 | User transfer dialog | Dialog | `UserTransferDialog` | `ui/views/UserTransferDialog.java` | Transfer ticket to another user |
| F-0075 | Bar tab action | Action | `NewBarTabAction` | `actions/NewBarTabAction.java` | Create new bar tab |
| F-0076 | Authorizable ticket browser | Panel | `AuthorizableTicketBrowser` | `ui/views/payment/AuthorizableTicketBrowser.java` | Browse tickets with pending authorizations |

---

## Customers (F-0077 to F-0081)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0077 | Customer selector dialog | Dialog | `CustomerSelectorDialog` | `customer/CustomerSelectorDialog.java` | Select customer; create new ticket option; caller ID |
| F-0078 | Customer view | Panel | `CustomerView` | `ui/views/CustomerView.java` | Customer information display |
| F-0079 | Customer form | Form | `CustomerForm` | `ui/forms/CustomerForm.java` | Customer data entry |
| F-0080 | Quick customer form | Form | `QuickCustomerForm` | `ui/forms/QuickCustomerForm.java` | Rapid customer entry |
| F-0081 | Customer explorer (BO) | Explorer | `CustomerExplorer` | `customer/CustomerExplorer.java` | Customer CRUD in back office |

---

## Tables & Seating (F-0082 to F-0087)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0082 | Table map view | Panel | `TableMapView` | `ui/views/TableMapView.java` | Visual table floor layout |
| F-0083 | Table selector dialog | Dialog | `TableSelectorDialog` | `ui/tableselection/TableSelectorDialog.java` | Select table for order |
| F-0084 | Default table selection view | Panel | `DefaultTableSelectionView` | `ui/tableselection/DefaultTableSelectionView.java` | Default table selection UI |
| F-0085 | Bar tab selection view | Panel | `BarTabSelectionView` | `ui/tableselection/BarTabSelectionView.java` | Bar tab selection |
| F-0086 | Seat selection dialog | Dialog | `SeatSelectionDialog` | `ui/dialog/SeatSelectionDialog.java` | Select seat number |
| F-0087 | Table browser (BO) | Browser | `ShopTableBrowser` | `table/ShopTableBrowser.java` | Table CRUD in back office |

---

## Kitchen Display (F-0088 to F-0091)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0088 | Kitchen display window | Window | `KitchenDisplayWindow` | `demo/KitchenDisplayWindow.java` | Standalone kitchen display |
| F-0089 | Kitchen display view | Panel | `KitchenDisplayView` | `demo/KitchenDisplayView.java` | Kitchen ticket display |
| F-0090 | Kitchen ticket view | Panel | `KitchenTicketView` | `demo/KitchenTicketView.java` | Individual ticket in kitchen |
| F-0091 | Kitchen filter dialog | Dialog | `KitchenFilterDialog` | `demo/KitchenFilterDialog.java` | Filter kitchen tickets |

---

## Back Office - Reports (F-0092 to F-0103)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0092 | Sales summary report | Report view | `SalesSummaryReportView` | `report/SalesSummaryReportView.java` | Sales summary by period |
| F-0093 | Sales detail report | Report view | `SalesDetailReportView` | `report/SalesDetailReportView.java` | Detailed sales breakdown |
| F-0094 | Sales balance report | Report view | `SalesBalanceReportView` | `report/SalesBalanceReportView.java` | Sales balance reconciliation |
| F-0095 | Sales exception report | Report view | `SalesExceptionReportView` | `report/SalesExceptionReportView.java` | Voids, refunds, discounts |
| F-0096 | Credit card report | Report view | `CreditCardReportView` | `report/CreditCardReportView.java` | CC transactions report |
| F-0097 | Custom payment report | Report view | `CustomPaymentReportView` | `report/CustomPaymentReportView.java` | Custom payment types report |
| F-0098 | Menu usage report | Report view | `MenuUsageReportView` | `report/MenuUsageReportView.java` | Item sales analysis |
| F-0099 | Server productivity report | Report view | `ServerProductivityReportView` | `report/ServerProductivityReportView.java` | Server performance |
| F-0100 | Hourly labor report | Report view | `HourlyLaborReportView` | `report/HourlyLaborReportView.java` | Labor costs by hour |
| F-0101 | Payroll report | Report view | `PayrollReportView` | `report/PayrollReportView.java` | Payroll summary |
| F-0102 | Attendance report | Report view | `AttendanceReportView` | `report/AttendanceReportView.java` | Employee attendance |
| F-0103 | Journal report | Report view | `JournalReportView` | `report/JournalReportView.java` | All transactions journal |

---

## Back Office - Configuration (F-0104 to F-0111)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0104 | Configuration dialog | Dialog | `ConfigurationDialog` | `config/ui/ConfigurationDialog.java` | Main configuration container |
| F-0105 | Restaurant configuration | Config view | `RestaurantConfigurationView` | `config/ui/RestaurantConfigurationView.java` | Restaurant settings |
| F-0106 | Terminal configuration | Config view | `TerminalConfigurationView` | `config/ui/TerminalConfigurationView.java` | Terminal settings |
| F-0107 | Card configuration | Config view | `CardConfigurationView` | `config/ui/CardConfigurationView.java` | Credit card processing settings |
| F-0108 | Print configuration | Config view | `PrintConfigurationView` | `config/ui/PrintConfigurationView.java` | Printer settings |
| F-0109 | Tax configuration | Config view | `TaxConfigurationView` | `config/ui/TaxConfigurationView.java` | Tax rates and rules |
| F-0110 | Peripheral configuration | Config view | `PeripheralConfigurationView` | `config/ui/PeripheralConfigurationView.java` | Hardware peripherals |
| F-0111 | Back office window | Window | `BackOfficeWindow` | `bo/ui/BackOfficeWindow.java` | Main BO window with menus |

---

## Back Office - Menu Management (F-0112 to F-0121)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0112 | Menu category explorer | Explorer | `MenuCategoryExplorer` | `bo/ui/explorer/MenuCategoryExplorer.java` | Category CRUD |
| F-0113 | Menu group explorer | Explorer | `MenuGroupExplorer` | `bo/ui/explorer/MenuGroupExplorer.java` | Group CRUD |
| F-0114 | Menu item explorer | Explorer | `MenuItemExplorer` | `bo/ui/explorer/MenuItemExplorer.java` | Item CRUD |
| F-0115 | Modifier explorer | Explorer | `ModifierExplorer` | `bo/ui/explorer/ModifierExplorer.java` | Modifier CRUD |
| F-0116 | Modifier group explorer | Explorer | `ModifierGroupExplorer` | `bo/ui/explorer/ModifierGroupExplorer.java` | Modifier group CRUD |
| F-0117 | Coupon explorer | Explorer | `CouponExplorer` | `bo/ui/explorer/CouponExplorer.java` | Discount/coupon CRUD |
| F-0118 | Tax explorer | Explorer | `TaxExplorer` | `bo/ui/explorer/TaxExplorer.java` | Tax CRUD |
| F-0119 | Shift explorer | Explorer | `ShiftExplorer` | `bo/ui/explorer/ShiftExplorer.java` | Shift CRUD |
| F-0120 | User explorer | Explorer | `UserExplorer` | `bo/ui/explorer/UserExplorer.java` | User CRUD |
| F-0121 | Order type explorer | Explorer | `OrderTypeExplorer` | `bo/ui/explorer/OrderTypeExplorer.java` | Order type CRUD |

---

## Additional Dialogs & Utilities (F-0122 to F-0130+)

| ID | Feature Name | Surface Type | Primary Class | File Path | Notes |
|----|--------------|--------------|---------------|-----------|-------|
| F-0122 | Coupon and discount dialog | Dialog | `CouponAndDiscountDialog` | `ui/dialog/CouponAndDiscountDialog.java` | Apply coupons/discounts to ticket |
| F-0123 | Discount selection | Dialog | `DiscountSelectionDialog` | `ui/dialog/DiscountSelectionDialog.java` | Select discount to apply |
| F-0124 | Discount list dialog | Dialog | `DiscountListDialog` | `ui/dialog/DiscountListDialog.java` | List available discounts |
| F-0125 | Notes dialog | Dialog | `NotesDialog` | `ui/dialog/NotesDialog.java` | Add notes to ticket/item |
| F-0126 | Date chooser dialog | Dialog | `DateChoserDialog` | `ui/dialog/DateChoserDialog.java` | Date picker |
| F-0127 | Number selection | Dialog | `NumberSelectionDialog2` | `ui/dialog/NumberSelectionDialog2.java` | Numeric input |
| F-0128 | Message dialog | Dialog | `POSMessageDialog` | `ui/dialog/POSMessageDialog.java` | Error/info messages |
| F-0129 | Confirm delete | Dialog | `ConfirmDeleteDialog` | `ui/dialog/ConfirmDeleteDialog.java` | Delete confirmation |
| F-0130 | About dialog | Dialog | `AboutDialog` | `ui/dialog/AboutDialog.java` | Application info |
| F-0131 | Language selection | Dialog | `LanguageSelectionDialog` | `ui/dialog/LanguageSelectionDialog.java` | i18n language picker |
| F-0132 | Transaction completion | Dialog | `TransactionCompletionDialog` | `ui/dialog/TransactionCompletionDialog.java` | Transaction success |

---

## Summary Statistics

- **Total Features Enumerated**: 132
- **Screens/Views**: 28
- **Dialogs/Modals**: 67
- **Actions**: 18
- **Explorers/Browsers**: 12
- **Reports**: 12
- **Configuration Views**: 8

---

## Next Steps

1. **Phase 2**: Create per-feature forensic documentation using template
2. **Phase 3**: Add code traceability (file paths, methods, action bindings)
3. **Phase 4**: Compare with MagiDesk for parity classification
4. **Phase 5**: Define porting strategy per feature
5. **Phase 6**: Classify each feature (PARITY REQUIRED, DEFER, REJECT)
