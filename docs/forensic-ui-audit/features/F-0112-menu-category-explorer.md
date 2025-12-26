# Feature: Menu Category Explorer (F-0112)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (categories exist but explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Menu items are organized into categories (Appetizers, Entrees, Desserts, Beverages). Categories appear on POS for navigation.
- **Evidence**: `MenuCategoryExplorer.java` - CRUD for categories; name, visibility, sort order, beverage flag.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Categories; NewMenuCategoryAction
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Menu management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Menu Category Explorer
2. View shows categories table: Name, Sort Order, Visible, Beverage
3. New/Edit/Delete actions
4. Category form includes:
   - Name
   - Sort order (display position)
   - Visible toggle
   - Beverage category flag
   - Button color/image
5. Save persists category
6. Categories appear in POS order entry

## Edge cases & failure paths
- **Delete with items**: Prevent or reassign items
- **Duplicate name**: May allow or warn
- **Sort order conflict**: Stable sort by secondary key

## Data / audit / financial impact
- **Writes/updates**: MenuCategory entity
- **Audit events**: Category changes logged
- **Financial risk**: Low - organizational

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `MenuCategoryExplorer` → `bo/ui/explorer/MenuCategoryExplorer.java`
- **Entry action(s)**: `CategoryExplorerAction` → `bo/actions/CategoryExplorerAction.java`
- **Workflow/service enforcement**: MenuCategoryDAO
- **Messages/labels**: Category labels

## MagiDesk parity notes
- **What exists today**: Menu category entity
- **What differs / missing**: CategoryExplorer UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: MenuCategory entity
- **API/DTO requirements**: GET/POST/PUT/DELETE /menu-categories
- **UI requirements**: CategoryExplorer with CRUD
- **Constraints for implementers**: Categories affect POS navigation
