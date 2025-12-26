# Backend Forensic Audit: F-0122 Coupon and Discount Dialog

## Feature Context
- **Feature**: Coupon and Discount Dialog
- **Trace from**: `F-0122-coupon-and-discount-dialog.md`
- **Reference**: `CouponAndDiscountDialog.java`

## Backend Invariants
1.  **Redundancy**: Seems to consolidate F-0048 and F-0049.
2.  **Priority**: If both applied, determining order (Discount then Tax? Or other way).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.
