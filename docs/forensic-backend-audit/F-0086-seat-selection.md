# Backend Forensic Audit: F-0086 Seat Selection Dialog

## Feature Context
- **Feature**: Seat Selection Dialog
- **Trace from**: `F-0086-seat-selection-dialog.md`
- **Reference**: `SeatSelectionDialog.java`

## Backend Invariants
1.  **Grouping**: `TicketItem` has an optional `SeatNumber` property.
2.  **Splitting**: Used as the primary key for "Split by Seat" logic.

## Forbidden States
-   **Invalid Seat**: Assigning Seat 0 or negative.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TicketItem.SeatNumber`.

## Alignment Strategy
1.  **None**.
