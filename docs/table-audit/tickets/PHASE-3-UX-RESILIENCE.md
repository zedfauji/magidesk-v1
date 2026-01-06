# Ticket: Phase 3 - UX & Resilience
**Priority**: P2 (Medium)
**Status**: Pending Phase 2

## Problem Statement
User experience issues create operational risks and frustration:
1.  **Canvas Clipping**: The Table Map is hardcoded to 2000x2000px. Tables placed outside this boundary in the Designer are invisible in the Map.
2.  **Concurrency/Network Jumps**: Dragging a table in the Designer updates the UI immediately. If the network call to save fails, the UI remains in the "new" position while the DB has the "old" position.

## Objectives
1.  Support dynamic or larger floor plans.
2.  Implement robust error handling for position updates.

## Technical Specifications

### 1. Dynamic Canvas Size
**File**: `Magidesk/Views/TableMapPage.xaml`

**Changes**:
-   Bind `Canvas.Width` and `Canvas.Height` to properties in ViewModel (e.g., `MapWidth`, `MapHeight`).
-   **Option A (Simple)**: Increase default to 5000x5000 and wrap in `ScrollViewer` (Done).
-   **Option B (Dynamic)**: Calculate bounding box of all loaded tables + padding.
    -   `MapWidth = Tables.Max(t => t.X + t.Width) + 100`
    -   `MapHeight = Tables.Max(t => t.Y + t.Height) + 100`

### 2. Optimistic Concurrency Rollback
**File**: `Magidesk/ViewModels/TableDesignerViewModel.cs`

**Changes**:
-   In `UpdateTablePositionAsync`:
    -   Capture `oldX`, `oldY` before call.
    -   `try { await _mediator.Send(...); }`
    -   `catch { table.X = oldX; table.Y = oldY; ShowError(); }`
    -   Requires `TableDto` to implement `INotifyPropertyChanged` properly so the View snaps back.

## Acceptance Criteria
-   [ ] **Canvas Test**: Place a table at X=3000 in Designer. Open Map. Scroll to find Table. (Must be visible).
-   [ ] **Rollback Test**: Simulate network failure (throw exception in Handler). Drag table. UI should snap back to original position.
