# Feature: Group Settle Ticket Dialog (F-0046)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: In table service, parties may want to pay together (consolidated) or one person pays for multiple tickets. Groups/parties need combined settlement.
- **Evidence**: `GroupSettleTicketDialog.java` + `GroupPaymentView.java` + `GroupSettleTicketAction.java` - select multiple tickets, view combined total, process single payment.

## User-facing surfaces
- **Surface type**: Dialog
- **UI entry points**: Switchboard → Group Settle button (permission-gated)
- **Exit paths**: Settle (processes payment) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Group settle permission
- **State checks**: Multiple open tickets available; tickets not already paid
- **Manager override**: May be required for cross-server tickets

## Step-by-step behavior (forensic)
1. User clicks Group Settle button
2. GroupSettleTicketSelectionWindow opens
3. User selects multiple tickets to consolidate:
   - Shows open tickets with totals
   - Checkboxes for selection
   - Running combined total
4. On confirm selection:
   - GroupSettleTicketDialog opens
   - Shows combined ticket details
   - GroupPaymentView for payment
5. Payment processed against combined total
6. All selected tickets marked as paid

## Edge cases & failure paths
- **Single ticket**: Falls back to regular settle
- **Tickets from different servers**: May require permission
- **Partial payment of group**: Complex - split handling
- **Some tickets already partial**: Combined with remaining balance

## Data / audit / financial impact
- **Writes/updates**: Multiple Ticket.paid states; shared Payment transaction
- **Audit events**: Group settlement logged
- **Financial risk**: Tip allocation across servers; payment split errors

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `GroupSettleTicketDialog` → `ui/views/payment/GroupSettleTicketDialog.java`
  - `GroupPaymentView` → `ui/views/payment/GroupPaymentView.java`
  - `GroupSettleTicketSelectionWindow` → `ui/views/GroupSettleTicketSelectionWindow.java`
- **Entry action(s)**: `GroupSettleTicketAction` → `actions/GroupSettleTicketAction.java`
- **Workflow/service enforcement**: TicketService for bulk operations
- **Messages/labels**: Selection labels, totals

## Uncertainties (STOP; do not guess)
- Tip split logic when multiple servers
- Partial group payment handling

## MagiDesk parity notes
- **What exists today**: Single ticket settlement only
- **What differs / missing**: Entire group settle workflow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - GroupSettleCommand with ticketIds, payment
  - Validation all tickets open
  - Atomic update of all tickets
- **API/DTO requirements**: POST /payments/group-settle
- **UI requirements**: Ticket multi-select; combined payment view
- **Constraints for implementers**: Must be atomic; tip allocation rules needed
