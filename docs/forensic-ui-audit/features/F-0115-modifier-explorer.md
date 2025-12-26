# Feature: Modifier Explorer (F-0115)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (basic modifiers exist)

## Problem / Why this exists (grounded)
- **Operational need**: Define modifiers (Extra Cheese, No Onions) that can be added to menu items with price impact.
- **Evidence**: `ModifierExplorer.java` - CRUD for modifiers; name, price, modifier group, availability.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Modifiers
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Menu management permission
- **State checks**: Modifier group should exist first
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Modifier Explorer
2. View shows modifiers table with columns:
   - Name
   - Price
   - Modifier Group
   - Enabled
3. New/Edit/Delete actions
4. Modifier form includes:
   - Name
   - Price delta (+/-)
   - Extra price (if applicable)
   - Modifier group assignment
   - Button color
   - Sort order
   - Enabled toggle
5. Save persists modifier
6. Modifier available on POS for applicable items

## Edge cases & failure paths
- **Negative price**: Allowed (subtraction)
- **No group**: Must assign to group
- **Delete in use**: Check ticket references

## Data / audit / financial impact
- **Writes/updates**: MenuModifier entity
- **Audit events**: Modifier changes logged
- **Financial risk**: Price impact affects totals

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ModifierExplorer` → `bo/ui/explorer/ModifierExplorer.java`
- **Entry action(s)**: `ModifierExplorerAction` → `bo/actions/ModifierExplorerAction.java`
- **Workflow/service enforcement**: MenuModifierDAO
- **Messages/labels**: Modifier labels

## MagiDesk parity notes
- **What exists today**: Basic modifier entity
- **What differs / missing**: Full ModifierExplorer UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: MenuModifier with name, price, group
- **API/DTO requirements**: GET/POST/PUT/DELETE /modifiers
- **UI requirements**: ModifierExplorer with CRUD
- **Constraints for implementers**: Price must be signed; linked to modifier group
