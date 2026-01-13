# Frontend Tickets: J — Security & Access Control

## FE-J-SEC-01: Integrate Manager PIN Authorization into Refund Wizard

**Category**: J — Security & Access Control  
**Priority**: P1  
**Status**: ❌ Not Started  
**Related Feature**: C.14 (Advanced Refund Management)  
**Phase**: Phase 3 (Frontend)

### Objective
Replace placeholder manager identity (`UserId(Guid.NewGuid())`) in `RefundWizardViewModel` with proper Manager PIN authorization using existing security infrastructure.

### Scope
- Invoke existing `ManagerPinDialog` when entering authorization step (Step 4)
- Use returned authorized `UserId` from successful PIN validation
- Enforce existing `UserPermission.RefundTicket` permission check
- Surface permission denial errors to user
- Remove all placeholder ID generation

### Explicitly OUT OF SCOPE
- Changing refund logic or UI flow
- Creating new dialogs or security services
- Modifying backend security rules
- Altering Feature C.14 core functionality

### Acceptance Criteria
1. Refund cannot proceed without valid manager PIN entry
2. Authorized manager `UserId` is passed to `RefundTicketCommand`
3. Permission denial (`UserPermission.RefundTicket`) is surfaced with clear error message
4. No placeholder `Guid.NewGuid()` remains in refund flow
5. Existing full/partial/specific refund behavior unchanged
6. Failed PIN attempts allow retry without losing wizard state

### Implementation Notes
- Reuse `ManagerPinDialog` from existing codebase
- Use `ISecurityService.HasPermissionAsync()` for validation
- Handle three failure modes:
  - Invalid PIN → show error, stay in wizard
  - Permission denied → show error, abort refund
  - Cancelled → return safely, no state change
- Ensure audit logs receive real manager identity

### Files to Modify
- `ViewModels/RefundWizardViewModel.cs` (Line 228: Replace placeholder)
- Integration point: Step 3→4 transition or Step 4 entry

### Dependencies
- Existing `ManagerPinDialog`
- Existing `ISecurityService`
- `UserPermission.RefundTicket` enum value

---

## FE-J.2-01: Role Management UI

**Ticket ID:** FE-J.2-01  
**Feature ID:** J.2  
**Type:** Frontend  
**Title:** Role Management UI  
**Priority:** P1

### Outcome
Interface to manage system roles and associated permissions.

### Scope
- Create `RoleManagementPage`
- Add/Edit Role dialog
- Permission checklist (Tree view of all capabilities)

### Acceptance Criteria
- [ ] Create custom roles
- [ ] Toggle individual permissions
- [ ] Prevent editing System Admin role

---

## FE-J.3-01: User Management Grid

**Ticket ID:** FE-J.3-01  
**Feature ID:** J.3  
**Type:** Frontend  
**Title:** User Management Grid  
**Priority:** P1

### Outcome
Admin page to list, search, and manage staff users.

### Scope
- Create `UserManagementPage`
- DataGrid of users (Name, Role, Status)
- Actions: Edit, Deactivate, Reset PIN

### Acceptance Criteria
- [ ] Lists all users
- [ ] Filter by Role or Status
- [ ] Add User workflow works

---

## FE-J.4-01: PIN Management UI

**Ticket ID:** FE-J.4-01  
**Feature ID:** J.4  
**Type:** Frontend  
**Title:** PIN Management UI  
**Priority:** P1

### Outcome
Secure interface for changing user PINs.

### Scope
- "Change PIN" dialog
- "Current PIN" validation (for self-service)
- Admin override (no current PIN needed)

### Acceptance Criteria
- [ ] Validates PIN complexity (if any)
- [ ] Confirms new PIN
- [ ] Success notification

---

## FE-J.5-01: Permission Group UI

**Ticket ID:** FE-J.5-01  
**Feature ID:** J.5  
**Type:** Frontend  
**Title:** Permission Group UI  
**Priority:** P2

### Outcome
Manage groups of permissions for easier assignment.

### Scope
- Create `PermissionGroupPage`
- Define groups (e.g., "Cash Handling", "Inventory Mgmt")
- Assign groups to Roles

### Acceptance Criteria
- [ ] Groups persist
- [ ] Assigning group grants all contained permissions

---

## FE-J.6-01: Audit Log Viewer

**Ticket ID:** FE-J.6-01  
**Feature ID:** J.6  
**Type:** Frontend  
**Title:** Audit Log Viewer  
**Priority:** P2

### Outcome
Searchable view of system security events.

### Scope
- Create `AuditLogPage`
- List login attempts, permission failures, sensitive actions
- Filter by Time, User, Severity

### Acceptance Criteria
- [ ] Loads logs efficiently
- [ ] Color-coded severity (Info, Warning, Critical)

---

## FE-J.7-01: Server Assignment Manager

**Ticket ID:** FE-J.7-01  
**Feature ID:** J.7  
**Type:** Frontend  
**Title:** Server Assignment Manager  
**Priority:** P1

### Outcome
Global view of which servers are assigned to which tables/sections.

### Scope
- Graphical floor map overlay with server names
- Drag-and-drop reassignment
- Shift handoff tool

### Acceptance Criteria
- [ ] Visual assignment clear
- [ ] "Clear All" for end of day
- [ ] Changes reflect on POS terminals

---

## FE-J.8-01: User Activity Log UI

**Ticket ID:** FE-J.8-01  
**Feature ID:** J.8  
**Type:** Frontend  
**Title:** User Activity Log UI  
**Priority:** P2

### Outcome
Detailed view of a specific user's actions.

### Scope
- Drill-down from User Management
- Timeline view of orders taken, voids, discounts
- Session login/logout history

### Acceptance Criteria
- [ ] Detailed timeline view
- [ ] Links to specific tickets/orders

---

## FE-J.10-01: Break Tracking UI

**Ticket ID:** FE-J.10-01  
**Feature ID:** J.10  
**Type:** Frontend  
**Title:** Break Tracking UI  
**Priority:** P2

### Outcome
Interface for staff to start/end breaks.

### Scope
- "Start Break" button in User Menu
- Break type selection (Paid/Unpaid)
- Timer overlay during break
- "End Break" action

### Acceptance Criteria
- [ ] Prevent POS actions during break
- [ ] Logs start/end times
- [ ] Manager override to force end break
