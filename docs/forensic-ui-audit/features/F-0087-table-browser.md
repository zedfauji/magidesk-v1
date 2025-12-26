# Feature: Table Browser (F-0087)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (TableManagementPage)

## Problem / Why this exists (grounded)
- **Operational need**: Configure restaurant tables - table numbers, capacity, floor section assignment, server sections.
- **Evidence**: `ShopTableBrowser.java` + `ShopTableForm.java` - CRUD for tables; set number, capacity, position, floor.

## User-facing surfaces
- **Surface type**: Browser panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Floor Plan menu → Tables; ShowTableBrowserAction
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Table management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Table Browser from Back Office
2. View shows table list:
   - Table number
   - Capacity
   - Floor/Section
   - Status
3. New/Edit/Delete actions
4. Table form includes:
   - Table number
   - Capacity (seats)
   - Floor assignment
   - Section assignment
   - X/Y position for floor map
   - Button appearance
5. Save persists table
6. Tables appear on POS table map

## Edge cases & failure paths
- **Duplicate table number**: Validation error
- **Table with open order**: Prevent delete
- **No floor assigned**: Uses default

## Data / audit / financial impact
- **Writes/updates**: ShopTable entity
- **Audit events**: Table config changes logged
- **Financial risk**: None (operational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `ShopTableBrowser` → `table/ShopTableBrowser.java`
  - `ShopTableForm` → `table/ShopTableForm.java`
- **Entry action(s)**: `ShowTableBrowserAction` → `table/ShowTableBrowserAction.java`
- **Workflow/service enforcement**: ShopTableDAO
- **Messages/labels**: Table labels

## MagiDesk parity notes
- **What exists today**: TableManagementPage with table CRUD
- **What differs / missing**: May need floor/section support; position for visual layout

## Porting strategy (PLAN ONLY)
- **Backend requirements**: ShopTable entity
- **API/DTO requirements**: GET/POST/PUT/DELETE /tables
- **UI requirements**: TableBrowser and TableForm
- **Constraints for implementers**: Table numbers unique; capacity affects UI
