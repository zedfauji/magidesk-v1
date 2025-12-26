# Feature: Drawer Count Dialog (F-0067)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (count may be inline in drawer pull)

## Problem / Why this exists (grounded)
- **Operational need**: Systematic cash counting by denomination. More accurate than total entry.
- **Evidence**: Drawer count pattern in `DrawerPullReportDialog.java` - count by denomination.

## User-facing surfaces
- **Surface type**: Modal dialog or panel
- **UI entry points**: DrawerPullReportDialog → Detailed Count; Shift End
- **Exit paths**: Count complete / Cancel

## Preconditions & protections
- **User/role/permission checks**: Cash count permission
- **State checks**: Drawer assigned
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User initiates drawer count
2. DrawerCountDialog shows denominations:
   - $100 bills: ___
   - $50 bills: ___
   - $20 bills: ___
   - $10 bills: ___
   - $5 bills: ___
   - $1 bills: ___
   - Coins: $0.25, $0.10, $0.05, $0.01
3. User enters count per denomination
4. Running total calculated
5. On confirm: Total recorded

## Edge cases & failure paths
- **Invalid entry**: Validation
- **Missing denomination**: Zero assumed

## Data / audit / financial impact
- **Writes/updates**: Drawer count record with breakdown
- **Audit events**: Count logged
- **Financial risk**: Accuracy for reconciliation

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Part of DrawerPullReportDialog
- **Entry action(s)**: Drawer count action
- **Workflow/service enforcement**: Count calculation
- **Messages/labels**: Denomination labels

## MagiDesk parity notes
- **What exists today**: May have simple count
- **What differs / missing**: Denomination breakdown

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Count by denomination support
- **API/DTO requirements**: Denomination breakdown in count
- **UI requirements**: DrawerCountDialog
- **Constraints for implementers**: Configurable denominations per currency
