# TECH-B004: Customer Command Handler Implementation

## Description
Implement missing gaps in customer assignment logic.

## Scope
-   **File**: `Magidesk.Application.Services.SetCustomerCommandHandler.cs`
-   **Feature**: `F-0061`

## Implementation Tasks
- [ ] Remove "Slice 2 Gap Fill" stub.
- [ ] Implement logic to link `CustomerId` to `Ticket`.
- [ ] Implement validation (Customer must exist).
- [ ] Update Ticket's "Delivery Address" if applicable.

## Acceptance Criteria
-   `SetCustomerCommand` successfully updates the Ticket entity.
-   Ticket audit log reflects customer assignment.
