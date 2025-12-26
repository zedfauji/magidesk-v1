# Backend Forensic Audit: F-0081 Floor Explorer

## Feature Context
- **Feature**: Floor Explorer
- **Trace from**: `F-0081-floor-explorer.md`
- **Reference**: `FloorExplorer.java`

## Backend Invariants
1.  **Hierarchy**: Tables belong to a `TableSection` (Floor/Zone).
2.  **Filter**: Queries to `Table` entity must support `WHERE SectionId = X`.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ `TableSection` entity exists.

## Alignment Strategy
1.  **None**.
