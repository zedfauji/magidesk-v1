# Backend Forensic Audit: F-0117 Coupon Explorer

## Feature Context
- **Feature**: Coupon Explorer
- **Trace from**: `F-0117-coupon-explorer.md`
- **Reference**: `CouponExplorer.java`

## Backend Invariants
1.  **Type**: Fixed Amount vs Percentage vs Buy-X-Get-Y.
2.  **Valid To/From**: Date range enforcement.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `COUPON_DEF_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `CouponDefinition`.

## Alignment Strategy
1.  **Implement** Model.
