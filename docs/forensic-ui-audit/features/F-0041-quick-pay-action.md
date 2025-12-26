# Feature: Quick Pay Action (F-0041)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (pay button exists)

## Problem / Why this exists (grounded)
- **Operational need**: Fast checkout with exact cash - one button to settle instead of entering amount.
- **Evidence**: `PayNowAction.java` + quick pay buttons - direct to payment with amount.

## User-facing surfaces
- **Surface type**: Action button
- **UI entry points**: OrderView → Pay Now button
- **Exit paths**: Payment view or direct settle

## Preconditions & protections
- **User/role/permission checks**: Payment permission
- **State checks**: Ticket has items; amount due > 0
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in OrderView with items
2. Clicks Pay Now / Quick Pay
3. Ticket finalized (if unsent items)
4. Amount calculated
5. Quick pay options:
   - If exact cash: Direct settle
   - Otherwise: Open payment view
6. Payment processed
7. Receipt printed

## Edge cases & failure paths
- **No items**: Disabled
- **Zero balance**: Skip payment view

## Data / audit / financial impact
- **Writes/updates**: Ticket settled; payment recorded
- **Audit events**: Payment logged
- **Financial risk**: Normal payment risk

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Button in OrderView
- **Entry action(s)**: `PayNowAction` → `actions/PayNowAction.java`
- **Workflow/service enforcement**: Settlement flow
- **Messages/labels**: Pay button

## MagiDesk parity notes
- **What exists today**: Payment controls
- **What differs / missing**: Quick pay button behavior

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Settlement command
- **API/DTO requirements**: Standard payment API
- **UI requirements**: Pay Now button
- **Constraints for implementers**: Fast path for exact cash
