# Feature: Order Type Explorer (F-0121)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (order types exist but explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Configure order types (dine-in, takeout, delivery) with different properties - tax rules, pricing, required fields, workflow.
- **Evidence**: `OrderTypeExplorer.java` + `OrdersTypeExplorerAction.java` - CRUD for order types with extensive properties.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Order Types
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Configuration permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Order Type Explorer
2. View shows order types table:
   - Name
   - Enabled
   - Bar Tab flag
   - Delivery flag
3. New/Edit/Delete actions
4. Order type form includes:
   - Name
   - Enabled toggle
   - Is bar tab
   - Is delivery
   - Requires table
   - Requires customer
   - Allow tips later
   - Tax rate override
   - Service charge
   - Price differential
   - Button color
5. Save persists order type
6. Appears as option in POS

## Edge cases & failure paths
- **Delete system type**: May be prevented
- **Disable all types**: At least one required
- **Conflicting properties**: Validation

## Data / audit / financial impact
- **Writes/updates**: OrderType entity
- **Audit events**: Config changes logged
- **Financial risk**: Tax/pricing rules affect revenue

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `OrderTypeExplorer` → `bo/ui/explorer/OrderTypeExplorer.java`
- **Entry action(s)**: `OrdersTypeExplorerAction` → `bo/actions/OrdersTypeExplorerAction.java`
- **Workflow/service enforcement**: OrderTypeDAO
- **Messages/labels**: Property labels

## MagiDesk parity notes
- **What exists today**: Basic order type entity
- **What differs / missing**: Full OrderTypeExplorer with all properties

## Porting strategy (PLAN ONLY)
- **Backend requirements**: OrderType entity with all properties
- **API/DTO requirements**: GET/POST/PUT/DELETE /order-types
- **UI requirements**: OrderTypeExplorer with comprehensive form
- **Constraints for implementers**: Properties affect entire order workflow
