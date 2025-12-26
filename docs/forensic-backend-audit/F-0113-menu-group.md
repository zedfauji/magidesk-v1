# Backend Forensic Audit: F-0113 Menu Group Explorer

## Feature Context
- **Feature**: Menu Group Explorer
- **Trace from**: `F-0113-menu-group-explorer.md`
- **Reference**: `MenuGroupExplorer.java`

## Backend Invariants
1.  **Parent**: Must belong to a `MenuCategory`.
2.  **Visibility**: Can be hidden without hiding children? Usually hiding group hides children.

## Forbidden States
-   **Orphan**: Group without a Category.

## Audit Requirements
-   **Event**: `MENU_GROUP_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `MenuGroup`.

## Alignment Strategy
1.  **None**.
