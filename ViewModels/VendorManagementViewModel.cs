using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.ViewModels;

public class VendorManagementViewModel : ViewModelBase
{
    private readonly IVendorRepository _vendorRepository;

    public ObservableCollection<Vendor> Vendors { get; } = new();

    private Vendor? _selectedVendor;
    public Vendor? SelectedVendor
    {
        get => _selectedVendor;
        set
        {
            if (SetProperty(ref _selectedVendor, value))
            {
                if (value != null)
                {
                    EditingName = value.Name;
                    EditingContact = value.ContactPerson ?? string.Empty;
                    EditingEmail = value.Email ?? string.Empty;
                    EditingPhone = value.PhoneNumber ?? string.Empty;
                    EditingAddress = value.Address ?? string.Empty;
                    IsEditing = true;
                }
                else
                {
                    IsEditing = false;
                }
            }
        }
    }

    private string _editingName = string.Empty;
    private string _editingContact = string.Empty;
    private string _editingEmail = string.Empty;
    private string _editingPhone = string.Empty;
    private string _editingAddress = string.Empty;
    private bool _isEditing;

    public string EditingName { get => _editingName; set => SetProperty(ref _editingName, value); }
    public string EditingContact { get => _editingContact; set => SetProperty(ref _editingContact, value); }
    public string EditingEmail { get => _editingEmail; set => SetProperty(ref _editingEmail, value); }
    public string EditingPhone { get => _editingPhone; set => SetProperty(ref _editingPhone, value); }
    public string EditingAddress { get => _editingAddress; set => SetProperty(ref _editingAddress, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    public ICommand LoadVendorsCommand { get; }
    public ICommand AddVendorCommand { get; }
    public ICommand SaveVendorCommand { get; }
    public ICommand DeleteVendorCommand { get; }

    public VendorManagementViewModel(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
        Title = "Vendor Management";

        LoadVendorsCommand = new AsyncRelayCommand(LoadVendorsAsync);
        AddVendorCommand = new AsyncRelayCommand(AddVendorAsync);
        SaveVendorCommand = new AsyncRelayCommand(SaveVendorAsync);
        DeleteVendorCommand = new AsyncRelayCommand(DeleteVendorAsync);
    }

    private async Task LoadVendorsAsync()
    {
        IsBusy = true;
        try
        {
            var vendors = await _vendorRepository.GetAllAsync();
            Vendors.Clear();
            foreach (var vendor in vendors) Vendors.Add(vendor);
        }
        finally { IsBusy = false; }
    }

    private async Task AddVendorAsync()
    {
        var newVendor = Vendor.Create("New Vendor");
        await _vendorRepository.AddAsync(newVendor);
        Vendors.Add(newVendor);
        SelectedVendor = newVendor;
    }

    private async Task SaveVendorAsync()
    {
        if (SelectedVendor == null) return;

        SelectedVendor.UpdateDetails(EditingName, EditingContact, EditingEmail, EditingPhone, EditingAddress);
        await _vendorRepository.UpdateAsync(SelectedVendor);
        
        // Refresh list
        var index = Vendors.IndexOf(SelectedVendor);
        if (index >= 0) Vendors[index] = SelectedVendor;
    }

    private async Task DeleteVendorAsync()
    {
        if (SelectedVendor == null) return;
        await _vendorRepository.DeleteAsync(SelectedVendor.Id);
        Vendors.Remove(SelectedVendor);
        SelectedVendor = null;
    }
}
