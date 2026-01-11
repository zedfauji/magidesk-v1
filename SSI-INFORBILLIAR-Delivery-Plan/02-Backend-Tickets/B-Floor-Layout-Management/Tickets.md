# Backend Tickets: Category B - Floor & Layout Management

> [!NOTE]
> This category has 55.6% parity (5 fully, 3 partial, 1 not implemented). Focus is on completing partial features and fixing gaps.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-B.2-01 | B.2 | Complete Multi-Floor Support | P1 | NOT_STARTED |
| BE-B.3-01 | B.3 | Complete Object Property Editing | P1 | NOT_STARTED |
| BE-B.4-01 | B.4 | Implement Background Grid Overlay | P2 | NOT_STARTED |
| BE-B.6-01 | B.6 | Add Floor Switching Validation | P2 | NOT_STARTED |
| BE-B.9-01 | B.9 | Implement Layout Undo/Redo Logic | P2 | NOT_STARTED |
| BE-B.10-01 | B.10 | Implement Alignment Guides | P2 | NOT_STARTED |
| BE-B.11-01 | B.11 | Add Zoom State Management | P2 | NOT_STARTED |
| BE-B.12-01 | B.12 | Implement Multi-Select Logic | P2 | NOT_STARTED |
| BE-B.13-01 | B.13 | Add Layout Scheduling System | P2 | NOT_STARTED |
| BE-B.14-01 | B.14 | Implement Layout Cloning | P2 | NOT_STARTED |
| BE-B.16-01 | B.16 | Implement Layout Version History | P2 | NOT_STARTED |

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

## BE-B.4-01: Implement Background Grid Overlay

**Ticket ID:** BE-B.4-01  
**Feature ID:** B.4  
**Type:** Backend  
**Title:** Implement Background Grid Overlay  
**Priority:** P2

### Outcome
Backend support for grid overlay configuration on floor layouts.

### Scope
- Add grid configuration to Floor entity (grid size, color, visibility)
- Store grid preferences per floor
- Provide grid snap calculation utilities

### Acceptance Criteria
- [ ] Grid configuration stored per floor
- [ ] Grid snap calculations accurate
- [ ] Tests cover grid logic

---

## BE-B.10-01: Implement Alignment Guides

**Ticket ID:** BE-B.10-01  
**Feature ID:** B.10  
**Type:** Backend  
**Title:** Implement Alignment Guides  
**Priority:** P2

### Outcome
Backend logic for calculating alignment guide positions.

### Scope
- Create alignment calculation service
- Detect edge/center alignment opportunities
- Provide snap-to-guide calculations

### Acceptance Criteria
- [ ] Alignment detection works for edges and centers
- [ ] Snap calculations accurate
- [ ] Performance acceptable for 100+ tables

---

## BE-B.11-01: Add Zoom State Management

**Ticket ID:** BE-B.11-01  
**Feature ID:** B.11  
**Type:** Backend  
**Title:** Add Zoom State Management  
**Priority:** P2

### Outcome
Backend support for persisting zoom/pan state per floor.

### Scope
- Add zoom level and pan offset to Floor or user preferences
- Store last viewed position per floor
- Restore zoom state on floor load

### Acceptance Criteria
- [ ] Zoom state persisted per floor
- [ ] State restored on floor switch
- [ ] Tests verify persistence

---

## BE-B.12-01: Implement Multi-Select Logic

**Ticket ID:** BE-B.12-01  
**Feature ID:** B.12  
**Type:** Backend  
**Title:** Implement Multi-Select Logic  
**Priority:** P2

### Outcome
Backend support for bulk table operations.

### Scope
- Create `UpdateMultipleTablesCommand`
- Support bulk position updates
- Support bulk property updates

### Implementation Notes
```csharp
public record UpdateMultipleTablesCommand(
    List<Guid> TableIds,
    double? DeltaX,
    double? DeltaY,
    string? Color,
    bool? IsEnabled
);
```

### Acceptance Criteria
- [ ] Bulk updates work for position
- [ ] Bulk updates work for properties
- [ ] Transaction ensures all-or-nothing
- [ ] Tests cover bulk operations

---

## BE-B.13-01: Add Layout Scheduling System

**Ticket ID:** BE-B.13-01  
**Feature ID:** B.13  
**Type:** Backend  
**Title:** Add Layout Scheduling System  
**Priority:** P2

### Outcome
Backend system for time-based layout activation.

### Scope
- Create `LayoutSchedule` entity
- Add day-of-week and time-of-day rules
- Implement background service for auto-activation
- Support manual override

### Implementation Notes
```csharp
public class LayoutSchedule
{
    public Guid Id { get; set; }
    public Guid FloorId { get; set; }
    public Guid LayoutId { get; set; }
    public DayOfWeek[] ActiveDays { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Priority { get; set; }
}
```

### Acceptance Criteria
- [ ] Schedules stored correctly
- [ ] Background service activates layouts
- [ ] Manual override works
- [ ] Conflict resolution by priority

---

## BE-B.14-01: Implement Layout Cloning

**Ticket ID:** BE-B.14-01  
**Feature ID:** B.14  
**Type:** Backend  
**Title:** Implement Layout Cloning  
**Priority:** P2

### Outcome
Backend command to duplicate layouts with all tables.

### Scope
- Create `CloneLayoutCommand`
- Deep copy all tables and properties
- Generate new IDs for cloned entities
- Preserve relative positions

### Implementation Notes
```csharp
public record CloneLayoutCommand(
    Guid SourceLayoutId,
    string NewLayoutName
);
```

### Acceptance Criteria
- [ ] Layout cloned with all tables
- [ ] New IDs generated
- [ ] Positions preserved
- [ ] Tests verify deep copy

---

## BE-B.16-01: Implement Layout Version History

**Ticket ID:** BE-B.16-01  
**Feature ID:** B.16  
**Type:** Backend  
**Title:** Implement Layout Version History  
**Priority:** P2

### Outcome
Backend system for layout versioning and rollback.

### Scope
- Create `LayoutVersion` entity
- Auto-save versions on publish
- Implement `RestoreLayoutVersionCommand`
- Limit history to 10 versions per layout

### Implementation Notes
```csharp
public class LayoutVersion
{
    public Guid Id { get; set; }
    public Guid LayoutId { get; set; }
    public int VersionNumber { get; set; }
    public string SnapshotData { get; set; } // JSON
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
```

### Acceptance Criteria
- [ ] Versions saved on publish
- [ ] Restore command works
- [ ] History limited to 10 versions
- [ ] Old versions auto-purged

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 2 | NOT_STARTED |
| P2 | 9 | NOT_STARTED |
| **Total** | **11** | **NOT_STARTED** |

---

*Last Updated: 2026-01-09*
