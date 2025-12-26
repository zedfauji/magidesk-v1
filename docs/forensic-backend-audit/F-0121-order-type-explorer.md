# Backend Forensic Audit: F-0121 Order Type Explorer

## Feature Context
- **Feature**: Order Type Explorer
- **Trace from**: `F-0121-order-type-explorer.md`
- **Reference**: `OrderTypeExplorer.java`

## Backend Invariants
1.  **Properties**: Each type (Bar, Dine In, Take Out) has flags: `ShouldPrintToKitchen`, `HasServiceCharge`, `RequiresAddress`.
2.  **Tax**: Can override tax rules? Usually yes.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `ORDER_TYPE_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `OrderType`.

## Alignment Strategy
1.  **None**.
