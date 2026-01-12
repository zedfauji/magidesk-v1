using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces.Persistence;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.ViewModels;

public class PriceLevelManagementViewModel : ViewModelBase
{
    private readonly IPriceLevelRepository _priceLevelRepository;

    private PriceLevel? _selectedPriceLevel;
    private bool _isEditing;
    private string _statusMessage = "Ready";

    // Editing Properties
    private string _editingName = string.Empty;
    private string _editingDescription = string.Empty;
    private bool _editingIsActive = true;
    private bool _editingIsDefault = false;
    private int _editingDisplayOrder;

    public ObservableCollection<PriceLevel> PriceLevels { get; } = new();

    public PriceLevel? SelectedPriceLevel
    {
        get => _selectedPriceLevel;
        set
        {
            if (SetProperty(ref _selectedPriceLevel, value))
            {
                if (value != null)
                {
                    IsEditing = true;
                    EditingName = value.Name;
                    EditingDescription = value.Description;
                    EditingIsActive = value.IsActive;
                    EditingIsDefault = value.IsDefault;
                    EditingDisplayOrder = value.DisplayOrder;
                    StatusMessage = $"Editing: {value.Name}";
                }
                else
                {
                    IsEditing = false;
                    StatusMessage = "Ready";
                    // Clear editing fields? Optional.
                }
            }
        }
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string EditingName
    {
        get => _editingName;
        set => SetProperty(ref _editingName, value);
    }

    public string EditingDescription
    {
        get => _editingDescription;
        set => SetProperty(ref _editingDescription, value);
    }

    public bool EditingIsActive
    {
        get => _editingIsActive;
        set => SetProperty(ref _editingIsActive, value);
    }

    public bool EditingIsDefault
    {
        get => _editingIsDefault;
        set => SetProperty(ref _editingIsDefault, value);
    }

    public int EditingDisplayOrder
    {
        get => _editingDisplayOrder;
        set => SetProperty(ref _editingDisplayOrder, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public PriceLevelManagementViewModel(IPriceLevelRepository priceLevelRepository)
    {
        _priceLevelRepository = priceLevelRepository;
        Title = "Price Level Management";

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddCommand = new AsyncRelayCommand(AddAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            PriceLevels.Clear();
            var levels = await _priceLevelRepository.GetAllAsync();
            foreach (var level in levels.OrderBy(l => l.DisplayOrder))
            {
                PriceLevels.Add(level);
            }
            StatusMessage = "Loaded successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddAsync()
    {
        IsBusy = true;
        try
        {
            var newLevel = PriceLevel.Create("New Price Level", "Description", false, 0);
            await _priceLevelRepository.AddAsync(newLevel);
            PriceLevels.Add(newLevel);
            SelectedPriceLevel = newLevel;
            StatusMessage = "New price level added.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error adding: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedPriceLevel == null) return;

        IsBusy = true;
        try
        {
            var level = SelectedPriceLevel;
            level.Update(EditingName, EditingDescription, EditingIsActive, EditingIsDefault, EditingDisplayOrder);
            
            // If setting as default, ensure others are unset? 
            // The logic for single default usually resides in domain service or handler. 
            // For now, we'll just update this one. User responsibility to not have multiple defaults 
            // or we need a service method "SetAsDefault(id)" that clears others.
            // Feature G.8 logic: "Users can create named pricing tiers". "Default" implies one default.
            
            if (EditingIsDefault)
            {
                // Simple logic: unset others in UI and DB
                var others = PriceLevels.Where(p => p.Id != level.Id && p.IsDefault).ToList();
                foreach (var other in others)
                {
                    // This is hacky to do in VM, should be in Domain Service. 
                    // But for now, let's just warn or handle simply.
                    // Ideally: _priceLevelDomainService.SetDefault(level.Id);
                    // We don't have that service yet. Use repo to update others?
                    // Let's assume the user manages it or we check it later.
                    // Implementing quick fix:
                    other.Update(other.Name, other.Description, other.IsActive, false, other.DisplayOrder);
                    await _priceLevelRepository.UpdateAsync(other);
                }
            }

            await _priceLevelRepository.UpdateAsync(level);
            StatusMessage = "Saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedPriceLevel == null) return;

        IsBusy = true;
        try
        {
            await _priceLevelRepository.DeleteAsync(SelectedPriceLevel);
            PriceLevels.Remove(SelectedPriceLevel);
            SelectedPriceLevel = null;
            StatusMessage = "Deleted successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
