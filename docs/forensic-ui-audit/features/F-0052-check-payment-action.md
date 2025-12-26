# Feature: Check Payment Action (F-0052)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Some businesses accept paper checks as payment. Need to record check number, validate, and track for deposit.
- **Evidence**: `CheckPaymentAction.java` - accepts check as payment; records check number; matches amount.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: PaymentView → Check button
- **Exit paths**: Accept check / Cancel

## Preconditions & protections
- **User/role/permission checks**: Check payment permission
- **State checks**: Ticket with due amount; check acceptance enabled
- **Manager override**: May be required above threshold

## Step-by-step behavior (forensic)
1. User in PaymentView clicks Check
2. Check payment dialog:
   - Check number entry
   - Amount (defaults to due)
   - Bank name (optional)
3. User enters check details
4. On accept:
   - CheckPayment transaction created
   - Check number recorded
   - Ticket balance updated
5. Check physically received
6. Appears in check deposit report

## Edge cases & failure paths
- **No check number**: Validation error
- **Amount mismatch**: Allows over-tender with cash back
- **Check declined later**: Reconciliation issue

## Data / audit / financial impact
- **Writes/updates**: PosTransaction with check details
- **Audit events**: Check payment logged
- **Financial risk**: Bad checks; deposit reconciliation

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action + dialog
- **Entry action(s)**: `CheckPaymentAction` → (path)
- **Workflow/service enforcement**: Payment processing
- **Messages/labels**: Check prompts

## MagiDesk parity notes
- **What exists today**: No check payment
- **What differs / missing**: Check payment type

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CheckPayment entity
- **API/DTO requirements**: Payment with type=CHECK
- **UI requirements**: Check payment dialog
- **Constraints for implementers**: Check tracking for deposits
