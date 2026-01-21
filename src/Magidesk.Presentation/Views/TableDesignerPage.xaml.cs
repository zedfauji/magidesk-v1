using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.ViewModels;
using Windows.Foundation;
using Windows.System;

namespace Magidesk.Presentation.Views;

public sealed partial class TableDesignerPage : Page
{
    private TableDto? _draggedTable;
    private Point _dragStartTablePosition;
    private Point _dragStartCanvasPosition;
    private bool _isDragging = false;
    private bool _isLassoSelecting = false;
    private Point _lassoStartPoint;
    private Rect _lassoRect;

    public TableDesignerViewModel ViewModel { get; }

    public TableDesignerPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TableDesignerViewModel>();
        ShapePalette.ViewModel = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            await ViewModel.LoadDataAsync();
        }
        catch (Exception ex)
        {
            try
            {
                var dialogService = App.Services.GetRequiredService<Magidesk.Application.Interfaces.IDialogService>();
                await dialogService.ShowErrorAsync("Designer Error", "Failed to load table designer.", ex.ToString());
            }
            catch { /* Last resort silence */ }
        }
    }

    // ============================================================================
    // HEADER BAR HANDLERS
    // ============================================================================

    private void LayoutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is TableLayoutDto selectedLayout)
            {
                // Only update if different to avoid infinite loops
                if (ViewModel.SelectedLayout?.Id != selectedLayout.Id)
                {
                    ViewModel.SelectedLayout = selectedLayout;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in LayoutComboBox_SelectionChanged: {ex.Message}");
        }
    }

    private void FloorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is FloorDto selectedFloor)
            {
                // Only update if different to avoid infinite loops
                if (ViewModel.SelectedFloor?.Id != selectedFloor.Id)
                {
                    ViewModel.SelectedFloor = selectedFloor;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in FloorComboBox_SelectionChanged: {ex.Message}");
        }
    }

    private void DesignModeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is ToggleSwitch toggle)
            {
                ViewModel.IsDesignMode = toggle.IsOn;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in DesignModeToggle_Toggled: {ex.Message}");
        }
    }

    // ============================================================================
    // CANVAS INTERACTIONS
    // ============================================================================

    private void Canvas_Tapped(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.IsDesignMode) return;

            // Check if Ctrl is pressed for multi-select
            var isCtrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
                .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            if (!isCtrlPressed)
            {
                // Clear selection when clicking empty canvas
                ViewModel.ClearSelection();
            }

            // Add table at click position
            var position = e.GetPosition(sender as UIElement);
            var point = new Point((int)position.X, (int)position.Y);
            ViewModel.AddTableCommand.Execute(point);
        }
        catch (Exception ex)
        {
            // Log error but don't crash
            System.Diagnostics.Debug.WriteLine($"Error in Canvas_Tapped: {ex.Message}");
        }
    }

    private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.IsDesignMode) return;

            var position = e.GetCurrentPoint(sender as UIElement).Position;
            
            // Check if clicking on empty space (for lasso selection)
            var clickedElement = e.OriginalSource as FrameworkElement;
            if (clickedElement?.DataContext is not TableDto)
            {
                // Start lasso selection
                _isLassoSelecting = true;
                _lassoStartPoint = position;
                _lassoRect = new Rect(position, new Size(0, 0));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in Canvas_PointerPressed: {ex.Message}");
        }
    }

    private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isLassoSelecting) return;

        var position = e.GetCurrentPoint(sender as UIElement).Position;
        
        // Update lasso rectangle
        var x = Math.Min(_lassoStartPoint.X, position.X);
        var y = Math.Min(_lassoStartPoint.Y, position.Y);
        var width = Math.Abs(position.X - _lassoStartPoint.X);
        var height = Math.Abs(position.Y - _lassoStartPoint.Y);
        
        _lassoRect = new Rect(x, y, width, height);
        
        // TODO: Show visual lasso rectangle overlay
    }

    private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_isLassoSelecting)
        {
            // Complete lasso selection
            if (_lassoRect.Width > 10 && _lassoRect.Height > 10) // Minimum size threshold
            {
                ViewModel.SelectTablesInRect(_lassoRect);
            }
            
            _isLassoSelecting = false;
            _lassoRect = Rect.Empty;
        }
    }

    // ============================================================================
    // TABLE INTERACTIONS
    // ============================================================================

    private void Table_Tapped(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            if (sender is FrameworkElement element && element.DataContext is TableDto table)
            {
                // Check if Ctrl is pressed for multi-select
                var isCtrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
                    .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

                ViewModel.SelectTable(table, isCtrlPressed);
                e.Handled = true; // Prevent canvas tap
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in Table_Tapped: {ex.Message}");
        }
    }

    private void Table_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.IsDesignMode) return;

            if (sender is FrameworkElement element && element.DataContext is TableDto table)
            {
                // If table not selected, select it first
                if (!table.IsSelected)
                {
                    var isCtrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
                        .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
                    ViewModel.SelectTable(table, isCtrlPressed);
                }

                _draggedTable = table;
                _dragStartTablePosition = new Point(table.X, table.Y);
                _dragStartCanvasPosition = e.Position;
                _isDragging = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in Table_ManipulationStarted: {ex.Message}");
            _isDragging = false;
            _draggedTable = null;
        }
    }

    private void Table_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (!_isDragging || _draggedTable == null || !ViewModel.IsDesignMode) return;

        var delta = e.Delta.Translation;

        // If multiple tables selected, move all of them
        if (ViewModel.SelectedTables.Count > 1)
        {
            ViewModel.MoveSelectedTables(delta.X, delta.Y);
        }
        else
        {
            // Single table move
            var newX = _dragStartTablePosition.X + delta.X;
            var newY = _dragStartTablePosition.Y + delta.Y;

            // Snap to grid if enabled
            if (ViewModel.SnapEnabled)
            {
                newX = Math.Round(newX / ViewModel.GridSize) * ViewModel.GridSize;
                newY = Math.Round(newY / ViewModel.GridSize) * ViewModel.GridSize;
            }

            // Update table position
            _draggedTable.X = (int)Math.Max(0, Math.Min(newX, ViewModel.CanvasWidth - _draggedTable.Width));
            _draggedTable.Y = (int)Math.Max(0, Math.Min(newY, ViewModel.CanvasHeight - _draggedTable.Height));

            // Mark as dirty
            ViewModel.IsDirty = true;
        }
    }

    private void Table_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (_draggedTable == null) return;

        // TODO: Record undo action
        // ViewModel.RecordTableMove(_draggedTable, _dragStartTablePosition);

        // TODO: Validate new position
        // ViewModel.ValidateTablePosition(_draggedTable);

        _isDragging = false;
        _draggedTable = null;
    }

    // ============================================================================
    // PROPERTIES PANEL HANDLERS
    // ============================================================================

    private void TableProperty_Changed(object sender, object e)
    {
        try
        {
            if (ViewModel.SelectedTable == null) return;

            // Read values from controls and update SelectedTable
            if (sender is NumberBox numberBox)
            {
                var value = (int)numberBox.Value;
                var tag = numberBox.Tag as string;
                
                switch (tag)
                {
                    case "TableNumber":
                        ViewModel.SelectedTable.TableNumber = value;
                        break;
                    case "Capacity":
                        ViewModel.SelectedTable.Capacity = value;
                        break;
                    case "X":
                        ViewModel.SelectedTable.X = value;
                        break;
                    case "Y":
                        ViewModel.SelectedTable.Y = value;
                        break;
                    case "Width":
                        ViewModel.SelectedTable.Width = value;
                        break;
                    case "Height":
                        ViewModel.SelectedTable.Height = value;
                        break;
                }
            }
            else if (sender is ComboBox comboBox && comboBox.SelectedItem != null)
            {
                var shapeString = comboBox.SelectedItem.ToString();
                if (Enum.TryParse<TableShapeType>(shapeString, out var shape))
                {
                    ViewModel.SelectedTable.Shape = shape;
                }
            }

            // Trigger validation and mark dirty
            ViewModel.ValidateSelection();
            ViewModel.IsDirty = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in TableProperty_Changed: {ex.Message}");
        }
    }

    private void AlignLeft_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.SelectedTables.Any()) return;

            var minX = ViewModel.SelectedTables.Min(t => t.X);
            foreach (var table in ViewModel.SelectedTables)
            {
                table.X = minX;
            }
            ViewModel.IsDirty = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in AlignLeft_Click: {ex.Message}");
        }
    }

    private void AlignTop_Click(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.SelectedTables.Any()) return;

        var minY = ViewModel.SelectedTables.Min(t => t.Y);
        foreach (var table in ViewModel.SelectedTables)
        {
            table.Y = minY;
        }
        ViewModel.IsDirty = true;
    }

    private void DistributeHorizontally_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedTables.Count < 3) return;

        var sorted = ViewModel.SelectedTables.OrderBy(t => t.X).ToList();
        var first = sorted.First();
        var last = sorted.Last();
        var totalSpace = last.X - first.X;
        var spacing = totalSpace / (sorted.Count - 1);

        for (int i = 1; i < sorted.Count - 1; i++)
        {
            sorted[i].X = first.X + (int)(spacing * i);
        }
        ViewModel.IsDirty = true;
    }

    // ============================================================================
    // KEYBOARD SHORTCUTS
    // ============================================================================

    private async void Page_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        var isCtrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
            .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        switch (e.Key)
        {
            case VirtualKey.Delete:
                if (ViewModel.SelectedTables.Any())
                {
                    await ViewModel.DeleteSelectedTablesAsync();
                    e.Handled = true;
                }
                break;

            case VirtualKey.A when isCtrlPressed:
                ViewModel.SelectAll();
                e.Handled = true;
                break;

            case VirtualKey.Escape:
                ViewModel.ClearSelection();
                e.Handled = true;
                break;

            case VirtualKey.S when isCtrlPressed:
                await ViewModel.SaveLayoutAsync();
                e.Handled = true;
                break;

            // TODO: Add more shortcuts (Ctrl+Z, Ctrl+C, Ctrl+V, etc.)
        }
    }
}
