# Backend Forensic Audit: F-0085 Bar Tab Selection View

## Feature Context
- **Feature**: Bar Tab Selection View
- **Trace from**: `F-0085-bar-tab-selection-view.md`
- **Reference**: `BarTabView.java`

## Backend Invariants
1.  **Identifier**: Uses Name or Card Name instead of Table Number.
2.  **Persistence**: `Ticket.TicketType = BAR_TAB`.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ Exists.

## Alignment Strategy
1.  **None**.
