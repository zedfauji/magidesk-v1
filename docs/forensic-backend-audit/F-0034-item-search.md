# Backend Forensic Audit: F-0034 Item Search Dialog

## Feature Context
- **Feature**: Item Search Dialog
- **Trace from**: `F-0034-item-search-dialog.md`
- **Reference**: `ItemSearchDialog.java`

## Backend Invariants
1.  **Visibility**: Search results MUST respect the `IsVisible` flag of MenuItems.
2.  **Scope**: Search by Name, Barcode, or ID.

## Forbidden States
-   **Leakage**: Showing "Archived" or "Deleted" items in search results.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ `SearchMenuItemsQuery`.

## Alignment Strategy
1.  **Optimize** for substring search performance.
