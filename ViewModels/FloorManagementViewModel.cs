using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public partial class FloorManagementViewModel : ViewModelBase
{
    private readonly IFloorRepository _floorRepository;
    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;

    [ObservableProperty]
    private ObservableCollection<FloorDto> _floors = new();

    [ObservableProperty]
    private FloorDto? _selectedFloor;

    [ObservableProperty]
    private string _floorName = string.Empty;

    [ObservableProperty]
    private string _floorDescription = string.Empty;

    [ObservableProperty]
    private int _floorWidth = 2000;

    [ObservableProperty]
    private int _floorHeight = 2000;

    [ObservableProperty]
    private string _backgroundColor = "#f8f8f8";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isEditMode;

    public IRelayCommand CreateFloorCommand { get; }
    public IRelayCommand UpdateFloorCommand { get; }
    public IRelayCommand DeleteFloorCommand { get; }
    public IRelayCommand ClearFormCommand { get; }
    public IRelayCommand NavigateToDesignerCommand { get; }
    public IRelayCommand GoBackCommand { get; }

    public FloorManagementViewModel(
        IFloorRepository floorRepository,
        ITableLayoutRepository tableLayoutRepository,
        NavigationService navigationService,
        IUserService userService,
        ISecurityService securityService)
    {
        _floorRepository = floorRepository;
        _tableLayoutRepository = tableLayoutRepository;
        _navigationService = navigationService;
        _userService = userService;
        _securityService = securityService;

        CreateFloorCommand = new AsyncRelayCommand(CreateFloorAsync);
        UpdateFloorCommand = new AsyncRelayCommand(UpdateFloorAsync);
        DeleteFloorCommand = new AsyncRelayCommand<FloorDto>(DeleteFloorAsync);
        ClearFormCommand = new RelayCommand(ClearForm);
        NavigateToDesignerCommand = new AsyncRelayCommand(NavigateToDesignerAsync);
        GoBackCommand = new RelayCommand(GoBack);

        Title = "Floor Management";
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            await LoadFloorsAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadFloorsAsync()
    {
        // For now, create default floors if none exist
        Floors.Clear();
        Floors.Add(new FloorDto
        {
            Id = Guid.NewGuid(),
            Name = "Main Floor",
            Description = "Primary dining area",
            Width = 2000,
            Height = 2000,
            BackgroundColor = "#f8f8f8",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        Floors.Add(new FloorDto
        {
            Id = Guid.NewGuid(),
            Name = "Outdoor Patio",
            Description = "Outdoor seating area",
            Width = 1500,
            Height = 1500,
            BackgroundColor = "#e8f5e8",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        SelectedFloor = Floors.FirstOrDefault();
    }

    private async Task CreateFloorAsync()
    {
        if (string.IsNullOrWhiteSpace(FloorName))
        {
            await ShowErrorAsync("Please enter a floor name.");
            return;
        }

        IsBusy = true;
        try
        {
            var newFloor = new FloorDto
            {
                Id = Guid.NewGuid(),
                Name = FloorName,
                Description = FloorDescription,
                Width = FloorWidth,
                Height = FloorHeight,
                BackgroundColor = BackgroundColor,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Floors.Add(newFloor);
            SelectedFloor = newFloor;
            ClearForm();

            await ShowSuccessAsync($"Floor '{FloorName}' created successfully.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error creating floor: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateFloorAsync()
    {
        if (SelectedFloor == null || string.IsNullOrWhiteSpace(FloorName))
        {
            await ShowErrorAsync("Please select a floor and enter a name.");
            return;
        }

        IsBusy = true;
        try
        {
            SelectedFloor.Name = FloorName;
            SelectedFloor.Description = FloorDescription;
            SelectedFloor.Width = FloorWidth;
            SelectedFloor.Height = FloorHeight;
            SelectedFloor.BackgroundColor = BackgroundColor;
            SelectedFloor.UpdatedAt = DateTime.UtcNow;

            await ShowSuccessAsync($"Floor '{FloorName}' updated successfully.");
            IsEditMode = false;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error updating floor: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteFloorAsync(FloorDto? floor)
    {
        if (floor == null)
        {
            await ShowErrorAsync("Please select a floor to delete.");
            return;
        }

        // Check if floor has layouts
        var layouts = await _tableLayoutRepository.GetLayoutsByFloorAsync(floor.Id);
        if (layouts.Any())
        {
            await ShowErrorAsync($"Cannot delete floor '{floor.Name}' because it has {layouts.Count} layout(s). Please delete layouts first.");
            return;
        }

        var confirmed = await ShowConfirmationAsync($"Delete Floor '{floor.Name}'?", 
            "This action cannot be undone. Are you sure you want to delete this floor?");
        
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            Floors.Remove(floor);
            
            if (SelectedFloor == floor)
            {
                SelectedFloor = Floors.FirstOrDefault();
            }

            await ShowSuccessAsync($"Floor '{floor.Name}' deleted successfully.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error deleting floor: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearForm()
    {
        FloorName = string.Empty;
        FloorDescription = string.Empty;
        FloorWidth = 2000;
        FloorHeight = 2000;
        BackgroundColor = "#f8f8f8";
        IsEditMode = false;
    }

    private async Task NavigateToDesignerAsync()
    {
        if (SelectedFloor == null) return;

        if (_userService.CurrentUser == null) return;

        var hasPermission = await _securityService.HasPermissionAsync(
            new UserId(_userService.CurrentUser.Id),
            UserPermission.ManageTableLayout);

        if (hasPermission)
        {
            _navigationService.Navigate(typeof(Views.TableDesignerPage));
        }
        else
        {
            await ShowErrorAsync("You do not have permission to access the Table Designer.");
        }
    }

    private void GoBack()
    {
        _navigationService.GoBack();
    }

    partial void OnSelectedFloorChanged(FloorDto? value)
    {
        if (value != null)
        {
            FloorName = value.Name;
            FloorDescription = value.Description;
            FloorWidth = value.Width;
            FloorHeight = value.Height;
            BackgroundColor = value.BackgroundColor;
            IsEditMode = true;
        }
    }

    private async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        // In a real implementation, this would show a confirmation dialog
        return true;
    }

    private async Task ShowErrorAsync(string message)
    {
        // In a real implementation, this would show an error dialog
        System.Diagnostics.Debug.WriteLine($"Error: {message}");
    }

    private async Task ShowSuccessAsync(string message)
    {
        // In a real implementation, this would show a success notification
        System.Diagnostics.Debug.WriteLine($"Success: {message}");
    }
}
