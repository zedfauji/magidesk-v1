# Current App (Magidesk) Forensic Navigation Map
**Audit Date:** 2026-01-01

## Navigation Flow

### 1. Application Startup
- **Entry**: `App.xaml.cs` -> `MainWindow.xaml` -> `MainFrame`
- **Root**: Navigates to `LoginPage` initially.

### 2. Login Flow
- **Screen**: `LoginPage`
- **Actions**:
  - `Login` -> Validates Credentials -> Navigates to `SwitchboardPage`
  - `Exit` -> Closes App

### 3. Switchboard (Main Hub)
- **Screen**: `SwitchboardPage`
- **Primary Actions**:
  - **New Ticket**: Navigates to `OrderEntryPage` (with new ticket)
  - **Edit Ticket**: Opens `OpenTicketsListDialog` -> Navigates to `OrderEntryPage`
  - **Table Map**: Navigates to `TableMapPage`
  - **Settle**: Navigates to `OpenTicketsListDialog` (or similar selection) -> `SettlePage`
  - **Manager**: Opens `ManagerFunctionsDialog` (or navigates to `BackOfficePage` directly?)
  - **Clock In/Out**: `TimeClock` interactions (Dialog/Service)
  - **Logout**: Returns to `LoginPage`

### 4. Order Entry Loop
- **Screen**: `OrderEntryPage`
- **Sub-Flows**:
  - **Add Items**: Menu Grid -> `ModifierSelectionDialog` (if needed) -> Cart
  - **Modifiers**: Select Item -> Edit -> `ModifierSelectionDialog`
  - **Ticket Ops**: `Save` (Back to Switchboard), `Pay` (Navigates to `SettlePage`), `Split` (Opens `SplitTicketDialog`)

### 5. Settlement / Payment Flow
- **Screen**: `SettlePage` / `PaymentPage`
- **Actions**:
  - `Cash` -> `CashEntryDialog` -> Finalize
  - `Credit` -> `SwipeCardDialog` / `AuthWaitDialog` -> Finalize
  - `Split Payment` -> Managed within Settle View (Partial Payments)

### 6. Backoffice / Admin
- **Screen**: `BackOfficePage` (Shell with Tabs/Navigation)
- **Sub-Pages**:
  - **Users**: `UserManagementPage`, `RoleManagementPage`
  - **Menu**: `MenuEditorPage`, `ModifierEditorPage`, `DiscountTaxPage`
  - **Inventory**: `InventoryPage`
  - **Sales/Reports**: `SalesReportsPage`, `CashSessionPage`, `ShiftExplorerPage`
  - **Config**: `SystemConfigPage`, `SettingsPage`

### 7. Kitchen Display
- **Screen**: `KitchenDisplayPage`
- **Entry**: Separate terminal mode or navigated from Switchboard (if config allows)

## Modal Hierarchy
- **Level 1 (Main Pages)**: Login, Switchboard, OrderEntry, Settle, TableMap, BackOffice
- **Level 2 (Critical Dialogs)**: SplitTicket, ModifierSelection, OpenTicketsList, PaymentProcessWait
- **Level 3 (Input Dialogs)**: CashEntry, SwipeCard, PasswordEntry, AuthorizationCode
