# Feature: Refund Action (F-0073)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (Ticket refund concept exists, action workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Customers may need refunds for paid orders (wrong order, quality issues, returns). Managers need to process refunds with proper controls and audit trail.
- **Evidence**: `RefundAction.java` - validates ticket is paid and not already refunded; prompts for refund amount; cannot exceed paid amount; handles gratuity refund; logs action.

## User-facing surfaces
- **Surface type**: Action + confirmation dialogs
- **UI entry points**: Switchboard → Refund button (permission-gated); Manager functions
- **Exit paths**: Complete refund / Cancel

## Preconditions & protections
- **User/role/permission checks**: Requires REFUND permission (UserPermission.REFUND)
- **State checks**: 
  - Ticket must be selected OR entered by ID
  - Ticket must be PAID (isPaid() == true)
  - Ticket must NOT be already refunded (isRefunded() == false)
- **Manager override**: Permission-based; action itself requires elevated access

## Step-by-step behavior (forensic)
1. User clicks Refund button with ticket selected (or enters ticket ID if none selected)
2. System validates:
   - Ticket is paid → Error: "Ticket is not paid"
   - Ticket not refunded → Error: "Ticket already refunded"
3. Full ticket loaded from DB (loadFullTicket)
4. Dialog shows:
   - Ticket ID
   - Paid amount
   - Gratuity amount (if exists)
5. User enters refund amount (defaults to paid amount)
6. Validation: refund cannot exceed paid amount
7. On confirm:
   - PosTransactionService.refundTicket(ticket, refundAmount)
   - Success message shown
   - Ticket list updated

## Edge cases & failure paths
- **Ticket not found**: Error dialog
- **Not paid**: Error "Ticket is not paid. Only paid ticket can be refunded"
- **Already refunded**: Error "This ticket has already been refunded"
- **Amount exceeds paid**: Error "Refund amount cannot be more than paid amount"
- **Partial refund**: Allowed (enter less than paid amount)
- **Gratuity**: Shown separately; may need separate refund handling

## Data / audit / financial impact
- **Writes/updates**: 
  - Ticket.refunded = true
  - PosTransaction created (refund type)
  - Terminal balance affected
- **Audit events**: ActionHistory logged; transaction recorded
- **Financial risk**: Unauthorized refunds; refund fraud; incorrect amounts

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `RefundAction` → `actions/RefundAction.java`
- **Entry action(s)**: Button in SwitchboardView, tied to ITicketList
- **Workflow/service enforcement**: 
  - `PosTransactionService.refundTicket()`
  - `TicketService.getTicket()`
  - `NumberSelectionDialog2` for amount input
- **Messages/labels**: `RefundAction.0` through `RefundAction.12`

## Uncertainties (STOP; do not guess)
- Gratuity refund workflow (separate from ticket refund?)
- Card transaction refund vs cash refund handling

## MagiDesk parity notes
- **What exists today**: Ticket.refunded state; basic refund concept
- **What differs / missing**: Full RefundAction workflow; permission gating; amount selection dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - RefundTicketCommand with ticketId, amount, reason
  - Validation: paid && !refunded
  - Create refund transaction
  - Update ticket state
- **API/DTO requirements**: POST /tickets/{id}/refund
- **UI requirements**: Refund button with permission; amount entry dialog; confirmation
- **Constraints for implementers**: Cannot refund more than paid; audit trail mandatory; affects drawer balance
