using System.Collections.ObjectModel;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class MenuEditorViewModel : ViewModelBase
{
    private readonly IMenuCategoryRepository _categoryRepository;
    private readonly IMenuGroupRepository _groupRepository;
    private readonly IMenuRepository _menuRepository;

    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IPrinterGroupRepository _printerGroupRepository;
    
    // ... (fields)

    private MenuCategory? _selectedCategory;
    private bool _isEditing;
    
    private string _editingName = string.Empty;
    private int _editingSortOrder;
    
    public string EditingName
    {
        get => _editingName;
        set => SetProperty(ref _editingName, value);
    }
    
    public int EditingSortOrder
    {
        get => _editingSortOrder;
        set => SetProperty(ref _editingSortOrder, value);
    }
    
    private string _editingPrice = "0.00";
    public string EditingPrice
    {
        get => _editingPrice;
        set => SetProperty(ref _editingPrice, value);
    }
    
    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    
    public ObservableCollection<MenuCategory> Categories { get; } = new();
    public ObservableCollection<MenuGroup> Groups { get; } = new();
    public ObservableCollection<MenuItem> Items { get; } = new();
    public ObservableCollection<PrinterGroup> PrinterGroups { get; } = new();

    private PrinterGroup? _selectedPrinterGroup;
    public PrinterGroup? SelectedPrinterGroup
    {
        get => _selectedPrinterGroup;
        set => SetProperty(ref _selectedPrinterGroup, value);
    }

    // Recipe Editor Properties
    public ObservableCollection<InventoryItem> InventoryOptions { get; } = new();
    public ObservableCollection<RecipeLineViewModel> RecipeLines { get; } = new();
    
    private InventoryItem? _selectedInventoryOption;
    public InventoryItem? SelectedInventoryOption
    {
        get => _selectedInventoryOption;
        set => SetProperty(ref _selectedInventoryOption, value);
    }
    
    private string _newRecipeQuantity = "1";
    public string NewRecipeQuantity
    {
        get => _newRecipeQuantity;
        set => SetProperty(ref _newRecipeQuantity, value);
    }
    
    private RecipeLineViewModel? _selectedRecipeLine;
    public RecipeLineViewModel? SelectedRecipeLine
    {
        get => _selectedRecipeLine;
        set => SetProperty(ref _selectedRecipeLine, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddCategoryCommand { get; }
    public ICommand AddGroupCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand AddRecipeLineCommand { get; }
    public ICommand RemoveRecipeLineCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    // Properties for binding the Detail View
    public MenuCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                // Reset Child Selection
                SelectedGroup = null;
                Groups.Clear();
                
                // Reset lower level
                SelectedItem = null;
                Items.Clear();
                
                if (value != null)
                {
                    IsEditing = true;
                    EditingName = value.Name;
                    EditingSortOrder = value.SortOrder;
                    StatusMessage = $"Editing Category: {value.Name}";
                    
                    if (value.PrinterGroupId.HasValue)
                        SelectedPrinterGroup = PrinterGroups.FirstOrDefault(pg => pg.Id == value.PrinterGroupId);
                    else
                        SelectedPrinterGroup = null;

                    _ = LoadGroupsAsync(value.Id);
                }
                else
                {
                    IsEditing = false;
                    StatusMessage = "Ready";
                }
            }
        }
    }
    
    private MenuGroup? _selectedGroup;
    public MenuGroup? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (SetProperty(ref _selectedGroup, value))
            {
                // Reset Child Selection
                SelectedItem = null;
                Items.Clear();

                if (value != null)
                {
                    IsEditing = true;
                    EditingName = value.Name;
                    EditingSortOrder = value.SortOrder; 
                    StatusMessage = $"Editing Group: {value.Name}";
                     
                    if (value.PrinterGroupId.HasValue)
                        SelectedPrinterGroup = PrinterGroups.FirstOrDefault(pg => pg.Id == value.PrinterGroupId);
                    else
                        SelectedPrinterGroup = null;

                    _ = LoadItemsAsync(value.CategoryId, value.Id);
                }
                else if (SelectedCategory != null)
                {
                     // Fallback to Category
                    EditingName = SelectedCategory.Name;
                    EditingSortOrder = SelectedCategory.SortOrder;
                    StatusMessage = $"Editing Category: {SelectedCategory.Name}";
                }
            }
        }
    }
    
    private MenuItem? _selectedItem;
    public MenuItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                if (value != null)
                {
                    IsEditing = true;
                    EditingName = value.Name;
                    EditingPrice = value.Price.Amount.ToString("F2"); 
                    StatusMessage = $"Editing Item: {value.Name}";
                    
                    if (value.PrinterGroupId.HasValue)
                    {
                        SelectedPrinterGroup = PrinterGroups.FirstOrDefault(pg => pg.Id == value.PrinterGroupId);
                    }
                    else
                    {
                        SelectedPrinterGroup = null;
                    }

                    _ = LoadRecipeLinesAsync(value);
                }
                else if (SelectedGroup != null)
                {
                    EditingName = SelectedGroup.Name;
                    EditingSortOrder = SelectedGroup.SortOrder;
                    StatusMessage = $"Editing Group: {SelectedGroup.Name}";
                }
            }
        }
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public MenuEditorViewModel(
        IMenuCategoryRepository categoryRepository,
        IMenuGroupRepository groupRepository,
        IMenuRepository menuRepository,
        IInventoryItemRepository inventoryRepository,
        IPrinterGroupRepository printerGroupRepository)
    {
        _categoryRepository = categoryRepository;
        _groupRepository = groupRepository;
        _menuRepository = menuRepository;
        _inventoryRepository = inventoryRepository;
        _printerGroupRepository = printerGroupRepository;
        Title = "Menu Editor";

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddCategoryCommand = new AsyncRelayCommand(AddCategoryAsync);
        AddGroupCommand = new AsyncRelayCommand(AddGroupAsync);
        AddItemCommand = new AsyncRelayCommand(AddItemAsync);
        
        AddRecipeLineCommand = new AsyncRelayCommand(AddRecipeLineAsync);
        RemoveRecipeLineCommand = new AsyncRelayCommand(RemoveRecipeLineAsync);
        
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task SaveAsync()
    {
        IsBusy = true;
        try
        {
            if (SelectedItem != null)
            {
                 SelectedItem.UpdateName(EditingName);
                 if (decimal.TryParse(EditingPrice, out var priceVal))
                 {
                     SelectedItem.UpdatePrice(new Magidesk.Domain.ValueObjects.Money(priceVal));
                 }
                 
                 // F-00XX: Printer Group Persistence (Item)
                 SelectedItem.SetPrinterGroup(SelectedPrinterGroup?.Id);
                 
                 await _menuRepository.UpdateAsync(SelectedItem);
                 StatusMessage = "Saved Item & Recipe successfully.";
            }
            else if (SelectedGroup != null)
            {
                 SelectedGroup.UpdateName(EditingName);
                 SelectedGroup.UpdateSortOrder(EditingSortOrder);
                 
                 // F-00XX: Printer Group Persistence (Group)
                 SelectedGroup.SetPrinterGroup(SelectedPrinterGroup?.Id);
                 
                 await _groupRepository.UpdateAsync(SelectedGroup);
                 StatusMessage = "Saved Group successfully.";
            }
            else if (SelectedCategory != null)
            {
                SelectedCategory.UpdateName(EditingName);
                SelectedCategory.UpdateSortOrder(EditingSortOrder);
                
                // F-00XX: Printer Group Persistence (Category)
                SelectedCategory.SetPrinterGroup(SelectedPrinterGroup?.Id);
                
                await _categoryRepository.UpdateAsync(SelectedCategory);
                StatusMessage = "Saved Category successfully.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save Failed: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task DeleteAsync()
    {
        IsBusy = true;
        try
        {
            if (SelectedItem != null)
            {
                var item = SelectedItem;
                await _menuRepository.DeleteAsync(item);
                Items.Remove(item);
                SelectedItem = null;
                StatusMessage = "Item deleted.";
                return;
            }

            if (SelectedGroup != null)
            {
                var group = SelectedGroup;

                var items = (await _menuRepository.GetAllAsync()).Where(i => i.GroupId == group.Id).ToList();
                foreach (var item in items)
                {
                    await _menuRepository.DeleteAsync(item);
                }

                await _groupRepository.DeleteAsync(group);

                Groups.Remove(group);
                Items.Clear();
                SelectedItem = null;
                SelectedGroup = null;
                StatusMessage = "Group deleted.";
                return;
            }

            if (SelectedCategory != null)
            {
                var category = SelectedCategory;

                var groups = (await _groupRepository.GetAllAsync()).Where(g => g.CategoryId == category.Id).ToList();
                foreach (var group in groups)
                {
                    var items = (await _menuRepository.GetAllAsync()).Where(i => i.GroupId == group.Id).ToList();
                    foreach (var item in items)
                    {
                        await _menuRepository.DeleteAsync(item);
                    }

                    await _groupRepository.DeleteAsync(group);
                }

                await _categoryRepository.DeleteAsync(category);

                Categories.Remove(category);
                Groups.Clear();
                Items.Clear();
                SelectedItem = null;
                SelectedGroup = null;
                SelectedCategory = null;
                StatusMessage = "Category deleted.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Delete Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddCategoryAsync()
    {
        var newCat = MenuCategory.Create("New Category", 0, false);
        
        IsBusy = true;
        try 
        {
            await _categoryRepository.AddAsync(newCat);
            Categories.Add(newCat);
            SelectedCategory = newCat; 
            StatusMessage = "Category Added";
        }
        finally { IsBusy = false; }
    }
    
    private async Task AddItemAsync()
    {
        if (SelectedGroup == null || SelectedCategory == null)
        {
            StatusMessage = "Select a Group first.";
            return;
        }

        IsBusy = true;
        try
        {
            var newItem = MenuItem.Create("New Item", Magidesk.Domain.ValueObjects.Money.Zero());
            newItem.SetCategory(SelectedCategory.Id);
            newItem.SetGroup(SelectedGroup.Id);
            
            await _menuRepository.AddAsync(newItem);
            Items.Add(newItem);
            SelectedItem = newItem;
            StatusMessage = "Item Added";
        }
        finally { IsBusy = false; }
    }
    
    private async Task AddGroupAsync()
    {
        if (SelectedCategory == null)
        {
            StatusMessage = "Select a Category first.";
            return;
        }

        IsBusy = true;
        try
        {
            var newGroup = MenuGroup.Create("New Group", SelectedCategory.Id);
            await _groupRepository.AddAsync(newGroup);
            Groups.Add(newGroup);
            SelectedGroup = newGroup;
            StatusMessage = "Group Added";
        }
        finally { IsBusy = false; }
    }

    private async Task LoadDataAsync()
    {
         IsBusy = true;
        try
        {
            Categories.Clear();
            var cats = await _categoryRepository.GetAllAsync();
            foreach (var cat in cats) Categories.Add(cat);
            
            // Load Inventory Options
            InventoryOptions.Clear();
            var invItems = await _inventoryRepository.GetAllAsync();
            foreach (var inv in invItems) InventoryOptions.Add(inv);

            // Load Printer Groups
            PrinterGroups.Clear();
            var printerGroups = await _printerGroupRepository.GetAllAsync();
            foreach (var pg in printerGroups) PrinterGroups.Add(pg);

            StatusMessage = "Loaded successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
    
    private async Task LoadGroupsAsync(Guid categoryId)
    {
        IsBusy = true;
        try
        {
            var groups = await _groupRepository.GetByCategoryIdAsync(categoryId);
            Groups.Clear();
            foreach (var g in groups) Groups.Add(g);
        }
        finally { IsBusy = false; }
    }

    private async Task LoadItemsAsync(Guid categoryId, Guid groupId)
    {
        IsBusy = true;
        try
        {
            var items = await _menuRepository.GetByGroupAsync(groupId); 
            Items.Clear();
            foreach (var i in items) Items.Add(i);
        }
        catch (Exception ex)
        {
             StatusMessage = $"Load Items Failed: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
    
    private async Task LoadRecipeLinesAsync(MenuItem item)
    {
        RecipeLines.Clear();
        
        // Ensure options loaded
        if (InventoryOptions.Count == 0)
        {
             var invItems = await _inventoryRepository.GetAllAsync();
             foreach (var inv in invItems) InventoryOptions.Add(inv);
        }
        
        foreach (var line in item.RecipeLines)
        {
            var invItem = InventoryOptions.FirstOrDefault(x => x.Id == line.InventoryItemId);
            if (invItem != null)
            {
                RecipeLines.Add(new RecipeLineViewModel(invItem, line.Quantity));
            }
        }
    }
    
    private async Task AddRecipeLineAsync()
    {
        if (SelectedItem == null || SelectedInventoryOption == null) return;
        
        if (decimal.TryParse(NewRecipeQuantity, out var qty))
        {
            // Add to Domain Entity
            SelectedItem.AddRecipeLine(SelectedInventoryOption.Id, qty);
            
            // Add to UI
            RecipeLines.Add(new RecipeLineViewModel(SelectedInventoryOption, qty));
            
            // Note: Not saving yet, User must click Save.
            StatusMessage = "Recipe line added.";
        }
        else
        {
            StatusMessage = "Invalid Quantity";
        }
    }
    
    private async Task RemoveRecipeLineAsync()
    {
        if (SelectedItem == null || SelectedRecipeLine == null) return;
        
        // Remove from Domain Entity
        SelectedItem.RemoveRecipeLine(SelectedRecipeLine.InventoryItemId);
        
        // Remove from UI
        RecipeLines.Remove(SelectedRecipeLine);
        SelectedRecipeLine = null;
        StatusMessage = "Recipe line removed.";
    }
}
