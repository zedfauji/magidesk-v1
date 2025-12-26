# Backend Forensic Audit: F-0109 Tax Configuration View

## Feature Context
- **Feature**: Tax Configuration View
- **Trace from**: `F-0109-tax-configuration-view.md`
- **Reference**: `TaxExplorer.java`

## Backend Invariants
1.  **Rate**: Percentage or Fixed Amount.
2.  **Type**: Inclusive (VAT) vs Exclusive (Sales Tax).
3.  **Hierarchy**: Item Tax Group -> Tax Rate.

## Forbidden States
-   **Circular**: Tax on Tax (unless configured as such).

## Audit Requirements
-   **Event**: `TAX_RATE_UPDATED`.

## Concurrency Semantics
-   **Versioning**: Changing a Tax Rate (8% -> 9%) must NOT change historical tickets.

## MagiDesk Backend Parity
-   **Model**: ✅ `TaxRate` entity.

## Alignment Strategy
1.  **None**.
