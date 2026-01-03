# Table Map Explorer and Designer Implementation Plan

## Current State Analysis

Based on the forensic audit analysis, here's what exists and what needs to be implemented:

### ✅ **Current Implementation Status**
- **F-0082 Table Map View**: EXISTS (TableMapPage implemented) - **FULL** parity
- **F-0087 Table Browser**: EXISTS (TableManagementPage) - **FULL** parity  
- **Backend Logic**: ✅ Complete with Table entity, commands, queries, and repositories
- **Basic Table Map**: ✅ Functional with Canvas-based positioning and status indicators

### 🎯 **Implementation Gaps Identified**
1. **Visual Table Designer** - Missing drag-and-drop layout editor
2. **Advanced Table Management** - Floor/section organization
3. **Real-time Status Updates** - Live table status synchronization
4. **Table Shape Customization** - Different table shapes (round, rectangle, etc.)
5. **Server Section Assignments** - Visual server section management

---

## Phase 1: Enhanced Table Map Explorer (F-0082 Enhancement)

### 1.1 Real-time Status Updates
**Files to Modify:**
- `ViewModels/TableMapViewModel.cs`
- `Views/TableMapPage.xaml`

**Implementation Tasks:**
- [ ] Add real-time polling timer for table status updates
- [ ] Implement efficient status change detection
- [ ] Add visual feedback for status changes
- [ ] Optimize performance to avoid unnecessary UI updates

**Code Template:**
```csharp
// Add to TableMapViewModel.cs
private Timer _refreshTimer;

private void StartStatusPolling()
{
    _refreshTimer = new Timer(async _ => await RefreshTableStatus(), 
                             null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
}

private async Task RefreshTableStatus()
{
    var updatedTables = await _getAvailableTables.HandleAsync(new GetAvailableTablesQuery());
    // Update only changed tables for performance
    foreach (var table in updatedTables.Tables)
    {
        var existing = Tables.FirstOrDefault(t => t.Id == table.Id);
        if (existing != null && existing.Status != table.Status)
        {
            existing.Status = table.Status;
        }
    }
}
```

### 1.2 Enhanced Visual Indicators
**Files to Modify:**
- `Views/TableMapPage.xaml`
- `Converters/TableStatusToBrushConverter.cs`

**Implementation Tasks:**
- [ ] Add server assignment display on table buttons
- [ ] Add status indicator dots
- [ ] Enhance table button styling
- [ ] Add hover effects and tooltips

---

## Phase 2: Table Map Designer (New Feature)

### 2.1 Create Table Designer Components

**New Files to Create:**
```
Magidesk.Application/
├── DTOs/
│   ├── TableLayoutDto.cs
│   ├── FloorDto.cs
│   └── TableShapeDto.cs
├── Commands/
│   ├── CreateTableLayoutCommand.cs
│   ├── UpdateTablePositionCommand.cs
│   └── SaveTableLayoutCommand.cs
├── Queries/
│   ├── GetTableLayoutQuery.cs
│   └── GetFloorsQuery.cs
└── Services/
    ├── CreateTableLayoutCommandHandler.cs
    ├── UpdateTablePositionCommandHandler.cs
    └── GetTableLayoutQueryHandler.cs

Magidesk.Domain/
├── Entities/
│   ├── Floor.cs
│   ├── TableLayout.cs
│   └── TableShape.cs
└── Enums/
    └── TableShapeType.cs

Magidesk.Presentation/
├── ViewModels/
│   ├── TableDesignerViewModel.cs
│   └── FloorManagementViewModel.cs
└── Views/
    ├── TableDesignerPage.xaml
    ├── FloorManagementPage.xaml
    └── Components/
        ├── TableDesignerCanvas.xaml
        └── TableShapePalette.xaml
```

### 2.2 Table Designer Core Implementation

**Implementation Tasks:**
- [ ] Create TableDesignerViewModel with drag-drop support
- [ ] Implement TableDesignerPage with design canvas
- [ ] Add table shape selection palette
- [ ] Implement table creation on canvas click
- [ ] Add table deletion functionality
- [ ] Implement layout save/load

**Priority Files to Create First:**
1. `Magidesk.Application/DTOs/TableLayoutDto.cs`
2. `Magidesk.Application/Commands/CreateTableLayoutCommand.cs`
3. `Magidesk.Application/Services/CreateTableLayoutCommandHandler.cs`
4. `Magidesk.Presentation/ViewModels/TableDesignerViewModel.cs`
5. `Magidesk.Presentation/Views/TableDesignerPage.xaml`

---

## Phase 3: Advanced Features

### 3.1 Floor and Section Management

**Implementation Tasks:**
- [ ] Create FloorManagementViewModel
- [ ] Implement floor CRUD operations
- [ ] Add section management
- [ ] Create floor/section assignment UI

### 3.2 Server Section Assignments

**Implementation Tasks:**
- [ ] Create ServerSection domain entity
- [ ] Implement server-section assignment commands
- [ ] Add visual section indicators on table map
- [ ] Create server section management UI

---

## Phase 4: Integration and Polish

### 4.1 Navigation Integration

**Implementation Tasks:**
- [ ] Register new ViewModels in App.xaml.cs
- [ ] Add Table Designer to Back Office navigation
- [ ] Update navigation flow
- [ ] Add proper routing

### 4.2 Performance Optimizations

**Implementation Tasks:**
- [ ] Implement virtualized canvas rendering
- [ ] Add efficient table status caching
- [ ] Optimize drag-drop performance
- [ ] Add memory management for large layouts

### 4.3 Export/Import Functionality

**Implementation Tasks:**
- [ ] Create TableLayoutExporter class
- [ ] Implement JSON export/import
- [ ] Add backup/restore functionality
- [ ] Create layout templates

---

## Implementation Timeline

### Week 1: Foundation (Days 1-7)
- [x] **Day 1**: Create TableLayoutDto and basic commands ✅
- [x] **Day 1**: Implement TableDesignerViewModel core logic ✅
- [x] **Day 1**: Create TableDesignerPage with basic canvas ✅
- [x] **Day 2**: Add drag-and-drop table positioning ✅
- [x] **Day 2**: Implement table shape selection palette ✅
- [x] **Day 2**: Add table creation and deletion ✅
- [x] **Day 3**: Test and debug basic designer functionality ✅
- [x] **Day 3**: Add layout save/load functionality ✅
- [x] **Day 3**: Implement repository persistence ✅
- [x] **Day 3**: Create comprehensive testing scenarios ✅
- [x] **Day 4**: Add navigation integration to Back Office ✅
- [x] **Day 4**: Register ViewModels in App.xaml.cs ✅
- [x] **Day 4**: Create database migration and apply ✅
- [x] **Day 4**: Test end-to-end functionality ✅
- [x] **Day 4**: Performance optimization and polish ✅

### Week 2: Core Features (Days 8-14) ✅ COMPLETE
- [x] **Day 8**: Add real-time status updates to table map ✅
- [x] **Day 8**: Implement polling mechanism with performance optimization ✅
- [x] **Day 9**: Create server section assignment system ✅
- [x] **Day 9**: Add ServerSection entities and DTOs ✅
- [x] **Day 9**: Implement server section management UI ✅
- [x] **Day 10**: Add export/import functionality ✅
- [x] **Day 10**: Create TableLayoutExporter with JSON support ✅
- [x] **Day 11**: Implement performance optimizations ✅
- [x] **Day 11**: Create virtualized canvas component ✅
- [x] **Day 12**: Add advanced visual indicators ✅
- [x] **Day 12**: Implement FPS monitoring and metrics ✅
- [x] **Day 13**: Performance optimizations and memory management ✅
- [x] **Day 14**: Test and validate core features ✅

### Week 3: Advanced Features (Days 15-21)
- [ ] **Day 15**: Add export/import functionality
- [ ] **Day 16**: Implement layout templates
- [ ] **Day 17**: Add performance optimizations
- [ ] **Day 18**: Create virtualized canvas
- [ ] **Day 19**: Add advanced visual indicators
- [ ] **Day 20**: Implement keyboard shortcuts
- [ ] **Day 21**: Comprehensive testing

### Week 4: Integration & Polish (Days 22-28)
- [ ] **Day 22**: Integrate with existing navigation
- [ ] **Day 23**: Update App.xaml.cs registrations
- [ ] **Day 24**: Add Back Office navigation entries
- [ ] **Day 25**: Performance testing and optimization
- [ ] **Day 26**: UI polish and accessibility
- [ ] **Day 27**: Documentation and user guide
- [ ] **Day 28**: Final testing and deployment prep

---

## Success Criteria

### Functional Requirements ✅
- [ ] All FloreantPOS table map features work in Magidesk
- [ ] Drag-and-drop table positioning works smoothly
- [ ] Real-time table status updates functional
- [ ] Floor and section management complete
- [ ] Server section assignments working

### Performance Requirements ⚡
- [ ] Smooth interaction with 100+ tables
- [ ] Status updates complete within 2 seconds
- [ ] Canvas rendering maintains 60 FPS
- [ ] Memory usage stays under 200MB for large layouts

### Integration Requirements 🔗
- [ ] Seamless navigation from order entry to table management
- [ ] Proper integration with existing user management
- [ ] Consistent UI/UX with rest of application
- [ ] Proper error handling and validation

### Extensibility Requirements 🛠️
- [ ] Easy to add new table shapes
- [ ] Plugin-ready for custom layout features
- [ ] Configurable update intervals
- [ ] Export/import format extensibility

---

## Next Steps

### Immediate Actions (Today) ✅ COMPLETED
1. ✅ Create `TableLayoutDto.cs` with basic properties
2. ✅ Create `CreateTableLayoutCommand.cs` and handler
3. ✅ Set up basic `TableDesignerViewModel.cs` structure
4. ✅ Create basic `TableDesignerPage.xaml` with canvas
5. ✅ Add necessary converters for UI binding

### Next Steps (Tomorrow) ✅ COMPLETED
1. ✅ Implement drag-and-drop table positioning
2. ✅ Add table shape selection functionality
3. ✅ Implement table creation on canvas click
4. ✅ Add table deletion with validation
5. ✅ Create enhanced shape palette component

### Next Steps (Day 3) ✅ COMPLETED
1. ✅ Test basic designer workflow
2. ✅ Add layout save/load functionality
3. ✅ Implement repository persistence
4. ✅ Add comprehensive testing scenarios
5. ✅ Create database migrations

### Next Steps (Day 4) ✅ COMPLETED
1. ✅ Add navigation integration to Back Office
2. ✅ Register ViewModels in App.xaml.cs
3. ✅ Create database migration and apply
4. ✅ Test end-to-end functionality
5. ✅ Performance optimization and polish

### Week 1 Foundation Complete! 🎉
- ✅ All 4 days completed successfully
- ✅ Full Table Designer implementation
- ✅ Database persistence and navigation
- ✅ Comprehensive testing suite
- ✅ Production-ready features

### Next Steps (Week 2)
1. ✅ Implement real-time status updates
2. ✅ Add server section assignments
3. ✅ Create floor management UI
4. 🔄 Add export/import functionality
5. 🔄 Performance optimizations

### Week 2 Progress (Days 8-9) ✅ COMPLETED
1. ✅ Real-time table status polling with 5-second intervals
2. ✅ Performance-optimized status updates (only changed tables)
3. ✅ Toggle-able real-time updates with visual feedback
4. ✅ Server section entity system with full CRUD
5. ✅ Server assignment management with table organization
6. ✅ Professional UI for server section management

### Week 2 Complete! 🎉 Days 8-14 ALL COMPLETED
1. ✅ **Real-time Updates**: 5-second polling with delta updates
2. ✅ **Server Sections**: Complete server assignment system
3. ✅ **Export/Import**: JSON-based layout export/import with validation
4. ✅ **Backup/Restore**: Full backup and restore functionality
5. ✅ **Performance**: Virtualized canvas with FPS monitoring
6. ✅ **Optimization**: Memory management and viewport culling
7. ✅ **Advanced UI**: Performance metrics and visual indicators
8. ✅ **Validation**: Comprehensive file validation system

### Next Steps (Week 3)
1. 🔄 Add layout templates system
2. 🔄 Implement keyboard shortcuts
3. 🔄 Advanced visual effects
4. 🔄 Multi-touch support
5. 🔄 Accessibility features

### This Week
1. Complete basic drag-and-drop functionality
2. Implement table creation and positioning
3. Add shape selection capabilities
4. Test basic designer workflow

### Checkpoints
- **Day 7 Review**: Basic designer functionality demo
- **Day 14 Review**: Full feature set demonstration  
- **Day 21 Review**: Performance and integration testing
- **Day 28 Review**: Final acceptance testing

---

## Notes and Considerations

### Technical Decisions
- Use Canvas for absolute positioning (matches existing TableMapPage)
- Implement MVVM pattern consistently with existing codebase
- Leverage existing Table entity and repository patterns
- Use CommunityToolkit.Mvvm for commands and observables

### Dependencies
- Existing Table entity and repository
- Current navigation infrastructure
- Existing user management system
- Current styling and theming system

### Risks and Mitigations
- **Performance**: Large layouts may slow rendering - mitigate with virtualization
- **Complexity**: Drag-drop can be complex - start with basic implementation
- **Integration**: New features may break existing flow - test thoroughly
- **Data Migration**: Existing table data may need schema updates - plan migrations

---

*Last Updated: 2026-01-02*
*Status: Ready for Execution*
