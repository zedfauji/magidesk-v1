# Feature: Bar Tab Selection View (F-0085)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Bar customers may run tabs, pre-authorizing a card and adding drinks throughout their visit. Staff need to select existing bar tabs to add items.
- **Evidence**: `BarTabSelectionView.java` - displays open bar tabs; select to add items; shows tab owner, amount, status.

## User-facing surfaces
- **Surface type**: Panel/View
- **UI entry points**: Login → Bar Tab order type; Switchboard → Bar Tab button
- **Exit paths**: Select tab (opens order) / New tab / Cancel

## Preconditions & protections
- **User/role/permission checks**: Bar tab permission
- **State checks**: Bar tabs must exist or be creatable
- **Manager override**: Not typically required

## Step-by-step behavior (forensic)
1. User selects Bar Tab order type
2. BarTabSelectionView opens
3. Shows existing open bar tabs:
   - Tab name/number
   - Customer name (if set)
   - Running total
   - Card pre-auth status
   - Server name
4. User can:
   - Select existing tab to add items
   - Create new bar tab
   - Search by name/number
5. Selected tab opens in OrderView
6. Items added to existing tab total

## Edge cases & failure paths
- **No open tabs**: Show create new option
- **Tab belongs to other server**: May require permission
- **Pre-auth expired**: Warning, may need re-auth
- **Tab closed but not paid**: Reopen option

## Data / audit / financial impact
- **Writes/updates**: Ticket items added to existing tab
- **Audit events**: Tab access logged
- **Financial risk**: Pre-auth fraud; tab walkouts; tips on pre-auth

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `BarTabSelectionView` → `ui/tableselection/BarTabSelectionView.java`
  - `BarTabButton` → `swing/BarTabButton.java`
- **Entry action(s)**: `NewBarTabAction` → `actions/NewBarTabAction.java`
- **Workflow/service enforcement**: Ticket queries with bar tab type
- **Messages/labels**: Tab labels

## MagiDesk parity notes
- **What exists today**: No bar tab workflow
- **What differs / missing**: Entire bar tab system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Bar tab order type with pre-auth support
  - Tab query by status
- **API/DTO requirements**: GET /tickets?orderType=BAR_TAB&status=OPEN
- **UI requirements**: BarTabSelectionView
- **Constraints for implementers**: Pre-auth capture on close; tip handling
