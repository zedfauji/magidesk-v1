# Backend Forensic Audit: F-0074 Drawer Bleed Dialog

## Feature Context
- **Feature**: Drawer Bleed Dialog
- **Trace from**: `F-0074-drawer-bleed-dialog.md`
- **Reference**: `DrawerBleedDialog.java`

## Backend Invariants
1.  **Semantic**: Identical to Cash Drop but often used for "Mid-shift skim".

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `DRAWER_TRANSACTION` (Type=BLEED).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TerminalTransaction`.

## Alignment Strategy
1.  **None**.
