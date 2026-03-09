using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for InventoryViewModel containing CRUD operation commands.
/// Handles create, edit, delete operations for inventory items and category management.
/// </summary>
public partial class InventoryViewModel
{
    /// <summary>
    /// Event raised when the user requests to create a new inventory item.
    /// The UI layer should handle this event to show the CreateInventoryItemDialog.
    /// </summary>
    public event EventHandler? CreateItemRequested;

    /// <summary>
    /// Event raised when the user requests to edit an existing inventory item.
    /// The UI layer should handle this event to show the EditInventoryItemDialog.
    /// </summary>
    public event EventHandler<Guid>? EditItemRequested;

    /// <summary>
    /// Event raised when the user requests to delete an inventory item.
    /// The UI layer should handle this event to show a confirmation dialog.
    /// </summary>
    public event EventHandler<Guid>? DeleteItemRequested;

    /// <summary>
    /// Event raised when the user requests to manage inventory categories.
    /// The UI layer should handle this event to show the CategoryManagementDialog.
    /// </summary>
    public event EventHandler? CategoryManagementRequested;

    /// <summary>
    /// Command to open the create inventory item dialog.
    /// Raises the CreateItemRequested event for the UI layer to handle.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteCrudCommands))]
    private void OpenCreateItem()
    {
        CreateItemRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Command to open the edit inventory item dialog for a specific item.
    /// Raises the EditItemRequested event with the item ID for the UI layer to handle.
    /// </summary>
    /// <param name="itemId">The unique identifier of the inventory item to edit.</param>
    [RelayCommand(CanExecute = nameof(CanExecuteCrudCommands))]
    private void OpenEditItem(Guid itemId)
    {
        EditItemRequested?.Invoke(this, itemId);
    }

    /// <summary>
    /// Command to delete an inventory item with confirmation.
    /// Raises the DeleteItemRequested event for the UI layer to show a confirmation dialog.
    /// The UI layer should call OnItemDeletedAsync() after successful deletion.
    /// </summary>
    /// <param name="itemId">The unique identifier of the inventory item to delete.</param>
    [RelayCommand(CanExecute = nameof(CanExecuteCrudCommands))]
    private void DeleteItem(Guid itemId)
    {
        DeleteItemRequested?.Invoke(this, itemId);
    }

    /// <summary>
    /// Command to open the category management dialog.
    /// Raises the CategoryManagementRequested event for the UI layer to handle.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteCrudCommands))]
    private void OpenCategoryManagement()
    {
        CategoryManagementRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Determines if CRUD commands can be executed.
    /// Commands are disabled when the ViewModel is busy.
    /// </summary>
    private bool CanExecuteCrudCommands() => !IsBusy;

    /// <summary>
    /// Notifies CRUD commands to re-evaluate their CanExecute state when IsBusy changes.
    /// This method should be called from the OnIsBusyChanged partial method.
    /// </summary>
    private void NotifyCrudCommandsCanExecuteChanged()
    {
        OpenCreateItemCommand.NotifyCanExecuteChanged();
        OpenEditItemCommand.NotifyCanExecuteChanged();
        DeleteItemCommand.NotifyCanExecuteChanged();
        OpenCategoryManagementCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Refreshes the inventory list after a CRUD operation.
    /// Called by the UI layer after successful create/edit/delete operations.
    /// </summary>
    public async Task OnItemCreatedAsync()
    {
        await LoadPageCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Refreshes the inventory list after an item is updated.
    /// Called by the UI layer after successful edit operations.
    /// </summary>
    public async Task OnItemUpdatedAsync()
    {
        await LoadPageCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Refreshes the inventory list after an item is deleted.
    /// Called by the UI layer after successful delete operations.
    /// </summary>
    public async Task OnItemDeletedAsync()
    {
        await LoadPageCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Refreshes the inventory data after categories are changed.
    /// Called by the UI layer after successful category management operations.
    /// Reloads both the category list and the current page to reflect any category changes.
    /// </summary>
    public async Task OnCategoriesChangedAsync()
    {
        await LoadCategoriesCommand.ExecuteAsync(null);
        await LoadPageCommand.ExecuteAsync(null);
    }
}
