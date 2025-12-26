# Backend Forensic Audit: F-0053 Split Button

## Feature Context
- **Feature**: Split Button
- **Trace from**: `F-0053-split-button.md`
- **Reference**: `SplitTicketAction.java`

## Backend Invariants
1.  **Precondition**: Ticket items > 1 (Cannot split a single item unless fractional split allowed).
2.  **Status**: Ticket must be `OPEN`. Settle/Paid tickets cannot be split (must be Reopened first).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None (Managed by Dialog).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.
