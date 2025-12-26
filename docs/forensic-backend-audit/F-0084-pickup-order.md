# Backend Forensic Audit: F-0084 Pickup Order View

## Feature Context
- **Feature**: Pickup Order View
- **Trace from**: `F-0084-pickup-order-view.md`
- **Reference**: `PickupView.java`

## Backend Invariants
1.  **Type Filter**: Shows `PICKUP` / `TAKE_OUT`.
2.  **Alerts**: Highlight if "Ready" but not picked up for X minutes.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `ORDER_PICKED_UP`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ⚠️ `PickupTime` tracking.

## Alignment Strategy
1.  **None**.
