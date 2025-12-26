# Backend Forensic Audit: F-0111 Back Office Window

## Feature Context
- **Feature**: Back Office Window
- **Trace from**: `F-0111-back-office-window.md`
- **Reference**: `BackOfficeWindow.java`

## Backend Invariants
1.  **Security**: Strict Role-Based Access Control (RBAC). Only "ADMIN" or "MANAGER" roles can access.
2.  **Context**: Operating on the global `Configuration` state.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `BACK_OFFICE_ACCESSED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.
