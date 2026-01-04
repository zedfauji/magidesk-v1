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
using Magidesk.Presentation.Services;
using MediatR;

namespace Magidesk.Presentation.ViewModels;

public partial class ServerSectionManagementViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IServerSectionRepository _serverSectionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<ServerSectionDto> _serverSections = new();

    [ObservableProperty]
    private ObservableCollection<UserDto> _servers = new();

    [ObservableProperty]
    private ObservableCollection<TableDto> _availableTables = new();

    [ObservableProperty]
    private ServerSectionDto? _selectedSection;

    [ObservableProperty]
    private UserDto? _selectedServer;

    [ObservableProperty]
    private string _sectionName = string.Empty;

    [ObservableProperty]
    private string _sectionDescription = string.Empty;

    [ObservableProperty]
    private string _sectionColor = "#3498db";

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _assignedTableCount;

    public IRelayCommand CreateSectionCommand { get; }
    public IRelayCommand UpdateSectionCommand { get; }
    public IRelayCommand DeleteSectionCommand { get; }
    public IRelayCommand ClearFormCommand { get; }
    public IRelayCommand AssignTablesCommand { get; }
    public IRelayCommand RemoveTablesCommand { get; }
    public IRelayCommand RefreshCommand { get; }

    public ServerSectionManagementViewModel(
        IMediator mediator,
        IServerSectionRepository serverSectionRepository,
        ITableRepository tableRepository,
        IUserRepository userRepository,
        NavigationService navigationService)
    {
        _mediator = mediator;
        _serverSectionRepository = serverSectionRepository;
        _tableRepository = tableRepository;
        _userRepository = userRepository;
        _navigationService = navigationService;

        CreateSectionCommand = new AsyncRelayCommand(CreateSectionAsync);
        UpdateSectionCommand = new AsyncRelayCommand(UpdateSectionAsync);
        DeleteSectionCommand = new AsyncRelayCommand<ServerSectionDto>(DeleteSectionAsync);
        ClearFormCommand = new RelayCommand(ClearForm);
        AssignTablesCommand = new AsyncRelayCommand(AssignTablesAsync);
        RemoveTablesCommand = new AsyncRelayCommand(RemoveTablesAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshDataAsync);

        Title = "Server Section Management";
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            await LoadServerSectionsAsync();
            await LoadServersAsync();
            await LoadAvailableTablesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadServerSectionsAsync()
    {
        var sections = await _mediator.Send(new GetServerSectionsQuery());
        ServerSections.Clear();
        
        foreach (var section in sections)
        {
            ServerSections.Add(section);
        }
    }

    private async Task LoadServersAsync()
    {
        // For now, create mock servers
        Servers.Clear();
        Servers.Add(new UserDto
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@restaurant.com",
            IsActive = true
        });
        
        Servers.Add(new UserDto
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@restaurant.com",
            IsActive = true
        });

        SelectedServer = Servers.FirstOrDefault();
    }

    private async Task LoadAvailableTablesAsync()
    {
        var tables = await _tableRepository.GetAvailableAsync();
        AvailableTables.Clear();
        
        foreach (var table in tables)
        {
            AvailableTables.Add(new TableDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                X = table.X,
                Y = table.Y,
                Status = table.Status,
                IsActive = table.IsActive
            });
        }
    }

    private async Task CreateSectionAsync()
    {
        if (string.IsNullOrWhiteSpace(SectionName) || SelectedServer == null)
        {
            await ShowErrorAsync("Please enter a section name and select a server.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new CreateServerSectionCommand(
                SectionName,
                SelectedServer.Id,
                SectionDescription,
                SectionColor
            );

            var newSection = await _mediator.Send(command);
            ServerSections.Add(newSection);
            
            ClearForm();
            await ShowSuccessAsync($"Server section '{SectionName}' created successfully.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error creating server section: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateSectionAsync()
    {
        if (SelectedSection == null || string.IsNullOrWhiteSpace(SectionName))
        {
            await ShowErrorAsync("Please select a section and enter a name.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new UpdateServerSectionCommand(
                SelectedSection.Id,
                SectionName,
                SectionDescription,
                SectionColor
            );

            var updatedSection = await _mediator.Send(command);
            
            // Update the section in the collection
            var index = ServerSections.IndexOf(SelectedSection);
            if (index >= 0)
            {
                ServerSections[index] = updatedSection;
                SelectedSection = updatedSection;
            }

            await ShowSuccessAsync($"Server section '{SectionName}' updated successfully.");
            IsEditMode = false;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error updating server section: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteSectionAsync(ServerSectionDto? section)
    {
        if (section == null)
        {
            await ShowErrorAsync("Please select a section to delete.");
            return;
        }

        if (section.TableCount > 0)
        {
            await ShowErrorAsync($"Cannot delete section '{section.Name}' because it has {section.TableCount} assigned tables. Please remove tables first.");
            return;
        }

        var confirmed = await ShowConfirmationAsync($"Delete Server Section '{section.Name}'?", 
            "This action cannot be undone. Are you sure you want to delete this server section?");
        
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var command = new DeleteServerSectionCommand(section.Id);
            var result = await _mediator.Send(command);

            if (result)
            {
                ServerSections.Remove(section);
                
                if (SelectedSection == section)
                {
                    SelectedSection = null;
                    ClearForm();
                }

                await ShowSuccessAsync($"Server section '{section.Name}' deleted successfully.");
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error deleting server section: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AssignTablesAsync()
    {
        if (SelectedSection == null)
        {
            await ShowErrorAsync("Please select a server section first.");
            return;
        }

        // Get selected tables (this would come from UI selection)
        var selectedTableIds = AvailableTables.Where(t => t.IsSelected).Select(t => t.Id).ToList();
        
        if (!selectedTableIds.Any())
        {
            await ShowErrorAsync("Please select tables to assign.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new AssignTablesToServerSectionCommand(
                SelectedSection.Id,
                selectedTableIds
            );

            var updatedSection = await _mediator.Send(command);
            
            // Update the section in the collection
            var index = ServerSections.IndexOf(SelectedSection);
            if (index >= 0)
            {
                ServerSections[index] = updatedSection;
                SelectedSection = updatedSection;
            }

            await ShowSuccessAsync($"Assigned {selectedTableIds.Count} tables to section '{SelectedSection.Name}'.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error assigning tables: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RemoveTablesAsync()
    {
        // Implementation for removing tables from section
        await ShowErrorAsync("Remove tables functionality not yet implemented.");
    }

    private async Task RefreshDataAsync()
    {
        await LoadDataAsync();
    }

    private void ClearForm()
    {
        SectionName = string.Empty;
        SectionDescription = string.Empty;
        SectionColor = "#3498db";
        SelectedSection = null;
        IsEditMode = false;
    }

    partial void OnSelectedSectionChanged(ServerSectionDto? value)
    {
        if (value != null)
        {
            SectionName = value.Name;
            SectionDescription = value.Description;
            SectionColor = value.Color;
            SelectedServer = Servers.FirstOrDefault(s => s.Id == value.ServerId);
            AssignedTableCount = value.TableCount;
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
