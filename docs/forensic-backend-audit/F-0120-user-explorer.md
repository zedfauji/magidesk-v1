# Backend Forensic Audit: F-0120 User Explorer

## Feature Context
- **Feature**: User Explorer
- **Trace from**: `F-0120-user-explorer.md`
- **Reference**: `UserExplorer.java`

## Backend Invariants
1.  **Credentials**: Password/Pin Hash storage.
2.  **Role**: Link to User Type (Admin, Manager, Server).
3.  **Active**: Soft Delete support.

## Forbidden States
-   **Zero Admins**: System must have at least 1 Admin user to prevent lockout.

## Audit Requirements
-   **Event**: `USER_CREATED` / `USER_PERMISSION_CHANGED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `User` entity.

## Alignment Strategy
1.  **Enforce** "Last Admin" protection.
