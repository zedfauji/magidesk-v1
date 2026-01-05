# TECH-B002: Discount Engine Implementation (Phase 3)

## Description
Implement the full discount application logic which is currently deferred to Phase 3.

## Scope
-   **File**: `Magidesk.Application.Services.ApplyDiscountCommandHandler.cs`
-   **Features**: `F-0123`, `F-0048`, `F-0049`

## Implementation Tasks
- [ ] Remove `NotImplementedException` from `ApplyDiscountCommandHandler`.
- [ ] Implement validation logic for Coupon Codes (`F-0122`).
- [ ] Implement percentage-based and fixed-amount discount calculations.
- [ ] Implement "Manager Override" checks for manual discounts.
- [ ] Update `OrderTotal` calculation to include applied discounts.

## Acceptance Criteria
-   Applying a discount code updates the Ticket Total.
-   Invalid codes return a validation error.
-   Discount appears on the specific Order Line or Subtotal.
