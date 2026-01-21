using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

public partial class PermissionItem : ObservableObject
{
    public UserPermission Value { get; }
    public string Name { get; }
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public PermissionItem(UserPermission value, string name, bool isSelected)
    {
        Value = value;
        Name = name;
        _isSelected = isSelected;
    }
}

public partial class RoleManagementViewModel : ViewModelBase
{
    private readonly IRoleRepository _roleRepository;
    private readonly ICommandHandler<CreateRoleCommand, CreateRoleResult> _createHandler;
    private readonly ICommandHandler<UpdateRoleCommand, UpdateRoleResult> _updateHandler;
    private readonly ICommandHandler<DeleteRoleCommand, DeleteRoleResult> _deleteHandler;

    public ObservableCollection<Role> Roles { get; } = new();
    public ObservableCollection<PermissionItem> Permissions { get; } = new();

    private Role? _selectedRole;
    public Role? SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (SetProperty(ref _selectedRole, value))
            {
                HydrateEditor();
                OnPropertyChanged(nameof(HasSelection));
                ((AsyncRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
            }
        }
    }

    public bool HasSelection => SelectedRole != null;

    private string _roleName = "";
    public string RoleName
    {
        get => _roleName;
        set => SetProperty(ref _roleName, value);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public RoleManagementViewModel(
        IRoleRepository roleRepository,
        ICommandHandler<CreateRoleCommand, CreateRoleResult> createHandler,
        ICommandHandler<UpdateRoleCommand, UpdateRoleResult> updateHandler,
        ICommandHandler<DeleteRoleCommand, DeleteRoleResult> deleteHandler)
    {
        Title = "Role Management";
        _roleRepository = roleRepository;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        AddCommand = new RelayCommand(PrepareAdd);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => HasSelection);

        InitializePermissions();
    }

    private void InitializePermissions()
    {
        Permissions.Clear();
        var values = Enum.GetValues<UserPermission>();
        foreach (var val in values)
        {
            if (val == UserPermission.None) continue;
            // Split CamelCase for display if desired, or just use ToString
            Permissions.Add(new PermissionItem(val, val.ToString(), false));
        }
    }

    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var roles = await _roleRepository.GetAllAsync();
            Roles.Clear();
            foreach (var r in roles) Roles.Add(r);
            StatusMessage = $"Loaded {Roles.Count} roles.";
            
            // If selection exists, re-select
            if (SelectedRole != null)
            {
                SelectedRole = Roles.FirstOrDefault(r => r.Id == SelectedRole.Id);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private void PrepareAdd()
    {
        SelectedRole = null;
        RoleName = "";
        
        // Clear checks
        foreach (var p in Permissions) p.IsSelected = false;
        
        StatusMessage = "Enter details for new role.";
    }

    private void HydrateEditor()
    {
        if (SelectedRole != null)
        {
            RoleName = SelectedRole.Name;
            // Map permissions logic
            foreach (var p in Permissions)
            {
                p.IsSelected = SelectedRole.Permissions.HasFlag(p.Value);
            }
            StatusMessage = $"Editing {SelectedRole.Name}";
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(RoleName))
        {
            StatusMessage = "Role Name is required.";
            return;
        }

        // Calculate Flag
        UserPermission combined = UserPermission.None;
        foreach (var p in Permissions)
        {
            if (p.IsSelected) combined |= p.Value;
        }

        IsBusy = true;
        try
        {
            if (SelectedRole == null)
            {
                // Create
                var result = await _createHandler.HandleAsync(new CreateRoleCommand(RoleName, combined));
                if (result.IsSuccess)
                {
                    StatusMessage = "Role Created.";
                    await LoadAsync();
                }
                else
                {
                    StatusMessage = $"Create Failed: {result.Message}";
                }
            }
            else
            {
                // Update
                var result = await _updateHandler.HandleAsync(new UpdateRoleCommand(SelectedRole.Id, RoleName, combined));
                if (result.IsSuccess)
                {
                    StatusMessage = "Role Updated.";
                    await LoadAsync(); // Refresh list to update View references
                }
                else
                {
                    StatusMessage = $"Update Failed: {result.Message}";
                }
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
        if (SelectedRole == null) return;

        IsBusy = true;
        try
        {
            var result = await _deleteHandler.HandleAsync(new DeleteRoleCommand(SelectedRole.Id));
            if (result.IsSuccess)
            {
                StatusMessage = "Role Deleted.";
                await LoadAsync();
                PrepareAdd();
            }
            else
            {
                StatusMessage = $"Delete Failed: {result.Message}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
}
