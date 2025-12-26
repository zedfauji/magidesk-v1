# Feature: Split Ticket (2–4 splits, optional split by seat, transaction transfer)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Split a ticket into multiple checks (2–4) and optionally split by seat number, while preserving table occupancy and correctly re-allocating payments/overpayments.
- **Evidence**: Dialog provides 2/3/4 split toggles + “Split by seat number”; persists split tickets inside a DB transaction; has logic to transfer transactions when original has paidAmount > total.

## User-facing surfaces
- **Surface type**: Dialog (full-screen)
- **UI entry points**: `SwitchboardView.doSplitTicket()` opens it.
- **Controls observed**:
  - Toggle: number of splits (2/3/4)
  - Toggle: split by seat number (visible only if order type allows seat-based order)
  - Ticket views: main + up to 3 split views
  - Buttons: Finish, Cancel

## Preconditions & protections
- **Finish guard**: If main ticket view has no items, shows `SplitTicketDialog.1` and aborts.
- **Seat-split visibility**: `btnSplitBySeat.setVisible(ticket.getOrderType().isAllowSeatBasedOrder())`.

## Step-by-step behavior (forensic)
- **Split by seat number**:
  1. Builds a map: seatNumber → list of ticket items.
  2. Iterates seat groups, transferring items from main view to split view #2, then #3, then #4.
  3. Makes view #3/#4 visible when used.
- **Changing split count**:
  - Switching to 2 or 3 hides extra views and transfers all their items back to main.
- **Finish**:
  1. Begins Hibernate transaction (`TicketDAO.createNewSession()`).
  2. Calls `saveTicket(view, session)` for each visible split view and main.
  3. Collects all tickets with ids and calls `ShopTableStatusDAO.addTicketsToShopTableStatus(original.tableNumbers, tickets, session)`.
  4. Commits.
  5. Logs `ActionHistory.SPLIT_CHECK`.

### Ticket persistence details (`saveTicket`)
- Sets split ticket’s order type to original.
- If split has no items, returns.
- Copies table numbers from original.
- If split ticket id differs and original paidAmount > original totalAmount:
  - Calls `transferTicketTransactionToSplitTicket` then updates paid/closed state via `updateModel`.
- Adds properties:
  - `Ticket.SPLIT=true`
  - `Ticket.SPLIT_NUMBER=<viewNumber>`
  - `Ticket.ORIGINAL_SPLIT_TICKET_ID=<originalTicketId>`
- Persists via `TicketDAO.saveOrUpdate(splitTicket, session)`.

## Edge cases & failure paths
- Transaction transfer tries to match a transaction whose amount equals the split due amount; otherwise clones first transaction and adjusts amounts.
- Exceptions roll back transaction and show POS error.

## Data / audit / financial impact
- **Writes/updates**:
  - New ticket records and properties for split tracking.
  - Table occupancy updated to include multiple tickets.
  - Transactions moved/duplicated for overpayment allocation.
  - Action history logged.
- **Financial risk**: Very high when reallocating payments across split tickets.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.SplitTicketDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/SplitTicketDialog.java`
- **Views**: `TicketForSplitView`
- **DAOs**: `TicketDAO`, `ShopTableStatusDAO`, `ActionHistoryDAO`

## Uncertainties (STOP; do not guess)
- **How `TicketForSplitView.transferTicketItem` updates modifiers/pricing**: requires inspection of `TicketForSplitView`.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Split ticket model + properties
  - Transaction reallocation semantics for overpayment
  - Atomic persistence + table status update
- **UI requirements**: Multi-pane split workspace and seat-based split shortcut.
- **Constraints for implementers**: Preserve transaction transfer logic and persistence atomicity.
