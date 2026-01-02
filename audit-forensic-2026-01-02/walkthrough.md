# Role Management Verification

### Automated Tests
- [x] **Unit Tests**: `dotnet test` passed for `RoleCommandHandlerTests`.
  - Verified Create, Update, Delete logic.
  - Verified valid/invalid state handling.

Since the UI for Role Management is not yet implemented, manual verification is deferred. Verification currently relies on Unit Tests passing.

## 1. Prerequisites
- [ ] Build and Run the Solution (`Magidesk.sln`).
- [ ] Log in with an Admin user (or default).
   - Re-select the role: Verify `OpenDrawer` is UNCHECKED and `ApplyDiscount` is CHECKED.

### Case C: Rename Role
1. Select `Test Manager`.
2. **Input**: Change Name to `Shift Lead`.
3. **Action**: Click `Save Role`.
4. **Expected Result**:
   - List updates to show `Shift Lead`.

### Case D: Delete Role
1. Select `Shift Lead`.
2. **Action**: Click `Delete`.
3. **Expected Result**:
   - "Role Deleted" message.
   - Role disappears from the list.
   - Form clears.
