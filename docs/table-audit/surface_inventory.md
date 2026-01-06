# Table System Surface Inventory

## 1. Table Designer
**File**: `Views/TableDesignerPage.xaml` / `ViewModels/TableDesignerViewModel.cs`

### UI Controls
- **Toggle**: Design Mode (Enable/Disable editing).
- **Button**: Add Table (Adds new table to center/click pos).
- **Button**: Delete Table (Removes selected).
- **Button**: Save Layout (Persists to DB).
- **Button**: Reload Layout (Reverts to DB state).
- **Input**: Layout Name (TextBox).
- **Input**: Floor Selector (ComboBox).
- **Input**: Shape Selector (`TableShapePalette`).
- **Canvas**: Interactive area for placing tables.

### Hidden / Implicit Behaviors
- **Drag & Drop**:
  - Implemented in **Code-Behind** (`TableDesignerPage.xaml.cs`).
  - Directly mutates `TableDto.X/Y` properties.
  - Does **not** trigger ViewModel command until potentially later or never (relies on dirty checking/Save).
  - Use of `e.GetPosition(null)` might yield incorrect coordinates if scrolled.
- **Virtualization**:
  - ViewModel has `VisibleTables` logic.
  - **MISMATCH**: XAML binds to `Tables` (full list), bypassing virtualization.
- **Boundaries**: Hardcoded checks in VM (0..2000).

## 2. Table Layout (Map)
**File**: `Views/TableMapPage.xaml` / `ViewModels/TableMapViewModel.cs`

### UI Controls
- **Toggle**: Real-time Updates (Polling).
- **Button**: Refresh (Manual).
- **Canvas**:
  - **FIXED SIZE**: Hardcoded `2000x2000` in XAML.
  - Displays all tables as `Buttons`.
- **Interactions**:
  - Click Table: Selects table. Trigger logic depends on state.

### Hidden / Implicit Behaviors
- **Ticket Creation Logic**:
  - **EMBEDDED**: `TableMapViewModel` contains full business logic for:
    - User/Terminal Context validation.
    - Shift/Session validation.
    - Order Type lookup ("DINE IN").
    - `CreateTicketCommand` dispatch.
  - This logic is **not shared** with Explorer.
- **Context Awareness**:
  - Shifts behavior based on `SourceTicketId` (Move Table vs New Ticket).
- **Real-time Polling**:
  - Uses `DispatcherQueue` to update generic List properties.
  - Only updates `Status` and `CurrentTicketId` (Position/Shape changes ignored).

## 3. Table Explorer
**File**: `Views/TableExplorerPage.xaml` / `ViewModels/TableExplorerViewModel.cs`

### UI Controls
- **Input**: Search Box (Filters by Table Number).
- **List**: Vertical list of tables.
- **Button**: Select (Navigate).

### Hidden / Implicit Behaviors
- **Navigation Logic**:
  - **INCONSISTENT**: If table is Available, it navigates to `OrderEntryPage` with `null` TicketId.
  - Does **not** perform the checks (Shift/Terminal) that Map does.
  - Does **not** create a ticket.
  - Relies on `OrderEntryPage` to handle null/creation? (Unknown/Implied risk).

## 4. Shared Domain
- **DTOs**: `TableDto` (Mutable X/Y used in UI).
- **Enums**: `TableStatus` (Available, Seat, etc.).

## 5. Critical Findings (Phase 1)
1.  **Canvas Size Mismatch**: Designer (Dynamic) vs Map (Fixed 2000).
2.  **Virtualization Mismatch**: Designer VM implements it, View ignores it.
3.  **Coordinate System**: Code-behind uses `GetPosition(null)`.
4.  **Business Logic Fragmentation**: Ticket creation logic lives inside `TableMapViewModel`, missing in `TableExplorerViewModel`.
