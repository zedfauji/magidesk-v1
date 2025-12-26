# Phase 0: UI-Bearing Codebase Map

> **Status**: COMPLETE  
> **Generated**: 2025-12-25  
> **Source**: FloreantPOS at `/projects/Code/Redesign-POS/floreantpos`

This document exhaustively enumerates ALL packages and classes in FloreantPOS that contain UI surfaces (Swing panels, dialogs, screens, views, menu actions, reports, configuration screens).

---

## Summary Statistics

| Package Category | Class Count | Primary Purpose |
|------------------|-------------|-----------------|
| Main Window | 4 | Application shell, main window, setup window |
| Actions | 29 | Menu/toolbar action handlers |
| Swing Components | 52 | Custom reusable Swing components |
| Dialogs | 60 | Modal dialogs (payment, void, split, manager, etc.) |
| Views | 33+ | Main screens (login, switchboard, order, payment) |
| Payment Views | 26 | Payment processing UI |
| Order Views | 24+ | Order entry UI + modifiers |
| Back Office UI | 10 | BO main window, model browser |
| Back Office Actions | 49 | BO menu actions (reports, explorers) |
| Back Office Explorers | 28 | CRUD explorers for entities |
| Configuration UI | 26 | System configuration dialogs |
| Reports | 46+ | Report views, models, templates |
| Customer UI | 7 | Customer selector dialogs |
| Table Management | 4 | Table/seating browser |
| Kitchen Display | 7 | Kitchen display views |
| **TOTAL** | **~350+** | |

---

## UI-Bearing Package Map

### 1. Main Application (`com.floreantpos.main`)

| File | Class | Feature Domain | UI Surface Type |
|------|-------|----------------|-----------------|
| `Application.java` | Application | System | Auto-triggered bootstrap, DB config dialog, update dialog, password dialog |
| `PosWindow.java` | PosWindow | System | Main window shell, status bar, clock timer, glass pane, shutdown flow |
| `Main.java` | Main | System | Entry point (no direct UI) |
| `SetUpWindow.java` | SetUpWindow | System | Initial setup wizard window |

---

### 2. Actions (`com.floreantpos.actions`)

| File | Class | Feature Domain | Action Type |
|------|-------|----------------|-------------|
| `AboutAction.java` | AboutAction | System | Opens About dialog |
| `ClockInOutAction.java` | ClockInOutAction | Attendance | Employee clock in/out |
| `CloseDialogAction.java` | CloseDialogAction | Utility | Generic close action |
| `DrawerAssignmentAction.java` | DrawerAssignmentAction | Cash Management | Assign drawer to user |
| `DrawerBleedAction.java` | DrawerBleedAction | Cash Management | Record cash drop |
| `DrawerKickAction.java` | DrawerKickAction | Cash Management | Open cash drawer |
| `DrawerPullAction.java` | DrawerPullAction | Cash Management | Execute drawer pull report |
| `GroupSettleTicketAction.java` | GroupSettleTicketAction | Payment | Settle multiple tickets |
| `HomeScreenViewAction.java` | HomeScreenViewAction | Navigation | Return to home screen |
| `LogoutAction.java` | LogoutAction | Authentication | User logout |
| `ManageTableLayoutAction.java` | ManageTableLayoutAction | Tables | Table layout management |
| `NewBarTabAction.java` | NewBarTabAction | Orders | Create bar tab |
| `PayoutAction.java` | PayoutAction | Cash Management | Record cash payout |
| `PosAction.java` | PosAction | Utility | Base action class |
| `RefundAction.java` | RefundAction | Payment | Process refund |
| `ServerTipsAction.java` | ServerTipsAction | Tips | View server tips |
| `SettleTicketAction.java` | SettleTicketAction | Payment | Settle single ticket |
| `ShowBackofficeAction.java` | ShowBackofficeAction | Navigation | Open back office |
| `ShowKitchenDisplayAction.java` | ShowKitchenDisplayAction | Kitchen | Open kitchen display |
| `ShowOnlineTicketManagementAction.java` | ShowOnlineTicketManagementAction | Orders | Online ticket management |
| `ShowOtherFunctionsAction.java` | ShowOtherFunctionsAction | Navigation | Other functions menu |
| `ShowTransactionsAuthorizationsAction.java` | ShowTransactionsAuthorizationsAction | Payment | View pending authorizations |
| `ShutDownAction.java` | ShutDownAction | System | Application shutdown |
| `SplitTicketAction.java` | SplitTicketAction | Orders | Split ticket items |
| `SwithboardViewAction.java` | SwithboardViewAction | Navigation | Go to switchboard |
| `UpdateAction.java` | UpdateAction | System | Check for updates |
| `ViewChangeAction.java` | ViewChangeAction | Navigation | Change view |
| `VoidTicketAction.java` | VoidTicketAction | Orders | Void ticket |

---

### 3. Swing Components (`com.floreantpos.swing`)

| File | Class | Component Type | Purpose |
|------|-------|---------------|---------|
| `BarTabButton.java` | BarTabButton | Button | Bar tab display button |
| `BeanTableModel.java` | BeanTableModel | Table Model | Generic bean table model |
| `ButtonColumn.java` | ButtonColumn | Table Column | Button in table cell |
| `ButtonUI.java` | ButtonUI | Look & Feel | Custom button UI |
| `CheckBoxList.java` | CheckBoxList | List | List with checkboxes |
| `ComboBoxModel.java` | ComboBoxModel | Model | Custom combobox model |
| `DoubleDocument.java` | DoubleDocument | Document | Double number input filter |
| `DoubleTextField.java` | DoubleTextField | Text Field | Double number input |
| `FixedLengthDocument.java` | FixedLengthDocument | Document | Fixed length input filter |
| `FixedLengthTextField.java` | FixedLengthTextField | Text Field | Fixed length input |
| `FocusedTextField.java` | FocusedTextField | Text Field | Auto-focused text field |
| `GlassPane.java` | GlassPane | Panel | Glass pane for modal overlay |
| `ImageComponent.java` | ImageComponent | Component | Image display |
| `IntegerDocument.java` | IntegerDocument | Document | Integer input filter |
| `IntegerTextField.java` | IntegerTextField | Text Field | Integer input |
| `IntroPage.java` | IntroPage | Panel | Intro/splash page |
| `ItemCheckBoxList.java` | ItemCheckBoxList | List | Item checkbox list |
| `MessageDialog.java` | MessageDialog | Dialog | Message display |
| `NumericKeypad.java` | NumericKeypad | Keypad | Numeric entry keypad |
| `OrderTypeButton.java` | OrderTypeButton | Button | Order type selection |
| `OrderTypeLoginButton.java` | OrderTypeLoginButton | Button | Order type on login |
| `POSButtonUI.java` | POSButtonUI | Look & Feel | POS button UI |
| `POSComboBox.java` | POSComboBox | ComboBox | Styled combobox |
| `POSFileChooser.java` | POSFileChooser | Dialog | File chooser |
| `POSLabel.java` | POSLabel | Label | Styled label |
| `POSPasswordField.java` | POSPasswordField | Password | Password input |
| `POSTextField.java` | POSTextField | Text Field | Styled text field |
| `POSTitleLabel.java` | POSTitleLabel | Label | Title label |
| `POSToggleButton.java` | POSToggleButton | Button | Toggle button |
| `POSToggleButtonUI.java` | POSToggleButtonUI | Look & Feel | Toggle button UI |
| `PaginatedListModel.java` | PaginatedListModel | Model | Paginated list |
| `PaginatedTableModel.java` | PaginatedTableModel | Model | Paginated table |
| `PosBlinkButton.java` | PosBlinkButton | Button | Blinking button |
| `PosButton.java` | PosButton | Button | Main POS button |
| `PosComboRenderer.java` | PosComboRenderer | Renderer | Combobox renderer |
| `PosOptionPane.java` | PosOptionPane | Dialog | Option pane wrapper |
| `PosScrollPane.java` | PosScrollPane | Scroll Pane | Touch-friendly scroll |
| `PosSmallButton.java` | PosSmallButton | Button | Small button variant |
| `PosUIManager.java` | PosUIManager | Utility | UI manager |
| `QwertyKeyPad.java` | QwertyKeyPad | Keypad | QWERTY keyboard |
| `ScrollableFlowPanel.java` | ScrollableFlowPanel | Panel | Scrollable flow layout |
| `ShopTableButton.java` | ShopTableButton | Button | Table selection button |
| `TimeSelector.java` | TimeSelector | Picker | Time selection |
| `TimerWatch.java` | TimerWatch | Timer | Time display |
| `TitledView.java` | TitledView | Panel | View with title |
| `TouchScrollHandler.java` | TouchScrollHandler | Handler | Touch scroll support |
| `TransparentPanel.java` | TransparentPanel | Panel | Transparent panel |
| `TransparentPanelUI.java` | TransparentPanelUI | Look & Feel | Transparent panel UI |
| `UserListDialog.java` | UserListDialog | Dialog | User selection |
| `ListBasedListModel.java` | ListBasedListModel | Model | List-backed model |
| `ListComboBoxModel.java` | ListComboBoxModel | Model | List combobox model |
| `ListModel.java` | ListModel | Model | Generic list model |
| `ListTableModel.java` | ListTableModel | Model | List table model |

---

### 4. UI Dialogs (`com.floreantpos.ui.dialog`)

| File | Class | Feature Domain | Dialog Purpose |
|------|-------|----------------|----------------|
| `AboutDialog.java` | AboutDialog | System | Application info |
| `AutomatedWeightInputDialog.java` | AutomatedWeightInputDialog | Orders | Automated weight entry |
| `BasicWeightInputDialog.java` | BasicWeightInputDialog | Orders | Manual weight entry |
| `BeanEditorDialog.java` | BeanEditorDialog | CRUD | Generic entity editor |
| `CashDropDialog.java` | CashDropDialog | Cash Management | Record cash drop |
| `ComboItemSelectionDialog.java` | ComboItemSelectionDialog | Orders | Combo item selection |
| `ConfirmDeleteDialog.java` | ConfirmDeleteDialog | CRUD | Delete confirmation |
| `CouponAndDiscountDialog.java` | CouponAndDiscountDialog | Discounts | Apply coupon/discount |
| `DateChoserDialog.java` | DateChoserDialog | Utility | Date picker |
| `DiscountListDialog.java` | DiscountListDialog | Discounts | List discounts |
| `DiscountSelectionDialog.java` | DiscountSelectionDialog | Discounts | Select discount |
| `DrawerPullReportDialog.java` | DrawerPullReportDialog | Cash Management | Drawer pull report |
| `GuestCheckTktFirstOpenDialog.java` | GuestCheckTktFirstOpenDialog | Orders | Guest check open |
| `GuestChkBillDialog.java` | GuestChkBillDialog | Orders | Guest check bill |
| `ItemSearchDialog.java` | ItemSearchDialog | Orders | Search menu items |
| `ItemSelectionDialog.java` | ItemSelectionDialog | Orders | Select menu item |
| `LanguageSelectionDialog.java` | LanguageSelectionDialog | Settings | Language selection |
| `LengthInputDialog.java` | LengthInputDialog | Orders | Length input (units) |
| `ManagerDialog.java` | ManagerDialog | Management | Manager functions menu |
| `MiscTicketItemDialog.java` | MiscTicketItemDialog | Orders | Misc item entry |
| `MultiCurrencyAssignDrawerDialog.java` | MultiCurrencyAssignDrawerDialog | Cash Management | Multi-currency drawer |
| `MultiCurrencyTenderDialog.java` | MultiCurrencyTenderDialog | Payment | Multi-currency tender |
| `NewCookongInstructionDialog.java` | NewCookongInstructionDialog | Orders | New cooking instruction |
| `NotesDialog.java` | NotesDialog | Orders | Add notes to item |
| `NumberSelectionDialog2.java` | NumberSelectionDialog2 | Utility | Number selection |
| `OkCancelOptionDialog.java` | OkCancelOptionDialog | Utility | OK/Cancel confirmation |
| `OpenTicketsListDialog.java` | OpenTicketsListDialog | Orders | List open tickets |
| `POSBackofficeDialog.java` | POSBackofficeDialog | System | Backoffice wrapper |
| `POSDialog.java` | POSDialog | Utility | Base POS dialog |
| `POSMessageDialog.java` | POSMessageDialog | Utility | Message dialog |
| `PasswordEntryDialog.java` | PasswordEntryDialog | Authentication | Password/PIN entry |
| `PaymentTypeSelectionDialog.java` | PaymentTypeSelectionDialog | Payment | Select payment type |
| `PayoutDialog.java` | PayoutDialog | Cash Management | Record payout |
| `SeatSelectionDialog.java` | SeatSelectionDialog | Orders | Select seat number |
| `SelectCookongInstructionDialog.java` | SelectCookongInstructionDialog | Orders | Select cooking instruction |
| `TableSelectionView.java` | TableSelectionView | Tables | Select table |
| `TaxSelectionDialog.java` | TaxSelectionDialog | Taxes | Select tax type |
| `TicketDetailDialog.java` | TicketDetailDialog | Orders | Ticket details view |
| `TicketItemDiscountSelectionDialog.java` | TicketItemDiscountSelectionDialog | Discounts | Item discount selection |
| `TipsCashoutReportDialog.java` | TipsCashoutReportDialog | Tips | Tips cashout report |
| `TransactionCompletionDialog.java` | TransactionCompletionDialog | Payment | Transaction complete |
| `UpdateDialog.java` | UpdateDialog | System | Application update |
| `UserListDialog.java` | UserListDialog | Users | User selection |
| `VoidTicketDialog.java` | VoidTicketDialog | Orders | Void ticket |

---

### 5. UI Views (`com.floreantpos.ui.views`)

| File | Class | Feature Domain | View Purpose |
|------|-------|----------------|--------------|
| `CashDropView.java` | CashDropView | Cash Management | Cash drop entry |
| `CashierSwitchBoardView.java` | CashierSwitchBoardView | Navigation | Cashier mode switchboard |
| `CookingInstructionSelectionView.java` | CookingInstructionSelectionView | Orders | Cooking instructions |
| `CustomerView.java` | CustomerView | Customers | Customer info display |
| `DateEntryView.java` | DateEntryView | Utility | Date entry form |
| `GroupSettleTicketSelectionWindow.java` | GroupSettleTicketSelectionWindow | Payment | Group settle selection |
| `IView.java` | IView | Interface | View interface |
| `LoginPasswordEntryView.java` | LoginPasswordEntryView | Authentication | Login password entry |
| `LoginView.java` | LoginView | Authentication | Login screen (main) |
| `NoteView.java` | NoteView | Orders | Note entry |
| `NumberSelectionView.java` | NumberSelectionView | Utility | Number selection |
| `OrderFiltersView.java` | OrderFiltersView | Orders | Order filtering |
| `OrderInfoDialog.java` | OrderInfoDialog | Orders | Order information |
| `OrderInfoView.java` | OrderInfoView | Orders | Order info view |
| `PayOutView.java` | PayOutView | Cash Management | Payout entry |
| `SplitTicketDialog.java` | SplitTicketDialog | Orders | Split ticket dialog |
| `SwitchboardOtherFunctionsDialog.java` | SwitchboardOtherFunctionsDialog | Navigation | Other functions dialog |
| `SwitchboardOtherFunctionsView.java` | SwitchboardOtherFunctionsView | Navigation | Other functions view |
| `SwitchboardView.java` | SwitchboardView | Navigation | Main switchboard (open tickets, activity) |
| `TableMapView.java` | TableMapView | Tables | Table map display |
| `TicketDetailView.java` | TicketDetailView | Orders | Ticket detail panel |
| `TicketReceiptView.java` | TicketReceiptView | Orders | Receipt preview |
| `UserTransferDialog.java` | UserTransferDialog | Users | Transfer ticket to user |

---

### 6. Payment Views (`com.floreantpos.ui.views.payment`)

| File | Class | Feature Domain | Payment Surface |
|------|-------|----------------|-----------------|
| `AuthorizableTicketBrowser.java` | AuthorizableTicketBrowser | Payment | Browse authorizable tickets |
| `AuthorizationCodeDialog.java` | AuthorizationCodeDialog | Payment | Enter auth code |
| `AuthorizationDialog.java` | AuthorizationDialog | Payment | Capture batch dialog |
| `AuthorizeDotNetProcessor.java` | AuthorizeDotNetProcessor | Integration | Authorize.Net processor |
| `CardInputListener.java` | CardInputListener | Interface | Card input events |
| `CardInputProcessor.java` | CardInputProcessor | Processing | Card input processing |
| `CardProcessor.java` | CardProcessor | Processing | Card transaction processor |
| `ConfirmPayDialog.java` | ConfirmPayDialog | Payment | Payment confirmation |
| `CreditCardTransactionProcessor.java` | CreditCardTransactionProcessor | Processing | CC transaction |
| `CustomPaymentSelectionDialog.java` | CustomPaymentSelectionDialog | Payment | Select custom payment |
| `GiftCertDialog.java` | GiftCertDialog | Payment | Gift certificate entry |
| `GratuityInputDialog.java` | GratuityInputDialog | Tips | Gratuity entry |
| `GroupPaymentView.java` | GroupPaymentView | Payment | Group payment view |
| `GroupSettleTicketDialog.java` | GroupSettleTicketDialog | Payment | Group settle dialog |
| `ManualCardEntryDialog.java` | ManualCardEntryDialog | Payment | Manual card entry |
| `MercuryPayProcessor.java` | MercuryPayProcessor | Integration | Mercury Pay processor |
| `PaymentListener.java` | PaymentListener | Interface | Payment events |
| `PaymentProcessWaitDialog.java` | PaymentProcessWaitDialog | Payment | Processing wait |
| `PaymentReferenceEntryDialog.java` | PaymentReferenceEntryDialog | Payment | Reference entry |
| `PaymentView.java` | PaymentView | Payment | Main payment view (keypad, tenders) |
| `PosPaymentWaitDialog.java` | PosPaymentWaitDialog | Payment | Payment wait |
| `SettleTicketDialog.java` | SettleTicketDialog | Payment | Main settle dialog |
| `SettleTicketProcessor.java` | SettleTicketProcessor | Processing | Settle processor |
| `SplitedTicketSelectionDialog.java` | SplitedTicketSelectionDialog | Payment | Split ticket selection |
| `SwipeCardDialog.java` | SwipeCardDialog | Payment | Card swipe input |

---

### 7. Order Views (`com.floreantpos.ui.views.order`)

| File | Class | Feature Domain | Order Surface |
|------|-------|----------------|---------------|
| `CashierModeNextActionDialog.java` | CashierModeNextActionDialog | Orders | Next action after order |
| `CategoryView.java` | CategoryView | Menu | Category selection |
| `DefaultOrderServiceExtension.java` | DefaultOrderServiceExtension | Orders | Order service extension |
| `GroupView.java` | GroupView | Menu | Group selection |
| `MenuItemView.java` | MenuItemView | Menu | Item selection grid |
| `ModifierView.java` | ModifierView | Modifiers | Modifier selection (legacy) |
| `OrderController.java` | OrderController | Orders | Order state controller |
| `OrderTypeSelectionDialog.java` | OrderTypeSelectionDialog | Orders | Order type selection |
| `OrderTypeSelectionDialog2.java` | OrderTypeSelectionDialog2 | Orders | Alt order type dialog |
| `OrderView.java` | OrderView | Orders | Main order entry screen |
| `RootView.java` | RootView | Navigation | Root view container |
| `SelectionView.java` | SelectionView | Menu | Item selection panel |
| `TicketForSplitView.java` | TicketForSplitView | Orders | Ticket split view |
| `TicketSelectionDialog.java` | TicketSelectionDialog | Orders | Ticket selection |
| `TicketView.java` | TicketView | Orders | Ticket panel (items, qty, totals) |
| `ViewPanel.java` | ViewPanel | Utility | View panel base |

#### Modifier Subdirectory (`com.floreantpos.ui.views.order.modifier`)

| File | Class | Feature Domain | Modifier Surface |
|------|-------|----------------|------------------|
| `ModifierGroupSelectionListener.java` | ModifierGroupSelectionListener | Interface | Group selection events |
| `ModifierGroupView.java` | ModifierGroupView | Modifiers | Modifier group view |
| `ModifierSelectionDialog.java` | ModifierSelectionDialog | Modifiers | Modifier selection dialog |
| `ModifierSelectionListener.java` | ModifierSelectionListener | Interface | Selection events |
| `ModifierSelectionModel.java` | ModifierSelectionModel | Modifiers | Selection model |
| `ModifierView.java` | ModifierView | Modifiers | Modifier view |
| `ModifierViewerTable.java` | ModifierViewerTable | Modifiers | Modifier table |
| `ModifierViewerTableCellRenderer.java` | ModifierViewerTableCellRenderer | Modifiers | Cell renderer |
| `ModifierViewerTableModel.java` | ModifierViewerTableModel | Modifiers | Table model |
| `TicketItemModifierTableView.java` | TicketItemModifierTableView | Modifiers | Item modifier view |

#### Pizza/Multipart Subdirectory (`com.floreantpos.ui.views.order.multipart`)

| File | Class | Feature Domain | Pizza Surface |
|------|-------|----------------|---------------|
| `PizzaModifierSelectionDialog.java` | PizzaModifierSelectionDialog | Pizza | Pizza modifier selection |
| `PizzaModifierView.java` | PizzaModifierView | Pizza | Pizza modifier view |
| `PizzaPriceTableModel.java` | PizzaPriceTableModel | Pizza | Price table model |
| `PizzaTicketItemTableModel.java` | PizzaTicketItemTableModel | Pizza | Ticket item model |

---

### 8. Back Office Window (`com.floreantpos.bo.ui`)

| File | Class | Feature Domain | BO Surface |
|------|-------|----------------|------------|
| `BackOfficeWindow.java` | BackOfficeWindow | BO | Main back office window |
| `BOMessageDialog.java` | BOMessageDialog | BO | Message dialog |
| `BrowserTable.java` | BrowserTable | BO | Browser table |
| `Command.java` | Command | BO | Command interface |
| `CustomCellRenderer.java` | CustomCellRenderer | BO | Cell renderer |
| `ModelBrowser.java` | ModelBrowser | BO | Model browser base |
| `RestaurantPropertyDialog.java` | RestaurantPropertyDialog | Settings | Restaurant properties |

---

### 9. Back Office Actions (`com.floreantpos.bo.actions`)

| File | Class | Feature Domain | BO Action |
|------|-------|----------------|-----------|
| `AttendanceHistoryAction.java` | AttendanceHistoryAction | HR | View attendance |
| `CategoryExplorerAction.java` | CategoryExplorerAction | Menu | Manage categories |
| `ConfigureRestaurantAction.java` | ConfigureRestaurantAction | Settings | Restaurant config |
| `CookingInstructionExplorerAction.java` | CookingInstructionExplorerAction | Menu | Cooking instructions |
| `CouponExplorerAction.java` | CouponExplorerAction | Discounts | Manage coupons |
| `CreditCardReportAction.java` | CreditCardReportAction | Reports | CC report |
| `CurrencyExplorerAction.java` | CurrencyExplorerAction | Settings | Manage currencies |
| `CustomPaymentReportAction.java` | CustomPaymentReportAction | Reports | Custom payment report |
| `DataExportAction.java` | DataExportAction | Data | Export data |
| `DataImportAction.java` | DataImportAction | Data | Import data |
| `DrawerPullReportExplorerAction.java` | DrawerPullReportExplorerAction | Reports | Drawer pull history |
| `EmployeeAttendanceAction.java` | EmployeeAttendanceAction | HR | Employee attendance |
| `GroupExplorerAction.java` | GroupExplorerAction | Menu | Manage groups |
| `HourlyLaborReportAction.java` | HourlyLaborReportAction | Reports | Labor report |
| `InventoryOnHandReportAction.java` | InventoryOnHandReportAction | Inventory | Inventory report |
| `ItemExplorerAction.java` | ItemExplorerAction | Menu | Manage items |
| `JournalReportAction.java` | JournalReportAction | Reports | Journal report |
| `KeyStatisticsSalesReportAction.java` | KeyStatisticsSalesReportAction | Reports | Key statistics |
| `LanguageSelectionAction.java` | LanguageSelectionAction | Settings | Language selection |
| `MenuItemSizeExplorerAction.java` | MenuItemSizeExplorerAction | Menu | Item sizes |
| `MenuUsageReportAction.java` | MenuUsageReportAction | Reports | Menu usage |
| `ModifierExplorerAction.java` | ModifierExplorerAction | Menu | Modifiers |
| `ModifierGroupExplorerAction.java` | ModifierGroupExplorerAction | Menu | Modifier groups |
| `MultiplierExplorerAction.java` | MultiplierExplorerAction | Menu | Multipliers |
| `NewMenuCategoryAction.java` | NewMenuCategoryAction | Menu | New category |
| `NewMenuGroupAction.java` | NewMenuGroupAction | Menu | New group |
| `NewMenuItemAction.java` | NewMenuItemAction | Menu | New item |
| `NewModifierAction.java` | NewModifierAction | Menu | New modifier |
| `NewModifierGroupAction.java` | NewModifierGroupAction | Menu | New modifier group |
| `OpenTicketSummaryReportAction.java` | OpenTicketSummaryReportAction | Reports | Open ticket summary |
| `OrdersTypeExplorerAction.java` | OrdersTypeExplorerAction | Settings | Order types |
| `PayrollReportAction.java` | PayrollReportAction | Reports | Payroll report |
| `PizzaCrustExplorerAction.java` | PizzaCrustExplorerAction | Pizza | Pizza crusts |
| `PizzaExplorerAction.java` | PizzaExplorerAction | Pizza | Pizza management |
| `PizzaItemExplorerAction.java` | PizzaItemExplorerAction | Pizza | Pizza items |
| `PizzaModifierExplorerAction.java` | PizzaModifierExplorerAction | Pizza | Pizza modifiers |
| `PurchaseReportAction.java` | PurchaseReportAction | Inventory | Purchase report |
| `SalesAnalysisReportAction.java` | SalesAnalysisReportAction | Reports | Sales analysis |
| `SalesBalanceReportAction.java` | SalesBalanceReportAction | Reports | Sales balance |
| `SalesDetailReportAction.java` | SalesDetailReportAction | Reports | Sales detail |
| `SalesExceptionReportAction.java` | SalesExceptionReportAction | Reports | Sales exceptions |
| `SalesReportAction.java` | SalesReportAction | Reports | Sales summary |
| `ServerProductivityReportAction.java` | ServerProductivityReportAction | Reports | Server productivity |
| `ShiftExplorerAction.java` | ShiftExplorerAction | Settings | Shifts |
| `TaxExplorerAction.java` | TaxExplorerAction | Taxes | Tax configuration |
| `TicketExplorerAction.java` | TicketExplorerAction | Orders | Ticket browser |
| `UserExplorerAction.java` | UserExplorerAction | Users | User management |
| `UserTypeExplorerAction.java` | UserTypeExplorerAction | Users | User types |
| `ViewGratuitiesAction.java` | ViewGratuitiesAction | Tips | View gratuities |

---

### 10. Back Office Explorers (`com.floreantpos.bo.ui.explorer`)

| File | Class | Feature Domain | Explorer Purpose |
|------|-------|----------------|------------------|
| `AttendanceHistoryExplorer.java` | AttendanceHistoryExplorer | HR | Attendance history |
| `CookingInstructionExplorer.java` | CookingInstructionExplorer | Menu | Cooking instructions |
| `CouponExplorer.java` | CouponExplorer | Discounts | Coupon management |
| `CurrencyDialog.java` | CurrencyDialog | Settings | Currency dialog |
| `CurrencyExplorer.java` | CurrencyExplorer | Settings | Currency management |
| `DrawerPullReportExplorer.java` | DrawerPullReportExplorer | Reports | Drawer pull history |
| `ExplorerButtonPanel.java` | ExplorerButtonPanel | Utility | Explorer button panel |
| `GratuityViewer2.java` | GratuityViewer2 | Tips | Gratuity viewer |
| `MenuCategoryExplorer.java` | MenuCategoryExplorer | Menu | Category management |
| `MenuGroupExplorer.java` | MenuGroupExplorer | Menu | Group management |
| `MenuItemExplorer.java` | MenuItemExplorer | Menu | Item management |
| `MenuItemSizeExplorer.java` | MenuItemSizeExplorer | Menu | Item sizes |
| `ModifierExplorer.java` | ModifierExplorer | Menu | Modifier management |
| `ModifierGroupExplorer.java` | ModifierGroupExplorer | Menu | Modifier groups |
| `MultiplierExplorer.java` | MultiplierExplorer | Menu | Multipliers |
| `OrderTypeExplorer.java` | OrderTypeExplorer | Settings | Order types |
| `PizzaCrustExplorer.java` | PizzaCrustExplorer | Pizza | Pizza crusts |
| `PizzaExplorer.java` | PizzaExplorer | Pizza | Pizza management |
| `PizzaItemExplorer.java` | PizzaItemExplorer | Pizza | Pizza items |
| `PizzaModifierExplorer.java` | PizzaModifierExplorer | Pizza | Pizza modifiers |
| `QuickMaintenanceExplorer.java` | QuickMaintenanceExplorer | Utility | Quick maintenance |
| `ShiftExplorer.java` | ShiftExplorer | Settings | Shift management |
| `TaxExplorer.java` | TaxExplorer | Taxes | Tax management |
| `TaxGroupExplorer.java` | TaxGroupExplorer | Taxes | Tax groups |
| `TicketExplorer.java` | TicketExplorer | Orders | Ticket browser |
| `UserExplorer.java` | UserExplorer | Users | User management |
| `UserTypeExplorer.java` | UserTypeExplorer | Users | User types |

---

### 11. Configuration UI (`com.floreantpos.config.ui`)

| File | Class | Feature Domain | Config Surface |
|------|-------|----------------|----------------|
| `AddPrinterDialog.java` | AddPrinterDialog | Printing | Add printer |
| `AddPrinterGroupDialog.java` | AddPrinterGroupDialog | Printing | Add printer group |
| `CardConfigurationView.java` | CardConfigurationView | Payment | Card config |
| `ConfigurationDialog.java` | ConfigurationDialog | Settings | Main config dialog |
| `ConfigurationView.java` | ConfigurationView | Settings | Config view base |
| `DatabaseConfigurationDialog.java` | DatabaseConfigurationDialog | System | DB config |
| `DatabaseConfigurationView.java` | DatabaseConfigurationView | System | DB config view |
| `DefaultMerchantGatewayConfigurationView.java` | DefaultMerchantGatewayConfigurationView | Payment | Gateway config |
| `InginicoConfigurationView.java` | InginicoConfigurationView | Payment | Ingenico config |
| `MultiPrinterPane.java` | MultiPrinterPane | Printing | Multi-printer config |
| `OtherConfigurationView.java` | OtherConfigurationView | Settings | Other settings |
| `PeripheralConfigurationView.java` | PeripheralConfigurationView | Hardware | Peripheral config |
| `PrintConfigurationView.java` | PrintConfigurationView | Printing | Print config |
| `PrintServiceComboRenderer.java` | PrintServiceComboRenderer | Printing | Printer renderer |
| `PrinterGroupView.java` | PrinterGroupView | Printing | Printer groups |
| `PrinterSelector.java` | PrinterSelector | Printing | Printer selection |
| `PrinterTypeSelectionDialog.java` | PrinterTypeSelectionDialog | Printing | Printer type |
| `RestaurantConfigurationView.java` | RestaurantConfigurationView | Settings | Restaurant config |
| `TaxConfigurationView.java` | TaxConfigurationView | Taxes | Tax config |
| `TerminalConfigurationView.java` | TerminalConfigurationView | System | Terminal config |
| `TerminalSetupDialog.java` | TerminalSetupDialog | System | Terminal setup |
| `TicketImportConfigurationView.java` | TicketImportConfigurationView | Integration | Ticket import |
| `UISaveHandler.java` | UISaveHandler | Utility | Save handler |
| `VirtualPrinterConfigDialog.java` | VirtualPrinterConfigDialog | Printing | Virtual printer |

---

### 12. Reports (`com.floreantpos.report`)

| File | Class | Feature Domain | Report Type |
|------|-------|----------------|-------------|
| `AttendanceReportView.java` | AttendanceReportView | HR | Attendance report |
| `CreditCardReportView.java` | CreditCardReportView | Payment | CC report |
| `CustomPaymentReportView.java` | CustomPaymentReportView | Payment | Custom payment |
| `HourlyLaborReportView.java` | HourlyLaborReportView | HR | Labor report |
| `InventoryOnHandReportView.java` | InventoryOnHandReportView | Inventory | Inventory report |
| `JournalReportView.java` | JournalReportView | Accounting | Journal |
| `MenuUsageReportView.java` | MenuUsageReportView | Menu | Menu usage |
| `OpenTicketSummaryReport.java` | OpenTicketSummaryReport | Orders | Open ticket summary |
| `PayrollReportView.java` | PayrollReportView | HR | Payroll |
| `PurchaseReportView.java` | PurchaseReportView | Inventory | Purchases |
| `ReportViewer.java` | ReportViewer | Utility | Report viewer |
| `SalesBalanceReportView.java` | SalesBalanceReportView | Sales | Sales balance |
| `SalesDetailReportView.java` | SalesDetailReportView | Sales | Sales detail |
| `SalesExceptionReportView.java` | SalesExceptionReportView | Sales | Exceptions |
| `SalesSummaryReportView.java` | SalesSummaryReportView | Sales | Sales summary |
| `ServerProductivityReportView.java` | ServerProductivityReportView | HR | Server productivity |

---

### 13. Customer UI (`com.floreantpos.customer`)

| File | Class | Feature Domain | Customer Surface |
|------|-------|----------------|------------------|
| `CustomerExplorer.java` | CustomerExplorer | Customers | Customer browser |
| `CustomerListTableModel.java` | CustomerListTableModel | Customers | Table model |
| `CustomerSelector.java` | CustomerSelector | Customers | Customer selector |
| `CustomerSelectorDialog.java` | CustomerSelectorDialog | Customers | Selection dialog |
| `CustomerSelectorFactory.java` | CustomerSelectorFactory | Customers | Factory |
| `CustomerTable.java` | CustomerTable | Customers | Customer table |
| `DefaultCustomerListView.java` | DefaultCustomerListView | Customers | List view |

---

### 14. Table Management (`com.floreantpos.table`)

| File | Class | Feature Domain | Table Surface |
|------|-------|----------------|---------------|
| `ShopTableBrowser.java` | ShopTableBrowser | Tables | Table browser |
| `ShopTableForm.java` | ShopTableForm | Tables | Table form |
| `ShopTableModelBrowser.java` | ShopTableModelBrowser | Tables | Model browser |
| `ShowTableBrowserAction.java` | ShowTableBrowserAction | Tables | Action |

---

### 15. Table Selection (`com.floreantpos.ui.tableselection`)

| File | Class | Feature Domain | Table Surface |
|------|-------|----------------|---------------|
| `BarTabSelectionView.java` | BarTabSelectionView | Bar Tabs | Bar tab selection |
| `DefaultTableSelectionView.java` | DefaultTableSelectionView | Tables | Default table view |
| `TableSelector.java` | TableSelector | Tables | Selector interface |
| `TableSelectorDialog.java` | TableSelectorDialog | Tables | Selection dialog |
| `TableSelectorFactory.java` | TableSelectorFactory | Tables | Factory |

---

### 16. Kitchen Display (`com.floreantpos.demo`)

| File | Class | Feature Domain | Kitchen Surface |
|------|-------|----------------|-----------------|
| `KitchenDisplayView.java` | KitchenDisplayView | Kitchen | Kitchen display |
| `KitchenDisplayWindow.java` | KitchenDisplayWindow | Kitchen | Kitchen window |
| `KitchenFilterDialog.java` | KitchenFilterDialog | Kitchen | Filter dialog |
| `KitchenPrinterView.java` | KitchenPrinterView | Kitchen | Printer view |
| `KitchenTicketListPanel.java` | KitchenTicketListPanel | Kitchen | Ticket list |
| `KitchenTicketStatusSelector.java` | KitchenTicketStatusSelector | Kitchen | Status selector |
| `KitchenTicketView.java` | KitchenTicketView | Kitchen | Ticket view |

---

### 17. UI Forms (`com.floreantpos.ui.forms`)

| File | Class | Feature Domain | Form Type |
|------|-------|----------------|-----------|
| `CustomerForm.java` | CustomerForm | Customers | Customer entry |
| `QuickCustomerForm.java` | QuickCustomerForm | Customers | Quick entry |
| `ShopTableSelectionDialog.java` | ShopTableSelectionDialog | Tables | Table selection |
| `UserForm.java` | UserForm | Users | User entry |
| `UserTypeForm.java` | UserTypeForm | Users | User type entry |

---

## Next Steps

With Phase 0 complete, proceed to:

1. **Phase 1**: Build complete feature index from each UI surface
2. **Phase 2**: Create per-feature forensic documentation
3. **Phase 3**: Add code traceability (file paths, methods)
4. **Phase 4**: Compare with MagiDesk for parity analysis
5. **Phase 5**: Define porting strategy
6. **Phase 6**: Classify each feature (PARITY REQUIRED, DEFER, REJECT)
