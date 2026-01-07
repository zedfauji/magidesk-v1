# Table Layout Designer - INTERACTION SPECIFICATION
## Phase 3: Detailed Interaction & Implementation Guide

**Date**: 2026-01-06  
**Status**: IMPLEMENTATION READY  
**Based On**: Phase 2 Target Architecture

---

## 1. EDIT MODE VS VIEW MODE

### 1.1 Design Mode (Edit)

**Activation**:
- Toggle button in header: "Design Mode" (default: ON)
- Keyboard: `Ctrl + E`
- Automatically ON when creating new layout

**Visual Indicators**:
- Toggle button: Blue background when active
- Canvas: Grid visible
- Tables: Show resize handles on selection
- Cursor: Changes based on tool (crosshair for add, hand for pan)

**Allowed Actions**:
- ✅ Add tables
- ✅ Delete tables
- ✅ Move tables
- ✅ Resize tables
- ✅ Edit properties
- ✅ Multi-select
- ✅ Copy/paste
- ✅ Undo/redo

**Restrictions**:
- ❌ Cannot edit Active layout directly (must clone or create new)
- ❌ Cannot move/delete tables with Status != Available

---

### 1.2 View Mode (Read-Only)

**Activation**:
- Toggle button: "Design Mode" OFF
- Automatically ON when viewing Active layout
- Keyboard: `Ctrl + E` (toggle)

**Visual Indicators**:
- Toggle button: Gray background
- Canvas: Grid hidden
- Tables: No resize handles
- Tables show real-time status (Available, Occupied, Reserved)
- Cursor: Default pointer

**Allowed Actions**:
- ✅ Pan canvas
- ✅ Zoom in/out
- ✅ Select tables (to view properties)
- ✅ View table status

**Restrictions**:
- ❌ No editing
- ❌ No adding/deleting
- ❌ No moving/resizing
- ❌ No property changes

**Use Cases**:
- Preview published layout
- View current floor status
- Check table availability
- Print layout for reference

---

## 2. SELECTION RULES

### 2.1 Single Selection

**Trigger**: Click on table

**Behavior**:
```
User clicks Table 5
  ↓
Deselect all other tables
  ↓
Select Table 5
  ↓
Show blue border (3px)
  ↓
Show 8 resize handles
  ↓
Update properties panel with Table 5 details
  ↓
Enable: Move, Resize, Delete, Edit Properties
```

**Visual**:
- Blue border: 3px solid
- Resize handles: 8 circles (10px diameter)
- Properties panel: Shows all table properties

---

### 2.2 Multi-Selection

**Triggers**:
- `Ctrl + Click`: Add to selection
- `Shift + Click`: Range select (by creation order)
- `Drag Selection Box`: Lasso select
- `Ctrl + A`: Select all

**Behavior**:
```
User Ctrl+Clicks Table 5, Table 7, Table 9
  ↓
Add each to selection set
  ↓
Show blue border (2px) on each
  ↓
NO resize handles (multi-select)
  ↓
Update properties panel: "Multi-Select (3 tables)"
  ↓
Enable: Group Move, Alignment, Bulk Actions
```

**Visual**:
- Blue border: 2px solid (thinner than single)
- No resize handles
- Bounding box: Dashed blue rectangle around all
- Properties panel: Shows bulk actions

**Group Operations**:
- **Move**: Drag any selected table, all move together
- **Delete**: Delete all (with confirmation)
- **Align**: Align left/center/right/top/middle/bottom
- **Distribute**: Even spacing horizontally/vertically
- **Set Property**: Apply same value to all (capacity, shape, etc.)

---

### 2.3 Lasso Selection

**Trigger**: Click and drag on empty canvas area

**Behavior**:
```
User clicks on empty canvas
  ↓
Start drag operation
  ↓
Show selection rectangle (dashed blue)
  ↓
Highlight tables intersecting rectangle (light blue)
  ↓
On mouse up:
    Select all intersecting tables
    Show multi-select visuals
```

**Visual**:
- Selection box: Dashed blue rectangle
- Intersecting tables: Light blue highlight (preview)
- Final selection: Blue border (2px)

---

## 3. SAVE FLOWS

### 3.1 Save Draft (Simple)

**Trigger**: Click "Save Draft" or `Ctrl + S`

**Preconditions**:
- Layout name not empty
- At least 1 table exists
- No validation errors

**Flow**:
```
User clicks "Save Draft"
  ↓
Validate layout:
  ✓ Name not empty
  ✓ At least 1 table
  ✓ No overlaps
  ✓ All tables in bounds
  ↓
IF validation fails THEN
    Show error dialog with details
    Highlight invalid tables
    STOP
END IF
  ↓
Show progress indicator
  ↓
Call SaveTableLayoutCommand
  ↓
ON SUCCESS:
    Clear dirty state (●Unsaved → ✓Saved)
    Show toast: "Draft saved successfully"
    Update _currentLayoutId
ON ERROR:
    Show error dialog
    Keep dirty state
    Offer retry
```

**Error Handling**:
- Network error: Auto-retry + "Save Locally" option
- Validation error: Show specific errors, highlight tables
- Concurrency error: Offer "Reload & Merge" or "Save As Copy"

---

### 3.2 Publish (Complex)

**Trigger**: Click "Publish" or `Ctrl + Shift + S`

**Preconditions**:
- All "Save Draft" preconditions
- Layout is currently Draft (not already Active)

**Flow**:
```
User clicks "Publish"
  ↓
Validate layout (same as Save Draft)
  ↓
Check if other layout is Active on this floor
  ↓
IF other Active layout exists THEN
    Show confirmation:
    "Publish 'Lunch Setup' as active layout?
     This will deactivate 'Dinner Setup'."
    [Publish] [Cancel]
    ↓
    IF Cancel THEN STOP
END IF
  ↓
Show progress indicator
  ↓
Call PublishTableLayoutCommand:
  - Deactivate other layouts on floor
  - Activate current layout
  - Save changes
  ↓
ON SUCCESS:
    Update badge: ● DRAFT → ✓ ACTIVE
    Clear dirty state
    Show toast: "Layout published and activated"
    Disable Design Mode (switch to View Mode)
ON ERROR:
    Show error dialog
    Keep draft state
    Offer retry
```

**Post-Publish Behavior**:
- Layout becomes read-only in View Mode
- To edit: Must clone or create new layout
- Other operators see new active layout immediately

---

### 3.3 Auto-Save (Background)

**Trigger**: Every 60 seconds if dirty

**Flow**:
```
Timer fires (60s)
  ↓
IF IsDirty AND IsDraftMode THEN
    Save silently to local storage
    Show subtle indicator: "Auto-saved at 2:45 PM"
    Keep dirty state (not a full save)
END IF
```

**Purpose**:
- Prevent data loss on crash
- Recover unsaved work on reload
- Does NOT clear dirty state
- Does NOT save to database (local only)

---

## 4. ERROR SURFACING

### 4.1 Validation Errors (Real-Time)

**Triggers**:
- Table moved
- Table resized
- Table added
- Property changed

**Flow**:
```
User moves Table 5
  ↓
Run validation:
  - Check overlap with other tables
  - Check bounds (fully inside canvas)
  - Check table number uniqueness
  ↓
IF overlap detected THEN
    Show red border on Table 5
    Show tooltip: "⚠ Overlaps with Table 3"
    Add to validation errors list
    Disable "Save" and "Publish" buttons
ELSE
    Clear red border
    Remove from validation errors list
    Enable "Save" and "Publish" (if no other errors)
END IF
```

**Visual Feedback**:
- **Red border**: 3px solid red
- **Tooltip**: Shows on hover, "⚠ [Error message]"
- **Error list**: In status bar or properties panel
- **Disabled buttons**: Gray out "Save" and "Publish"

---

### 4.2 Save Errors (Dialog)

**Trigger**: Save/Publish fails

**Flow**:
```
Save fails with error
  ↓
Show error dialog:
  Title: "Save Failed"
  Message: [Error details]
  Icon: ❌
  Actions: [Retry] [Cancel]
  ↓
User clicks Retry:
    Attempt save again
User clicks Cancel:
    Close dialog, keep dirty state
```

**Error Types**:
- **Network Error**: "Connection lost. Check your network."
- **Validation Error**: "Layout has errors: [list]"
- **Concurrency Error**: "Layout was modified by another user."
- **Permission Error**: "You don't have permission to save this layout."

---

### 4.3 Load Errors (Banner)

**Trigger**: Load layout/floor fails

**Flow**:
```
Load fails with error
  ↓
Show error banner at top of page:
  "⚠ Failed to load layout. [Error details]"
  [Retry] [Dismiss]
  ↓
User clicks Retry:
    Attempt load again
User clicks Dismiss:
    Hide banner, show empty canvas
```

**Fallback**:
- If floor load fails: Create default floor
- If layout load fails: Show empty canvas
- Always log error to console

---

## 5. WINUI 3 IMPLEMENTATION DETAILS

### 5.1 XAML Structure

```xml
<Page x:Class="TableDesignerPage">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="60"/> <!-- Header -->
            <RowDefinition Height="*"/>  <!-- Content -->
            <RowDefinition Height="40"/> <!-- Status -->
        </Grid.RowDefinitions>

        <!-- HEADER BAR -->
        <Grid Grid.Row="0" Background="{ThemeResource CardBackgroundFillColorDefaultBrush}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/> <!-- Floor selector -->
                <ColumnDefinition Width="Auto"/> <!-- Layout selector -->
                <ColumnDefinition Width="Auto"/> <!-- Draft badge -->
                <ColumnDefinition Width="*"/>    <!-- Spacer -->
                <ColumnDefinition Width="Auto"/> <!-- Actions -->
            </Grid.ColumnDefinitions>

            <!-- Floor Selector -->
            <ComboBox Grid.Column="0" 
                      Header="Floor"
                      ItemsSource="{x:Bind ViewModel.Floors}"
                      SelectedItem="{x:Bind ViewModel.SelectedFloor, Mode=TwoWay}"
                      Width="200" Margin="8"/>

            <!-- Layout Selector -->
            <ComboBox Grid.Column="1"
                      Header="Layout"
                      ItemsSource="{x:Bind ViewModel.Layouts}"
                      SelectedItem="{x:Bind ViewModel.SelectedLayout, Mode=TwoWay}"
                      Width="200" Margin="8"/>

            <!-- Draft/Active Badge -->
            <InfoBadge Grid.Column="2"
                       Value="{x:Bind ViewModel.LayoutStatusText}"
                       Style="{x:Bind ViewModel.LayoutStatusStyle}"
                       Margin="8"/>

            <!-- Action Buttons -->
            <StackPanel Grid.Column="4" Orientation="Horizontal" Spacing="8" Margin="8">
                <Button Content="New Layout" Command="{x:Bind ViewModel.NewLayoutCommand}"/>
                <Button Content="Clone" Command="{x:Bind ViewModel.CloneLayoutCommand}"/>
                <Button Content="Save Draft" Command="{x:Bind ViewModel.SaveDraftCommand}"/>
                <Button Content="Publish" Command="{x:Bind ViewModel.PublishCommand}" Style="{StaticResource AccentButtonStyle}"/>
            </StackPanel>
        </Grid>

        <!-- CONTENT AREA (3 Panels) -->
        <Grid Grid.Row="1">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="280"/>  <!-- Left Panel -->
                <ColumnDefinition Width="*"/>    <!-- Canvas -->
                <ColumnDefinition Width="320"/>  <!-- Right Panel -->
            </Grid.ColumnDefinitions>

            <!-- LEFT PANEL -->
            <Grid Grid.Column="0" Background="{ThemeResource LayerFillColorDefaultBrush}">
                <!-- Tools, Shapes, Layers -->
            </Grid>

            <!-- CANVAS -->
            <Grid Grid.Column="1">
                <!-- Canvas controls toolbar -->
                <Grid Height="40" VerticalAlignment="Top">
                    <!-- Zoom, Grid, Snap, Undo/Redo -->
                </Grid>

                <!-- Scrollable canvas -->
                <ScrollViewer Margin="0,40,0,0">
                    <Canvas Width="{x:Bind ViewModel.CanvasWidth}"
                            Height="{x:Bind ViewModel.CanvasHeight}"
                            Background="{x:Bind ViewModel.CanvasBackground}">
                        
                        <!-- Grid overlay -->
                        <Canvas.Background>
                            <ImageBrush ImageSource="{x:Bind ViewModel.GridPattern}" 
                                        Opacity="0.3"/>
                        </Canvas.Background>

                        <!-- Tables -->
                        <ItemsControl ItemsSource="{x:Bind ViewModel.Tables}">
                            <ItemsControl.ItemsPanel>
                                <ItemsPanelTemplate>
                                    <Canvas/>
                                </ItemsPanelTemplate>
                            </ItemsControl.ItemsPanel>

                            <ItemsControl.ItemContainerStyle>
                                <Style TargetType="ContentPresenter">
                                    <Setter Property="Canvas.Left" Value="{Binding X}"/>
                                    <Setter Property="Canvas.Top" Value="{Binding Y}"/>
                                </Style>
                            </ItemsControl.ItemContainerStyle>

                            <ItemsControl.ItemTemplate>
                                <DataTemplate x:DataType="dto:TableDto">
                                    <local:TableControl Table="{x:Bind}"
                                                        IsSelected="{x:Bind IsSelected, Mode=TwoWay}"
                                                        IsDesignMode="{x:Bind ViewModel.IsDesignMode}"/>
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </Canvas>
                </ScrollViewer>
            </Grid>

            <!-- RIGHT PANEL -->
            <Grid Grid.Column="2" Background="{ThemeResource LayerFillColorDefaultBrush}">
                <!-- Properties panel -->
            </Grid>
        </Grid>

        <!-- STATUS BAR -->
        <Grid Grid.Row="2" Background="{ThemeResource CardBackgroundFillColorDefaultBrush}">
            <StackPanel Orientation="Horizontal" Spacing="16" Margin="8">
                <TextBlock Text="{x:Bind ViewModel.TableCountText}"/>
                <TextBlock Text="{x:Bind ViewModel.SelectedCountText}"/>
                <TextBlock Text="{x:Bind ViewModel.ZoomText}"/>
                <TextBlock Text="{x:Bind ViewModel.DirtyStateText}" 
                           Foreground="{x:Bind ViewModel.DirtyStateBrush}"/>
            </StackPanel>
        </Grid>
    </Grid>
</Page>
```

---

### 5.2 Custom TableControl

```xml
<UserControl x:Class="TableControl">
    <Grid Width="{x:Bind Table.Width}" 
          Height="{x:Bind Table.Height}"
          ManipulationMode="TranslateX,TranslateY"
          ManipulationStarted="OnManipulationStarted"
          ManipulationDelta="OnManipulationDelta"
          ManipulationCompleted="OnManipulationCompleted"
          Tapped="OnTapped">

        <!-- Table visual -->
        <Border Background="{x:Bind Table.BackgroundBrush}"
                BorderBrush="{x:Bind SelectionBorderBrush}"
                BorderThickness="{x:Bind SelectionBorderThickness}"
                CornerRadius="{x:Bind Table.CornerRadius}">
            
            <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                <TextBlock Text="{x:Bind Table.TableNumber}" 
                           FontSize="20" FontWeight="Bold"/>
                <TextBlock Text="{x:Bind Table.CapacityText}" 
                           FontSize="12" Foreground="Gray"/>
            </StackPanel>
        </Border>

        <!-- Resize handles (only when selected and design mode) -->
        <Grid Visibility="{x:Bind ShowResizeHandles}">
            <!-- 8 resize handles at corners and midpoints -->
            <Ellipse Width="10" Height="10" Fill="Blue" 
                     HorizontalAlignment="Left" VerticalAlignment="Top"
                     PointerPressed="OnResizeHandlePressed"/>
            <!-- ... 7 more handles ... -->
        </Grid>

        <!-- Validation error indicator -->
        <Border BorderBrush="Red" BorderThickness="3" 
                Visibility="{x:Bind Table.HasValidationError}">
            <ToolTipService.ToolTip>
                <ToolTip Content="{x:Bind Table.ValidationErrorMessage}"/>
            </ToolTipService.ToolTip>
        </Border>
    </Grid>
</UserControl>
```

---

### 5.3 ViewModel Structure

```csharp
public class TableDesignerViewModel : ViewModelBase
{
    // Context
    [ObservableProperty] private ObservableCollection<FloorDto> _floors;
    [ObservableProperty] private FloorDto? _selectedFloor;
    [ObservableProperty] private ObservableCollection<LayoutDto> _layouts;
    [ObservableProperty] private LayoutDto? _selectedLayout;

    // Tables
    [ObservableProperty] private ObservableCollection<TableDto> _tables;
    [ObservableProperty] private ObservableCollection<TableDto> _selectedTables;

    // State
    [ObservableProperty] private bool _isDesignMode = false;
    [ObservableProperty] private bool _isDirty = false;
    [ObservableProperty] private bool _isBusy = false;

    // Canvas
    [ObservableProperty] private int _canvasWidth = 2000;
    [ObservableProperty] private int _canvasHeight = 2000;
    [ObservableProperty] private string _canvasBackground = "#f8f8f8";
    [ObservableProperty] private bool _gridVisible = true;
    [ObservableProperty] private bool _snapEnabled = true;
    [ObservableProperty] private double _zoomLevel = 1.0;

    // Undo/Redo
    private Stack<IUndoableAction> _undoStack = new();
    private Stack<IUndoableAction> _redoStack = new();

    // Commands
    public IAsyncRelayCommand NewLayoutCommand { get; }
    public IAsyncRelayCommand CloneLayoutCommand { get; }
    public IAsyncRelayCommand SaveDraftCommand { get; }
    public IAsyncRelayCommand PublishCommand { get; }
    public IAsyncRelayCommand RevertCommand { get; }
    public IRelayCommand UndoCommand { get; }
    public IRelayCommand RedoCommand { get; }
    public IAsyncRelayCommand<Point> AddTableCommand { get; }
    public IAsyncRelayCommand<TableDto> DeleteTableCommand { get; }
    public IRelayCommand<TableDto> SelectTableCommand { get; }
    public IRelayCommand ToggleDesignModeCommand { get; }
}
```

---

### 5.4 Code-Behind Responsibilities

```csharp
public sealed partial class TableDesignerPage : Page
{
    // Drag & Drop state
    private TableDto? _draggedTable;
    private Point _dragStartPosition;
    private Point _dragStartTablePosition;

    // Manipulation handlers (ALLOWED in code-behind)
    private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (!ViewModel.IsDesignMode) return;
        
        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            _draggedTable = table;
            _dragStartPosition = e.Position;
            _dragStartTablePosition = new Point(table.X, table.Y);
        }
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (_draggedTable == null) return;

        var delta = e.Delta.Translation;
        var newX = _dragStartTablePosition.X + delta.X;
        var newY = _dragStartTablePosition.Y + delta.Y;

        // Snap to grid if enabled
        if (ViewModel.SnapEnabled)
        {
            newX = Math.Round(newX / 50) * 50;
            newY = Math.Round(newY / 50) * 50;
        }

        _draggedTable.X = (int)newX;
        _draggedTable.Y = (int)newY;

        // Trigger validation (ViewModel responsibility)
        ViewModel.ValidateTablePosition(_draggedTable);
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (_draggedTable == null) return;

        // Create undo action (ViewModel responsibility)
        ViewModel.RecordTableMove(_draggedTable, _dragStartTablePosition);
        
        _draggedTable = null;
    }
}
```

---

## 6. IMPLEMENTATION CHECKLIST

### Phase 1: Core Structure ✅
- [ ] Create 3-panel layout (Header, Content, Status)
- [ ] Implement Floor selector with dropdown
- [ ] Implement Layout selector with dropdown
- [ ] Add Draft/Active badge
- [ ] Create basic canvas with ScrollViewer
- [ ] Render tables on canvas (ItemsControl + Canvas)

### Phase 2: Basic Interactions ✅
- [ ] Implement table drag & drop (ManipulationDelta)
- [ ] Implement single selection (click)
- [ ] Implement multi-selection (Ctrl+Click, lasso)
- [ ] Add table via click on canvas
- [ ] Delete table (with confirmation)

### Phase 3: Properties & Validation ✅
- [ ] Create properties panel (right panel)
- [ ] Bind table properties (number, capacity, shape)
- [ ] Implement real-time validation (overlap, bounds)
- [ ] Show validation errors (red border, tooltip)
- [ ] Disable save when validation errors exist

### Phase 4: Save/Load ✅
- [ ] Implement Save Draft command
- [ ] Implement Publish command
- [ ] Implement Revert command
- [ ] Add dirty state tracking
- [ ] Add navigation guards (unsaved changes prompt)

### Phase 5: Professional Features ✅
- [ ] Implement undo/redo stack
- [ ] Add zoom controls (zoom in/out/reset)
- [ ] Add grid overlay (toggle)
- [ ] Add snap-to-grid (toggle)
- [ ] Implement keyboard shortcuts (Ctrl+Z, Ctrl+S, etc.)

### Phase 6: Advanced Features ✅
- [ ] Add alignment tools (left, center, right, etc.)
- [ ] Add distribution tools (horizontal, vertical)
- [ ] Implement copy/paste
- [ ] Add layers panel
- [ ] Implement resize handles

### Phase 7: Polish & Safety ✅
- [ ] Add error handling (network, validation, concurrency)
- [ ] Implement auto-save (local storage)
- [ ] Add accessibility (ARIA, keyboard nav)
- [ ] Optimize performance (virtualization for > 100 tables)
- [ ] Add touch support (pinch zoom, pan)

---

## CONCLUSION

This interaction specification provides:
- ✅ **Clear mode separation** - Design vs View
- ✅ **Explicit selection rules** - Single, multi, lasso
- ✅ **Robust save flows** - Draft, publish, auto-save
- ✅ **Comprehensive error handling** - Validation, network, concurrency
- ✅ **WinUI 3 implementation guide** - XAML structure, code-behind boundaries
- ✅ **Implementation checklist** - 7 phases, 40+ tasks

**Ready for**: Implementation

---

**Status**: SPECIFICATION COMPLETE ✅  
**Next**: Begin Implementation Phase 1
