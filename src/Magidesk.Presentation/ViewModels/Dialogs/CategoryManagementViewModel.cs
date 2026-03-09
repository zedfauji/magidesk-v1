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
using MediatR;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for managing inventory categories (CRUD operations).
/// Supports creating, editing, and deleting categories with validation.
/// </summary>
public partial class CategoryManagementViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IQueryHandler<GetInventoryCategoriesQuery, List<InventoryCategoryDto>> _getCategoriesHandler;
    private readonly ILogger<CategoryManagementViewModel> _logger;

    private InventoryCategoryDto? _selectedCategory;
    private string _newCategoryName = string.Empty;
    private int _newCategorySortOrder;
    private string _editCategoryName = string.Empty;
    private int _editCategorySortOrder;
    private bool _isEditMode;
    private readonly Dictionary<string, string> _validationErrors = new();

    /// <summary>
    /// Gets the collection of active categories.
    /// </summary>
    public ObservableCollection<InventoryCategoryDto> Categories { get; } = new();

    /// <summary>
    /// Gets or sets the selected category for editing or deletion.
    /// </summary>
    public InventoryCategoryDto? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    /// <summary>
    /// Gets or sets the name for a new category.
    /// </summary>
    public string NewCategoryName
    {
        get => _newCategoryName;
        set
        {
            if (SetProperty(ref _newCategoryName, value))
            {
                ValidateNewCategoryName();
                CreateCategoryCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the sort order for a new category.
    /// </summary>
    public int NewCategorySortOrder
    {
        get => _newCategorySortOrder;
        set
        {
            if (SetProperty(ref _newCategorySortOrder, value))
            {
                ValidateNewCategorySortOrder();
                CreateCategoryCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the name for editing the selected category.
    /// </summary>
    public string EditCategoryName
    {
        get => _editCategoryName;
        set
        {
            if (SetProperty(ref _editCategoryName, value))
            {
                ValidateEditCategoryName();
                UpdateCategoryCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the sort order for editing the selected category.
    /// </summary>
    public int EditCategorySortOrder
    {
        get => _editCategorySortOrder;
        set
        {
            if (SetProperty(ref _editCategorySortOrder, value))
            {
                ValidateEditCategorySortOrder();
                UpdateCategoryCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is in edit mode.
    /// </summary>
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    /// <summary>
    /// Gets the validation errors dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, string> ValidationErrors => _validationErrors;

    /// <summary>
    /// Gets a value indicating whether a new category can be created.
    /// </summary>
    public bool CanCreateCategory => 
        !string.IsNullOrWhiteSpace(_newCategoryName) && 
        _newCategorySortOrder >= 0 &&
        !_validationErrors.ContainsKey(nameof(NewCategoryName)) &&
        !_validationErrors.ContainsKey(nameof(NewCategorySortOrder));

    /// <summary>
    /// Gets a value indicating whether the selected category can be updated.
    /// </summary>
    public bool CanUpdateCategory => 
        _selectedCategory != null &&
        !string.IsNullOrWhiteSpace(_editCategoryName) && 
        _editCategorySortOrder >= 0 &&
        !_validationErrors.ContainsKey(nameof(EditCategoryName)) &&
        !_validationErrors.ContainsKey(nameof(EditCategorySortOrder));

    /// <summary>
    /// Event raised when categories are modified (created, updated, or deleted).
    /// </summary>
    public event EventHandler? CategoriesChanged;

    /// <summary>
    /// Gets the command to load categories from the database.
    /// </summary>
    public IAsyncRelayCommand LoadCategoriesCommand { get; }

    /// <summary>
    /// Gets the command to create a new category.
    /// </summary>
    public IAsyncRelayCommand CreateCategoryCommand { get; }

    /// <summary>
    /// Gets the command to update the selected category.
    /// </summary>
    public IAsyncRelayCommand UpdateCategoryCommand { get; }

    /// <summary>
    /// Gets the command to delete a category.
    /// </summary>
    public IAsyncRelayCommand<Guid> DeleteCategoryCommand { get; }

    /// <summary>
    /// Gets the command to enter edit mode for the selected category.
    /// </summary>
    public IRelayCommand EnterEditModeCommand { get; }

    /// <summary>
    /// Gets the command to cancel edit mode.
    /// </summary>
    public IRelayCommand CancelEditCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryManagementViewModel"/> class.
    /// </summary>
    public CategoryManagementViewModel(
        IMediator mediator,
        IQueryHandler<GetInventoryCategoriesQuery, List<InventoryCategoryDto>> getCategoriesHandler,
        ILogger<CategoryManagementViewModel> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _getCategoriesHandler = getCategoriesHandler ?? throw new ArgumentNullException(nameof(getCategoriesHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadCategoriesCommand = new AsyncRelayCommand(ExecuteLoadCategoriesAsync);
        CreateCategoryCommand = new AsyncRelayCommand(ExecuteCreateCategoryAsync, CanExecuteCreateCategory);
        UpdateCategoryCommand = new AsyncRelayCommand(ExecuteUpdateCategoryAsync, CanExecuteUpdateCategory);
        DeleteCategoryCommand = new AsyncRelayCommand<Guid>(ExecuteDeleteCategoryAsync);
        EnterEditModeCommand = new RelayCommand(ExecuteEnterEditMode, CanExecuteEnterEditMode);
        CancelEditCommand = new RelayCommand(ExecuteCancelEdit);

        Title = "Manage Categories";
    }
}
