using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Magidesk.Application.DTOs;
using Windows.Foundation;

namespace Magidesk.Presentation.ViewModels;

public partial class TableDesignerViewModel
{
    // ============================================================================
    // SELECTION MANAGEMENT (Phase 2)
    // ============================================================================

    [ObservableProperty]
    private ObservableCollection<TableDto> _selectedTables = new();

    [ObservableProperty]
    private int _selectedCount = 0;

    /// <summary>
    /// Selects a single table, clearing other selections unless Ctrl is held.
    /// </summary>
    public void SelectTable(TableDto table, bool isCtrlPressed = false)
    {
        if (!isCtrlPressed)
        {
            // Single selection - clear all others
            ClearSelection();
            table.IsSelected = true;
            SelectedTables.Add(table);
            SelectedTable = table;
        }
        else
        {
            // Multi-selection - toggle this table
            if (table.IsSelected)
            {
                // Deselect
                table.IsSelected = false;
                SelectedTables.Remove(table);
                SelectedTable = SelectedTables.FirstOrDefault();
            }
            else
            {
                // Add to selection
                table.IsSelected = true;
                SelectedTables.Add(table);
                SelectedTable = table; // Last selected becomes primary
            }
        }

        SelectedCount = SelectedTables.Count;
        UpdateSelectionState();
    }

    /// <summary>
    /// Clears all table selections.
    /// </summary>
    public void ClearSelection()
    {
        foreach (var table in SelectedTables.ToList())
        {
            table.IsSelected = false;
        }
        SelectedTables.Clear();
        SelectedTable = null;
        SelectedCount = 0;
        UpdateSelectionState();
    }

    /// <summary>
    /// Selects all tables on the canvas.
    /// </summary>
    public void SelectAll()
    {
        ClearSelection();
        foreach (var table in Tables)
        {
            table.IsSelected = true;
            SelectedTables.Add(table);
        }
        SelectedTable = SelectedTables.FirstOrDefault();
        SelectedCount = SelectedTables.Count;
        UpdateSelectionState();
    }

    /// <summary>
    /// Selects tables within a rectangular area (lasso selection).
    /// </summary>
    public void SelectTablesInRect(Rect selectionRect)
    {
        ClearSelection();

        foreach (var table in Tables)
        {
            var tableRect = new Rect(table.X, table.Y, table.Width, table.Height);
            
            // Check if table intersects with selection rectangle
            if (RectIntersects(selectionRect, tableRect))
            {
                table.IsSelected = true;
                SelectedTables.Add(table);
            }
        }

        SelectedTable = SelectedTables.FirstOrDefault();
        SelectedCount = SelectedTables.Count;
        UpdateSelectionState();
    }

    /// <summary>
    /// Checks if two rectangles intersect.
    /// </summary>
    private bool RectIntersects(Rect rect1, Rect rect2)
    {
        return !(rect1.Right < rect2.Left ||
                 rect1.Left > rect2.Right ||
                 rect1.Bottom < rect2.Top ||
                 rect1.Top > rect2.Bottom);
    }

    /// <summary>
    /// Moves all selected tables by the specified delta.
    /// </summary>
    public void MoveSelectedTables(double deltaX, double deltaY)
    {
        if (!SelectedTables.Any()) return;

        foreach (var table in SelectedTables)
        {
            var newX = table.X + (int)deltaX;
            var newY = table.Y + (int)deltaY;

            // Apply snap-to-grid if enabled
            if (SnapEnabled)
            {
                newX = (int)(Math.Round(newX / (double)GridSize) * GridSize);
                newY = (int)(Math.Round(newY / (double)GridSize) * GridSize);
            }

            // Ensure within bounds
            table.X = Math.Max(0, Math.Min(newX, CanvasWidth - table.Width));
            table.Y = Math.Max(0, Math.Min(newY, CanvasHeight - table.Height));
        }

        IsDirty = true;
    }

    /// <summary>
    /// Deletes all selected tables with confirmation.
    /// </summary>
    public async Task DeleteSelectedTablesAsync()
    {
        try
        {
            if (!SelectedTables.Any())
            {
                await ShowErrorAsync("No tables selected to delete.");
                return;
            }

            var count = SelectedTables.Count;
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Delete Tables",
                $"Are you sure you want to delete {count} table(s)?",
                "Delete",
                "Cancel");

            if (!confirmed) return;

            // Remove from main collection
            foreach (var table in SelectedTables.ToList())
            {
                Tables.Remove(table);
            }

            ClearSelection();
            IsDirty = true;

            await ShowSuccessAsync($"Deleted {count} table(s).");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error deleting tables: {ex.Message}");
        }
    }

    // ============================================================================
    // GRID & SNAP SETTINGS (Phase 2)
    // ============================================================================

    [ObservableProperty]
    private bool _gridVisible = true;

    [ObservableProperty]
    private bool _snapEnabled = true;

    [ObservableProperty]
    private int _gridSize = 50;

    /// <summary>
    /// Toggles grid visibility.
    /// </summary>
    public void ToggleGrid()
    {
        GridVisible = !GridVisible;
    }

    /// <summary>
    /// Toggles snap-to-grid.
    /// </summary>
    public void ToggleSnap()
    {
        SnapEnabled = !SnapEnabled;
    }
}
