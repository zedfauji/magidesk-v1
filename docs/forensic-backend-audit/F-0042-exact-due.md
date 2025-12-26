# Backend Forensic Audit: F-0042 Exact Due Button

## Feature Context
- **Feature**: Exact Due Button
- **Trace from**: `F-0042-exact-due-button.md`
- **Reference**: `PaymentView.java`

## Backend Invariants
1.  **Precision**: Must use the exact `BalanceDue` from the backend (check for updates before applying).

## Forbidden States
-   **Stale Amount**: Paying 10.00 when the backend balance changed to 12.00 in the background.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: UI behavior.

## Alignment Strategy
1.  **None** backend specific.
