using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

public partial class TableDesignerViewModel
{
    // ============================================================================
    // LAYOUT LIFECYCLE METHODS (NEW - Phase 1)
    // ============================================================================

    private async Task NewLayoutAsync()
    {
        if (await CheckDirtyStateAsync())
        {
            // User chose to cancel
            return;
        }

        // TODO: Show dialog to get layout name
        var layoutName = "New Layout"; // Placeholder
        
        // Create new blank layout
        SelectedLayout = new TableLayoutDto
        {
            Id = Guid.NewGuid(),
            Name = layoutName,
            FloorId = SelectedFloor?.Id,
            IsDraft = true,
            IsActive = false,
            Tables = new List<TableDto>()
        };

        // Add to layouts collection
        Layouts.Add(SelectedLayout);

        // Clear tables
        Tables.Clear();

        // Update UI
        LayoutName = layoutName;
        LayoutStatusBadge = "\u25cf DRAFT";
        LayoutStatusText = "Draft";
        IsDirty = false;
    }

    private async Task CloneLayoutAsync()
    {
        if (SelectedLayout == null)
        {
            await ShowErrorAsync("No layout selected to clone.");
            return;
        }

        if (await CheckDirtyStateAsync())
        {
            return;
        }

        // Create clone
        var clonedLayout = new TableLayoutDto
        {
            Id = Guid.NewGuid(),
            Name = $"{SelectedLayout.Name} Copy",
            FloorId = SelectedLayout.FloorId,
            IsDraft = true,
            IsActive = false,
            Tables = SelectedLayout.Tables.Select(t => new TableDto
            {
                Id = Guid.NewGuid(),
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                X = t.X,
                Y = t.Y,
                Shape = t.Shape,
                Width = t.Width,
                Height = t.Height,
                Status = TableStatus.Available,
                IsActive = true
            }).ToList()
        };

        // Add to collection and select
        Layouts.Add(clonedLayout);
        SelectedLayout = clonedLayout;

        // Load tables
        Tables.Clear();
        foreach (var table in clonedLayout.Tables)
        {
            Tables.Add(table);
        }

        // Update UI
        LayoutName = clonedLayout.Name;
        LayoutStatusBadge = "\u25cf DRAFT";
        LayoutStatusText = "Draft";
        IsDirty = false;
    }

    private async Task DeleteLayoutAsync()
    {
        if (SelectedLayout == null)
        {
            await ShowErrorAsync("No layout selected to delete.");
            return;
        }

        if (SelectedLayout.IsActive)
        {
            await ShowErrorAsync("Cannot delete active layout. Please deactivate it first.");
            return;
        }

        var confirmed = await ShowConfirmationAsync(
            "Delete Layout",
            $"Are you sure you want to delete layout '{SelectedLayout.Name}'? This action cannot be undone.");

        if (!confirmed) return;

        // Remove from collection
        Layouts.Remove(SelectedLayout);

        // Select first available layout or create blank
        SelectedLayout = Layouts.FirstOrDefault();

        if (SelectedLayout == null)
        {
            // No layouts left, create blank
            await NewLayoutAsync();
        }
    }

    private async Task PublishLayoutAsync()
    {
        if (SelectedLayout == null)
        {
            await ShowErrorAsync("No layout selected to publish.");
            return;
        }

        // Validate before publishing
        if (!Tables.Any())
        {
            await ShowErrorAsync("Cannot publish empty layout. Please add tables first.");
            return;
        }

        // Check if another layout is active
        var activeLayout = Layouts.FirstOrDefault(l => l.IsActive && l.Id != SelectedLayout.Id);
        if (activeLayout != null)
        {
            var confirmed = await ShowConfirmationAsync(
                "Publish Layout",
                $"Publishing '{SelectedLayout.Name}' will deactivate '{activeLayout.Name}'. Continue?");

            if (!confirmed) return;
        }

        IsBusy = true;
        try
        {
            // Save current layout first
            await SaveLayoutAsync();

            // Deactivate other layouts
            if (SelectedFloor != null && SelectedLayout.FloorId.HasValue)
            {
                await _tableLayoutRepository.DeactivateOtherLayoutsAsync(
                    SelectedLayout.FloorId.Value,
                    SelectedLayout.Id);
            }

            // Update local state
            foreach (var layout in Layouts.Where(l => l.Id != SelectedLayout.Id))
            {
                layout.IsActive = false;
            }

            SelectedLayout.IsActive = true;
            SelectedLayout.IsDraft = false;

            // Update UI
            LayoutStatusBadge = "\u2713 ACTIVE";
            LayoutStatusText = "Active";
            IsDesignMode = false; // Switch to view mode

            await ShowSuccessAsync($"Layout '{SelectedLayout.Name}' published successfully.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error publishing layout: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Checks if there are unsaved changes and prompts user to save.
    /// Returns true if user cancelled the operation.
    /// </summary>
    private async Task<bool> CheckDirtyStateAsync()
    {
        if (!IsDirty) return false;

        var result = await _dialogService.ShowConfirmationAsync(
            "Unsaved Changes",
            $"Save changes to '{LayoutName}' before continuing?",
            "Save",
            "Discard");

        if (!result) // User clicked Discard or closed dialog
        {
            return false; // Continue without saving
        }

        // User clicked Save
        await SaveLayoutAsync();
        return false; // Continue after saving
    }

    partial void OnSelectedFloorChanged(FloorDto? oldValue, FloorDto? newValue)
    {
        if (newValue != null && !IsBusy)
        {
            _ = LoadLayoutsForFloorAsync(newValue.Id);
        }
    }

    partial void OnSelectedLayoutChanged(TableLayoutDto? oldValue, TableLayoutDto? newValue)
    {
        if (newValue != null && !IsBusy)
        {
            _ = LoadLayoutTablesAsync(newValue);
        }
    }

    private async Task LoadLayoutsForFloorAsync(Guid floorId)
    {
        if (await CheckDirtyStateAsync())
        {
            // User cancelled, revert floor selection
            SelectedFloor = Floors.FirstOrDefault(f => f.Id == (SelectedLayout?.FloorId ?? Guid.Empty));
            return;
        }

        IsBusy = true;
        try
        {
            var layouts = await _tableLayoutRepository.GetLayoutsByFloorAsync(floorId);
            Layouts.Clear();
            foreach (var layout in layouts.OrderByDescending(l => l.IsActive).ThenBy(l => l.Name))
            {
                Layouts.Add(layout);
            }

            // Select active layout or first one
            SelectedLayout = Layouts.FirstOrDefault(l => l.IsActive) ?? Layouts.FirstOrDefault();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error loading layouts: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadLayoutTablesAsync(TableLayoutDto layout)
    {
        if (await CheckDirtyStateAsync())
        {
            // User cancelled, revert layout selection
            SelectedLayout = Layouts.FirstOrDefault(l => l.Id == _currentLayoutId);
            return;
        }

        IsBusy = true;
        try
        {
            // Load full layout with tables
            var fullLayout = await _tableLayoutRepository.GetLayoutWithTablesAsync(layout.Id);
            if (fullLayout != null)
            {
                Tables.Clear();
                foreach (var table in fullLayout.Tables)
                {
                    Tables.Add(table);
                }

                LayoutName = fullLayout.Name;
                _currentLayoutId = fullLayout.Id;

                // Update status badge
                if (fullLayout.IsActive)
                {
                    LayoutStatusBadge = "\u2713 ACTIVE";
                    LayoutStatusText = "Active";
                    IsDesignMode = false; // View mode for active layouts
                }
                else
                {
                    LayoutStatusBadge = "\u25cf DRAFT";
                    LayoutStatusText = "Draft";
                    IsDesignMode = true; // Design mode for drafts
                }

                IsDirty = false;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error loading layout tables: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
