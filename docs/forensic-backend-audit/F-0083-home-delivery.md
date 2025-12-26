# Backend Forensic Audit: F-0083 Home Delivery View

## Feature Context
- **Feature**: Home Delivery View
- **Trace from**: `F-0083-home-delivery-view.md`
- **Reference**: `HomeDeliveryView.java`

## Backend Invariants
1.  **Type Filter**: Shows only `DELIVERY` tickets.
2.  **Workflow**: States: `COOKING`, `READY`, `OUT_FOR_DELIVERY`, `DELIVERED`.

## Forbidden States
-   **Driver Assignment**: Sending out for delivery without assigning a Driver.

## Audit Requirements
-   **Event**: `DELIVERY_DISPATCHED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ⚠️ Status flow `DISPATCHED` needed.

## Alignment Strategy
1.  **Implement** `AssignDriverCommand`.
