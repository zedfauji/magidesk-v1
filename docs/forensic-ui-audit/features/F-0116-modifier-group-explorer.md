# Feature: Modifier Group Explorer (F-0116)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (modifier groups exist but explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Modifiers are organized into groups (e.g., "Toppings", "Sizes", "Cooking Options"). Groups define selection rules (required, min/max selections).
- **Evidence**: `ModifierGroupExplorer.java` - CRUD for modifier groups with selection rules.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Modifier Groups
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Menu management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Modifier Group Explorer
2. View shows modifier groups table:
   - Name
   - Required flag
   - Exclusive flag
   - Min/Max selections
3. New/Edit/Delete actions
4. Modifier group form includes:
   - Name
   - Required toggle (must select from this group)
   - Exclusive toggle (single selection)
   - Minimum selections
   - Maximum selections
   - Sort order
   - Associated modifiers
5. Save persists group
6. Group available for menu item assignment

## Edge cases & failure paths
- **Delete with modifiers**: Orphans modifiers or cascades
- **Conflicting min/max**: Validation (min <= max)
- **Required with no modifiers**: Warning

## Data / audit / financial impact
- **Writes/updates**: ModifierGroup entity
- **Audit events**: Menu changes logged
- **Financial risk**: Low - affects ordering workflow

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ModifierGroupExplorer` → `bo/ui/explorer/ModifierGroupExplorer.java`
- **Entry action(s)**: `ModifierGroupExplorerAction` → `bo/actions/ModifierGroupExplorerAction.java`
- **Workflow/service enforcement**: ModifierGroupDAO
- **Messages/labels**: Group labels

## MagiDesk parity notes
- **What exists today**: Basic modifier group concept
- **What differs / missing**: Full ModifierGroupExplorer with selection rules

## Porting strategy (PLAN ONLY)
- **Backend requirements**: ModifierGroup with required, exclusive, min, max
- **API/DTO requirements**: GET/POST/PUT/DELETE /modifier-groups
- **UI requirements**: ModifierGroupExplorer
- **Constraints for implementers**: Selection rules enforced in POS
