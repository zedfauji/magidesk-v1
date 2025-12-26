# Backend Forensic Audit: F-0124 Table Section Configuration

## Feature Context
- **Feature**: Table Section Configuration
- **Trace from**: `F-0124-table-section-configuration.md`
- **Reference**: `ShopFloorExplorer.java` (Redundant with F-0081?)

## Backend Invariants
1.  **Grouping**: Zone Name (Patio).
2.  **Surcharge**: Some zones might have auto-gratuity/cover charge.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `ZONE_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TableSection`.

## Alignment Strategy
1.  **None**.
