# Table Designer Rebuild Tickets

The following tasks are required to recover the Table Designer as a safe, parity-compliant admin tool.

## 1. Backend / Contract Alignment

### [T-DR-001] Coordinate Type Unification
- **Goal**: Align Domain `Table` entity and `TableDto` for coordinate precision.
- **Action**: Change Domain `X`, `Y`, `Width`, `Height` to `double` OR enforce strict integer rounding in the Application Mapping layer. 
- **Priority**: High (Prevents data drift).

### [T-DR-002] Layout Versioning Logic
- **Goal**: Implement "Draft" states for TableLayouts.
- **Action**: Add `IsDraft` field to `TableLayout`. Ensure Designer only edits Drafts.
- **Priority**: Critical.

## 2. Frontend Rebuild (Admin UX)

### [T-DR-003] Admin-Only Access Guard
- **Goal**: Enforce `ManageTableLayout` permission.
- **Action**: Use `IUserService` and `UserPermission` flags to show/hide Designer entry points (replace current hard-freeze comments).
- **Priority**: High.

### [T-DR-004] Design Mode State machine
- **Goal**: Implement the workflow defined in `designer_workflow.md`.
- **Action**: Add `DesignState` enum to `TableDesignerViewModel` (Idle, Designing, Configuring).
- **Priority**: Medium.

### [T-DR-005] Live Table Lock
- **Goal**: Prevent moving occupied tables.
- **Action**: Add a `IsLocked` visual state to tables in the designer if `Status != Available`.
- **Priority**: Medium.

## 3. Type Safety & Validation

### [T-DR-006] Mapping Layer Enforcement
- **Goal**: Centralize UI <-> Domain mapping.
- **Action**: Implement explicit Mapping classes. Remove ad-hoc conversions in ViewModels.
- **Priority**: High.
