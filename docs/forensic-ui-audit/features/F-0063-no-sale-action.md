# Feature: No Sale Action (F-0063)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Open cash drawer without a sale transaction. Legitimate uses: making change, placing float. Must be tracked for loss prevention.
- **Evidence**: `NoSaleAction.java` - opens drawer; logs "NO SALE" transaction; requires permission.

## User-facing surfaces
- **Surface type**: Action (button)
- **UI entry points**: ManagerDialog → No Sale; Switchboard (permission-gated)
- **Exit paths**: Immediate action

## Preconditions & protections
- **User/role/permission checks**: No Sale permission (often manager-only)
- **State checks**: Drawer must be assigned
- **Manager override**: Action typically IS manager-level

## Step-by-step behavior (forensic)
1. User with permission clicks No Sale
2. System validates:
   - User has NO_SALE permission
   - Drawer is assigned to terminal
3. Cash drawer triggered to open
4. "No Sale" transaction logged:
   - Timestamp
   - User
   - Terminal
   - Reason (if prompted)
5. No money changes hands (just drawer access)

## Edge cases & failure paths
- **No drawer assigned**: Error message
- **Drawer mechanism failure**: Hardware error
- **Frequent no-sales**: Loss prevention alert

## Data / audit / financial impact
- **Writes/updates**: NoSale transaction record
- **Audit events**: No Sale logged (appears on exception report)
- **Financial risk**: Unauthorized drawer access; cash theft opportunity

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `NoSaleAction` → `actions/NoSaleAction.java`
- **Workflow/service enforcement**: Drawer kick; transaction logging
- **Messages/labels**: No Sale prompts

## MagiDesk parity notes
- **What exists today**: Drawer open mechanism
- **What differs / missing**: No Sale action with logging

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - LogNoSaleCommand
  - Drawer kick interface
- **API/DTO requirements**: POST /cash-drawer/no-sale
- **UI requirements**: No Sale button (manager)
- **Constraints for implementers**: Must be audited; appears on exception report
