# Feature: Discount Application Dialog (F-0123)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (discount may exist but dialog differs)

## Problem / Why this exists (grounded)
- **Operational need**: Apply discounts to ticket - percentage, fixed amount, or item-level. Promotional pricing.
- **Evidence**: `CouponAndDiscountDialog.java` + discount handling - discount selection and application.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: OrderView → Discount button; ManagerDialog → Discount
- **Exit paths**: Discount applied / Cancel

## Preconditions & protections
- **User/role/permission checks**: Discount permission; amount limits by role
- **State checks**: Ticket has items; within discount limits
- **Manager override**: Required above threshold

## Step-by-step behavior (forensic)
1. User clicks Discount
2. DiscountDialog opens:
   - Pre-defined discounts (10%, 20%, Employee)
   - Custom percentage entry
   - Custom amount entry
   - Apply to: Ticket or selected item
3. User selects discount type
4. Enter amount if custom
5. On apply:
   - Discount calculated
   - Ticket total reduced
   - Reason may be required
6. Discount shown on ticket

## Edge cases & failure paths
- **Over limit**: Manager override required
- **Multiple discounts**: May stack or replace
- **Zero total**: Prevented or allowed

## Data / audit / financial impact
- **Writes/updates**: Ticket.discountAmount
- **Audit events**: Discount logged with reason
- **Financial risk**: Revenue loss; fraud potential

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `CouponAndDiscountDialog` → `ui/dialog/CouponAndDiscountDialog.java`
- **Entry action(s)**: Discount action
- **Workflow/service enforcement**: Discount limits
- **Messages/labels**: Discount names

## MagiDesk parity notes
- **What exists today**: Discount entity type
- **What differs / missing**: Full discount application dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Discount with limits, tracking
- **API/DTO requirements**: POST /tickets/{id}/discount
- **UI requirements**: DiscountDialog
- **Constraints for implementers**: Role-based limits; audit trail
