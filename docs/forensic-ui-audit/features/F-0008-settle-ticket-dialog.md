# Feature: Settle Ticket dialog (ticket list + payment)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide a dedicated settlement workspace showing ticket items and totals alongside payment controls, including special-case bar-tab settlement.
- **Evidence**: `SettleTicketDialog` composes `TicketViewerTable` + totals + `PaymentView`, and handles bar-tab preauthorization capture/void.

## User-facing surfaces
- **Surface type**: Dialog
- **UI entry points**: Invoked via switchboard settlement action (`SettleTicketAction` → not inspected here)

## Preconditions & protections
- **Receipt consolidation**: If `ticket.orderType.isConsolidateItemsInReceipt()` then `ticket.consolidateTicketItems()` happens in constructor.
- **Delivery charge visibility**: In totals panel, delivery charge row is only visible when `orderType.isDelivery()` and `!ticket.isCustomerWillPickup()`.

## Step-by-step behavior (forensic)
1. Constructed with a `Ticket` and current user.
2. Initializes `SettleTicketProcessor(currentUser, this)` and registers as a `PaymentListener`.
3. Builds UI:
   - Ticket info header: ticket id, optional table numbers, optional customer name (from ticket property `Ticket.CUSTOMER_NAME`).
   - Center: `TicketViewerTable` for the ticket.
   - Bottom: subtotal/discount/delivery/tax/gratuity/total fields.
   - Right: `PaymentView` bound to the same processor.
4. On `paymentDataChanged()`: updates totals, payment view, and ticket table.
5. On `ticketDataChanged()`: recalculates price, saves order (`OrderController.saveOrder(ticket)`), refreshes totals/payment, and refreshes `OrderView` if visible.
6. Refresh flow reloads full ticket by id and re-applies `ticketDataChanged()`.

### Bar tab settlement branch (`settleBartab`)
- If ticket has a bar-tab transaction that is not captured and not voided:
  1. Prompts: “Pay using” with options “Pre-Authorized Tab” vs “Other Payment”.
  2. If pre-authorized tab chosen: calls `payUsingPreAuthorizedBartab(...)`.
- Otherwise (or if choosing not to use preauth):
  1. Prompts warning: “This will void Pre-Authorized amount in Tab” with “Void” vs “Cancel”.
  2. If cancel: shows “Payment canceled”, marks canceled and disposes.
  3. If void: `ticketProcessor.doVoidBartab(...)` then message “Pre-Authorized tab is voided”.

### Pre-authorized bar tab capture (`payUsingPreAuthorizedBartab`)
1. Prints ticket (behavior depends on gratuity amount; if none prints with add-tips-later flag).
2. Prompts for tips amount via `NumberSelectionDialog2.takeDoubleInput("Enter tips amount", 0.0)`; if >0 sets gratuity amount and recalculates.
3. Sets transaction tender/tips/amount fields; calls `ticketProcessor.captureBartabTransaction(...)`.
4. Updates ticket paid amounts; if due becomes 0, marks ticket paid and possibly closed depending on `orderType.isCloseOnPaid()`.
5. Persists ticket changes in a new Hibernate session/transaction.
6. Calls `ticketProcessor.doAfterSettleTask(...)`.

## Edge cases & failure paths
- **Tax included mode**: displays a message token from `Messages.getString("TicketView.35")` instead of numeric tax.
- **DB transaction failure during bar-tab capture**: transaction rollback and exception propagation.

## Data / audit / financial impact
- **Writes/updates**:
  - Ticket paid/closed state, closing date.
  - `PosTransaction` updates for bar tab capture.
  - Receipt printing.
- **Financial risk**: Incorrect bar-tab capture/void sequencing can lead to double-charging or failing to void preauthorized amounts.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.payment.SettleTicketDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/payment/SettleTicketDialog.java`
- **Payment UI**: `PaymentView`
- **Processor**: `SettleTicketProcessor`
- **Prompts**: `POSMessageDialog`, `NumberSelectionDialog2`

## Uncertainties (STOP; do not guess)
- **Full settle flow**: The standard non-bar-tab settle workflow is primarily in `SettleTicketProcessor` and payment processors; this doc only proves UI composition and bar-tab special-case handling.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Bar-tab transaction state machine: preauth → capture OR void
  - Atomic persistence for paid/closed transitions
  - Receipt printing before tips capture in preauth path
- **UI requirements**:
  - Side-by-side ticket list + payment
  - Explicit two-step prompts for bar-tab decisioning
- **Constraints for implementers**:
  - Preserve the exact prompt/branch semantics (use preauth vs other payment; void warning).
  - Preserve printing-before-tips prompt in the preauth path.
