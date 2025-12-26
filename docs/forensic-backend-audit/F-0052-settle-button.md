# Backend Forensic Audit: F-0052 Settle Button

## Feature Context
- **Feature**: Settle Button
- **Trace from**: `F-0052-settle-button.md`
- **Reference**: `SettleTicketAction.java`

## Backend Invariants
1.  **Redundancy**: Alias to F-0008 (Settle Dialog).
2.  **Precondition**: Ticket must have items and be Open.

## Forbidden States
-   **Empty Ticket**: Cannot settle a ticket with 0 items (unless it's a specific "Open Drawer" dummy ticket, but that's different).

## Audit Requirements
-   **Event**: None (Managed by Dialog).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.
