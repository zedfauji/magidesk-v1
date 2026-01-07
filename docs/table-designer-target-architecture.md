# Table Layout Designer - TARGET UI ARCHITECTURE
## Phase 2: Authoritative Design Specification

**Design Date**: 2026-01-06  
**Status**: AUTHORITATIVE SPECIFICATION  
**Based On**: Phase 1 Forensic Audit Findings  
**Objective**: Enterprise-grade table layout designer with zero ambiguity

---

## DESIGN PRINCIPLES (NON-NEGOTIABLE)

1. **Explicit Over Implicit** - Every action must be explicit and visible
2. **Safety First** - Undo, dirty checks, confirmations for destructive actions
3. **Domain Fidelity** - UI must match Floor → Layout → Tables hierarchy
4. **Professional Grade** - Match industry standards (Figma, Adobe XD, AutoCAD)
5. **Zero Ambiguity** - Operator always knows: what, where, and state

---

## 1. PAGE LAYOUT STRUCTURE

### Master Layout (3-Panel Design)

```
┌─────────────────────────────────────────────────────────────┐
│ HEADER BAR (60px)                                           │
│ [Floor: Main Dining ▼] [Layout: Lunch Setup ▼] [●Draft]    │
│ [New Layout] [Clone] [Delete] | [Save] [Publish] [Revert]  │
└─────────────────────────────────────────────────────────────┘
┌──────────┬────────────────────────────────────┬─────────────┐
│          │                                    │             │
│  LEFT    │         CANVAS AREA                │    RIGHT    │
│  PANEL   │                                    │    PANEL    │
│  (280px) │                                    │   (320px)   │
│          │                                    │             │
│  Tools   │    [Table Layout Canvas]          │ Properties  │
│  Shapes  │                                    │             │
│  Layers  │    Drag, Drop, Resize              │ Selection   │
│          │    Zoom, Pan, Grid                 │ Details     │
│          │                                    │             │
│          │                                    │             │
└──────────┴────────────────────────────────────┴─────────────┘
┌─────────────────────────────────────────────────────────────┐
│ STATUS BAR (40px)                                           │
│ Tables: 25 | Selected: 3 | Zoom: 100% | Grid: ON | ●Unsaved│
└─────────────────────────────────────────────────────────────┘
```

### Responsive Behavior
- **Minimum Width**: 1280px (professional designer standard)
- **Panels Collapsible**: Left/Right panels can collapse to icons
- **Canvas Priority**: Canvas always gets remaining space

---

## 2. HEADER BAR (CONTEXT & ACTIONS)

### 2.1 Context Selectors (Left Side)

#### Floor Selector (Primary Context)
```
┌────────────────────────────┐
│ Floor: Main Dining Room ▼  │
├────────────────────────────┤
│ ✓ Main Dining Room         │
│   Patio                    │
│   Bar Area                 │
│   Private Room A           │
├────────────────────────────┤
│ + Create New Floor...      │
│ ⚙ Manage Floors...         │
└────────────────────────────┘
```

**Behavior**:
- Shows current floor name
- Dropdown lists all floors
- Checkmark on active floor
- "Create New Floor" opens dialog
- "Manage Floors" opens floor management page

**Dirty State Protection**:
```
IF layout has unsaved changes THEN
    Show dialog: "Save changes to 'Lunch Setup' before switching floors?"
    [Save & Switch] [Discard & Switch] [Cancel]
END IF
```

#### Layout Selector (Secondary Context)
```
┌──────────────────────────────────┐
│ Layout: Lunch Setup ▼            │
├──────────────────────────────────┤
│ ✓ Lunch Setup          ●Draft    │
│   Dinner Setup         ✓Active   │
│   Weekend Brunch       ●Draft    │
│   Holiday Special      ✓Active   │
├──────────────────────────────────┤
│ + New Layout                     │
│ 📋 Clone Current Layout          │
│ 📁 Browse All Layouts...         │
└──────────────────────────────────┘
```

**Behavior**:
- Shows current layout name
- Lists all layouts for selected floor
- Icons: ●Draft, ✓Active (published)
- Sorted: Active first, then Draft, then by name
- "New Layout" creates blank layout
- "Clone Current" duplicates current layout
- "Browse All" opens layout library

**Dirty State Protection**: Same as Floor selector

#### Draft/Published Indicator
```
┌─────────────┐
│ ● DRAFT     │  ← Orange badge, prominent
└─────────────┘

┌─────────────┐
│ ✓ ACTIVE    │  ← Green badge, published
└─────────────┘
```

**Behavior**:
- Always visible
- Color-coded (Orange = Draft, Green = Active)
- Tooltip: "This layout is in draft mode. Publish to make it active."

---

### 2.2 Action Buttons (Right Side)

#### Layout Lifecycle Actions
```
[New Layout] [Clone] [Rename] [Delete]
```

**New Layout**:
- Opens dialog: "Create New Layout"
- Fields: Name, Description, Clone From (optional)
- Validates: Name uniqueness per floor
- Creates in Draft mode by default

**Clone**:
- Duplicates current layout
- Auto-names: "{Original Name} Copy"
- Opens rename dialog immediately
- Inherits all tables with positions

**Rename**:
- Inline edit or dialog
- Validates uniqueness
- Updates immediately (no save needed)

**Delete**:
- Confirmation: "Delete layout '{Name}'? This cannot be undone."
- Disabled if layout is Active (must deactivate first)
- Removes from database immediately

#### Save/Publish Actions
```
[Save Draft] [Publish] [Revert]
```

**Save Draft**:
- Saves current state as draft
- Does NOT activate layout
- Shows toast: "Draft saved successfully"
- Clears dirty state

**Publish**:
- Confirmation: "Publish '{Name}' as active layout?"
- Deactivates other layouts on same floor
- Validates: Must have at least 1 table
- Shows toast: "Layout published and activated"

**Revert**:
- Confirmation: "Discard all changes since last save?"
- Reloads from database
- Clears dirty state

---

## 3. LEFT PANEL (TOOLS & SHAPES)

### 3.1 Tool Palette
```
┌─────────────────────────┐
│ TOOLS                   │
├─────────────────────────┤
│ ◉ Select (V)            │ ← Default
│ ○ Pan (H)               │
│ ○ Add Table (T)         │
│ ○ Measure (M)           │
└─────────────────────────┘
```

**Keyboard Shortcuts**:
- `V` = Select tool
- `H` = Pan/Hand tool
- `T` = Add table tool
- `M` = Measure tool
- `Space` = Temporary pan (hold)

---

### 3.2 Shape Palette
```
┌─────────────────────────┐
│ TABLE SHAPES            │
├─────────────────────────┤
│ ┌─┐ Rectangle (R)       │
│ ┌─┐ Square (S)          │
│ ( ) Round (C)           │
│ (─) Oval (O)            │
│ ◇   Diamond (D)         │
│ ⬡   Hexagon (H)         │
└─────────────────────────┘
```

**Behavior**:
- Click to select shape
- Next table added uses selected shape
- Keyboard shortcuts for quick access
- Visual preview on hover

---

### 3.3 Layers Panel
```
┌─────────────────────────────┐
│ LAYERS                      │
├─────────────────────────────┤
│ 👁 Table 1 (4-seat)         │
│ 👁 Table 2 (6-seat)         │
│ 👁 Table 3 (2-seat)         │
│ 👁 Table 4 (8-seat)         │
├─────────────────────────────┤
│ [Lock All] [Unlock All]     │
└─────────────────────────────┘
```

**Features**:
- List of all tables in layout
- Eye icon = visibility toggle
- Lock icon = prevent editing
- Click to select table
- Drag to reorder (z-index)

---

## 4. CANVAS AREA (DESIGN SURFACE)

### 4.1 Canvas Controls (Top Toolbar)
```
┌────────────────────────────────────────────────────────┐
│ [-] [100%] [+] | [Grid ✓] [Snap ✓] | [Undo] [Redo]   │
└────────────────────────────────────────────────────────┘
```

**Zoom Controls**:
- `-` = Zoom out (Ctrl + -)
- `100%` = Reset to 100% (Ctrl + 0)
- `+` = Zoom in (Ctrl + +)
- Dropdown: 25%, 50%, 75%, 100%, 150%, 200%, Fit

**Grid & Snap**:
- `Grid` = Toggle grid visibility
- `Snap` = Toggle snap-to-grid (default: ON, 50px grid)
- Settings: Grid size, snap threshold

**Undo/Redo**:
- `Undo` = Ctrl + Z (up to 50 actions)
- `Redo` = Ctrl + Y or Ctrl + Shift + Z
- Disabled when stack empty

---

### 4.2 Canvas Background
```
┌────────────────────────────────────────┐
│ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ │ ← Grid overlay
│ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ │
│ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ │
│                                        │
│   [Tables rendered here]               │
│                                        │
└────────────────────────────────────────┘
```

**Properties**:
- Size: From Floor.Width × Floor.Height
- Background: Floor.BackgroundColor
- Grid: 50px × 50px (configurable)
- Rulers: Top and left (optional)

---

### 4.3 Table Rendering

#### Normal State
```
┌──────────────┐
│   Table 5    │ ← Table number (large, bold)
│   (4 seats)  │ ← Capacity (small, gray)
└──────────────┘
```

#### Selected State
```
┌══════════════┐ ← Blue border (3px)
║   Table 5    ║
║   (4 seats)  ║
║  ◉  ◉  ◉  ◉  ║ ← Resize handles (8 points)
└══════════════┘
```

#### Multi-Select State
```
┌──────────────┐ ← Blue border (2px)
│   Table 5    │
│   (4 seats)  │
└──────────────┘
```

#### Locked State
```
┌──────────────┐
│   Table 5 🔒 │ ← Lock icon
│   (4 seats)  │
└──────────────┘
```

#### Occupied State (Read-Only Mode)
```
┌──────────────┐
│   Table 5    │ ← Red background
│   OCCUPIED   │ ← Status text
└──────────────┘
```

---

### 4.4 Interaction Behaviors

#### Single Select
- **Click**: Select table
- **Drag**: Move table
- **Resize Handles**: Resize table
- **Double-Click**: Edit properties (opens right panel)

#### Multi-Select
- **Ctrl + Click**: Add to selection
- **Shift + Click**: Range select (by creation order)
- **Drag Selection Box**: Lasso select
- **Group Drag**: Move all selected tables together

#### Keyboard Operations
- **Arrow Keys**: Nudge 1px (or grid size if snap enabled)
- **Shift + Arrow**: Nudge 10px
- **Delete**: Delete selected tables (with confirmation)
- **Ctrl + C**: Copy selected tables
- **Ctrl + V**: Paste tables (offset by 20px)
- **Ctrl + D**: Duplicate selected tables
- **Ctrl + A**: Select all tables

#### Alignment (Multi-Select)
- **Align Left**: Align left edges
- **Align Center**: Align horizontal centers
- **Align Right**: Align right edges
- **Align Top**: Align top edges
- **Align Middle**: Align vertical centers
- **Align Bottom**: Align bottom edges
- **Distribute Horizontally**: Even spacing
- **Distribute Vertically**: Even spacing

---

### 4.5 Validation & Feedback

#### Real-Time Validation
```
┌──────────────┐
│   Table 5    │ ← Red border = Invalid
│   (4 seats)  │
└──────────────┘
  ⚠ Overlaps with Table 3
```

**Validation Rules**:
- ❌ Overlapping tables (50px minimum spacing)
- ❌ Out of bounds (must be fully inside canvas)
- ❌ Duplicate table numbers
- ❌ Invalid capacity (must be > 0)

**Visual Indicators**:
- Red border = Invalid
- Yellow border = Warning
- Tooltip shows error message
- Cannot save if validation errors exist

---

## 5. RIGHT PANEL (PROPERTIES)

### 5.1 No Selection State
```
┌─────────────────────────────┐
│ PROPERTIES                  │
├─────────────────────────────┤
│                             │
│   No table selected         │
│                             │
│   Click a table to edit     │
│   its properties            │
│                             │
└─────────────────────────────┘
```

---

### 5.2 Single Selection State
```
┌─────────────────────────────┐
│ TABLE PROPERTIES            │
├─────────────────────────────┤
│ Table Number: [5      ]     │
│ Capacity:     [4      ] ▲▼  │
│ Shape:        [Rectangle ▼] │
│                             │
│ POSITION & SIZE             │
│ X: [150  ] Y: [200  ]       │
│ W: [100  ] H: [100  ]       │
│                             │
│ APPEARANCE                  │
│ Color:  [🎨 #3498db]        │
│ Border: [2px    ] ▲▼        │
│                             │
│ METADATA                    │
│ Server Section: [A    ▼]    │
│ Notes: [                 ]  │
│        [                 ]  │
│                             │
│ [Apply] [Reset]             │
└─────────────────────────────┘
```

**Behavior**:
- Live preview on canvas as you type
- `Apply` = Commit changes
- `Reset` = Revert to original values
- Validation: Table number must be unique

---

### 5.3 Multi-Selection State
```
┌─────────────────────────────┐
│ MULTI-SELECT (3 tables)     │
├─────────────────────────────┤
│ Capacity:     [Mixed   ] ▲▼ │
│ Shape:        [Mixed    ▼]  │
│                             │
│ BULK ACTIONS                │
│ [Set Capacity...]           │
│ [Change Shape...]           │
│ [Set Server Section...]     │
│                             │
│ ALIGNMENT                   │
│ [←] [↔] [→]                 │
│ [↑] [↕] [↓]                 │
│                             │
│ DISTRIBUTION                │
│ [Horizontal] [Vertical]     │
│                             │
│ [Delete All]                │
└─────────────────────────────┘
```

**Behavior**:
- Shows "Mixed" for properties that differ
- Bulk actions apply to all selected tables
- Alignment buttons use icons
- Delete requires confirmation

---

## 6. STATUS BAR (FEEDBACK)

```
┌────────────────────────────────────────────────────────────┐
│ Tables: 25 | Selected: 3 | Zoom: 100% | Grid: ON | ●Unsaved│
└────────────────────────────────────────────────────────────┘
```

**Information Displayed**:
- **Tables**: Total count
- **Selected**: Count of selected tables
- **Zoom**: Current zoom level
- **Grid**: Grid on/off status
- **Dirty State**: ●Unsaved (orange dot) or ✓Saved (green check)

**Behavior**:
- Updates in real-time
- Dirty state appears immediately on any change
- Click "Unsaved" to see unsaved changes list

---

## 7. LAYOUT LIFECYCLE WORKFLOWS

### 7.1 Create New Layout
```
User clicks "New Layout"
  ↓
Dialog: "Create New Layout"
  - Name: [____________]
  - Description: [____________]
  - Clone from: [None ▼] (optional)
  - Start as: ○ Blank  ● Clone
  ↓
[Create] clicked
  ↓
Validation:
  - Name not empty
  - Name unique for floor
  ↓
Create layout in Draft mode
  ↓
Switch to new layout
  ↓
Canvas shows blank or cloned tables
```

---

### 7.2 Edit Existing Layout
```
User selects layout from dropdown
  ↓
IF current layout has unsaved changes THEN
    Show save prompt
END IF
  ↓
Load selected layout
  ↓
Update canvas with tables
  ↓
Update properties panel
  ↓
Update header (layout name, draft/active status)
```

---

### 7.3 Save Draft
```
User clicks "Save Draft"
  ↓
Validation:
  - Layout name not empty
  - At least 1 table
  - No validation errors
  ↓
Save to database
  ↓
Clear dirty state
  ↓
Show toast: "Draft saved"
```

---

### 7.4 Publish Layout
```
User clicks "Publish"
  ↓
Validation:
  - Layout name not empty
  - At least 1 table
  - No validation errors
  ↓
Confirmation dialog:
  "Publish 'Lunch Setup' as active layout?
   This will deactivate 'Dinner Setup'."
  [Publish] [Cancel]
  ↓
Deactivate other layouts on floor
  ↓
Activate current layout
  ↓
Save to database
  ↓
Update badge: ● DRAFT → ✓ ACTIVE
  ↓
Show toast: "Layout published"
```

---

### 7.5 Delete Layout
```
User clicks "Delete"
  ↓
IF layout is Active THEN
    Error: "Cannot delete active layout. Deactivate first."
    STOP
END IF
  ↓
Confirmation dialog:
  "Delete layout 'Weekend Brunch'?
   This action cannot be undone."
  [Delete] [Cancel]
  ↓
Delete from database
  ↓
Switch to first available layout
  ↓
Show toast: "Layout deleted"
```

---

## 8. FLOOR LIFECYCLE WORKFLOWS

### 8.1 Create New Floor
```
User clicks "Create New Floor" in dropdown
  ↓
Dialog: "Create New Floor"
  - Name: [____________]
  - Description: [____________]
  - Width: [2000] px
  - Height: [2000] px
  - Background: [🎨 #f8f8f8]
  ↓
[Create] clicked
  ↓
Validation:
  - Name not empty
  - Name unique
  - Dimensions > 0
  ↓
Create floor in database
  ↓
Add to floor list
  ↓
Switch to new floor
  ↓
Create default "Main Layout" (blank)
```

---

### 8.2 Manage Floors
```
User clicks "Manage Floors" in dropdown
  ↓
Navigate to Floor Management page
  ↓
Show list of floors:
  - Name
  - Dimensions
  - Layout count
  - Active/Inactive
  ↓
Actions:
  - Edit floor properties
  - Delete floor (if no layouts)
  - Activate/Deactivate
```

---

## 9. SAFETY MECHANISMS

### 9.1 Dirty State Detection
```
Track changes:
  - Table added
  - Table deleted
  - Table moved
  - Table resized
  - Table properties changed
  - Layout name changed
  ↓
Set IsDirty = true
  ↓
Show ●Unsaved in status bar
  ↓
Enable "Save Draft" button
```

---

### 9.2 Navigation Guards
```
User attempts to:
  - Change floor
  - Change layout
  - Navigate away from page
  - Close window
  ↓
IF IsDirty THEN
    Dialog: "Save changes to 'Lunch Setup'?"
    [Save & Continue] [Discard] [Cancel]
    ↓
    IF Save THEN
        Save layout
        Continue action
    ELSE IF Discard THEN
        Discard changes
        Continue action
    ELSE
        Cancel action
    END IF
END IF
```

---

### 9.3 Undo/Redo Stack
```
Action performed:
  - Add table
  - Delete table
  - Move table
  - Resize table
  - Change property
  ↓
Create undo snapshot:
  - Action type
  - Before state
  - After state
  ↓
Push to undo stack (max 50)
  ↓
Clear redo stack
  ↓
Enable "Undo" button
```

**Undo Operation**:
```
User presses Ctrl+Z
  ↓
Pop from undo stack
  ↓
Restore before state
  ↓
Push to redo stack
  ↓
Update canvas
```

---

### 9.4 Validation Feedback
```
User performs action
  ↓
Run validation:
  - Table overlap check
  - Bounds check
  - Duplicate number check
  ↓
IF invalid THEN
    Show red border on table
    Show tooltip with error
    Add to validation errors list
    Disable "Save" and "Publish"
ELSE
    Clear validation errors
    Enable "Save" and "Publish"
END IF
```

---

## 10. KEYBOARD SHORTCUTS (COMPLETE)

### Navigation
- `V` = Select tool
- `H` = Pan tool
- `T` = Add table tool
- `Space` (hold) = Temporary pan

### Editing
- `Delete` = Delete selected tables
- `Ctrl + C` = Copy
- `Ctrl + V` = Paste
- `Ctrl + D` = Duplicate
- `Ctrl + A` = Select all
- `Ctrl + Z` = Undo
- `Ctrl + Y` = Redo
- `Ctrl + Shift + Z` = Redo (alternate)

### Movement
- `Arrow Keys` = Nudge 1px (or grid size)
- `Shift + Arrow` = Nudge 10px

### View
- `Ctrl + +` = Zoom in
- `Ctrl + -` = Zoom out
- `Ctrl + 0` = Reset zoom to 100%
- `Ctrl + G` = Toggle grid
- `Ctrl + ;` = Toggle snap

### Shapes (when Add Table tool active)
- `R` = Rectangle
- `S` = Square
- `C` = Circle/Round
- `O` = Oval
- `D` = Diamond
- `H` = Hexagon

### Save/Publish
- `Ctrl + S` = Save draft
- `Ctrl + Shift + S` = Publish
- `Ctrl + R` = Revert changes

---

## 11. RESPONSIVE BEHAVIOR

### Panel Collapse
```
Window width < 1600px:
  - Collapse right panel to icons
  - Show properties on hover/click

Window width < 1280px:
  - Show warning: "Minimum width 1280px required"
  - Disable editing
  - Show read-only mode
```

### Touch Support
- **Pinch to Zoom**: Two-finger pinch
- **Pan**: Two-finger drag
- **Select**: Single tap
- **Multi-Select**: Long press + tap
- **Move**: Drag table
- **Resize**: Drag corner handles (larger hit area)

---

## 12. ACCESSIBILITY (WCAG 2.1 AA)

### Keyboard Navigation
- All actions accessible via keyboard
- Tab order: Header → Left Panel → Canvas → Right Panel → Status
- Focus indicators visible (2px blue outline)
- Escape key cancels dialogs

### Screen Reader Support
- ARIA labels on all interactive elements
- Live regions for status updates
- Table count announced on change
- Validation errors announced

### Color Contrast
- Text: 4.5:1 minimum
- Interactive elements: 3:1 minimum
- Error states: Red + icon (not color alone)
- Draft/Active: Orange/Green + text label

---

## 13. PERFORMANCE REQUIREMENTS

### Rendering
- **60 FPS** for drag operations
- **Virtualization** for > 100 tables
- **Debounced** validation (300ms)
- **Throttled** canvas updates (16ms)

### Load Times
- **< 500ms** to load layout with 50 tables
- **< 1s** to load layout with 200 tables
- **< 100ms** to switch layouts (same floor)
- **< 300ms** to switch floors

### Memory
- **< 50MB** for typical layout (50 tables)
- **< 200MB** for large layout (500 tables)
- **Cleanup** on layout switch (dispose old canvas)

---

## 14. ERROR HANDLING

### Network Errors
```
Save fails due to network error
  ↓
Show error banner:
  "Failed to save layout. Check connection."
  [Retry] [Save Locally]
  ↓
Keep dirty state
  ↓
Auto-retry every 30s
```

### Validation Errors
```
User clicks "Save" with validation errors
  ↓
Show error dialog:
  "Cannot save layout with errors:
   - Table 5 overlaps with Table 3
   - Table 8 is out of bounds"
  [Fix Errors] [Cancel]
  ↓
Highlight invalid tables in red
  ↓
Scroll to first error
```

### Concurrency Errors
```
Save fails due to concurrent modification
  ↓
Show error dialog:
  "Layout was modified by another user.
   Your changes cannot be saved."
  [Reload & Merge] [Discard] [Save As Copy]
```

---

## 15. WINUI 3 CONTROL MAPPING

### Header Bar
- `Grid` with `ColumnDefinitions`
- `ComboBox` for Floor/Layout selectors
- `Button` for actions
- `InfoBadge` for Draft/Active indicator

### Left Panel
- `NavigationView` (compact mode) for tools
- `GridView` for shape palette
- `ListView` for layers

### Canvas
- `ScrollViewer` with `Canvas` inside
- `ItemsControl` with `Canvas` as `ItemsPanel`
- Custom `UserControl` for table rendering
- `ManipulationDelta` for drag
- `PointerPressed` for selection

### Right Panel
- `PropertyGrid` (custom control) or `StackPanel` with controls
- `TextBox`, `NumberBox`, `ComboBox` for properties
- `ColorPicker` for color selection

### Status Bar
- `Grid` with `TextBlock` elements
- `ProgressRing` for busy indicator

### Dialogs
- `ContentDialog` for confirmations
- `Flyout` for quick actions
- `TeachingTip` for hints

---

## 16. MVVM BOUNDARIES

### View Responsibilities (XAML + Code-Behind)
- Rendering tables on canvas
- Handling pointer events (drag, click)
- Managing selection visuals
- Zoom/pan gestures
- Focus management
- Accessibility attributes

### ViewModel Responsibilities
- Floor/Layout data management
- Table collection (ObservableCollection)
- Dirty state tracking
- Undo/redo stack
- Validation logic
- Save/Load commands
- Business rule enforcement

### What SHOULD NOT Be in Code-Behind
- Business logic
- Data persistence
- Validation rules
- State management
- API calls

### What CAN Be in Code-Behind
- UI-specific event handlers (drag, drop)
- Animation triggers
- Focus management
- Scroll position management
- Canvas coordinate calculations

---

## 17. IMPLEMENTATION PHASES

### Phase 1: Core Structure (Week 1)
- [ ] Header bar with Floor/Layout selectors
- [ ] 3-panel layout
- [ ] Basic canvas rendering
- [ ] Table drag & drop
- [ ] Save/Load basic functionality

### Phase 2: Professional Features (Week 2)
- [ ] Undo/redo
- [ ] Multi-select
- [ ] Snap-to-grid
- [ ] Zoom/pan
- [ ] Properties panel
- [ ] Keyboard shortcuts

### Phase 3: Safety & Polish (Week 3)
- [ ] Dirty state detection
- [ ] Navigation guards
- [ ] Validation feedback
- [ ] Error handling
- [ ] Accessibility
- [ ] Performance optimization

### Phase 4: Advanced Features (Week 4)
- [ ] Alignment tools
- [ ] Copy/paste
- [ ] Layers panel
- [ ] Export/import
- [ ] Touch support
- [ ] Responsive behavior

---

## CONCLUSION

This architecture provides:
- ✅ **Clear Floor/Layout separation** - Explicit hierarchy
- ✅ **Professional designer features** - Undo, multi-select, snap, zoom
- ✅ **Safety mechanisms** - Dirty checks, validation, confirmations
- ✅ **Zero ambiguity** - Operator always knows state
- ✅ **Enterprise-grade UX** - Matches industry standards
- ✅ **Scalable** - Supports many floors, many layouts
- ✅ **Accessible** - WCAG 2.1 AA compliant
- ✅ **Performant** - 60 FPS, virtualization

**Next Phase**: Interaction Specification & WinUI 3 Implementation Details

---

**Status**: ARCHITECTURE COMPLETE ✅  
**Ready For**: Implementation Planning
