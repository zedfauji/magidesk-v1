# Feature: Ticket panel (search/add items, qty controls, pay-now)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide the fast, operator-driven ticket manipulation surface: add items by barcode/id, adjust quantities (including fractional units), enforce stock rules, print-to-kitchen gating, and jump into payment.
- **Evidence**: `TicketView` wires item search, quantity buttons, edit/delete, kitchen print triggers, and `doPayNow()`.

## User-facing surfaces
- **Surface type**: Panel within order flow
- **UI entry points**:
  - Used inside `OrderView` (ticketViewContainer)
- **Controls observed**:
  - Search text box + search dialog
  - Scroll up/down
  - Increase/decrease quantity
  - Delete selection
  - Edit selection (seat items vs modifier dialog)
  - Total button (invokes pay-now)

## Preconditions & protections
- **Drawer assignment gating before pay**: `doPayNow()` returns early if `!POSUtil.checkDrawerAssignment()`.
- **Stock gating**:
  - For items with `disableWhenStockAmountIsZero`, operations can be blocked with “Items are not available in stock”.
  - Separate logic for fractional units.
- **Kitchen-print gating**:
  - After printing to kitchen (`sendTicketToKitchen()`), `cancelable` and `allowToLogOut` are changed; selection controls are disabled for printed items depending on config (`TerminalConfig.isAllowedToDeletePrintedTicketItem()`).

## Step-by-step behavior (forensic)
- **Add item by search box**:
  1. On Enter: if empty, shows “Please enter item number or barcode”.
  2. Tries barcode lookup first; if not found, tries integer item id lookup.
  3. Applies order-type filter (menu item’s order types must contain ticket order type).
  4. Applies stock filter.
  5. On success: delegates to `OrderController.itemSelected(menuItem)`.
- **Search dialog button**:
  - Opens `ItemSearchDialog`, then applies the same add-by-barcode/id logic.
- **Total button**:
  - If `ticket.orderType.isHasForHereAndToGo()`, opens `OrderTypeSelectionDialog2` and, if chosen, updates ticket item prices by order type before paying.
  - Calls `doPayNow()`.
- **Pay now (`doPayNow`)**:
  1. Checks drawer assignment.
  2. `updateModel()` (guards empty ticket unless bar tab; recalculates price).
  3. Saves order (`OrderController.saveOrder(ticket)`).
  4. Fires `payOrderSelected(ticket)` to listeners.
- **Send to kitchen**:
  - Saves ticket, then if `orderType.shouldPrintToKitchen` and `ticket.needsKitchenPrint()`, prints to kitchen and marks as non-cancelable/non-logoff.
- **Edit selection**:
  - If selected is a seat marker (`TicketItem.isTreatAsSeat()`), opens `SeatSelectionDialog` and can prompt for manual seat number; updates seat marker name and propagates seat number forward until next seat marker.
  - Else opens modifier dialog via `OrderController.openModifierDialog((ITicketItem)object)`.
- **Increase quantity**:
  - For fractional units uses either `AutomatedWeightInputDialog` or `BasicWeightInputDialog` depending on `TerminalConfig.getScaleActivationValue()`.

## Edge cases & failure paths
- **Stale state during pay**: shows reload dialog.
- **Ticket null**: `updateView()` attempts to reference `ticket.getTicketType()` while also checking `ticket == null`; this suggests either dead code or a latent bug; behavior is not provable from this alone.

## Data / audit / financial impact
- **Writes/updates**:
  - Kitchen printing (`ReceiptPrintService.printToKitchen`) affects operational output.
  - Save order persists ticket changes.
- **Financial risk**: stock gating mistakes can permit selling out-of-stock items.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.order.TicketView` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/order/TicketView.java`
- **Dialogs**: `ItemSearchDialog`, `SeatSelectionDialog`, `AutomatedWeightInputDialog`, `BasicWeightInputDialog`
- **Workflow**: `OrderController`, `ReceiptPrintService`, `POSUtil.checkDrawerAssignment`, `TerminalConfig.isAllowedToDeletePrintedTicketItem`

## Uncertainties (STOP; do not guess)
- **Exact keyboard shortcuts**: This file uses action listeners but does not prove explicit keyboard accelerators (KeyStroke bindings); additional scanning required.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Menu item lookup by barcode and by id
  - Order-type and stock enforcement rules
  - Fractional unit support
  - Kitchen print decisioning
- **UI requirements**:
  - Extremely low-latency add-item flow
  - Quantity controls with conditional disabling when printed
  - Weight/quantity input dialogs for fractional units
- **Constraints for implementers**:
  - Preserve the order of item lookup (barcode first, then numeric id).
  - Preserve the stock gating semantics for both fractional and non-fractional units.
