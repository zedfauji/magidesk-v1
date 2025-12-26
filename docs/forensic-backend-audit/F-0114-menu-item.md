# Backend Forensic Audit: F-0114 Menu Item Explorer

## Feature Context
- **Feature**: Menu Item Explorer
- **Trace from**: `F-0114-menu-item-explorer.md`
- **Reference**: `MenuItemExplorer.java`

## Backend Invariants
1.  **Pricing**: Base Price vs Shift Price vs Terminal Price.
2.  **Tax**: Must link to a `TaxGroup`.
3.  **Printer**: Must link to a `PrinterGroup`.

## Forbidden States
-   **Zero Price**: Allowed only if `VariablePrice` is true (F-0035).

## Audit Requirements
-   **Event**: `MENU_ITEM_CREATED` / `UPDATED` / `DELETED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `MenuItem`.

## Alignment Strategy
1.  **None**.
