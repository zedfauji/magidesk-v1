# Feature: Shift End Dialog (F-0061)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (shift close exists but end dialog may differ)

## Problem / Why this exists (grounded)
- **Operational need**: End-of-shift procedures - count closing cash, reconcile drawer, close shift record.
- **Evidence**: `DrawerPullReportDialog.java` + shift management - shift close with final count.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Manager → End Shift; DrawerPullReportDialog
- **Exit paths**: Shift ended / Cancel

## Preconditions & protections
- **User/role/permission checks**: Shift end permission
- **State checks**: Shift must be open; no pending orders (or force close)
- **Manager override**: May be required to force close

## Step-by-step behavior (forensic)
1. Manager initiates shift end
2. ShiftEndDialog shows:
   - Expected cash (calculated)
   - Cash count entry
   - Variance calculation
   - Non-cash summary
3. User counts cash drawer
4. System calculates:
   - Expected vs counted
   - Over/short
5. On confirm:
   - Shift record closed
   - Final balance recorded
   - Variance logged
   - Reports available

## Edge cases & failure paths
- **Open orders**: Warn or prevent close
- **Variance limit exceeded**: Manager required
- **Force close**: Log with reason

## Data / audit / financial impact
- **Writes/updates**: Shift record closed; final balances
- **Audit events**: Shift end logged with variance
- **Financial risk**: Variance tracking; accountability

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `DrawerPullReportDialog` → `ui/dialog/DrawerPullReportDialog.java`
- **Entry action(s)**: Shift management
- **Workflow/service enforcement**: Shift close
- **Messages/labels**: Count prompts

## MagiDesk parity notes
- **What exists today**: Shift close functionality
- **What differs / missing**: Comprehensive end dialog with count

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CloseShiftCommand with count
- **API/DTO requirements**: PUT /shifts/{id}/close
- **UI requirements**: ShiftEndDialog
- **Constraints for implementers**: Record actual count; calculate variance
