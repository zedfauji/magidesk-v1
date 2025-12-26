# Feature: Tip Adjustment Action (F-0056)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: After card payment, customers write tip on receipt. Staff need to adjust the charge amount before batch settlement.
- **Evidence**: `AdjustTipsAction.java` + related - add tip to existing card transaction; pre-batch capture.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: ManagerDialog → Adjust Tips; TicketExplorer actions
- **Exit paths**: Tip adjusted / Cancel

## Preconditions & protections
- **User/role/permission checks**: Tip adjustment permission
- **State checks**: Card transaction exists; not yet captured/settled
- **Manager override**: May be required above threshold

## Step-by-step behavior (forensic)
1. User opens ticket with card payment
2. Clicks Adjust Tips
3. TipAdjustDialog shows:
   - Original payment amount
   - Current tip (if any)
   - New tip entry field
   - New total calculation
4. User enters tip amount
5. On confirm:
   - Transaction updated with tip
   - New total = original + tip
   - Staged for capture
6. Tip appears on gratuity report

## Edge cases & failure paths
- **Already captured**: Tip adjustment prevented
- **Excessive tip**: Warning or manager approval
- **Zero tip**: Allowed
- **Negative tip**: Prevented

## Data / audit / financial impact
- **Writes/updates**: 
  - PosTransaction.gratuityAmount
  - Gratuity entity (if separate)
  - Pre-auth adjustment
- **Audit events**: Tip adjustment logged
- **Financial risk**: Tip fraud; unauthorized adjustments

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action + dialog
- **Entry action(s)**: `AdjustTipsAction` → `actions/AdjustTipsAction.java`
- **Workflow/service enforcement**: Card transaction tip adjustment
- **Messages/labels**: Tip prompts

## MagiDesk parity notes
- **What exists today**: Gratuity entity
- **What differs / missing**: Tip adjustment on existing card transaction

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - AdjustTipCommand
  - Pre-capture tip addition
- **API/DTO requirements**: PATCH /payments/{id}/tip
- **UI requirements**: TipAdjustDialog
- **Constraints for implementers**: Must be before batch capture; audit trail
