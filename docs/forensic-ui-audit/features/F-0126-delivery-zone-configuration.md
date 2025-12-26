# Feature: Delivery Zone Configuration (F-0126)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Define delivery areas with zone-based fees and delivery time estimates.
- **Evidence**: `DeliveryZone` entity + `ZipCode` mapping - zone-based delivery configuration.

## User-facing surfaces
- **Surface type**: Configuration view (in Back Office)
- **UI entry points**: BackOfficeWindow → Configuration → Delivery Zones
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Delivery configuration permission
- **State checks**: Delivery order type enabled
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Delivery Zone Configuration
2. View shows zones:
   - Zone name
   - Delivery fee
   - Min order amount
   - Estimated time
   - Zip codes/areas
3. Create/Edit/Delete zones
4. Zone form:
   - Name
   - Delivery fee
   - Minimum order
   - Estimated delivery time
   - Zip code list
5. Save persists zone

## Edge cases & failure paths
- **Overlapping zip codes**: Warning
- **Customer address not in zone**: Show options

## Data / audit / financial impact
- **Writes/updates**: DeliveryZone entity; ZipCode mappings
- **Audit events**: Config changes
- **Financial risk**: Fee accuracy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Zone configuration views
- **Entry action(s)**: Configuration menu
- **Workflow/service enforcement**: Zone lookup by address
- **Messages/labels**: Zone labels

## MagiDesk parity notes
- **What exists today**: No delivery zones
- **What differs / missing**: Entire zone system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: DeliveryZone entity with fee, time, zip mapping
- **API/DTO requirements**: GET/POST/PUT/DELETE /delivery-zones
- **UI requirements**: Zone configuration view
- **Constraints for implementers**: Address to zone lookup
