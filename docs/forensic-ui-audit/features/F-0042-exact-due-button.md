# Feature: Exact Due Button (F-0042)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (similar buttons may exist)

## Problem / Why this exists (grounded)
- **Operational need**: One-touch cash payment for exact amount due. No change calculation needed.
- **Evidence**: `PaymentView.java` - Exact button that sets tender = due amount.

## User-facing surfaces
- **Surface type**: Action button
- **UI entry points**: PaymentView → Exact button
- **Exit paths**: Amount entered and payment initiated

## Preconditions & protections
- **User/role/permission checks**: Cash payment permission
- **State checks**: Amount due > 0
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in PaymentView
2. Clicks Exact button
3. Tender amount set to exact due amount
4. If combined with Cash:
   - Payment processed
   - No change needed
   - Ticket settled

## Edge cases & failure paths
- **Zero due**: Button disabled
- **Already exact**: No change

## Data / audit / financial impact
- **Writes/updates**: Tender amount
- **Audit events**: Part of payment
- **Financial risk**: Low - exact match

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Button in PaymentView
- **Entry action(s)**: Button click
- **Workflow/service enforcement**: Amount entry
- **Messages/labels**: Exact button

## MagiDesk parity notes
- **What exists today**: May have exact functionality
- **What differs / missing**: Dedicated button

## Porting strategy (PLAN ONLY)
- **Backend requirements**: None (UI only)
- **API/DTO requirements**: Standard payment
- **UI requirements**: Exact button
- **Constraints for implementers**: Simple amount copy
