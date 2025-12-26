# Feature: Menu Item Explorer (F-0114)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (Menu management exists but may lack full features)

## Problem / Why this exists (grounded)
- **Operational need**: Restaurant managers need to manage menu items - add new items, edit prices, set availability, configure modifiers, assign to categories/groups.
- **Evidence**: `MenuItemExplorer.java` - full CRUD for menu items; table view with all items; new/edit/delete buttons; search/filter capabilities.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office tabbed pane)
- **UI entry points**: BackOfficeWindow → Explorers menu → Menu Items; NewMenuItemAction
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Menu management permission
- **State checks**: Category and group must exist before item creation
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User opens Menu Item Explorer from Back Office
2. Explorer shows table of all menu items with columns:
   - Name
   - Category
   - Group
   - Price
   - Active status
3. User can:
   - Search/filter items
   - Click New to add item
   - Double-click to edit
   - Delete selected item
4. Edit opens MenuItemForm with:
   - Basic info (name, description, barcode)
   - Pricing (base price, order type prices)
   - Category/group assignment
   - Modifier groups
   - Printer group assignment
   - Stock tracking settings
   - Image upload
5. Save persists changes

## Edge cases & failure paths
- **Duplicate barcode**: Validation error
- **Item in use (on tickets)**: May restrict deletion
- **No category/group**: Must create first
- **Price validation**: Must be positive

## Data / audit / financial impact
- **Writes/updates**: MenuItem entity; price relationships
- **Audit events**: Menu changes logged
- **Financial risk**: Incorrect prices affect revenue

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `MenuItemExplorer` → `bo/ui/explorer/MenuItemExplorer.java`
- **Entry action(s)**: `ItemExplorerAction` → `bo/actions/ItemExplorerAction.java`
- **Workflow/service enforcement**: MenuItemDAO
- **Messages/labels**: Column headers, form labels

## Uncertainties (STOP; do not guess)
- Order type specific pricing implementation details
- Recipe/ingredient linking (inventory integration)

## MagiDesk parity notes
- **What exists today**: Menu management page (needs verification)
- **What differs / missing**: Full item form with all fields; printer group; stock tracking

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - MenuItem entity with all properties
  - MenuItemService for CRUD
  - Price validation
- **API/DTO requirements**: 
  - GET /menu-items
  - POST/PUT /menu-items
  - DELETE /menu-items/{id}
- **UI requirements**: MenuItemExplorer with table, search, CRUD buttons; full edit form
- **Constraints for implementers**: Pricing must support order type variations; barcode must be unique
