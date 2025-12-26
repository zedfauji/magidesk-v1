# Feature: Misc Ticket Item Dialog (F-0029)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Sometimes items not on menu need to be charged (special requests, catering items, one-off sales). Staff need to add misc items with custom description and price.
- **Evidence**: `MiscTicketItemDialog.java` - enter item name, quantity, price; adds to ticket without menu item reference.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: TicketView → Misc button; keyboard shortcut
- **Exit paths**: Add (adds to ticket) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Misc item permission (may require manager)
- **State checks**: Ticket must be open
- **Manager override**: Often required

## Step-by-step behavior (forensic)
1. User clicks Misc Item button
2. MiscTicketItemDialog opens
3. User enters:
   - Item name/description
   - Quantity
   - Unit price
   - Tax selection
4. On Add:
   - TicketItem created with misc flag
   - No menu item reference
   - Custom price applied
   - Tax calculated
5. Item appears on ticket

## Edge cases & failure paths
- **Zero price**: Allowed (promotional)
- **Negative price**: May be allowed (adjustment)
- **Empty name**: Validation error
- **High price**: May trigger approval

## Data / audit / financial impact
- **Writes/updates**: TicketItem with beverageNo = true or misc flag
- **Audit events**: Misc item logged (potential abuse indicator)
- **Financial risk**: Revenue leakage; unauthorized items

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `MiscTicketItemDialog` → `ui/dialog/MiscTicketItemDialog.java`
- **Entry action(s)**: Button in TicketView
- **Workflow/service enforcement**: Ticket update
- **Messages/labels**: Field labels

## MagiDesk parity notes
- **What exists today**: OrderLine supports custom items
- **What differs / missing**: MiscTicketItemDialog UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: MiscItem flag on OrderLine; custom price support
- **API/DTO requirements**: OrderLine without menuItemId
- **UI requirements**: MiscTicketItemDialog with name, qty, price, tax
- **Constraints for implementers**: Should be auditable; may require manager auth
