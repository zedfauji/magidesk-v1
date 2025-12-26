# Feature: Coupon And Discount Dialog (F-0122)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (discounts exist but dialog workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Staff need to apply coupons/discounts to tickets for promotions, loyalty, manager courtesy, etc. Discounts affect ticket totals and must be tracked.
- **Evidence**: `CouponAndDiscountDialog.java` - lists available coupons; shows discount details (type, value); can edit value; validates minimum buy; applies to ticket.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: OrderView/TicketView → Discount button; PaymentView coupon button
- **Exit paths**: OK (applies discount) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Discount permission; some discounts require manager
- **State checks**: Ticket must exist; minimum buy requirements checked
- **Manager override**: Required for certain discount types or amounts

## Step-by-step behavior (forensic)
1. User clicks Discount/Coupon button
2. CouponAndDiscountDialog opens
3. Dialog shows:
   - Available coupons list (from DiscountDAO)
   - Coupon name, number, type, value fields
   - Up/Down navigation for list
   - Edit Value button
4. User selects coupon from list
5. Details populate (type, value, minimum)
6. User can edit value (for percentage/amount discounts)
7. On OK:
   - Validates minimum buy if required
   - Creates TicketDiscount snapshot
   - Applies to ticket
   - Recalculates totals

## Edge cases & failure paths
- **Minimum buy not met**: Error message, discount not applied
- **Invalid coupon value**: Validation error
- **Multiple discounts**: Only max discount applies (policy in FloreantPOS)
- **Coupon expired**: Not shown in list or error on apply

## Data / audit / financial impact
- **Writes/updates**: TicketDiscount entity; Ticket totals recalculated
- **Audit events**: Discount application logged
- **Financial risk**: Unauthorized discounts; incorrect amounts; revenue loss

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `CouponAndDiscountDialog` → `ui/dialog/CouponAndDiscountDialog.java`
- **Entry action(s)**: Button in OrderView, PaymentView
- **Workflow/service enforcement**: DiscountDAO; Ticket.calculatePrice()
- **Messages/labels**: Labels, coupon list renderer

## Uncertainties (STOP; do not guess)
- Coupon code entry (vs. pre-loaded list)
- Stackable discount support

## MagiDesk parity notes
- **What exists today**: TicketDiscount entity; discount application in Ticket
- **What differs / missing**: Coupon selection dialog; minimum buy validation

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Discount entity with type, value, minimumBuy
  - ApplyDiscountCommand with validation
  - TicketDiscount snapshot creation
- **API/DTO requirements**: GET /discounts; POST /tickets/{id}/discounts
- **UI requirements**: DiscountSelectionDialog with list, details, validation
- **Constraints for implementers**: Discount must be immutable snapshot; recalculate totals on apply
