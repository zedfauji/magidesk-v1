# Feature: Change Table Action (F-0080)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (table assignment exists but change workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Move party to different table - request, availability, seating optimization. Keep order with table change.
- **Evidence**: `ChangeTableAction.java` + related - reassigns ticket to different table.

## User-facing surfaces
- **Surface type**: Action + table selection
- **UI entry points**: TicketView → Move Table button
- **Exit paths**: Table changed / Cancel

## Preconditions & protections
- **User/role/permission checks**: Table management permission
- **State checks**: Ticket assigned to table; target table available
- **Manager override**: Not typically required

## Step-by-step behavior (forensic)
1. User with dine-in ticket
2. Clicks Move Table
3. Table selection dialog shows available tables
4. User selects new table
5. On confirm:
   - Ticket.tableNumbers updated
   - Original table freed
   - New table marked occupied
   - Server maintains ticket (or transfers)
6. Kitchen may be notified

## Edge cases & failure paths
- **Target table occupied**: Prevented or offer to merge
- **Different capacity**: Warning
- **Party splitting**: May need to split ticket first

## Data / audit / financial impact
- **Writes/updates**: Ticket.tableNumbers; table status
- **Audit events**: Table change logged
- **Financial risk**: Low - operational

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action + TableSelector
- **Entry action(s)**: `ChangeTableAction` → `actions/ChangeTableAction.java`
- **Workflow/service enforcement**: Ticket update; table status
- **Messages/labels**: Table selection

## MagiDesk parity notes
- **What exists today**: Table assignment exists
- **What differs / missing**: Formal change table action

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - ChangeTableCommand
  - Table availability check
- **API/DTO requirements**: PUT /tickets/{id}/table
- **UI requirements**: Table selection dialog
- **Constraints for implementers**: Update table status accordingly
