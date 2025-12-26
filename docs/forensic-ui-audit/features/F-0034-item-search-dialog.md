# Feature: Item Search Dialog (F-0034)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (basic search may exist)

## Problem / Why this exists (grounded)
- **Operational need**: Large menus make scrolling impractical. Staff need to search items by name, barcode, or item number.
- **Evidence**: `ItemSearchDialog.java` - search field with results list; supports barcode scanning; keyboard entry.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: TicketView → Search button; barcode scan triggers
- **Exit paths**: Select item / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket must be open
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User clicks Search or scans barcode
2. ItemSearchDialog opens
3. User types search query
4. Results filter in real-time:
   - Match by name (contains)
   - Match by barcode (exact)
   - Match by item number
5. User selects item from results
6. Item added to ticket
7. If modifiers required, ModifierSelectionDialog opens

## Edge cases & failure paths
- **No results**: Empty list, keep searching
- **Multiple matches**: User selects from list
- **Item out of stock**: May be hidden or marked
- **Barcode not found**: Error message

## Data / audit / financial impact
- **Writes/updates**: Ticket item added
- **Audit events**: Part of order creation
- **Financial risk**: Low

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ItemSearchDialog` → `ui/dialog/ItemSearchDialog.java`
- **Entry action(s)**: Button in TicketView
- **Workflow/service enforcement**: MenuItemDAO.findByNameOrBarcode()
- **Messages/labels**: Search prompt

## MagiDesk parity notes
- **What exists today**: Basic menu display
- **What differs / missing**: Full search dialog with barcode support

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Menu item search query
- **API/DTO requirements**: GET /menu-items?search=
- **UI requirements**: ItemSearchDialog with text input, results list
- **Constraints for implementers**: Real-time filtering; barcode scanner integration
