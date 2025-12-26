# Feature: Pickup Order View (F-0084)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Pickup orders need customer name, phone, ready time. Displayed separately for pickup queue management.
- **Evidence**: `PickupOrderView.java` - pickup-specific order flow; ready notifications.

## User-facing surfaces
- **Surface type**: View/Screen
- **UI entry points**: Login → Pickup/To-Go order type; phone order entry
- **Exit paths**: Order created / Cancel

## Preconditions & protections
- **User/role/permission checks**: Pickup order permission
- **State checks**: Pickup order type enabled
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User selects Pickup order type
2. Customer identification:
   - Name (required)
   - Phone number
   - Email (for notifications)
3. Order entry as normal
4. Pickup-specific:
   - Promised pickup time
   - Ready notifications
   - Order status display
5. Order appears in pickup queue
6. Mark as ready (notifies customer)
7. Mark as picked up (closes)

## Edge cases & failure paths
- **Customer no-show**: Order on hold, may void after time
- **Early/late**: Time adjustment
- **Customer changes mind**: Cancel or modify

## Data / audit / financial impact
- **Writes/updates**: Ticket with pickup info; notifications
- **Audit events**: Ready, picked up events
- **Financial risk**: Low - standard order

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `PickupOrderView` → (path to be confirmed)
- **Entry action(s)**: Order type selection
- **Workflow/service enforcement**: Pickup queue management
- **Messages/labels**: Pickup labels

## MagiDesk parity notes
- **What exists today**: No pickup-specific workflow
- **What differs / missing**: Pickup queue; ready notifications

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Pickup order type
  - Ready time field
  - Notification service
- **API/DTO requirements**: Mark as ready endpoint
- **UI requirements**: PickupOrderView; pickup queue display
- **Constraints for implementers**: Time tracking; customer communication
