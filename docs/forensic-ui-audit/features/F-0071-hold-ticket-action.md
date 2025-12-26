# Feature: Hold Ticket Action (F-0071)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: PARTIAL (ticket persistence exists but explicit hold may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Save ticket without closing/paying to work on another order. Essential for multi-tasking servers handling multiple tables.
- **Evidence**: `HoldTicketAction.java` - saves current ticket state, returns to switchboard/table map.

## User-facing surfaces
- **Surface type**: Action (button)
- **UI entry points**: OrderView → Hold button; key shortcut
- **Exit paths**: Returns to switchboard

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket must have at least one item (or allow empty hold)
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in order entry with items added
2. User clicks Hold
3. Ticket saved with current state:
   - All items
   - Customer if assigned
   - Table if assigned
   - Notes
4. Ticket appears in "Open Tickets" list
5. User returned to switchboard or table map
6. Ticket can be resumed later

## Edge cases & failure paths
- **Empty ticket**: May prevent hold or allow
- **New items not sent**: Saved but not sent to kitchen (send separate action)
- **Hold then timeout**: Ticket remains open

## Data / audit / financial impact
- **Writes/updates**: Ticket persisted to database
- **Audit events**: Not typically logged (routine operation)
- **Financial risk**: Low - just saves for later

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `HoldTicketAction` → `actions/HoldTicketAction.java`
- **Workflow/service enforcement**: TicketDAO.saveOrUpdate()
- **Messages/labels**: Button label

## MagiDesk parity notes
- **What exists today**: Tickets persist automatically
- **What differs / missing**: Explicit Hold action; return to switchboard

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Ticket persistence (already exists)
- **API/DTO requirements**: POST/PUT /tickets (save draft)
- **UI requirements**: Hold button; navigation back
- **Constraints for implementers**: Must preserve all ticket state
