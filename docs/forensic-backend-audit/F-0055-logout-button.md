# Backend Forensic Audit: F-0055 Logout Button

## Feature Context
- **Feature**: Logout Button
- **Trace from**: `F-0055-logout-button.md`
- **Reference**: `LogoutAction.java`

## Backend Invariants
1.  **Session Close**: Must terminate the current `UserSession`.
2.  **Resource Release**: Release any "Soft Locks" held by the user (e.g., Ticket currently being edited).
3.  **Clock Out**: Logout != Clock Out. User remains clocked in unless Auto-Clock-Out is configured.

## Forbidden States
-   **Dangling Locks**: User logs out but ticket remains "Locked by User" indefinitely.

## Audit Requirements
-   **Event**: `USER_LOGOUT`
    -   Payload: UserId, SessionDuration, TerminalId.
    -   Severity: INFO.

## Concurrency Semantics
-   **Immediate**: Token revocation.

## MagiDesk Backend Parity
-   **Service**: ✅ `AuthService.Logout` exists.

## Alignment Strategy
1.  **Implement** `ReleaseAllLocksCommand` on Logout.
