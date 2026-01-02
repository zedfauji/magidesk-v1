# FloreantPOS Forensic Feature Inventory
**Audit Date:** 2026-01-01
**Source:** FloreantPOS Codebase (`src/com/floreantpos/...`)

## 1. Core Domain Features (Model)
Derived from `com.floreantpos.model`

### Orders & Tickets
- **Ticket Creation**: `Ticket`, `TicketItem`, `ShopTableTicket`, `TicketType`
- **Modifiers**: `TicketItemModifier`, `TicketItemModifierGroup`, `Modifier`, `MenuItemModifierGroup`
- **Cooking Instructions**: `CookingInstruction`, `TicketCookingInstruction`
- **Pizza Builder**: `PizzaPrice`, `PizzaModifierPrice` (Specialized handling)
- **Discounts**: `Discount`, `TicketDiscount`, `TicketItemDiscount`, `CouponAndDiscount`
- **Voiding**: `VoidReason`, `VoidTransaction`, `VoidTicketEntry`
- **Delivery**: `DeliveryAddress`, `DeliveryCharge`, `DeliveryInstruction`, `DeliveryConfiguration`, `ZipCodeVsDeliveryCharge`

### Payments & Finance
- **Transactions**: `PosTransaction`, `CashTransaction`, `CreditCardTransaction`, `DebitCardTransaction`, `GiftCertificateTransaction`, `RefundTransaction`, `VoidTransaction`
- **Cash Drawer**: `CashDrawer`, `CashDrawerResetHistory`, `DrawerPullReport`, `DrawerAssignedHistory`
- **Taxation**: `Tax`, `TaxGroup`, `TaxSelectionDialog`
- **Gratuity**: `Gratuity`, `TipsCashoutReport`
- **Currency**: `Currency`, `CurrencyBalance`, `MultiCurrencyTender`

### Inventory & Menu
- **Menu Items**: `MenuItem`, `MenuGroup`, `MenuCategory`, `MenuModifier`
- **Inventory Control**: `InventoryItem`, `InventoryGroup`, `InventoryLocation`, `InventoryTransaction`, `InventoryVendor`, `Recepie`, `PurchaseOrder`
- **Stock Management**: `InventoryMetacode`, `PackagingUnit`

### Tables & Floor
- **Floor Plan**: `ShopFloor`, `ShopFloorTemplate`
- **Table Management**: `ShopTable`, `ShopTableType`, `ShopTableStatus`, `TableBookingInfo`

## 2. User Interface (Screens & Dialogs)
Derived from `com.floreantpos.ui`

### Major Views
- **Login**: `LoginView`, `PasswordScreen`, `LoginPasswordEntryView`
- **Dashboard**: `SwitchboardView`, `CashierSwitchBoardView`, `SwitchboardOtherFunctionsView`
- **Order Entry**: `OrderView` (implied), `OrderInfoView`, `TicketDetailView`, `OpenTicketView`
- **Payment**: `SettleTicketView`, `CardPaymentView`, `PayOutView`
- **Table Map**: `TableMapView`, `TableSelectionView`
- **Kitchen**: `KitchenDisplayView` (implied from action/model)

### Critical Dialogs
- **Ticket Management**: `SplitTicketDialog`, `VoidTicketDialog`, `OpenTicketsListDialog`, `OrderInfoDialog`, `MiscTicketItemDialog`
- **Modifiers & Editing**: `NotesDialog`, `CookingInstructionSelectionView`, `ModifierSelectionDialog` (implied), `SeatSelectionDialog`
- **Financial**: `PayoutDialog`, `CashDropDialog`, `MultiCurrencyAssignDrawerDialog`, `TipsCashoutReportDialog`
- **System**: `ManagerDialog`, `POSBackofficeDialog`, `DatabaseConfigurationDialog`, `NumberSelectionDialog`

## 3. Reporting & Output
Derived from `com.floreantpos.report` & `com.floreantpos.print`

- **Sales Reports**: `SalesBalanceReport`, `SalesSummaryReport`, `SalesDetailedReport`, `SalesExceptionReport`, `SalesAnalysisReport`
- **Employee Reports**: `PayrollReport`, `ServerProductivityReport`, `AttendanceReport`
- **Inventory Reports**: `InventoryOnHandReport`
- **Journal**: `JournalReport`
- **Printing**: `TicketReceiptView`, `KitchenTicketPrint`, `ReceiptPrintService`

## 4. System & Configuration
Derived from `com.floreantpos.config` & `com.floreantpos.main`

- **Hardware**: `TerminalConfig`, `PrinterConfiguration`, `CardReader`, `PosPrinters`
- **Configuration**: `GlobalConfig`, `AppProperties`
- **Security**: `User`, `UserPermission`, `UserType`
