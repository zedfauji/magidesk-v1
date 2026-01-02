# FloreantPOS Forensic Navigation Map
**Audit Date:** 2026-01-01

## Navigation Flow

### 1. Application Startup
- **Entry**: `Main.main()` -> `Application.start()`
- **Initialization**: 
  - `DatabaseConfigurationDialog` (if config missing)
  - `InitializeSystem` (Fonts, Locale, Terminal Config)
- **Root View**: Sets `LoginView` as the initial root.

### 2. Login Flow
- **Screen**: `LoginView`
- **Actions**:
  - `Login` -> Authenticates User -> Transitions to `SwitchboardView`
  - `Shutdown` -> Exits App
  - `Reset Database` (Debug/Dev mode)

### 3. Switchboard (Main Hub)
- **Screen**: `SwitchboardView` (or `CashierSwitchBoardView` based on role)
- **Primary Actions (Buttons)**:
  - **Orders**: `New Ticket`, `Edit Ticket`, `Open Tickets` (Dialog)
  - **Tables**: `Table Map` (Transitions to `TableMapView`)
  - **Operations**: `Refund`, `Void`, `Pay Out` (Dialog), `Cash Drop` (Dialog)
  - **System**: `Manager` (Backoffice), `Logout` (Returns to `LoginView`), `Shut Down`

### 4. Order Entry Loop
- **Entry**: From Switchboard (`New Ticket` or `Table Selection`)
- **Screen**: `OrderView` / `TicketDetailView`
- **Sub-Flows**:
  - **Add Items**: Group Selection -> Item Selection -> Modifier Dialog (if applicable) -> Added to List
  - **Edit Items**: Select Item -> `Edit` -> `Modifier Selection Dialog` or `Cooking Instruction Dialog`
  - **Ticket Ops**: `Save`, `Pay` (Transitions to Settle), `Void`, `Split` (Dialog)

### 5. Settlement / Payment Flow
- **Entry**: From Order View (`Pay`) or Switchboard (`Settle`)
- **Screen**: `SettleTicketView` / `PaymentSelectionView`
- **Actions**:
  - `Cash` -> `CashDrawer` logic -> Receipt Print -> Close Ticket
  - `Card` -> `CardPaymentView` / `CardSwipe` -> Authorize -> Close
  - `Split Payment` -> Partial Tender -> Remaining Balance Loop

### 6. Manager / Backoffice
- **Entry**: From Switchboard (`Manager` button)
- **Screen**: `BackOfficeWindow` (Tabbed Interface)
- **Tabs**:
  - `Administration`: Users, Config
  - `Inventory`: Items, Groups, Vendors
  - `Menu`: Categories, Groups, Items, Modifiers
  - `Reports`: Selection of Report Types -> `ReportViewer`

### 7. Table Management
- **Entry**: From Switchboard (`Table Map`)
- **Screen**: `TableMapView`
- **Actions**:
  - Select Table -> `New Ticket` or `Open Existing Ticket`
  - Check Status (Occupied, Free, Dirty)

## Modal Hierarchy
- **Level 1 (Main Screens)**: Login, Switchboard, Order, TableMap, Backoffice
- **Level 2 (Critical Dialogs)**: Settle, OpenTicketsList, SplitTicket, ManagerPassword
- **Level 3 (Functional Dialogs)**: Modifiers, Notes, CookingInstructions, NumberSelection
