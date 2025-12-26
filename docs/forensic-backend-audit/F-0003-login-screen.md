# Backend Forensic Audit: F-0003 Login Screen

## Feature Context
- **Feature**: Login Screen
- **Trace from**: `F-0003-login-screen.md`
- **Reference**: `LoginView.java`, `PasswordEntryDialog.java`

## Backend Invariants
1.  **Authentication Authority**: Validating a PIN/Password MUST happen against the authoritative user store (Database/Service).
2.  **Role Enforcement**: A successful login MUST return the verified `UserRole` and `Permissions` snapshot.
3.  **Session Uniqueness**: (Ideally) A specific user should not be logged in to multiple terminals performing conflicting actions (though FloreantPOS allows this, MagiDesk should enforce strict attribution).
4.  **Credential Protection**: PINs must never be logged or transmitted in plain text.

## Forbidden States
-   **Bypass**: Accessing `RootView` or `OrderView` without a valid `UserSession` token.
-   **Privilege Escalation**: Modifying client-side state to acquire "Manager" role without backend validation.

## Audit Requirements
-   **Event**: `USER_LOGIN_SUCCESS`
    -   Payload: UserId, TerminalId.
    -   Severity: INFO.
-   **Event**: `USER_LOGIN_FAILURE`
    -   Payload: TerminalId, AttemptedId (if available).
    -   Severity: WARN.

## Concurrency Semantics
-   **Atomic Session Creation**: The login process must atomically create a session record to prevent race conditions during Shift start.

## MagiDesk Backend Parity
-   **Authentication**: ✅ Exists (`UserService`).
-   **Permissions**: ⚠️ Partial. Needs specific mapping to Floreant's granular permission set.
-   **Audit**: ⚠️ Missing structured Login/Logout event logging.

## Alignment Strategy
1.  **Harden** `AuthService` to return full Permission set on login.
2.  **Implement** rigorous audit logging for all authentication events.
