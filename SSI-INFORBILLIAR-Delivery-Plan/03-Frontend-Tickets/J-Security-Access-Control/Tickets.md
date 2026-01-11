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
