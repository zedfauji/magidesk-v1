# Feature: Home Delivery View (F-0083)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Delivery orders need customer address, delivery zone, driver assignment, delivery status tracking.
- **Evidence**: `HomeDeliveryView.java` + delivery-related classes - delivery order management; driver dispatch.

## User-facing surfaces
- **Surface type**: View/Screen
- **UI entry points**: Login → Home Delivery order type; SwitchboardView
- **Exit paths**: Order created / Cancel

## Preconditions & protections
- **User/role/permission checks**: Delivery order permission
- **State checks**: Delivery order type enabled
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User selects Home Delivery order type
2. Customer selection/entry (address required):
   - CustomerSelectorDialog or inline
   - Address validation/lookup
   - Delivery zone determination
3. Order entry as normal
4. Delivery-specific fields:
   - Estimated delivery time
   - Driver assignment
   - Delivery instructions
5. Order saved with delivery status
6. Appears in delivery dispatch queue
7. Driver can mark out-for-delivery, delivered

## Edge cases & failure paths
- **Customer address out of zone**: Warning or prevent
- **No drivers available**: Queue for later assignment
- **Driver cancels**: Reassignment required

## Data / audit / financial impact
- **Writes/updates**: Ticket with delivery info; DriverAssignment
- **Audit events**: Dispatch, delivery events logged
- **Financial risk**: Delivery fee calculation; driver tips

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `HomeDeliveryView` → (path to be confirmed)
  - `DriverSelectionDialog` → ui/dialog/DriverSelectionDialog.java
- **Entry action(s)**: Order type selection
- **Workflow/service enforcement**: Delivery tracking
- **Messages/labels**: Delivery labels

## MagiDesk parity notes
- **What exists today**: No delivery workflow
- **What differs / missing**: Entire delivery order management

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Delivery order type properties
  - Driver entity
  - DeliveryAssignment tracking
- **API/DTO requirements**: Delivery-specific endpoints
- **UI requirements**: HomeDeliveryView; driver dispatch
- **Constraints for implementers**: Address validation; zone-based delivery fees
