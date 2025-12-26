# Feature: Logout Action (F-0008)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (logout exists but workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Users must log out to secure the terminal, switch users, or end their session. Security requirement.
- **Evidence**: `LogoutAction.java` - clears current user; returns to login screen; may prompt for open tickets.

## User-facing surfaces
- **Surface type**: Action
- **UI entry points**: Switchboard → Logout button; key shortcut; timeout trigger
- **Exit paths**: Logout complete / Cancel (if open tickets)

## Preconditions & protections
- **User/role/permission checks**: Current user authenticated
- **State checks**: Check for user's open tickets
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User clicks Logout (or timeout triggers)
2. System checks for user's open tickets
3. If open tickets exist:
   - Warning dialog shown
   - Option to transfer or close
4. If confirmed:
   - Current user cleared
   - Session ended
   - LoginView displayed
5. Terminal ready for next user

## Edge cases & failure paths
- **Open tickets**: Must handle before logout
- **Auto-logout timeout**: May force logout regardless
- **Pending transactions**: Should complete or cancel

## Data / audit / financial impact
- **Writes/updates**: Session end logged
- **Audit events**: Logout time recorded
- **Financial risk**: Unattended terminal if no auto-logout

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `LogoutAction` → `actions/LogoutAction.java`
- **Workflow/service enforcement**: Session management
- **Messages/labels**: Logout prompts

## MagiDesk parity notes
- **What exists today**: Basic logout
- **What differs / missing**: Open ticket handling; auto-timeout

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Session tracking; open ticket query
- **API/DTO requirements**: POST /auth/logout
- **UI requirements**: Logout button; open ticket warning
- **Constraints for implementers**: Must handle open tickets gracefully
