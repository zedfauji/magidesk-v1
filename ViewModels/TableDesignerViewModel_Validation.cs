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
    // VALIDATION (Phase 3)
    // ============================================================================

    [ObservableProperty]
    private ObservableCollection<string> _validationErrors = new();

    [ObservableProperty]
    private bool _hasValidationErrors = false;

    [ObservableProperty]
    private bool _hasSelection = false;

    [ObservableProperty]
    private bool _isSingleSelection = false;

    [ObservableProperty]
    private bool _isMultiSelection = false;

    /// <summary>
    /// Validates the currently selected table(s) and updates validation errors.
    /// </summary>
    public void ValidateSelection()
    {
        ValidationErrors.Clear();

        if (SelectedTable == null)
        {
            HasValidationErrors = false;
            return;
        }

        // Validate table number uniqueness
        var duplicates = Tables.Where(t => t.Id != SelectedTable.Id && t.TableNumber == SelectedTable.TableNumber).ToList();
        if (duplicates.Any())
        {
            ValidationErrors.Add($"Table number {SelectedTable.TableNumber} is already in use.");
        }

        // Validate bounds
        if (SelectedTable.X < 0 || SelectedTable.Y < 0)
        {
            ValidationErrors.Add("Table position cannot be negative.");
        }

        if (SelectedTable.X + SelectedTable.Width > CanvasWidth)
        {
            ValidationErrors.Add("Table extends beyond canvas width.");
        }

        if (SelectedTable.Y + SelectedTable.Height > CanvasHeight)
        {
            ValidationErrors.Add("Table extends beyond canvas height.");
        }

        // Validate overlaps
        var overlaps = DetectOverlaps(SelectedTable);
        if (overlaps.Any())
        {
            var overlappingNumbers = string.Join(", ", overlaps.Select(t => t.TableNumber));
            ValidationErrors.Add($"Table overlaps with: {overlappingNumbers}");
        }

        // Validate size
        if (SelectedTable.Width < 50 || SelectedTable.Height < 50)
        {
            ValidationErrors.Add("Table size must be at least 50x50.");
        }

        if (SelectedTable.Width > 500 || SelectedTable.Height > 500)
        {
            ValidationErrors.Add("Table size cannot exceed 500x500.");
        }

        HasValidationErrors = ValidationErrors.Any();
    }

    /// <summary>
    /// Detects tables that overlap with the given table.
    /// </summary>
    private List<TableDto> DetectOverlaps(TableDto table)
    {
        var overlapping = new List<TableDto>();
        var tableRect = new Rect(table.X, table.Y, table.Width, table.Height);

        foreach (var other in Tables.Where(t => t.Id != table.Id))
        {
            var otherRect = new Rect(other.X, other.Y, other.Width, other.Height);
            
            if (RectIntersects(tableRect, otherRect))
            {
                overlapping.Add(other);
            }
        }

        return overlapping;
    }

    /// <summary>
    /// Updates selection state flags based on current selection.
    /// </summary>
    public void UpdateSelectionState()
    {
        HasSelection = SelectedTables.Any();
        IsSingleSelection = SelectedTables.Count == 1;
        IsMultiSelection = SelectedTables.Count > 1;

        if (IsSingleSelection)
        {
            ValidateSelection();
        }
        else
        {
            ValidationErrors.Clear();
            HasValidationErrors = false;
        }
    }

    /// <summary>
    /// Validates all tables and returns true if all are valid.
    /// </summary>
    public bool ValidateAllTables()
    {
        var allErrors = new List<string>();

        foreach (var table in Tables)
        {
            // Check for duplicate table numbers
            var duplicates = Tables.Where(t => t.Id != table.Id && t.TableNumber == table.TableNumber).ToList();
            if (duplicates.Any())
            {
                allErrors.Add($"Table {table.TableNumber}: Duplicate table number.");
            }

            // Check bounds
            if (table.X < 0 || table.Y < 0 ||
                table.X + table.Width > CanvasWidth ||
                table.Y + table.Height > CanvasHeight)
            {
                allErrors.Add($"Table {table.TableNumber}: Out of bounds.");
            }

            // Check overlaps
            var overlaps = DetectOverlaps(table);
            if (overlaps.Any())
            {
                var overlappingNumbers = string.Join(", ", overlaps.Select(t => t.TableNumber));
                allErrors.Add($"Table {table.TableNumber}: Overlaps with {overlappingNumbers}.");
            }
        }

        if (allErrors.Any())
        {
            var errorMessage = "Validation failed:\n" + string.Join("\n", allErrors);
            _ = ShowErrorAsync(errorMessage);
            return false;
        }

        return true;
    }
}
