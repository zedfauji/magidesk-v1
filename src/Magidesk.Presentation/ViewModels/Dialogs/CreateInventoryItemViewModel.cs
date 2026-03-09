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
/// ViewModel for creating new inventory items.
/// </summary>
public partial class CreateInventoryItemViewModel : ViewModelBase
{
    private readonly ICommandHandler<CreateInventoryItemCommand, Guid> _createHandler;
    private readonly IQueryHandler<GetInventoryCategoriesQuery, List<InventoryCategoryDto>> _getCategoriesHandler;
    private readonly ILogger<CreateInventoryItemViewModel> _logger;

    private string _name = string.Empty;
    private string _unit = string.Empty;
    private string? _skuCode;
    private decimal _stockQuantity;
    private decimal _reorderPoint;
    private Guid? _selectedCategoryId;
    private string? _errorMessage;
    private readonly Dictionary<string, string> _validationErrors = new();

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
    /// Event raised when an item is successfully created.
    /// </summary>
    public event EventHandler<Guid>? ItemCreated;

    /// <summary>
    /// Event raised when the user cancels the operation.
    /// </summary>
    public event EventHandler? Cancelled;

    /// <summary>
    /// Gets the command to confirm and create the item.
    /// </summary>
    public IAsyncRelayCommand ConfirmCommand { get; }

    /// <summary>
    /// Gets the command to cancel the operation.
    /// </summary>
    public IRelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInventoryItemViewModel"/> class.
    /// </summary>
    public CreateInventoryItemViewModel(
        ICommandHandler<CreateInventoryItemCommand, Guid> createHandler,
        IQueryHandler<GetInventoryCategoriesQuery, List<InventoryCategoryDto>> getCategoriesHandler,
        ILogger<CreateInventoryItemViewModel> logger)
    {
        _createHandler = createHandler ?? throw new ArgumentNullException(nameof(createHandler));
        _getCategoriesHandler = getCategoriesHandler ?? throw new ArgumentNullException(nameof(getCategoriesHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ConfirmCommand = new AsyncRelayCommand(ExecuteConfirmAsync, CanExecuteConfirm);
        CancelCommand = new RelayCommand(ExecuteCancel);

        Title = "Create Inventory Item";
    }

    /// <summary>
    /// Loads categories from the database.
    /// </summary>
    public async Task LoadCategoriesAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;

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
            ErrorMessage = "Failed to load categories. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecuteConfirmAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var command = new CreateInventoryItemCommand(
                Name,
                Unit,
                StockQuantity,
                ReorderPoint,
                SkuCode,
                SelectedCategoryId);

            var itemId = await _createHandler.HandleAsync(command);

            _logger.LogInformation("Created inventory item {ItemId} with name {Name}", itemId, Name);

            ItemCreated?.Invoke(this, itemId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create inventory item: {Message}", ex.Message);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating inventory item");
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

    private void ValidateName()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors[nameof(Name)] = "Name is required.";
        }
        else if (_name.Length > 200)
        {
            _validationErrors[nameof(Name)] = "Name must not exceed 200 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(Name));
        }
    }

    private void ValidateUnit()
    {
        if (string.IsNullOrWhiteSpace(_unit))
        {
            _validationErrors[nameof(Unit)] = "Unit is required.";
        }
        else if (_unit.Length > 50)
        {
            _validationErrors[nameof(Unit)] = "Unit must not exceed 50 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(Unit));
        }
    }

    private void ValidateSkuCode()
    {
        if (!string.IsNullOrWhiteSpace(_skuCode) && _skuCode.Length > 50)
        {
            _validationErrors[nameof(SkuCode)] = "SKU code must not exceed 50 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(SkuCode));
        }
    }

    private void ValidateStockQuantity()
    {
        if (_stockQuantity < 0)
        {
            _validationErrors[nameof(StockQuantity)] = "Stock quantity must be non-negative.";
        }
        else
        {
            _validationErrors.Remove(nameof(StockQuantity));
        }
    }

    private void ValidateReorderPoint()
    {
        if (_reorderPoint < 0)
        {
            _validationErrors[nameof(ReorderPoint)] = "Reorder point must be non-negative.";
        }
        else
        {
            _validationErrors.Remove(nameof(ReorderPoint));
        }
    }
}
