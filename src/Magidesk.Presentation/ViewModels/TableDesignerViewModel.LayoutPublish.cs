using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;
using Windows.Foundation;
using MediatR;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels;

public partial class TableDesignerViewModel
{
    // ============================================================================
    // LAYOUT PUBLISH METHODS (Phase 1 - Continued)
    // ============================================================================

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
}
