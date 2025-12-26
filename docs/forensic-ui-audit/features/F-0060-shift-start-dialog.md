# Feature: Shift Start Dialog (F-0060)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (shifts exist but start dialog may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Start-of-shift procedures - count opening cash, assign drawer, begin shift record.
- **Evidence**: Shift management + `DrawerPullReportDialog.java` - shift open with starting balance.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Login → shift not started; Manager → Start Shift
- **Exit paths**: Shift started / Cancel

## Preconditions & protections
- **User/role/permission checks**: Shift start permission
- **State checks**: No open shift for drawer
- **Manager override**: May be required

## Step-by-step behavior (forensic)
1. Manager/user initiates shift start
2. ShiftStartDialog shows:
   - Date/time
   - Drawer assignment
   - Opening cash count
   - Starting float configuration
3. User counts opening cash
4. On confirm:
   - Shift record created
   - Opening balance recorded
   - Drawer assigned
   - User can take orders

## Edge cases & failure paths
- **Shift already open**: Prevent duplicate
- **Count mismatch**: Log discrepancy
- **No drawer available**: Can't start

## Data / audit / financial impact
- **Writes/updates**: Shift record; opening balance
- **Audit events**: Shift start logged
- **Financial risk**: Opening count accuracy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Shift start dialog
- **Entry action(s)**: Shift management
- **Workflow/service enforcement**: Shift creation
- **Messages/labels**: Shift prompts

## MagiDesk parity notes
- **What exists today**: Shift entity
- **What differs / missing**: Shift start dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: StartShiftCommand with balance
- **API/DTO requirements**: POST /shifts
- **UI requirements**: ShiftStartDialog
- **Constraints for implementers**: Record opening balance
