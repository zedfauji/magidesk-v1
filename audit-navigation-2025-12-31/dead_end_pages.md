# Dead-end UI Surfaces (Current App)

**Definition**: A surface with **no outbound navigation** (no proven navigation actions originating from it).

**Important**: A page can appear as a dead-end if navigation occurs via:
- `GoBack()` (implicit return)
- dialog close buttons
- or code paths not yet scanned

## Likely dead-ends (by current evidence)
- **CashSessionPage**
  - **Inbound**: MainWindow sidebar
  - **Outbound**: no explicit navigation calls found in `CashSessionViewModel`.

- **PaymentPage**
  - **Inbound**: MainWindow sidebar
  - **Outbound**: no explicit navigation calls found in `PaymentViewModel`.

- **DiscountTaxPage**
  - **Inbound**: MainWindow sidebar / BackOffice internal nav
  - **Outbound**: UNKNOWN (no navigation edges captured yet).

- **PrintPage**
  - **Inbound**: MainWindow sidebar
  - **Outbound**: UNKNOWN (no navigation edges captured yet).

- **SalesReportsPage**
  - **Inbound**: MainWindow sidebar / BackOffice internal nav
  - **Outbound**: UNKNOWN (no navigation edges captured yet).

- **UserManagementPage**
  - **Inbound**: MainWindow sidebar / BackOffice internal nav
  - **Outbound**: UNKNOWN (no navigation edges captured yet).

- **SettingsPage**
  - **Inbound**: MainWindow sidebar / BackOffice internal nav
  - **Outbound**: UNKNOWN (no navigation edges captured yet).

- **SystemConfigPage**
  - **Inbound**: ManagerFunctionsViewModel navigation / BackOffice internal nav
  - **Outbound**: no explicit navigation calls found in `SystemConfigViewModel` (it shows confirmation dialogs).

- **KitchenDisplayPage**
  - **Inbound**: Switchboard button
  - **Outbound**: UNKNOWN (no navigation edges captured yet).

## Not dead-ends (have outbound edges)
- **SwitchboardPage**
- **ManagerFunctionsDialog**
- **TicketPage**
- **TableMapPage**
- **OrderEntryPage**
- **SettlePage**
- **BackOfficePage**

## UNKNOWN
- Many surfaces rely on the global `GoBack()` or dialog close semantics, which are not explicit edges.
