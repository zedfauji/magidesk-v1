# Backend Tickets: Category B - Floor & Layout Management

> [!NOTE]
> This category has 55.6% parity (5 fully, 3 partial, 1 not implemented). Focus is on completing partial features and fixing gaps.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-B.2-01 | B.2 | Complete Multi-Floor Support | P1 | NOT_STARTED |
| BE-B.3-01 | B.3 | Complete Object Property Editing | P1 | NOT_STARTED |
| BE-B.6-01 | B.6 | Add Floor Switching Validation | P2 | NOT_STARTED |
| BE-B.9-01 | B.9 | Implement Layout Undo/Redo Logic | P2 | NOT_STARTED |

---

## BE-B.2-01: Complete Multi-Floor Support

**Ticket ID:** BE-B.2-01  
**Feature ID:** B.2  
**Type:** Backend  
**Title:** Complete Multi-Floor Support  
**Priority:** P1

### Outcome (measurable, testable)
Full backend support for managing multiple floors with floor-specific table assignments.

### Scope
- Validate Floor entity has all required properties
- Add floor-level aggregate queries (tables per floor, occupancy per floor)
- Add cross-floor table search
- Ensure floor deletion handles table reassignment

### Current State (Partial)
- Floor entity exists
- Tables can be assigned to floors
- **Missing:** Floor-level statistics, cross-floor operations

### Implementation Notes
```csharp
// Add to Floor entity or service
public record FloorStatistics(
    Guid FloorId,
    string FloorName,
    int TotalTables,
    int OccupiedTables,
    int AvailableTables,
    int ReservedTables
);

// New query
public record GetFloorStatisticsQuery(Guid? FloorId);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Floor entity | Exists |
| SOFT | Table entity | Exists |

### Acceptance Criteria
- [ ] Floor statistics calculated correctly
- [ ] Cross-floor table search works
- [ ] Floor deletion reassigns or prevents deletion
- [ ] Tests cover all scenarios

---

## BE-B.3-01: Complete Object Property Editing

**Ticket ID:** BE-B.3-01  
**Feature ID:** B.3  
**Type:** Backend  
**Title:** Complete Object Property Editing  
**Priority:** P1

### Outcome (measurable, testable)
Backend support for editing layout object properties beyond basic positioning.

### Scope
- Add `UpdateTablePropertiesCommand` for extended properties
- Support custom colors, labels, rotation angle
- Validate properties in domain

### Current State (Partial)
- Can update position
- **Missing:** Color, rotation, custom label support

### Implementation Notes
```csharp
public record UpdateTablePropertiesCommand(
    Guid TableId,
    string DisplayName,
    string Color,
    int Capacity,
    double Rotation,
    string IconType,
    bool IsEnabled
);

// Add to Table entity
public void UpdateDisplayProperties(string displayName, string color, double rotation, string iconType);
```

### Acceptance Criteria
- [ ] Command updates all properties
- [ ] Color validated (hex format)
- [ ] Rotation validated (0-359)
- [ ] Changes persisted correctly

---

## BE-B.6-01: Add Floor Switching Validation

**Ticket ID:** BE-B.6-01  
**Feature ID:** B.6  
**Type:** Backend  
**Title:** Add Floor Switching Validation  
**Priority:** P2

### Outcome (measurable, testable)
Backend validation to prevent floor switching when unsaved changes exist.

### Scope
- Create layout change tracking
- Add validation before floor switch
- Support force-switch with discard

### Implementation Notes
```csharp
// This is primarily UI-side tracking, but backend can validate
public record SwitchFloorCommand(
    Guid TargetFloorId,
    bool DiscardUnsavedChanges
);
```

### Acceptance Criteria
- [ ] Switch prevented with unsaved changes
- [ ] Force switch with discard works
- [ ] Saved changes persist correctly

---

## BE-B.9-01: Implement Layout Undo/Redo Logic

**Ticket ID:** BE-B.9-01  
**Feature ID:** B.9  
**Type:** Backend  
**Title:** Implement Layout Undo/Redo Logic  
**Priority:** P2

### Outcome (measurable, testable)
State management for undo/redo operations in layout editing.

### Scope
- Create layout history tracking structure
- Support undo/redo of position changes
- Support undo/redo of add/delete operations
- Limit history depth (20 operations)

### Implementation Notes
```csharp
// This is primarily a ViewModel concern
// Backend provides snapshot/restore capability

public record CreateLayoutSnapshotCommand(
    Guid FloorId,
    string SnapshotName
);

public record RestoreLayoutSnapshotCommand(
    Guid SnapshotId
);
```

### Acceptance Criteria
- [ ] Undo reverses last operation
- [ ] Redo re-applies undone operation
- [ ] History limited to 20 operations
- [ ] Snapshot create/restore works

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 2 | NOT_STARTED |
| P2 | 2 | NOT_STARTED |
| **Total** | **4** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
