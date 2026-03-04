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

using System.Threading.Tasks;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for UI interaction and helper methods.
/// Handles drag, selection, and helper dialogs.
/// </summary>
public partial class TableDesignerViewModel
{
    private void StartDrag(TableDto? table)
    {
        if (table == null || !IsDesignMode) return;
        SelectedTable = table;
    }

    private void SelectTable(TableDto? table)
    {
        if (table == null) return;
        SelectedTable = table;
    }

    private void ToggleDesignMode()
    {
        IsDesignMode = !IsDesignMode;
    }

    private async Task DiscardChangesAsync()
    {
        if (IsDirty)
        {
            var confirmed = await ShowConfirmationAsync("Discard Changes?", 
                "Are you sure you want to discard all unsaved changes to this layout?");
            if (!confirmed) return;
        }

        await LoadTablesAsync();
    }

    partial void OnSelectedFloorChanged(FloorDto? value)
    {
        if (value != null && !IsBusy)
        {
            _ = LoadTablesAsync();
        }
    }

    private int GetNextTableNumber()
    {
        return Tables.Count > 0 ? Tables.Max(t => t.TableNumber) + 1 : 1;
    }

    private async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        return await _dialogService.ShowConfirmationAsync(title, message);
    }

    private bool _isDialogOpen = false;

    private async Task ShowErrorAsync(string message)
    {
        // Prevent multiple dialogs from opening at once
        if (_isDialogOpen)
        {
            System.Diagnostics.Debug.WriteLine($"Dialog already open, queued error: {message}");
            return;
        }

        try
        {
            _isDialogOpen = true;
            await _dialogService.ShowErrorAsync("Table Designer Error", message);
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private async Task ShowSuccessAsync(string message)
    {
        await _dialogService.ShowMessageAsync("Success", message);
    }

    // Performance optimization methods
}
