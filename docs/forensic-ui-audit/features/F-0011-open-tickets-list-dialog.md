# Feature: Open Tickets List (manager view) + transfer server + void

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide an operator/manager list of currently open tickets, with operational actions (void, transfer server) and a cashier-mode shortcut to resume a ticket.
- **Evidence**: Loads open tickets via `TicketDAO.findOpenTickets()` or per-user in cashier mode, and provides Void/Transfer buttons when not cashier mode.

## User-facing surfaces
- **Surface type**: Dialog (modal)
- **UI entry points**: Via `ManagerDialog.doShowOpenTickets()`.
- **Controls observed**:
  - Scroll up/down
  - Close
  - (Not cashier mode only) Void, Transfer Server
  - Ticket list table: Ticket ID, Server, Date/Time, Total

## Preconditions & protections
- **Cashier mode behavior**:
  - Data scope becomes “open tickets for current user”: `TicketDAO.findOpenTicketsForUser(Application.getCurrentUser())`.
  - Row selection immediately loads the full ticket and sets it into `OrderView`, then disposes the dialog.
- **Non-cashier mode**:
  - Void and Transfer Server buttons are present.

## Step-by-step behavior (forensic)
- **Close**:
  - Disposes.
  - In cashier mode, opens `CashierModeNextActionDialog` with message `Messages.getString("OpenTicketsListDialog.0")`.
- **Transfer server**:
  1. Requires a selected ticket; else error `SELECT_TICKET`.
  2. Opens `UserListDialog`.
  3. If not canceled and selected user differs from current owner, sets `ticket.owner=selectedUser` and `TicketDAO.update(ticket)`.
- **Void**:
  1. Requires a selected ticket.
  2. Loads full ticket: `TicketDAO.loadFullTicket(ticket.getId())`.
  3. Opens `VoidTicketDialog` with that ticket.
  4. If void dialog not canceled, removes ticket from list.

## Edge cases & failure paths
- Transfer server errors show generic POS error message.

## Data / audit / financial impact
- **Writes/updates**: Transfer server updates ticket ownership.
- **Financial risk**: Void path leads to refund/void logic in `VoidTicketDialog` (separate feature).

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.dialog.OpenTicketsListDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/dialog/OpenTicketsListDialog.java`
- **Entry**: `ManagerDialog.doShowOpenTickets()`
- **Dependencies**: `TicketDAO`, `UserListDialog`, `VoidTicketDialog`, `OrderView` (cashier mode)

## Uncertainties (STOP; do not guess)
- **Why cashier-mode closes opens next-action dialog**: Behavior depends on `CashierModeNextActionDialog` logic not inspected here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Query open tickets (global and per-user), transfer ticket ownership, ticket void entry.
- **UI requirements**: Ticket list dialog with mode-dependent behavior and actions.
- **Constraints for implementers**: Preserve cashier-mode “select row loads ticket into order view” shortcut.
