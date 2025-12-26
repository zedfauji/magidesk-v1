# Feature: Order entry view container (dine-in/retail dual-mode)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Centralize order entry and ticket editing, including table/guest/seat workflows, customer selection, kitchen send/hold/save/cancel, and special constraints (e.g., post-kitchen-send cancellation confirmation).
- **Evidence**: `OrderView` holds `CategoryView`, `GroupView`, `MenuItemView`, and `TicketView` and wires action buttons to ticket operations and dialogs.

## User-facing surfaces
- **Surface type**: Screen
- **UI entry points**:
  - Switchboard “Edit Ticket” navigates to `OrderView` and focuses item search.
- **Notable controls**:
  - Customer, Delivery Info, Table, Guest count, Seat selection
  - Cooking instructions
  - Misc item insert
  - Hold, Send to Kitchen, Cancel, Save/Done

## Preconditions & protections
- **Kitchen-send cancellation guard**:
  - If `ticketView.isCancelable()` false, cancel requires explicit confirmation: “Items have been sent to kitchen…”.
- **Hold privileged override (conditional)**:
  - For certain order types (table selection + required customer data) and when current user lacks `UserPermission.HOLD_TICKET`, hold requires “privileged password” via `PasswordEntryDialog.show(...)` and then permission check against `UserDAO.findUserBySecretKey(password)`.
- **Empty ticket guard on hold (non-bar-tab)**:
  - If not bar tab and no items → error `TICKET_IS_EMPTY_`.

## Step-by-step behavior (forensic)
- **Save/Done**: delegates to `ticketView.doFinishOrder()`; stale state shows reload dialog; domain exceptions shown.
- **Send to Kitchen**:
  1. Calls `ticketView.sendTicketToKitchen()`.
  2. Calls `ticketView.updateView()`.
  3. Shows “Items sent to kitchen”.
- **Cancel**:
  - If ticket cancelable → `ticketView.doCancelOrder()`.
  - Else confirmation required; on confirm cancels and sets `allowToLogOut=true`.
- **Customer selection**: Opens `CustomerSelectorDialog` full-screen undecorated, binds selected customer to ticket and updates button label.
- **Table number update**:
  1. Opens `TableSelectorDialog` full-screen undecorated.
  2. On confirm, updates `ticket.tableNumbers`, removes ticket from previous table statuses and occupies tables.
- **Guest number**:
  1. Opens `NumberSelectionDialog2` with current guest count.
  2. Disallows 0.
  3. Updates ticket numberOfGuests.
- **Seat flow**:
  - Opens `SeatSelectionDialog` with current tables + existing seat numbers.
  - Allows “enter seat number” via number dialog.
  - Inserts seat-marker ticket item (`treatAsSeat=true`, `shouldPrintToKitchen=true`) and sets seat number.
- **Cooking instructions**:
  - Only for selected `TicketItem` that is not printed to kitchen; opens `CookingInstructionSelectionView` and attaches `TicketItemCookingInstruction` list.
- **Retail order mode**:
  - If `orderType.isRetailOrder()` the view switches to show ticket + payment side-by-side and hides many order buttons.

## Edge cases & failure paths
- **StaleStateException**: uses `POSMessageDialog.showMessageDialogWithReloadButton(...)`.
- **Tax-included mode**: ticket summary tax displays a message (not numeric) when price includes tax.

## Data / audit / financial impact
- **Writes/updates**:
  - Table occupancy updates via `ShopTableStatusDAO` + `ShopTableDAO.occupyTables`.
  - Customer assignment persists with ticket.
  - Kitchen send triggers printing and state changes downstream.
- **Operational risk**:
  - Incorrect cancellation behavior after kitchen send can cause inventory/kitchen production mismatch.
  - Incorrect hold permission gate could allow unauthorized “park ticket” behavior.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.order.OrderView` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/order/OrderView.java`
- **Key dialogs**: `CustomerSelectorDialog`, `TableSelectorDialog`, `SeatSelectionDialog`, `MiscTicketItemDialog`, `CookingInstructionSelectionView`, `PasswordEntryDialog`
- **Workflow**: `OrderController`, `TicketDAO`, `UserDAO`

## Uncertainties (STOP; do not guess)
- **Discount application UI**: `addDiscount()` is largely commented and does not prove the intended ticket-item discount UI behavior.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Ticket lifecycle: hold, save, send-to-kitchen, cancel
  - Table occupancy transitions
  - Permission model with manager override via secret key
- **UI requirements**:
  - Full-screen selection dialogs (customer/table)
  - Seat marker insertion behavior
  - Retail mode view composition with embedded payment
- **Constraints for implementers**:
  - Preserve cancel confirmation semantics when items have been sent to kitchen.
  - Preserve hold permission override flow (secret key lookup and permission check).
