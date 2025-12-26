# Feature: User Transfer Dialog (F-0074)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: When servers go on break or off shift, their open tickets need to be transferred to another server. Tips and responsibility must transfer with the ticket.
- **Evidence**: `UserTransferDialog.java` - shows list of active users; allows selection; transfers ticket ownership. Called from OpenTicketsListDialog.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: OpenTicketsListDialog → Transfer Server button; Manager functions
- **Exit paths**: Select user (transfers) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Requires transfer permission; manager may be required
- **State checks**: Ticket must be open (not closed/paid)
- **Manager override**: May be required for cross-section transfers

## Step-by-step behavior (forensic)
1. Manager/server selects ticket in open tickets list
2. Clicks Transfer Server button
3. UserTransferDialog opens with list of active users
4. User selects target server
5. On confirm:
   - Ticket.owner updated to new user
   - Gratuity.owner updated if exists
   - Ticket saved
   - List refreshed

## Edge cases & failure paths
- **Same user selected**: No-op or error
- **User not clocked in**: May not appear in list
- **User has no table section privilege**: May be prevented
- **Ticket partially paid**: Transfer still allowed

## Data / audit / financial impact
- **Writes/updates**: Ticket.owner; Gratuity.owner
- **Audit events**: Transfer logged in ActionHistory
- **Financial risk**: Tips attributed to wrong server; accountability issues

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `UserTransferDialog` → `ui/views/UserTransferDialog.java`
- **Entry action(s)**: Button in OpenTicketsListDialog
- **Workflow/service enforcement**: TicketDAO.saveTicket()
- **Messages/labels**: User list labels

## Uncertainties (STOP; do not guess)
- Partial transfer (some items only) - likely not supported
- Tip pool implications

## MagiDesk parity notes
- **What exists today**: Ticket has owner association
- **What differs / missing**: Transfer dialog; user selection; audit logging

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - TransferTicketCommand with ticketId, newOwnerId
  - Validate ticket state and user permissions
  - Update owner on ticket and related entities
- **API/DTO requirements**: POST /tickets/{id}/transfer
- **UI requirements**: UserTransferDialog with user list
- **Constraints for implementers**: Must audit transfer; tips must follow ticket
