# Backend Forensic Audit: F-0127 Printer Group Configuration

## Feature Context
- **Feature**: Printer Group Configuration
- **Trace from**: `F-0127-printer-group-configuration.md`
- **Reference**: `PrinterGroupExplorer.java`

## Backend Invariants
1.  **Mapping**: Virtual (Group) -> Physical (Printer).
2.  **Usage**: Menu Items link to Group, not Printer directly (allows hardware swap).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `PRINTER_GROUP_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `PrinterGroup`.

## Alignment Strategy
1.  **None**.
