using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class CustomerListViewModel : ViewModelBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICommandHandler<CreateCustomerCommand, CreateCustomerResult> _createHandler;
    private readonly ICommandHandler<UpdateCustomerCommand, UpdateCustomerResult> _updateHandler;
    private readonly IDialogService _dialogService;

    public ObservableCollection<Customer> Customers { get; } = new();

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (SetProperty(ref _selectedCustomer, value))
            {
                HydrateEditor();
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    public bool HasSelection => SelectedCustomer != null;

    private CustomerSearchResultDto? _selectedCustomerDto;
    public CustomerSearchResultDto? SelectedCustomerDto
    {
        get => _selectedCustomerDto;
        set
        {
            if (SetProperty(ref _selectedCustomerDto, value))
            {
                if (value != null)
                {
                    _ = SelectByIdAsync(value.Id);
                }
            }
        }
    }

    private async Task SelectByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer != null)
        {
            SelectedCustomer = customer;
        }
    }

    // Editor Fields
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _phone = string.Empty;
    private string? _email;
    private string? _address;
    private string? _city;
    private string? _postalCode;
    private bool _isActive = true;

    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
    public string? Email { get => _email; set => SetProperty(ref _email, value); }
    public string? Address { get => _address; set => SetProperty(ref _address, value); }
    public string? City { get => _city; set => SetProperty(ref _city, value); }
    public string? PostalCode { get => _postalCode; set => SetProperty(ref _postalCode, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                _ = SearchAsync();
            }
        }
    }

    private string _statusMessage = "Ready";
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public ICommand LoadCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ClearSearchCommand { get; }

    public CustomerListViewModel(
        ICustomerRepository customerRepository,
        ICommandHandler<CreateCustomerCommand, CreateCustomerResult> createHandler,
        ICommandHandler<UpdateCustomerCommand, UpdateCustomerResult> updateHandler,
        IDialogService dialogService)
    {
        Title = "Customer Records";
        _customerRepository = customerRepository;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _dialogService = dialogService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        AddCommand = new RelayCommand(Add);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ClearSearchCommand = new RelayCommand(() => SearchTerm = string.Empty);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var customers = await _customerRepository.GetActiveAsync();
            Customers.Clear();
            foreach (var c in customers) Customers.Add(c);
            StatusMessage = $"Loaded {Customers.Count} active customers.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading customers: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
        {
            await LoadAsync();
            return;
        }

        IsBusy = true;
        try
        {
            var results = await _customerRepository.SearchAsync(SearchTerm);
            Customers.Clear();
            foreach (var c in results) Customers.Add(c);
            StatusMessage = $"Found {Customers.Count} customers matching '{SearchTerm}'.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Search error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private void HydrateEditor()
    {
        if (SelectedCustomer != null)
        {
            FirstName = SelectedCustomer.FirstName;
            LastName = SelectedCustomer.LastName;
            Phone = SelectedCustomer.Phone;
            Email = SelectedCustomer.Email;
            Address = SelectedCustomer.Address;
            City = SelectedCustomer.City;
            PostalCode = SelectedCustomer.PostalCode;
            IsActive = SelectedCustomer.IsActive;
            StatusMessage = $"Editing {SelectedCustomer.FullName}";
        }
        else
        {
            ClearEditor();
            StatusMessage = "Ready";
        }
    }

    private void ClearEditor()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Phone = string.Empty;
        Email = null;
        Address = null;
        City = null;
        PostalCode = null;
        IsActive = true;
    }

    private void Add()
    {
        SelectedCustomer = null;
        ClearEditor();
        StatusMessage = "Enter new customer details and click Save.";
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Phone))
        {
            await _dialogService.ShowMessageAsync("Validation Error", "First Name, Last Name, and Phone are required.");
            return;
        }

        IsBusy = true;
        try
        {
            if (SelectedCustomer == null)
            {
                var cmd = new CreateCustomerCommand
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    Email = Email,
                    Address = Address,
                    City = City,
                    PostalCode = PostalCode
                };

                var result = await _createHandler.HandleAsync(cmd);
                if (result.IsSuccess)
                {
                    StatusMessage = "Customer created successfully.";
                    await SearchAsync();
                    // Optionally select the new customer? 
                    // result.CustomerId ...
                }
                else
                {
                    await _dialogService.ShowErrorAsync("Create Failed", result.Message);
                }
            }
            else
            {
                var cmd = new UpdateCustomerCommand
                {
                    CustomerId = SelectedCustomer.Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    Email = Email,
                    Address = Address,
                    City = City,
                    PostalCode = PostalCode,
                    IsActive = IsActive
                };

                var result = await _updateHandler.HandleAsync(cmd);
                if (result.IsSuccess)
                {
                    StatusMessage = "Customer updated successfully.";
                    await SearchAsync();
                }
                else
                {
                    await _dialogService.ShowErrorAsync("Update Failed", result.Message);
                }
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Operation Failed", ex.Message);
        }
        finally { IsBusy = false; }
    }
}
