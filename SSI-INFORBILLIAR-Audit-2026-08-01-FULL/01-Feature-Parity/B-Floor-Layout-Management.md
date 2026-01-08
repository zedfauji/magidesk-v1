# Category B: Floor & Layout Management (Physical Space)

## B.1 Floor / room definitions (e.g., Main Hall, VIP, Patio)

**Feature ID:** B.1  
**Feature Name:** Floor / room definitions  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Floors` (Id, Name, Description, Width, Height, BackgroundColor, IsActive)
- Domain entities: `Floor.cs`
- Services: `FloorRepository.cs`
- APIs / handlers: Floor CRUD operations
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `FloorManagementPage.xaml`
- ViewModels: `FloorManagementViewModel.cs`
- Navigation path: Admin → Floor Management
- User-visible workflow: Can create, edit, delete floors

**Notes:**
- Full CRUD for floors implemented
- Can define multiple floors with names
- Description and dimensions configurable

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.2 Multiple floors per venue

**Feature ID:** B.2  
**Feature Name:** Multiple floors per venue  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Floors` table supports multiple records
- Domain entities: `Floor.cs` - no venue FK limit
- Services: `FloorRepository.GetAllAsync()`
- APIs / handlers: List all floors
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `FloorManagementPage.xaml`, `TableMapPage.xaml` floor selector
- ViewModels: `FloorManagementViewModel.cs`, `TableMapViewModel.cs`
- Navigation path: Table Map has floor selector
- User-visible workflow: Can switch between floors

**Notes:**
- No limit on number of floors
- Floor selector in table map view
- Each floor independent

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.3 Floor dimensions (width × height)

**Feature ID:** B.3  
**Feature Name:** Floor dimensions  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Floors.Width`, `Floors.Height`
- Domain entities: `Floor.Width`, `Floor.Height` properties
- Services: `Floor.UpdateDimensions()`
- APIs / handlers: Update floor
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `FloorManagementPage.xaml` - dimension inputs
- ViewModels: `FloorManagementViewModel.cs`
- Navigation path: Floor Management → Edit Floor
- User-visible workflow: Can set width/height in pixels

**Notes:**
- Default 2000x2000 pixels
- Configurable per floor
- Used for canvas sizing

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.4 Background configuration (color / grid)

**Feature ID:** B.4  
**Feature Name:** Background configuration  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Floors.BackgroundColor`
- Domain entities: `Floor.BackgroundColor` property
- Services: `Floor.UpdateBackgroundColor()`
- APIs / handlers: Update floor
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `FloorManagementPage.xaml` - color picker
- ViewModels: `FloorManagementViewModel.cs`
- Navigation path: Floor Management → Edit Floor
- User-visible workflow: Can set background color

**Notes:**
- Background color supported
- Grid overlay NOT implemented
- No background image support

**Risks / Gaps:**
- No grid lines for alignment
- No background image (venue floorplan)

**Recommendation:** EXTEND - Add grid overlay option

---

## B.5 Table layout designer (visual editor)

**Feature ID:** B.5  
**Feature Name:** Table layout designer  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `TableLayouts`, `Tables`
- Domain entities: `TableLayout.cs`, `Table.cs`
- Services: `CreateTableLayoutCommandHandler.cs`, `TableLayoutRepository.cs`
- APIs / handlers: Layout CRUD commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml`
- ViewModels: `TableDesignerViewModel.cs` (multiple partial files)
- Navigation path: Admin → Table Designer
- User-visible workflow: Full visual canvas editor

**Notes:**
- Canvas-based editor implemented
- Add, remove, position tables
- Save layouts to database

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.6 Drag-and-drop table placement

**Feature ID:** B.6  
**Feature Name:** Drag-and-drop placement  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Tables.X`, `Tables.Y`
- Domain entities: `Table.UpdatePosition()`
- Services: `TableLayout.UpdateTablePosition()`
- APIs / handlers: Save layout
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - Canvas with dragging
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Table Designer
- User-visible workflow: Drag tables on canvas

**Notes:**
- Mouse/touch drag implemented
- Position saved on layout save
- Real-time visual updates

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.7 Resize tables (width / height)

**Feature ID:** B.7  
**Feature Name:** Resize tables  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Tables.Width`, `Tables.Height`
- Domain entities: `Table.UpdateGeometry()`
- Services: Table update commands
- APIs / handlers: Update table
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - resize handles
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Table Designer → Select Table → Resize
- User-visible workflow: Drag handles to resize

**Notes:**
- Width and Height properties exist
- Default 100x100
- Resize handles in designer

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.8 Table shape configuration

**Feature ID:** B.8  
**Feature Name:** Table shape configuration  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Tables.Shape`
- Domain entities: `Table.Shape` (TableShapeType enum), `TableShape.cs`
- Services: `Table.UpdateGeometry()`
- APIs / handlers: Update table
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - shape selector
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Table Designer → Select Table → Shape dropdown
- User-visible workflow: Choose Rectangle, Circle, Diamond, Square

**Notes:**
- Four shapes available
- Shape affects visual rendering
- No custom shape support

**Risks / Gaps:**
- Limited shape options
- No pool-table-specific shapes

**Recommendation:** KEEP AS-IS - Sufficient for basic needs

---

## B.9 Snap-to-grid alignment

**Feature ID:** B.9  
**Feature Name:** Snap-to-grid alignment  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - no snap logic
- ViewModels: `TableDesignerViewModel.cs` - no snap implementation
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Free-form placement only
- No grid snap option
- Tables can be placed at any coordinate

**Risks / Gaps:**
- Difficult to align tables precisely
- Layouts may appear uneven

**Recommendation:** EXTEND - Add configurable snap-to-grid

---

## B.10 Alignment guides

**Feature ID:** B.10  
**Feature Name:** Alignment guides  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No smart guides when dragging
- No "snap to center/edge" with other tables
- PowerPoint-style alignment not implemented

**Risks / Gaps:**
- Manual alignment tedious
- Professional layouts difficult

**Recommendation:** EXTEND - Add alignment guides

---

## B.11 Zoom and pan

**Feature ID:** B.11  
**Feature Name:** Zoom and pan  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - ScrollViewer
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Table Designer
- User-visible workflow: Scroll to pan (zoom unclear)

**Notes:**
- ScrollViewer provides basic pan
- Zoom functionality unclear from code review
- No zoom slider visible

**Risks / Gaps:**
- Large layouts difficult to navigate
- Detail work requires zoom

**Recommendation:** EXTEND - Add zoom slider/wheel support

---

## B.12 Multi-select and group move

**Feature ID:** B.12  
**Feature Name:** Multi-select and group move  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - single selection only
- ViewModels: `TableDesignerViewModel_Selection.cs`
- Navigation path: Table Designer
- User-visible workflow: Click to select single table

**Notes:**
- Single selection mode only
- No Ctrl+click multi-select
- No marquee selection

**Risks / Gaps:**
- Cannot move groups of tables
- Layout rearrangement tedious

**Recommendation:** EXTEND - Add multi-select capability

---

## B.13 Layout versions per floor (weekday / weekend / events)

**Feature ID:** B.13  
**Feature Name:** Layout versions per floor  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `TableLayouts.FloorId`, multiple layouts per floor
- Domain entities: `Floor.TableLayouts` collection
- Services: `TableLayoutRepository.cs`
- APIs / handlers: CRUD layouts
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `FloorManagementPage.xaml` - layout list
- ViewModels: `FloorManagementViewModel.cs`
- Navigation path: Floor Management → Layouts list
- User-visible workflow: Can create multiple layouts per floor

**Notes:**
- Multiple layouts per floor supported
- No weekday/weekend/event naming convention enforced
- No schedule-based auto-switching

**Risks / Gaps:**
- Manual layout switching required
- No time-based layout activation

**Recommendation:** EXTEND - Add scheduled layout activation

---

## B.14 Clone layout

**Feature ID:** B.14  
**Feature Name:** Clone layout  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No "Duplicate Layout" option
- Must recreate layouts manually
- Common feature for event variations

**Risks / Gaps:**
- Time-consuming to create similar layouts
- Error-prone manual recreation

**Recommendation:** EXTEND - Add clone layout feature

---

## B.15 Draft vs published layout

**Feature ID:** B.15  
**Feature Name:** Draft vs published layout  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `TableLayouts.IsDraft`
- Domain entities: `TableLayout.IsDraft`, `TableLayout.SetDraftStatus()`
- Services: Layout save operations
- APIs / handlers: Update layout
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - publish button
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Table Designer → Save as Draft / Publish
- User-visible workflow: Can mark as draft or published

**Notes:**
- IsDraft flag implemented
- SetDraftStatus() method available
- Draft layouts not visible in operations

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.16 Layout rollback / revert

**Feature ID:** B.16  
**Feature Name:** Layout rollback / revert  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no version history)
- Domain entities: `TableLayout.Version` - single version only
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No version history stored
- Version property exists but only increments
- Cannot revert to previous layout state

**Risks / Gaps:**
- Destructive edits cannot be undone
- No safety net for layout changes

**Recommendation:** EXTEND - Add layout version history

---

## B.17 Visual occupancy map per floor

**Feature ID:** B.17  
**Feature Name:** Visual occupancy map  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Tables.Status`, `Tables.CurrentTicketId`
- Domain entities: `Table.Status`, `TableStatus` enum
- Services: `GetTableMapQueryHandler.cs`
- APIs / handlers: `GetTableMapQuery`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableMapPage.xaml`
- ViewModels: `TableMapViewModel.cs`
- Navigation path: Switchboard → Table Map
- User-visible workflow: Color-coded tables showing occupancy

**Notes:**
- Table map shows status colors
- Real-time status updates
- Filter by floor

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## B.18 Layout persistence and reload safety

**Feature ID:** B.18  
**Feature Name:** Layout persistence and reload safety  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `TableLayouts`, `Tables` with proper FK relationships
- Domain entities: `TableLayout.cs`, `Table.cs`
- Services: `TableLayoutRepository.GetLayoutAsync()`
- APIs / handlers: Load/save commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml`
- ViewModels: `TableDesignerViewModel_SaveLoad.cs`
- Navigation path: Table Designer → Load/Save
- User-visible workflow: Layouts persist across sessions

**Notes:**
- Full database persistence
- FK relationships maintained
- Reload after restart works

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## Category B COMPLETE

- Features audited: 18
- Fully implemented: 10
- Partially implemented: 4
- Not implemented: 4
