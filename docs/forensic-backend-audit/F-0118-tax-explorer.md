# Backend Forensic Audit: F-0118 Tax Explorer

## Feature Context
- **Feature**: Tax Explorer
- **Trace from**: `F-0118-tax-explorer.md`
- **Reference**: `TaxExplorer.java` (CRU)

## Backend Invariants
1.  **Uniqueness**: Tax Name must be unique.
2.  **Rate**: Double precision required.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `TAX_DEF_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TaxRate`.

## Alignment Strategy
1.  **None**.
