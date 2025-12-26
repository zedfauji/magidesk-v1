# Feature: Delete Ticket Item Action (F-0028)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (item removal exists but workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Remove items from tickets before sending to kitchen or settling. Mistakes happen - wrong item, customer changes mind.
- **Evidence**: `DeleteTicketItemAction.java` - removes selected item from ticket; validates state; logs removal.

## User-facing surfaces
- **Surface type**: Action (button/key)
- **UI entry points**: TicketView → Delete/Remove button; context menu
- **Exit paths**: Immediate action (confirmation may be required)

## Preconditions & protections
- **User/role/permission checks**: Depends on item state:
  - Unsent items: Order entry permission
  - Sent items: Void permission (manager may be required)
- **State checks**: Item must be selected; ticket open
- **Manager override**: Required for sent items

## Step-by-step behavior (forensic)
1. User selects item in ticket view
2. User clicks Delete/Remove
3. System checks item state:
   - **Unsent**: Direct removal
   - **Sent to kitchen**: Requires void permission, logs void
4. Confirmation prompt (for sent items)
5. Item removed from ticket
6. Ticket totals recalculated
7. If voided: Kitchen notified (void ticket printed)

## Edge cases & failure paths
- **No item selected**: Error/disabled button
- **Last item on ticket**: May allow or switch to void ticket
- **Item partially paid**: Complex - may prevent or require adjustment
- **Modifier-only deletion**: May require parent item deletion

## Data / audit / financial impact
- **Writes/updates**: TicketItem removed; Ticket totals updated
- **Audit events**: Deletion/void logged with reason
- **Financial risk**: Unauthorized voids; inventory discrepancy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `DeleteTicketItemAction` → `actions/DeleteTicketItemAction.java`
- **Workflow/service enforcement**: Ticket.removeTicketItem()
- **Messages/labels**: Confirmation prompts

## MagiDesk parity notes
- **What exists today**: OrderLine removal exists
- **What differs / missing**: Void workflow for sent items; kitchen notification

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - RemoveOrderLineCommand (unsent)
  - VoidOrderLineCommand (sent - requires manager)
- **API/DTO requirements**: DELETE /tickets/{id}/items/{itemId}
- **UI requirements**: Delete button; confirmation for sent
- **Constraints for implementers**: Void requires audit; sent items need reason
