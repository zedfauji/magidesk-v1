# Backend Forensic Audit: F-0116 Modifier Group Explorer

## Feature Context
- **Feature**: Modifier Group Explorer
- **Trace from**: `F-0116-modifier-group-explorer.md`
- **Reference**: `ModifierGroupExplorer.java`

## Backend Invariants
1.  **Rules**: `MinQuantity` (Mandatory Selection) and `MaxQuantity` (Limit).
2.  **Binding**: Linked to MenuItems or MenuGroups.

## Forbidden States
-   **Min > Max**: Logical error.

## Audit Requirements
-   **Event**: `MODIFIER_GROUP_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `ModifierGroup`.

## Alignment Strategy
1.  **None**.
