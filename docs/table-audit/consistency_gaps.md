# Table System Consistency Gaps

## 1. Table Visibility Logic
| View | Query Method | Filter Logic | Result |
|------|-------------|--------------|--------|
| **Table Map** | `GetActiveAsync` | `IsActive == true` | Shows ALL tables (Available, Seat, Dirty). |
| **Table Designer** | `GetAvailableAsync` | `Status == Available` | **HIDES Occupied/Dirty tables.** |
| **Table Explorer** | `GetAvailableAsync` | `Status == Available` | **HIDES Occupied/Dirty tables.** |

### Critical Impact
- **Designer**: If a table is currently occupied, the manager cannot move it or edit its shape. It appears as if the table was deleted. Risk of creating duplicates.
- **Explorer**: The search/list view cannot be used to find an open ticket (e.g., "Find Table 5's ticket"). The `SelectTable` method contains logic to resume tickets (`Status == Seat`), but this code is **unreachable** because `Seat` tables are filtered out by the query.

## 2. Ticket Creation Logic
- **Map**: Explicitly orchestrates generic ticket creation (User/Terminal/Shift/OrderType checks).
- **Explorer**: Navigates to `OrderEntry` with `null` ticket ID. Does not create ticket.
- **Result**: Inconsistent user experience. Clicking a table in Map starts a ticket. Clicking a table in Explorer might land on a blank screen or rely on OrderEntry to handle creation (implicit vs explicit).

## 3. Canvas & Coordinate System
- **Designer**:
  - Viewport: Dynamic, scrollable.
  - Bounds: 0..2000 (Checked in VM).
  - Shape: Dynamic (Round, Square, etc.).
- **Map**:
  - Viewport: Fixed 2000x2000 in XAML.
  - **Risk**: Tables placed > 2000px in Designer will be invisible on Map.

## 4. State Synchronization
- **Map**: Polls for `Status` updates. **Ignores** position/shape/number changes.
- **Designer**: No polling. Stale data if multiple designers active (Unlikely but possible).
- **Explorer**: No polling. Snapshot only.

## 5. Naming Convention
- **Tables**: `TableNumber` (Int) is the identifier.
- **Display**: "Table {0}" in Explorer.
- **Search**: String match on Table Number.
- **Consistency**: Appears consistent.
