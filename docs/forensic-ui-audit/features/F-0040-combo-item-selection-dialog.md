# Feature: Combo Item Selection Dialog (F-0040)

## Classification
- **Parity classification**: DEFER (advanced feature)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Combo meals/bundles require selection of included items (e.g., "Pick your side" for a combo).
- **Evidence**: `ComboSelectionDialog.java` - presents combo options; required selections; price adjustments.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Auto-opens when adding combo item
- **Exit paths**: Complete selections / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Item is combo type with selection groups
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds combo meal item
2. ComboSelectionDialog opens
3. For each selection group:
   - "Choose your drink"
   - "Choose your side"
   - List of options
   - Price differential (if any)
4. User makes required selections
5. Validation: All required groups selected
6. On OK: Combo added with selected components
7. Components shown on ticket

## Edge cases & failure paths
- **Skip required selection**: Prevented
- **Optional groups**: May skip
- **Price upgrade**: Shows price difference

## Data / audit / financial impact
- **Writes/updates**: TicketItem with combo components
- **Audit events**: Part of order
- **Financial risk**: Combo vs a-la-carte pricing

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ComboSelectionDialog` → (path)
- **Entry action(s)**: Auto-triggered
- **Workflow/service enforcement**: MenuItem combo structure
- **Messages/labels**: Selection prompts

## MagiDesk parity notes
- **What exists today**: No combo support
- **What differs / missing**: Entire combo system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: ComboGroup, ComboComponent entities
- **API/DTO requirements**: Combo structure in menu item
- **UI requirements**: ComboSelectionDialog
- **Constraints for implementers**: Complex pricing; kitchen behavior
