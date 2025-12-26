# Feature: Password Entry Dialog (F-0007)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (login exists but dialog workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Secure user authentication via PIN/password. Also used for manager overrides on protected actions.
- **Evidence**: `PasswordEntryDialog.java` - numeric keypad for PIN entry; password field; validates against user database; supports manager mode.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Login screen; protected actions (void, discount, refund); manager functions
- **Exit paths**: Login success / Cancel

## Preconditions & protections
- **User/role/permission checks**: Validates user credentials
- **State checks**: None
- **Manager override**: Dialog IS the override mechanism

## Step-by-step behavior (forensic)
1. Dialog triggered (login or override)
2. PasswordEntryDialog opens with:
   - PIN/password field
   - Numeric keypad (touch-friendly)
   - Optional user selection
3. User enters PIN
4. System validates:
   - PIN matches user record
   - User has required permission (if override)
   - User active status
5. On success: Returns User object to caller
6. On failure: Error message, retry

## Edge cases & failure paths
- **Wrong PIN**: Error, allow retry
- **User locked**: Account locked message
- **Max attempts**: May lock account
- **Timeout**: Dialog closes on inactivity

## Data / audit / financial impact
- **Writes/updates**: None (authentication only)
- **Audit events**: Login attempts logged; failed attempts tracked
- **Financial risk**: Unauthorized access if weak PINs

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `PasswordEntryDialog` → `ui/dialog/PasswordEntryDialog.java`
- **Entry action(s)**: Called from login flow, protected actions
- **Workflow/service enforcement**: UserDAO.findUserBySecretKey()
- **Messages/labels**: Login prompts, error messages

## MagiDesk parity notes
- **What exists today**: Login mechanism exists
- **What differs / missing**: Manager override dialog; attempt tracking

## Porting strategy (PLAN ONLY)
- **Backend requirements**: User authentication; PIN validation
- **API/DTO requirements**: POST /auth/login or local validation
- **UI requirements**: PINEntryDialog with keypad
- **Constraints for implementers**: PIN must be hashed; track failed attempts
