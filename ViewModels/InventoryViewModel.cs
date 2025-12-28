using System.Collections.ObjectModel;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.ViewModels;

public class InventoryViewModel : ViewModelBase
{
    private readonly IInventoryItemRepository _inventoryRepository;
    
    private InventoryItem? _selectedItem;
    private string _editingName = string.Empty;
    private string _editingUnit = "unit";
    private string _editingStock = "0";
    private string _editingReorder = "0";
    private bool _isEditing;

    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();

    public InventoryItem? SelectedItem
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
                    EditingUnit = value.Unit;
                    EditingStock = value.StockQuantity.ToString();
                    EditingReorder = value.ReorderPoint.ToString();
                    StatusMessage = $"Editing: {value.Name}";
                }
                else
                {
                    IsEditing = false;
                    StatusMessage = "Ready";
                }
            }
        }
    }

    public string EditingName { get => _editingName; set => SetProperty(ref _editingName, value); }
    public string EditingUnit { get => _editingUnit; set => SetProperty(ref _editingUnit, value); }
    public string EditingStock { get => _editingStock; set => SetProperty(ref _editingStock, value); }
    public string EditingReorder { get => _editingReorder; set => SetProperty(ref _editingReorder, value); }
    
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    private string _statusMessage = "Ready";
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public ICommand LoadDataCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public InventoryViewModel(IInventoryItemRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
        Title = "Inventory Management";
        
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddItemCommand = new AsyncRelayCommand(AddItemAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            InventoryItems.Clear();
            var items = await _inventoryRepository.GetAllAsync();
            foreach (var item in items) InventoryItems.Add(item);
            StatusMessage = $"Loaded {InventoryItems.Count} items";
        }
        finally { IsBusy = false; }
    }

    private async Task AddItemAsync()
    {
        IsBusy = true;
        try
        {
            var newItem = InventoryItem.Create("New Item", "unit", 0, 0);
            await _inventoryRepository.AddAsync(newItem);
            InventoryItems.Add(newItem);
            SelectedItem = newItem;
            StatusMessage = "New item added";
        }
        finally { IsBusy = false; }
    }

    private async Task SaveAsync()
    {
        if (SelectedItem == null) return;
        
        IsBusy = true;
        try
        {
            if (decimal.TryParse(EditingStock, out var stock) && decimal.TryParse(EditingReorder, out var reorder))
            {
               // Domain entity might not have direct setters for these if adhering to strict DDD, 
               // but looking at InventoryItem.cs in step 7709:
               // UpdateName, UpdateUnit, AdjustStock, SetReorderPoint.
               // AdjustStock is delta-based. So we need to calculate delta or set directly if allowed?
               // The entity has `StockQuantity` with private set. `AdjustStock` adds delta.
               // To set absolute value: delta = new - current.
               
               SelectedItem.UpdateName(EditingName);
               SelectedItem.UpdateUnit(EditingUnit);
               
               var currentStock = SelectedItem.StockQuantity;
               var delta = stock - currentStock;
               if (delta != 0) SelectedItem.AdjustStock(delta);
               
               SelectedItem.SetReorderPoint(reorder);

               await _inventoryRepository.UpdateAsync(SelectedItem);
               StatusMessage = "Item saved";
               
               // Refresh list item (UI might not update automatically if property not observable or raised)
               // In a real app we'd trigger PropertyChanged on the entity or reload. 
               // For now, assume binding updates or simple reload.
               var index = InventoryItems.IndexOf(SelectedItem);
               if (index >= 0) InventoryItems[index] = SelectedItem; // Trigger update
            }
            else
            {
                StatusMessage = "Invalid number format";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
    
    private async Task DeleteAsync()
    {
        if (SelectedItem == null) return;
        
        IsBusy = true;
        try
        {
            await _inventoryRepository.DeleteAsync(SelectedItem.Id);
            InventoryItems.Remove(SelectedItem);
            SelectedItem = null;
            StatusMessage = "Item deleted";
        }
        finally { IsBusy = false; }
    }
}
