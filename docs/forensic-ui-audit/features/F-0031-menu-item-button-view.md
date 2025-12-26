# Feature: Menu Item Button View (F-0031)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (menu display exists but button grid may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Display menu items as touchable buttons. Visual grid for fast item selection.
- **Evidence**: `MenuItemView.java` + `GroupView.java` - grid of menu item buttons; category/group navigation; button appearance.

## User-facing surfaces
- **Surface type**: Panel/View
- **UI entry points**: OrderView → right panel (item selection area)
- **Exit paths**: Item selected (adds to ticket)

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket must be open
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in OrderView
2. MenuItemView displays:
   - Category buttons (top row)
   - Group buttons (sub-category)
   - Item buttons (grid)
3. User taps category → shows groups
4. User taps group → shows items
5. User taps item:
   - If modifiers required → ModifierDialog
   - If price open → PriceDialog
   - Otherwise → adds to ticket
6. Item appears in ticket view

## Edge cases & failure paths
- **Item out of stock**: Button disabled or hidden
- **86'd item**: Hidden from menu
- **Large menu**: Scrolling or pagination

## Data / audit / financial impact
- **Writes/updates**: TicketItem added
- **Audit events**: Part of order creation
- **Financial risk**: Low

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `MenuItemView` → `ui/views/order/MenuItemView.java`
  - `GroupView` → `ui/views/order/GroupView.java`
  - `CategoryView` → `ui/views/order/CategoryView.java`
- **Entry action(s)**: Part of OrderView layout
- **Workflow/service enforcement**: MenuItem lookup
- **Messages/labels**: Item names, prices

## MagiDesk parity notes
- **What exists today**: Menu item display
- **What differs / missing**: Button grid appearance; category/group navigation

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Menu item queries by category/group
- **API/DTO requirements**: GET /menu-items?categoryId=&groupId=
- **UI requirements**: MenuItemView with button grid
- **Constraints for implementers**: Touch-friendly button sizes
