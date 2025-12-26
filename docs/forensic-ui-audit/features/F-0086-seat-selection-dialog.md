# Feature: Seat Selection Dialog (F-0086)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Fine dining tracks items by seat number for proper delivery. Split by seat allows separate checks per diner.
- **Evidence**: `SeatSelectionDialog.java` - select seat number when adding items; enables seat-based ordering and split.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: OrderView when seat tracking enabled; SplitTicketDialog
- **Exit paths**: Select seat / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Table must have seat count configured; ticket open
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds item with seat tracking enabled
2. SeatSelectionDialog opens (or seat selector in UI)
3. Shows seat numbers (1 to table capacity)
4. User selects seat number
5. Item tagged with seat number
6. On split by seat:
   - SplitTicketDialog uses seat numbers
   - Creates separate tickets per seat
7. Kitchen ticket shows seat numbers

## Edge cases & failure paths
- **No seat selected**: May default to 1 or prompt
- **Seat already has items**: Allowed (add more)
- **Change seat**: Edit item seat assignment

## Data / audit / financial impact
- **Writes/updates**: TicketItem.seatNumber
- **Audit events**: Part of order
- **Financial risk**: Low (operational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `SeatSelectionDialog` → `ui/dialog/SeatSelectionDialog.java`
- **Entry action(s)**: Called from OrderView
- **Workflow/service enforcement**: Ticket, TicketItem
- **Messages/labels**: Seat number labels

## MagiDesk parity notes
- **What exists today**: No seat tracking
- **What differs / missing**: Seat selection dialog; seat-based split

## Porting strategy (PLAN ONLY)
- **Backend requirements**: SeatNumber field on OrderLine
- **API/DTO requirements**: Seat included in order item
- **UI requirements**: SeatSelectionDialog; seat indicator on items
- **Constraints for implementers**: Seat tracking optional per table/order type
