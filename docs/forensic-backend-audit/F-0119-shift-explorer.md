# Backend Forensic Audit: F-0119 Shift Explorer

## Feature Context
- **Feature**: Shift Explorer
- **Trace from**: `F-0119-shift-explorer.md`
- **Reference**: `ShiftExplorer.java`

## Backend Invariants
1.  **Definition**: Shifts are Time Ranges (Morning: 8am-4pm) or Logical Blocks.
2.  **Usage**: Used for reporting aggregation.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `SHIFT_DEF_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `ShiftDefinition`.

## Alignment Strategy
1.  **None**.
