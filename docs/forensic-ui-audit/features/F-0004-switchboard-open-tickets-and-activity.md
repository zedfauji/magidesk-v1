# Feature: Switchboard — Open Tickets & Activity

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide a centralized operational dashboard to locate tickets and perform ticket-level operations (edit, settle, split, void, refund, reopen, assign driver, close).
- **Evidence**: `SwitchboardView` composes `TicketListView` plus an activity panel of actions; selection rules enforce exactly one selected ticket for many actions.

## User-facing surfaces
- **Surface type**: Screen
- **UI entry points**:
  - Login “Orders” button sets `TerminalConfig.setDefaultView(SwitchboardView.VIEW_NAME)` then logs in.
- **Primary UI regions**:
  - Ticket list + activity panel (`ticketList` is `com.floreantpos.ui.TicketListView`)
  - Right-side order type buttons (`OrderTypeButton`) and optional “NEW BAR TAB” button

## Preconditions & protections
- **Permission gating**: `updateView()` disables buttons, then enables based on `UserType.permissions`:
  - `VOID_TICKET` enables void
  - `SETTLE_TICKET` enables settle + group settle
  - `REOPEN_TICKET` enables reopen
  - `SPLIT_TICKET` enables split
  - `CREATE_TICKET` enables edit
- **Drawer gating**:
  - Settlement path requires `POSUtil.checkDrawerAssignment()`.
- **Ticket selection protection**:
  - `getFirstSelectedTicket()` requires exactly one selected ticket else shows message `Messages.getString("SwitchboardView.22")`.

## Step-by-step behavior (forensic)
- **Close order** (`doCloseOrder()`):
  1. Requires one selected ticket.
  2. Reloads full ticket (`TicketDAO.loadFullTicket`).
  3. Shows confirm dialog referencing ticket id.
  4. Calls `OrderController.closeOrder(ticket)` then refreshes ticket list.
- **Assign driver** (`doAssignDriver()`):
  1. Requires one selected ticket.
  2. Requires delivery order type; else error `SwitchboardView.8`.
  3. If already assigned, asks confirmation (`SwitchboardView.9`).
  4. Delegates to `orderServiceExtension.assignDriver(ticket.getId())`.
- **Reopen ticket** (`doReopenTicket()`):
  1. Prompts for ticket id using `NumberSelectionDialog2.takeIntInput`.
  2. Loads full ticket; validates exists.
  3. Requires ticket is closed and not voided.
  4. Sets `closed=false`, clears closing date, sets `reOpened=true`, saves.
  5. Shows `OrderInfoDialog` for the reopened ticket, refreshes list.
- **Settle ticket** (`doSettleTicket()`):
  1. Requires drawer assignment check.
  2. Uses selected ticket if present; else prompts for ticket id.
  3. Executes `new SettleTicketAction(ticket.getId()).execute()`.
  4. Refreshes list.
- **Edit ticket** (`doEditTicket()` / `editTicket(ticket)`):
  1. Uses selected ticket or prompts for ticket id.
  2. If ticket is paid: shows message `SwitchboardView.14` and aborts.
  3. Loads full ticket, sets as current in `OrderView`, navigates to `OrderView`, focuses search box.
- **Split ticket** (`doSplitTicket()`):
  1. Requires one selected ticket.
  2. Loads full ticket and opens `SplitTicketDialog`.

## Edge cases & failure paths
- **Ticket not found / invalid states**: thrown as `PosException` with specific messages.
- **Cash drawer presence**: Activity button layout differs based on `terminal.isHasCashDrawer()`.
- **Bar tab creation**: “NEW BAR TAB” appears if `FloorLayoutPlugin` exists and bar-tab order types exist; if multiple bar-tab order types, an order type selection dialog is required.

## Data / audit / financial impact
- **Writes/updates**: Reopen modifies ticket closed state and re-open flag.
- **Financial risk**:
  - Settlement triggers payments; incorrect drawer gating risks mis-attribution.
  - Void/refund flows likely create transactions/audit entries (requires action/service inspection).

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.SwitchboardView` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/SwitchboardView.java`
- **Key collaborators**: `TicketListView`, `TicketDAO`, `TicketService`, `OrderController`, `SettleTicketAction`, `VoidTicketAction`, `RefundAction`, `SplitTicketDialog`, `OrderInfoDialog`

## Uncertainties (STOP; do not guess)
- **Ticket list filtering logic**: The ticket list UI mentions filters (`PaymentStatusFilter`, order type filter) but the underlying filter application is inside `TicketListView` and/or config; not proven here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Ticket search/load-by-id
  - Ticket reopen state transitions
  - Close order transition
  - Permission checks mirroring `UserPermission`
  - Drawer assignment checks before settle
- **UI requirements**: Ticket list with selection rules + activity panel whose enabled/visible actions depend on permissions and terminal cash-drawer presence.
- **Constraints for implementers**:
  - Preserve the exact precondition checks and user prompts before destructive operations (close, reopen, settle).
