# Backend Forensic Audit: F-0077 Table Selection View

## Feature Context
- **Feature**: Table Selection View
- **Trace from**: `F-0077-table-selection-view.md`
- **Reference**: `TableSelectionView.java`

## Backend Invariants
1.  **Status**: Tables have states: `FREE`, `OCCUPIED`, `DIRTY` (optional), `RESERVED`.
2.  **Occupancy**: Opening a ticket on Table X sets it to OCCUPIED.

## Forbidden States
-   **Double Book**: Opening a NEW ticket on an OCCUPIED table (usually implies Merging or adding to existing tab, but blindly creating new occupancy is invalid).

## Audit Requirements
-   **Event**: `TABLE_OCCUPIED`.

## Concurrency Semantics
-   **Race**: Two servers hitting Table 5 same second. First wins.

## MagiDesk Backend Parity
-   **Model**: ✅ `ShopTable` status.

## Alignment Strategy
1.  **Enforce** Locking on Table Selection.
