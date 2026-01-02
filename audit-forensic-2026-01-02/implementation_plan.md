# Role Management Implementation Plan (Revised)

## Goal
Finish the implementation of Role Management by consolidating logic into individual backend Command Handlers (Set A), adding critical validations (name uniqueness, user assignment checks), and removing redundant/overriding registrations in the UI layer.

## Proposed Changes

### Magidesk.Application

#### [MODIFY] [IRoleRepository.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Application/Interfaces/IRoleRepository.cs)
- Add `GetByNameAsync` method to support uniqueness validation.

#### [MODIFY] [IUserRepository.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Application/Interfaces/IUserRepository.cs)
- Add `HasUsersInRoleAsync` method to support deletion safety check.

#### [MODIFY] [CreateRoleCommandHandler.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Application/Commands/SystemConfig/CreateRoleCommandHandler.cs)
- Implement name uniqueness validation: return failure if a role with the same name already exists.

#### [MODIFY] [DeleteRoleCommandHandler.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Application/Commands/SystemConfig/DeleteRoleCommandHandler.cs)
- Implement safety check: prevent deletion of a role if any users are currently assigned to it.

#### [DELETE] [RoleCommandHandlers.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Application/Services/System/RoleCommandHandlers.cs)
- Remove the redundant composite handler to ensure Set A handlers are used.

---

### Magidesk.Infrastructure

#### [MODIFY] [RoleRepository.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Infrastructure/Repositories/RoleRepository.cs)
- Implement `GetByNameAsync` using EF Core `FirstOrDefaultAsync`.

#### [MODIFY] [UserRepository.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/Magidesk.Infrastructure/Repositories/UserRepository.cs)
- Implement `HasUsersInRoleAsync` using EF Core `AnyAsync`.

---

### Magidesk.Presentation

#### [MODIFY] [App.xaml.cs](file:///C:/Users/giris/Documents/Code/Redesign-POS/Windows%20Based%20POS/Magidesk/App.xaml.cs)
- Remove the `RoleCommandHandlers` registrations (lines 118-121) that currently override the canonical registrations in `AddApplication()`.

## Verification Plan

### Automated Tests
- Run `dotnet build` to ensure all interface changes and handler updates are consistent.

### Manual Verification
1.  **Launch App**: Navigate to **Back Office** -> **Users & Roles** -> **Role Management**.
2.  **Uniqueness Test**: Try to create a role with a name that already exists. **Verify**: Error message "Role name already exists" (or similar).
3.  **Deletion Safety Test**: Try to delete a role assigned to "admin" or "operator". **Verify**: Error message "Cannot delete role. It is assigned to one or more users."
4.  **Standard CRUD**: Perform full Create, Update, and Delete on a new test role ("Test Manager").

