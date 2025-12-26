# Feature: Merge Tickets Action (F-0075)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Combine two tickets into one - parties joining together, server consolidation, order correction.
- **Evidence**: `MergeTicketAction.java` - combines items from source ticket into target ticket.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: TicketView → Merge button; OpenTicketsListDialog
- **Exit paths**: Merge complete / Cancel

## Preconditions & protections
- **User/role/permission checks**: Merge ticket permission
- **State checks**: Both tickets open; not partially paid
- **Manager override**: May be required

## Step-by-step behavior (forensic)
1. User has ticket A open
2. Clicks Merge
3. Dialog shows other open tickets (B)
4. User selects ticket B to merge into A
5. On confirm:
   - All items from B moved to A
   - Discounts/notes combined
   - Ticket B voided/deleted
   - Ticket A recalculated
6. Single ticket with all items

## Edge cases & failure paths
- **Different order types**: Warning/prevent
- **Different customers**: Choose which to keep
- **Partial payment on one**: Prevents merge or handles complex
- **Same table**: Allowed

## Data / audit / financial impact
- **Writes/updates**: 
  - Ticket A updated with items
  - Ticket B deleted/voided
- **Audit events**: Merge logged
- **Financial risk**: Low if not paid; complex if partial

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action + selection dialog
- **Entry action(s)**: `MergeTicketAction` → `actions/MergeTicketAction.java`
- **Workflow/service enforcement**: Ticket item transfer
- **Messages/labels**: Merge prompts

## MagiDesk parity notes
- **What exists today**: No merge functionality
- **What differs / missing**: Entire merge workflow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - MergeTicketsCommand
  - Transfer items between tickets
  - Recalculate totals
- **API/DTO requirements**: POST /tickets/{id}/merge-from/{sourceId}
- **UI requirements**: Merge dialog with ticket selection
- **Constraints for implementers**: Complex with discounts, tips, partial payments
