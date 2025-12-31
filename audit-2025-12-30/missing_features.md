# Missing & Partial Features

## Scope
This list groups features that are **MISSING** or **PARTIAL** in the current app relative to FloreantPOS reference behaviors.
Evidence is based on this audit’s static scan and may be refined in later passes.

## Core POS
- **Order entry cohesive split-screen workflow**
  - **Status**: Partial
  - **Evidence**: `OrderEntryPage.xaml` exists but full throughput workflow parity not established.

- **Stable user/terminal/session context propagation**
  - **Status**: Partial
  - **Evidence**: Hardcoded fallback IDs in `SwitchboardViewModel.cs`.

## Payments
- **Gift certificate dedicated UI**
  - **Status**: Partial
  - **Evidence**: Gift cert fields simulated in `SettleViewModel.cs`; no dedicated dialog found.

- **Debit card / other tender dedicated flows**
  - **Status**: Partial
  - **Evidence**: `SettleViewModel` supports `PaymentType` parsing; UI parity not verified.

- **Hardware-backed swipe/authorization/capture lifecycle**
  - **Status**: Partial
  - **Evidence**: swipe/auth dialogs exist; processing is simulated in places.

## Orders
- **Resume/edit open tickets with full context**
  - **Status**: Partial
  - **Evidence**: switchboard navigates to order entry by ticket ID; end-to-end parity not verified.

- **Refund UI workflow**
  - **Status**: Partial/Missing UI
  - **Evidence**: refund commands exist; UI surface not confirmed.

- **Transfer ticket UI workflow**
  - **Status**: Partial/Missing UI
  - **Evidence**: `TransferTicketCommand.cs` exists; UI surface not confirmed.

## Kitchen
- **Stateful KDS workflow (NEW -> COOKING -> DONE) comparable to Floreant expectations**
  - **Status**: Partial
  - **Evidence**: `KitchenController` supports bump/void; deeper state machine parity not verified.

## Tables
- **Table selection dialog integrated into new ticket flow**
  - **Status**: Missing
  - **Evidence**: switchboard TODO for table-required order types; no dedicated table selection dialog located.

- **Customer selection integrated into new ticket flow**
  - **Status**: Missing
  - **Evidence**: switchboard TODO for customer-required order types.

## Inventory
- **Inventory on hand reporting and purchase flows**
  - **Status**: Unverified/likely partial
  - **Evidence**: `InventoryPage.xaml` exists; Floreant has inventory report views; parity not established.

## Reports
- **Full report surface parity for all Floreant back-office reports**
  - **Status**: Partial
  - **Evidence**: `ReportsController` implements multiple report endpoints; UI parity for each report is not confirmed.

## Admin / Config
- **Printer configuration UI and routing/printer-group management**
  - **Status**: Missing
  - **Evidence**: No dedicated printer setup UI found; Floreant has multiple printer config dialogs.

- **First-time setup wizard comparable to Floreant `SetUpWindow`**
  - **Status**: Missing
  - **Evidence**: No setup wizard UI found; system init currently reports failure via loading overlay.
