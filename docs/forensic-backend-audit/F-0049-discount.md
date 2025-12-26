# Backend Forensic Audit: F-0049 Discount Button

## Feature Context
- **Feature**: Discount Button
- **Trace from**: `F-0049-discount-button.md`
- **Reference**: `DiscountSelectionDialog.java`

## Backend Invariants
1.  **Definition**: Select from pre-defined Discounts (e.g., "Staff Meal 50%", "Police 10%").
2.  **Calculation**: Backend calculates the amount. (Fixed $ or %).
3.  **Authorization**: Some discounts require Manager Permission.

## Forbidden States
-   **Negative Total**: Discount cannot exceed the Ticket Total (unless refund logic applies, but usually capped at 100%).

## Audit Requirements
-   **Event**: `DISCOUNT_APPLIED`
    -   Payload: TicketId, DiscountName, Amount.
    -   Severity: INFO.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `Discount` entity.

## Alignment Strategy
1.  **Ensure** permissions are checked in `ApplyDiscountCommand`.
