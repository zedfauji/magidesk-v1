# Backend Forensic Audit: F-0067 Order Info View

## Feature Context
- **Feature**: Order Info View
- **Trace from**: `F-0067-order-info-view.md`
- **Reference**: `OrderInfoView.java`

## Backend Invariants
1.  **Persistence**: Display Attributes (e.g., Table Name, Guest Count, Server Name) must vary based on Order Type.
2.  **Consistency**: Must match the `Ticket` entity state.

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
