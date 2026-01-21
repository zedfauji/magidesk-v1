using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels;

public partial class TableDesignerViewModel
{
    // ============================================================================
    // SAVE/LOAD ENHANCEMENTS (Phase 4)
    // ============================================================================

    /// <summary>
    /// Reverts all changes and reloads the current layout from the database.
    /// </summary>
    public async Task RevertChangesAsync()
    {
        if (!IsDirty)
        {
            await ShowMessageAsync("No Changes", "There are no unsaved changes to revert.");
            return;
        }

        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Revert Changes",
            "Are you sure you want to discard all unsaved changes? This cannot be undone.",
            "Revert",
            "Cancel");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            if (SelectedLayout != null)
            {
                // Reload the layout from database
                await LoadLayoutTablesAsync(SelectedLayout);
                await ShowSuccessAsync("Changes reverted successfully.");
            }
            else
            {
                // No layout selected, just clear tables
                Tables.Clear();
                IsDirty = false;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error reverting changes: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Shows a message dialog.
    /// </summary>
    private async Task ShowMessageAsync(string title, string message)
    {
        await _dialogService.ShowMessageAsync(title, message);
    }

    /// <summary>
    /// Checks if save button should be enabled.
    /// </summary>
    public bool CanSave()
    {
        return IsDirty && 
               !string.IsNullOrWhiteSpace(LayoutName) && 
               Tables.Any() && 
               !HasValidationErrors;
    }

    /// <summary>
    /// Checks if publish button should be enabled.
    /// </summary>
    public bool CanPublish()
    {
        return SelectedLayout != null &&
               !SelectedLayout.IsActive &&
               Tables.Any() &&
               !HasValidationErrors &&
               !IsDirty; // Must save before publishing
    }

    /// <summary>
    /// Updates command can-execute states.
    /// </summary>
    public void UpdateCommandStates()
    {
        // Trigger property changed for computed properties
        OnPropertyChanged(nameof(CanSave));
        OnPropertyChanged(nameof(CanPublish));
    }
}
