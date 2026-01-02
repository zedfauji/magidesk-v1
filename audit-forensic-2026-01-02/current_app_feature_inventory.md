# Current App (Magidesk) Forensic Feature Inventory
**Audit Date:** 2026-01-01
**Source:** Magidesk Codebase (`Magidesk.Presentation`, `Magidesk.Application`, `Magidesk.Domain`)

## 1. Core Domain Features (Domain & Application)

### Orders & Tickets
- **Creation**: `CreateTicketCommandHandler`, `Ticket`, `OrderLine`
- **Modifiers**: `ModifierSelectionDialog`, `OrderLineModifier`, `ModifierAttributes` (Fractional)
- **Cooking Instructions**: `KitchenOrder`, `CookingInstruction` (Implied in `KitchenOrderItem`)
- **Workflow**: `SplitTicketCommandHandler`, `MergeTicketsCommandHandler`, `TransferTicketCommandHandler`
- **Types**: `OrderType` (Dine In, Carry Out, etc.), `CreateOrderTypeCommandHandler`

### Payments & Finance
- **Processing**: `ProcessPaymentCommandHandler`, `SettleTicketCommandHandler`, `PayNowCommandHandler`
- **Method Types**: `CashPayment`, `CreditCardPayment`, `DebitCardPayment`, `GiftCertificatePayment`
- **Cash Control**: `CashSession`, `DrawerBleed`, `CashDrop`, `DrawerPullReport`
- **Gateway**: `MerchantBatchService`, `AuthorizationCaptureBatch`, `SwipeCardDialog`
- **Discounts/Coupons**: `Discount`, `ApplyDiscountCommandHandler`, `ApplyCouponCommandHandler`
- **Taxation**: `Tax`, `TaxGroup` (Implied in calculations)

### Kitchen Display System (KDS)
- **Routing**: `KitchenRoutingService`, `PrintToKitchenCommandHandler`
- **Display**: `KitchenDisplayPage`, `KitchenOrder`, `KitchenStatusService`

### Inventory & Menu
- **Menu Management**: `MenuEditorPage`, `MenuCategory`, `MenuGroup`, `MenuItem`
- **Inventory**: `InventoryPage`, `InventoryItem`, `Recipe` (RecipeLine)

### Tables & Floor
- **Map**: `TableMapPage`, `Table`
- **Actions**: `AssignTableToTicket`, `ChangeTable`, `ReleaseTable`, `GetAvailableTables`

## 2. User Interface (Views & Dialogs)

### Main Navigation
- **Login**: `LoginPage`
- **Switchboard**: `SwitchboardPage` (Central Hub)
- **Order Entry**: `OrderEntryPage`
- **Payment**: `PaymentPage`, `SettlePage`
- **Admin**: `BackOfficePage` (Tabbed container)

### Operational Dialogs
- **Ticket**: `SplitTicketDialog`, `VoidTicketDialog`, `OpenTicketsListDialog`, `GroupSettleTicketDialog`
- **Payment**: `CashEntryDialog`, `SwipeCardDialog`, `AuthorizationCodeDialog`
- **Modifiers**: `ModifierSelectionDialog`
- **System**: `PasswordEntryDialog`, `ManagerFunctionsDialog`, `CashDropManagementDialog`

### Management Pages (Backoffice/Admin)
- **User**: `UserManagementPage`, `RoleManagementPage`
- **Menu**: `MenuEditorPage`, `ModifierEditorPage`, `OrderTypeExplorerPage`
- **Sales**: `SalesReportsPage`, `ShiftExplorerPage`, `CashSessionPage`
- **Inventory**: `InventoryPage`, `DiscountTaxPage`
- **System**: `SystemConfigPage`, `SettingsPage`, `PrintPage`

## 3. Reporting
- **Sales**: `SalesReportsPage` (ViewModel implies multiple report types)
- **Shift**: `ShiftExplorerPage`, `ShiftReport`
- **Cash**: `DrawerPullReportDialog`

## 4. System Services
- **Hardware**: `PrintReceiptCommandHandler`, `PrintToKitchenCommandHandler`
- **Security**: `LoginViewModel`, `Role`, `User`
- **Config**: `SystemConfigViewModel`, `RestaurantConfiguration`
