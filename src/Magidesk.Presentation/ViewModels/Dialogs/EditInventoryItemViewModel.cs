using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.Inventory;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for editing existing inventory items.
/// </summary>
public partial class EditInventoryItemViewModel : ViewModelBase
{
    private readonly ICommandHandler<UpdateInventoryItemCommand> _updateHandler;
    private readonly IQueryHandler<GetInventoryItemByIdQuery, InventoryItemDto?> _getItemHandler;
    private readonly IQueryHandler<GetInventoryCategoriesQuery, List<InventoryCategoryDto>> _getCategoriesHandler;
    private readonly ILogger<EditInventoryItemViewModel> _logger;

    private Guid _itemId;
    private string _name = string.Empty;
    private string _unit = string.Empty;
    private string? _skuCode;
    private decimal _stockQuantity;
    private decimal _reorderPoint;
    private Guid? _selectedCategoryId;
    private bool _isActive = true;
    private string? _errorMessage;
    private readonly Dictionary<string, string> _validationErrors = new();

    /// <summary>
    /// Gets the item ID being edited.
    /// </summary>
    public Guid ItemId
    {
        get => _itemId;
        private set => SetProperty(ref _itemId, value);
    }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                ValidateName();
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the unit of measure.
    /// </summary>
    public string Unit
    {
        get => _unit;
        set
        {
            if (SetProperty(ref _unit, value))
            {
                ValidateUnit();
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the optional SKU code.
    /// </summary>
    public string? SkuCode
    {
        get => _skuCode;
        set
        {
            if (SetProperty(ref _skuCode, value))
            {
                ValidateSkuCode();
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the stock quantity.
    /// </summary>
    public decimal StockQuantity
    {
        get => _stockQuantity;
        set
        {
            if (SetProperty(ref _stockQuantity, value))
            {
                ValidateStockQuantity();
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the reorder point.
    /// </summary>
    public decimal ReorderPoint
    {
        get => _reorderPoint;
        set
        {
            if (SetProperty(ref _reorderPoint, value))
            {
                ValidateReorderPoint();
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected category ID.
    /// </summary>
    public Guid? SelectedCategoryId
    {
        get => _selectedCategoryId;
        set => SetProperty(ref _selectedCategoryId, value);
    }

    /// <summary>
    /// Gets or sets whether the item is active.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Gets the collection of available categories.
    /// </summary>
    public ObservableCollection<InventoryCategoryDto> Categories { get; } = new();

    /// <summary>
    /// Gets the validation errors dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, string> ValidationErrors => _validationErrors;

    /// <summary>
    /// Gets a value indicating whether the form is valid.
    /// </summary>
    public bool IsValid => _validationErrors.Count == 0;

    /// <summary>
    /// Event raised when an item is successfully updated.
    /// </summary>
    public event EventHandler<Guid>? ItemUpdated;

    /// <summary>
    /// Event raised when the user cancels the operation.
    /// </summary>
    public event EventHandler? Cancelled;

    /// <summary>
    /// Gets the command to confirm and update the item.
    /// </summary>
    public IAsyncRelayCommand ConfirmCommand { get; }

    /// <summary>
    /// Gets the command to cancel the operation.
    /// </summary>
    public IRelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditInventoryItemViewModel"/> class.
    /// </summary>
    public EditInventoryItemViewModel(
        ICommandHandler<UpdateInventoryItemCommand> updateHandler,
        IQueryHandler<GetInventoryItemByIdQuery, InventoryItemDto?> getItemHandler,
        IQueryHandler<GetInventoryCategoriesQuery, List<InventoryCategoryDto>> getCategoriesHandler,
        ILogger<EditInventoryItemViewModel> logger)
    {
        _updateHandler = updateHandler ?? throw new ArgumentNullException(nameof(updateHandler));
        _getItemHandler = getItemHandler ?? throw new ArgumentNullException(nameof(getItemHandler));
        _getCategoriesHandler = getCategoriesHandler ?? throw new ArgumentNullException(nameof(getCategoriesHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ConfirmCommand = new AsyncRelayCommand(ExecuteConfirmAsync, CanExecuteConfirm);
        CancelCommand = new RelayCommand(ExecuteCancel);

        Title = "Edit Inventory Item";
    }

    /// <summary>
    /// Loads the item data and categories from the database.
    /// </summary>
    /// <param name="itemId">The ID of the item to load.</param>
    public async Task LoadItemAsync(Guid itemId)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            await LoadCategoriesAsync();

            var query = new GetInventoryItemByIdQuery(itemId);
            var item = await _getItemHandler.HandleAsync(query);

            if (item == null)
            {
                ErrorMessage = "Item not found";
                _logger.LogWarning("Item {ItemId} not found", itemId);
                return;
            }

            ItemId = item.Id;
            Name = item.Name;
            Unit = item.Unit;
            SkuCode = item.SkuCode;
            StockQuantity = item.StockQuantity;
            ReorderPoint = item.ReorderPoint;
            SelectedCategoryId = item.CategoryId;
            IsActive = item.IsActive;

            _logger.LogInformation("Loaded item {ItemId} for editing", itemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load item {ItemId}", itemId);
            ErrorMessage = "Failed to load item. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var query = new GetInventoryCategoriesQuery();
            var categories = await _getCategoriesHandler.HandleAsync(query);

            Categories.Clear();
            foreach (var category in categories.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                Categories.Add(category);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
        }
    }

    private async Task ExecuteConfirmAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var command = new UpdateInventoryItemCommand(
                ItemId,
                Name,
                Unit,
                StockQuantity,
                ReorderPoint,
                SkuCode,
                SelectedCategoryId,
                IsActive);

            await _updateHandler.HandleAsync(command);

            _logger.LogInformation("Updated inventory item {ItemId} with name {Name}", ItemId, Name);

            ItemUpdated?.Invoke(this, ItemId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update inventory item: {Message}", ex.Message);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating inventory item");
            ErrorMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanExecuteConfirm()
    {
        return IsValid && !IsBusy;
    }

    private void ExecuteCancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
}
