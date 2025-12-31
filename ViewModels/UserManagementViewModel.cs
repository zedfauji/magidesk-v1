using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class UserManagementViewModel : ViewModelBase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICommandHandler<CreateUserCommand, CreateUserResult> _createHandler;
    private readonly ICommandHandler<UpdateUserCommand, UpdateUserResult> _updateHandler;
    private readonly ICommandHandler<DeleteUserCommand, DeleteUserResult> _deleteHandler;

    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Role> Roles { get; } = new();

    private User? _selectedUser;
    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (SetProperty(ref _selectedUser, value))
            {
                HydrateEditor();
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    public bool HasSelection => SelectedUser != null;

    // Editor Fields
    private string _firstName = "";
    private string _lastName = "";
    private string _pin = "";
    private Role? _selectedRole;
    private bool _isActive;

    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    public string Pin { get => _pin; set => SetProperty(ref _pin, value); }
    
    public Role? SelectedRole 
    { 
        get => _selectedRole; 
        set => SetProperty(ref _selectedRole, value); 
    }

    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }

    private string _statusMessage = "Ready";
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public UserManagementViewModel(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICommandHandler<CreateUserCommand, CreateUserResult> createHandler,
        ICommandHandler<UpdateUserCommand, UpdateUserResult> updateHandler,
        ICommandHandler<DeleteUserCommand, DeleteUserResult> deleteHandler)
    {
        Title = "User Management";
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        AddCommand = new AsyncRelayCommand(AddAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => HasSelection);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var users = await _userRepository.GetAllAsync();
            var roles = await _roleRepository.GetAllAsync();
            
            Users.Clear();
            foreach (var u in users) Users.Add(u);
            
            Roles.Clear();
            foreach (var r in roles) Roles.Add(r);
            
            StatusMessage = $"Loaded {Users.Count} users.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private void HydrateEditor()
    {
        if (SelectedUser != null)
        {
            FirstName = SelectedUser.FirstName;
            LastName = SelectedUser.LastName;
            Pin = ""; // Clearing PIN for security
            SelectedRole = Roles.FirstOrDefault(r => r.Id == SelectedUser.RoleId);
            IsActive = SelectedUser.IsActive;
            StatusMessage = $"Editing {SelectedUser.FirstName}";
        }
        else
        {
            FirstName = "";
            LastName = "";
            Pin = "";
            SelectedRole = Roles.FirstOrDefault(); // Default to first role
            IsActive = true;
            StatusMessage = "Ready";
        }
        // Force refresh command state
        ((AsyncRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
    }

    private async Task AddAsync()
    {
        SelectedUser = null;
        StatusMessage = "Enter details for new user and click Save.";
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            StatusMessage = "First Name is required.";
            return;
        }
        if (SelectedRole == null)
        {
            StatusMessage = "Role is required.";
            return;
        }

        IsBusy = true;
        try
        {
            if (SelectedUser == null)
            {
                // Create
                var cmd = new CreateUserCommand
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Pin = Pin,
                    RoleId = SelectedRole.Id,
                    Username = FirstName.ToLower()
                };

                var result = await _createHandler.HandleAsync(cmd);
                if (result.IsSuccess)
                {
                    StatusMessage = "User Created.";
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
                var cmd = new UpdateUserCommand
                {
                    UserId = SelectedUser.Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    Pin = Pin,
                    RoleId = SelectedRole.Id,
                    IsActive = IsActive
                };
                
                var result = await _updateHandler.HandleAsync(cmd);
                if (result.IsSuccess)
                {
                    StatusMessage = "User Updated.";
                    await LoadAsync();
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
        if (SelectedUser == null) return;
        
        // Confirmation? Ideally yes, but sticking to logic.
        IsBusy = true;
        try
        {
            var result = await _deleteHandler.HandleAsync(new DeleteUserCommand { UserId = SelectedUser.Id });
            if (result.IsSuccess)
            {
                StatusMessage = "User Deleted.";
                await LoadAsync();
                SelectedUser = null;
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
