# Backend Forensic Audit: F-0048 Coupon Button

## Feature Context
- **Feature**: Coupon Button
- **Trace from**: `F-0048-coupon-button.md`
- **Reference**: `CouponSelectionDialog.java`

## Backend Invariants
1.  **Validation**: Coupon code/barcode MUST be validated against active promotions.
2.  **Scope**: Coupon applies to Ticket or specific Items? Backend must enforce scope.
3.  **Exclusivity**: Can multiple coupons be stacked? Usually forbidden by default.

## Forbidden States
-   **Expired Coupon**: Applying a coupon that expired yesterday.
-   **Double Dip**: Applying the same unique coupon code twice.

## Audit Requirements
-   **Event**: `COUPON_APPLIED`
    -   Payload: TicketId, CouponCode, DiscountAmount.
    -   Severity: INFO.

## Concurrency Semantics
-   **Usage Limit**: If "First 100 customers only", need atomic check-and-increment of usage count.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `Coupon` entity and validation logic needed.

## Alignment Strategy
1.  **Implement** `ApplyCouponCommand`.
