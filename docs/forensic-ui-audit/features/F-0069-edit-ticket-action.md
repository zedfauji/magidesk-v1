# Feature: Edit Ticket Action (F-0069)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (ticket editing implemented)

## Problem / Why this exists (grounded)
- **Operational need**: Modify existing ticket - add/remove items, change modifiers, update notes.
- **Evidence**: Ticket edit capability in OrderView - standard order modification.

## User-facing surfaces
- **Surface type**: View transition
- **UI entry points**: OpenTicketsDialog → Select → Edit; Table tap on open ticket
- **Exit paths**: Hold (save) / Pay / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order edit permission; own ticket or override
- **State checks**: Ticket open/not settled
- **Manager override**: Required for other's tickets; sent items

## Step-by-step behavior (forensic)
1. User selects existing open ticket
2. Ticket opens in OrderView
3. User can:
   - Add new items
   - Modify existing items (if unsent or with permission)
   - Delete items (if unsent or with permission)
   - Apply discounts
   - Add notes
4. Changes saved on Hold or Pay

## Edge cases & failure paths
- **Concurrent edit**: Lock or merge
- **Ticket paid while editing**: Error, refresh
- **Sent item modification**: Void/reorder flow

## Data / audit / financial impact
- **Writes/updates**: Ticket and items modified
- **Audit events**: Changes logged
- **Financial risk**: Unauthorized modifications

## Code traceability (REQUIRED)
- **Primary UI class(es)**: OrderView + ticket loading
- **Entry action(s)**: Open ticket selection
- **Workflow/service enforcement**: Ticket access control
- **Messages/labels**: None specific

## MagiDesk parity notes
- **What exists today**: Ticket editing
- **What differs / missing**: Sent item handling

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Ticket concurrency control
- **API/DTO requirements**: GET /tickets/{id}; PUT /tickets/{id}
- **UI requirements**: OrderView with existing ticket
- **Constraints for implementers**: Lock or optimistic concurrency
