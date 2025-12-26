# Feature: User Explorer (F-0120)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (basic user management exists)

## Problem / Why this exists (grounded)
- **Operational need**: Managers must create/edit user accounts, assign roles/permissions, set PINs, manage employee information for POS access.
- **Evidence**: `UserExplorer.java` - CRUD for users; `UserForm.java` - detailed user entry with permissions, PIN, shift assignment, wage info.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Users
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: User management permission; admin only
- **State checks**: None
- **Manager override**: Action requires admin

## Step-by-step behavior (forensic)
1. Open User Explorer from Back Office
2. View shows table of all users:
   - User ID
   - Name
   - Role/Type
   - Active status
3. Actions:
   - New: Opens empty UserForm
   - Edit: Opens UserForm with user data
   - Delete: Removes user (if no dependencies)
4. UserForm includes:
   - Name, PIN/password
   - User type (role assignment)
   - Permissions (if custom)
   - Driver info (for delivery)
   - Wage rate (for payroll)
   - Shift assignment
   - Active/inactive toggle
5. Save persists user

## Edge cases & failure paths
- **Duplicate PIN**: Validation error
- **Delete user with open tickets**: Prevent or transfer first
- **Deactivate logged-in user**: Allow but session ends
- **Weak PIN**: May have minimum length rule

## Data / audit / financial impact
- **Writes/updates**: User entity; UserType association
- **Audit events**: User changes logged
- **Financial risk**: Unauthorized access; wage data accuracy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `UserExplorer` → `bo/ui/explorer/UserExplorer.java`
  - `UserForm` → `ui/forms/UserForm.java`
- **Entry action(s)**: `UserExplorerAction` → `bo/actions/UserExplorerAction.java`
- **Workflow/service enforcement**: UserDAO
- **Messages/labels**: Form field labels

## Uncertainties (STOP; do not guess)
- Password hashing implementation
- Permission granularity beyond roles

## MagiDesk parity notes
- **What exists today**: Basic user entity and management
- **What differs / missing**: Full UserForm with all fields; UserExplorer UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - User entity with PIN, role, wage, etc.
  - UserService for CRUD
  - PIN uniqueness validation
- **API/DTO requirements**: GET/POST/PUT/DELETE /users
- **UI requirements**: UserExplorer table; UserForm with all fields
- **Constraints for implementers**: PIN must be secure; cannot delete user with history
