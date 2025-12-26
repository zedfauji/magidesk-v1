# Backend Forensic Audit: F-0038 Modifier Selection Dialog

## Feature Context
- **Feature**: Modifier Selection Dialog
- **Trace from**: `F-0038-modifier-selection-dialog.md`
- **Reference**: `ModifierSelectionDialog.java`

## Backend Invariants
1.  **Group Config**: Respect `MinQuantity` (Mandatory) and `MaxQuantity`.
2.  **Multi-Select**: Allow multiple selections if Max > 1.

## Forbidden States
-   **Constraint Violation**: Saving a TicketItem with 0 modifiers when Group Min=1.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ `ModifierGroup` validation exists.

## Alignment Strategy
1.  **Ensure** backend validation mirrors UI constraints.
