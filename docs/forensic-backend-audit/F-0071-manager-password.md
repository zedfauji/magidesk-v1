# Backend Forensic Audit: F-0071 Manager Password Entry Dialog

## Feature Context
- **Feature**: Manager Password Entry Dialog
- **Trace from**: `F-0071-manager-password-entry.md`
- **Reference**: `PasswordEntryDialog.java`

## Backend Invariants
1.  **Elevation**: Used to temporarily elevate privileges for a single action.
2.  **Audit**: Must log WHO authorized the action (The Manager, not the current session user).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `PRIVILEGE_ELEVATION_GRANTED`
    -   Payload: AuthorizingUserId, Action.
    -   Severity: WARN.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Service**: ✅ `AuthService.VerifyPassword`.

## Alignment Strategy
1.  **Implements** "Supervisor Override" pattern.
