# Feature: Void Ticket (with refund + tips refund + delete-vs-void)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide a controlled void workflow that can optionally perform a cash refund (including optional tip refund), record reason/waste disposition, update cash drawer balance, and emit audit/print artifacts.
- **Evidence**: `btnVoidActionPerformed` branches on paid amount, prompts for refund, optionally refunds tips, decides delete vs void based on kitchen-print state, prints refund + generic report, and logs `ActionHistory.VOID_CHECK`.

## User-facing surfaces
- **Surface type**: Dialog (modal)
- **UI entry points**:
  - From switchboard via `VoidTicketAction` (not yet traced)
  - From open tickets list via `OpenTicketsListDialog.doVoidTicket()`
- **Controls observed**:
  - Void reason dropdown + add-new void reason (“…”) via `NotesDialog`
  - Checkbox: Items wasted
  - Ticket detail view
  - Buttons: Void, Cancel

## Preconditions & protections
- **Void reasons**: Loaded from `VoidReasonDAO.findAll()`; failure shows `CANNOT_LOAD_VOID_REASONS`.
- **Refund amount prompt**: Only when `ticket.getPaidAmount() > 0`.

## Step-by-step behavior (forensic)
1. If `ticket.paidAmount > 0`:
   - Computes tips amount and ticket-total-without-tips.
   - Prompts for refund amount with default:
     - If `paidAmount < ticketTotalWithoutTips`: default = `paidAmount`.
     - Else default = `paidAmount - tipsAmount`.
   - If refund amount == -1 returns (cancels).
   - If tips > 0, asks “Do you want to refund tips?”; if YES sets `ticket.gratuity.refunded=true` and adds gratuity amount to refund.
   - Validates refundAmount <= paidAmount.
   - Creates/updates a `RefundTransaction` (cash, debit) and adjusts terminal current balance.
2. Else (unpaid): if gratuity exists, sets gratuity amount to 0.
3. Applies void metadata:
   - `ticket.voidReason` from selected `VoidReason.reasonText`.
   - `ticket.wasted = chkItemsWasted.isSelected()`.
   - `ticket.voidedBy = Application.getCurrentUser()`.
4. Delete-vs-void decision:
   - If `ticket.paidAmount == 0` AND `!printedToKitchen()` → `TicketDAO.deleteTickets([ticket])`.
   - Else → `TicketDAO.voidTicket(ticket)`.
5. Printing:
   - If refundTransaction exists and refundAmount > 0: `ReceiptPrintService.printRefundTicket(ticket, refundTransaction)`.
   - Always prints a generic report “Ticket <id> was voided”.
6. Audit:
   - `ActionHistoryDAO.saveHistory(currentUser, ActionHistory.VOID_CHECK, "TicketNo:<id>; Total: <total>")`.
7. Closes dialog with `canceled=false`.

## Edge cases & failure paths
- Refund amount > paid amount: shows message and aborts.
- Printing failure: shows error `VoidTicketDialog.9` + exception message.

## Data / audit / financial impact
- **Writes/updates**:
  - Ticket void/delete state, void reason, wasted flag, voidedBy, refunded flag.
  - Refund transaction creation/update and terminal balance adjustment.
  - ActionHistory record.
- **Financial risk**: Very high. Incorrect refund math or terminal balance adjustment will break reconciliation.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.dialog.VoidTicketDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/dialog/VoidTicketDialog.java`
- **DAOs**: `VoidReasonDAO`, `TicketDAO`, `ActionHistoryDAO`
- **Printing**: `ReceiptPrintService.printRefundTicket`, `.printGenericReport`

## Uncertainties (STOP; do not guess)
- **What `TicketDAO.voidTicket` does internally**: Likely writes transactions and marks ticket status; must be inspected for full behavioral reconstruction.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Void reason management
  - Printed-to-kitchen detection
  - Refund transaction persistence and terminal cash-balance adjustments
  - Action history/audit trail
- **UI requirements**: Void dialog with refund prompt, tips refund prompt, reason selection + add-new, items wasted.
- **Constraints for implementers**:
  - Preserve delete-vs-void logic gate (unpaid + not printed-to-kitchen → delete).
  - Preserve refund default calculation and tips refund prompt.
