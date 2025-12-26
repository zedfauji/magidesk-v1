# Feature: Menu Group Explorer (F-0113)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (groups may exist but explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Menu items within categories are organized into groups (e.g., Appetizers category → Hot Appetizers, Cold Appetizers groups). Groups provide additional navigation level.
- **Evidence**: `MenuGroupExplorer.java` - CRUD for menu groups; name, category assignment, visibility.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Groups; NewMenuGroupAction
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Menu management permission
- **State checks**: Category must exist first
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Menu Group Explorer
2. View shows groups table:
   - Name
   - Category
   - Visible
   - Sort Order
3. New/Edit/Delete actions
4. Group form includes:
   - Name
   - Parent category
   - Sort order
   - Visible toggle
   - Button color/image
5. Save persists group
6. Groups appear in POS under category

## Edge cases & failure paths
- **Delete with items**: Prevent or reassign
- **No category**: Must select parent category
- **Duplicate name in category**: May warn

## Data / audit / financial impact
- **Writes/updates**: MenuGroup entity
- **Audit events**: Menu changes logged
- **Financial risk**: Low - organizational

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `MenuGroupExplorer` → `bo/ui/explorer/MenuGroupExplorer.java`
- **Entry action(s)**: `GroupExplorerAction` → `bo/actions/GroupExplorerAction.java`
- **Workflow/service enforcement**: MenuGroupDAO
- **Messages/labels**: Group labels

## MagiDesk parity notes
- **What exists today**: Menu category/group structure
- **What differs / missing**: GroupExplorer UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: MenuGroup entity with category FK
- **API/DTO requirements**: GET/POST/PUT/DELETE /menu-groups
- **UI requirements**: MenuGroupExplorer
- **Constraints for implementers**: Groups belong to categories
