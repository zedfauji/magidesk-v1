# Backend Forensic Audit: F-0070 Shift Selection Dialog

## Feature Context
- **Feature**: Shift Selection Dialog
- **Trace from**: `F-0070-shift-selection-dialog.md`
- **Reference**: `ShiftSelectionDialog.java`

## Backend Invariants
1.  **Clock In**: Often part of Clock In flow (F-0009).
2.  **Assignment**: Binds `UserSession` to a specific `Shift` (e.g., "Morning Shift").

## Forbidden States
-   **Closed Shift**: Cannot select a shift that has already been reconciled/closed.

## Audit Requirements
-   **Event**: `SHIFT_SELECTED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `Shift` entity exists.

## Alignment Strategy
1.  **None**.
