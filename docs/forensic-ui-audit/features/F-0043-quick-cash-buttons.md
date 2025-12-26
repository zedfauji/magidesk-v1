# Feature: Quick Cash Buttons (F-0043)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Pre-set cash denomination buttons ($5, $10, $20) for common tender amounts.
- **Evidence**: `PaymentView.java` - quick cash buttons for common bills.

## User-facing surfaces
- **Surface type**: Action buttons
- **UI entry points**: PaymentView → Quick cash buttons
- **Exit paths**: Amount entered

## Preconditions & protections
- **User/role/permission checks**: Cash payment permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in PaymentView
2. Quick cash buttons show:
   - $1, $5, $10, $20, $50, $100
   - Or configured denominations
3. User taps denomination
4. Tender amount set/added
5. May combine (tap $20 + $10 = $30)

## Edge cases & failure paths
- **Multi-currency**: Show configured currency denominations
- **Cumulative mode**: Add amounts or replace

## Data / audit / financial impact
- **Writes/updates**: Tender amount
- **Audit events**: Part of payment
- **Financial risk**: Faster, less error-prone

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Buttons in PaymentView
- **Entry action(s)**: Button click
- **Workflow/service enforcement**: Amount entry
- **Messages/labels**: Denomination values

## MagiDesk parity notes
- **What exists today**: Keypad for entry
- **What differs / missing**: Quick denomination buttons

## Porting strategy (PLAN ONLY)
- **Backend requirements**: None (UI only)
- **API/DTO requirements**: Standard payment
- **UI requirements**: Quick cash button panel
- **Constraints for implementers**: Configurable denominations
