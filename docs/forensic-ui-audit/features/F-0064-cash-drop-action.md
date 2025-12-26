# Feature: Cash Drop Action (F-0064)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Remove excess cash from drawer during shift to safe/bank. Reduces theft risk and robbery impact. Must be tracked for accountability.
- **Evidence**: `DrawerBleedAction.java` + `CashDropDialog.java` - record cash removal amount; update drawer balance.

## User-facing surfaces
- **Surface type**: Action + Dialog
- **UI entry points**: ManagerDialog → Cash Drop/Drawer Bleed
- **Exit paths**: Complete / Cancel

## Preconditions & protections
- **User/role/permission checks**: Cash drop permission (manager)
- **State checks**: Drawer must be assigned with cash
- **Manager override**: Action itself is manager-level

## Step-by-step behavior (forensic)
1. Manager clicks Cash Drop
2. CashDropDialog opens:
   - Current drawer balance shown
   - Amount to drop field
   - Note/reason field (optional)
3. Manager enters drop amount
4. On confirm:
   - CashDropTransaction created (DEBIT type)
   - Terminal balance reduced
   - Transaction logged
5. Physical cash moved to safe
6. Drawer balance updated

## Edge cases & failure paths
- **Drop exceeds balance**: May allow or warn
- **Multi-currency**: Per-currency amounts
- **Zero drop**: Prevented

## Data / audit / financial impact
- **Writes/updates**: 
  - CashDropTransaction
  - Terminal.currentBalance
- **Audit events**: Cash drop logged
- **Financial risk**: Essential for accountability; mismatched drops

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `DrawerBleedAction` → `actions/DrawerBleedAction.java`
  - `CashDropDialog` → `ui/dialog/CashDropDialog.java`
- **Entry action(s)**: ManagerDialog button
- **Workflow/service enforcement**: CashDropTransaction creation
- **Messages/labels**: Drop prompts

## MagiDesk parity notes
- **What exists today**: No cash drop
- **What differs / missing**: Entire cash drop workflow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - RecordCashDropCommand
  - Debit cash session balance
- **API/DTO requirements**: POST /cash-sessions/{id}/drops
- **UI requirements**: CashDropDialog
- **Constraints for implementers**: Must appear on drawer pull report
