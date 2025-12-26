# Feature: Floor Explorer (F-0081)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Configure restaurant floors/sections (Main Dining, Patio, Bar Area). Each floor has its own table layout.
- **Evidence**: `ShopFloorExplorer.java` - CRUD for floor plans; visual layout editing.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Floor Plan → Floors
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Floor plan management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Floor Explorer
2. View shows floors list:
   - Floor name
   - Active status
   - Table count
   - Image/background
3. New/Edit/Delete actions
4. Floor form includes:
   - Name
   - Active toggle
   - Background image upload
   - Dimensions (for layout)
5. Save persists floor
6. Floor available in table assignment

## Edge cases & failure paths
- **Delete with tables**: Must reassign tables first
- **No floors**: At least one default required

## Data / audit / financial impact
- **Writes/updates**: ShopFloor entity
- **Audit events**: Floor config changes logged
- **Financial risk**: None (operational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ShopFloorExplorer` → `bo/ui/explorer/ShopFloorExplorer.java`
- **Entry action(s)**: `ShowShopFloorExplorerAction` → related action
- **Workflow/service enforcement**: ShopFloorDAO
- **Messages/labels**: Floor labels

## MagiDesk parity notes
- **What exists today**: No floor concept
- **What differs / missing**: Entire floor system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Floor entity with image support
- **API/DTO requirements**: GET/POST/PUT/DELETE /floors
- **UI requirements**: FloorExplorer with image upload
- **Constraints for implementers**: Tables reference floors
