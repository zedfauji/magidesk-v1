# Designer vs. Runtime Separation Rules

To prevent data corruption and ensure POS stability, a strict boundary is enforced between the **Table Designer (Design-Time)** and the **Table Map (Runtime)**.

## 1. Governance Rules

| Scope | Role | Authorization | Data Access |
|-------|------|---------------|-------------|
| **Table Map** | Operator / Server | All Staff | **Read-Only** Layout. No geometry edits. |
| **Table Designer** | Administrator | Admin Only | **Read/Write** Drafts and Versions. |

## 2. Operational Separation

### Table Map (Runtime)
- **Source**: Must only load `IsActive = true` layouts.
- **Operations**: Seat Table, Change Table, Pay Ticket.
- **Geometry**: Coordinates and shapes are absolute; the UI cannot move tables.
- **Error Handling**: Missing coordinates must result in a default "Overflow List" rather than a crash.

### Table Designer (Design-Time)
- **Work Area**: Operates on a **Draft Layout**. 
- **Persistence**: Edits are NOT live until "Save/Publish" is triggered.
- **Collision Logic**: Must prevent overlapping table numbers and invalid (negative) coordinates.
- **Conflict Resolution**: If a table is currently `Occupied` at runtime, its `Id` cannot be changed or deleted in the designer without an explicit warning and "Force Clear" action.

## 3. The "Save" Barrier

- Runtime (Map) must NEVER call commands like `UpdateTablePositionCommand`.
- Design-time changes MUST be batch-committed or saved as a new `Version` of the `TableLayout`, never as atomic, unversioned coordinate updates on the live entity.
