using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public partial class BackOfficeViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;
    private string _headerText = "Back Office";
    private Type _currentPageType;

    public string HeaderText
    {
        get => _headerText;
        set => SetProperty(ref _headerText, value);
    }

    public Type CurrentPageType
    {
        get => _currentPageType;
        set => SetProperty(ref _currentPageType, value);
    }

    public ICommand NavigateCommand { get; }
    public ICommand GoBackCommand { get; }
    public ICommand CaptureBatchCommand { get; }

    public BackOfficeViewModel(
        NavigationService navigationService,
        IUserService userService,
        ISecurityService securityService)
    {
        _navigationService = navigationService;
        _userService = userService;
        _securityService = securityService;
        Title = "Back Office";
        
        NavigateCommand = new RelayCommand<NavigationItem>(Navigate);
        GoBackCommand = new RelayCommand(GoBack);
        CaptureBatchCommand = new AsyncRelayCommand(CaptureBatchAsync);
        
        _ = LoadNavigationItemsAsync();
    }

    private async Task LoadNavigationItemsAsync()
    {
        NavigationItems.Clear();
        NavigationItems.Add(new NavigationItem("Menu Editor", "Edit Categories, Groups, Items", "\uE70F", typeof(MenuEditorPage)));
        NavigationItems.Add(new NavigationItem("Modifiers", "Manage Options & Toppings", "\uE74C", typeof(ModifierEditorPage)));
        NavigationItems.Add(new NavigationItem("Inventory", "Manage Stock & Ingredients", "\uE8F2", typeof(InventoryPage)));
        NavigationItems.Add(new NavigationItem("Vendors", "Supplier Management", "\uE716", typeof(VendorsPage)));
        NavigationItems.Add(new NavigationItem("Purchase Orders", "Stock Procurement", "\uEA37", typeof(PurchaseOrdersPage)));
        NavigationItems.Add(new NavigationItem("Table Map", "Spatial Floor Plan", "\uE8F1", typeof(TableMapPage)));
        NavigationItems.Add(new NavigationItem("Table Explorer", "List Floor Plan", "\uE179", typeof(TableExplorerPage)));
        
        if (_userService.CurrentUser != null)
        {
            var hasDesignerPermission = await _securityService.HasPermissionAsync(
                new UserId(_userService.CurrentUser.Id), 
                UserPermission.ManageTableLayout);

            if (hasDesignerPermission)
            {
                NavigationItems.Add(new NavigationItem("Table Designer", "Design Table Layouts", "\uE70F", typeof(Magidesk.Presentation.Views.TableDesignerPage)));
            }
        }

        NavigationItems.Add(new NavigationItem("Users", "Manage Staff & Permissions", "\uE77B", typeof(Magidesk.Presentation.Views.UserManagementPage)));
        NavigationItems.Add(new NavigationItem("Roles", "Manage User Roles", "\uE716", typeof(Magidesk.Presentation.Views.RoleManagementPage)));
        NavigationItems.Add(new NavigationItem("Tax / Discount", "Discount & Tax tools", "\uE8D7", typeof(Magidesk.Presentation.Views.DiscountTaxPage)));
        NavigationItems.Add(new NavigationItem("Order Types", "Manage order type rules", "\uE8A1", typeof(OrderTypeExplorerPage)));
        NavigationItems.Add(new NavigationItem("Shifts", "Manage shift definitions", "\uE823", typeof(ShiftExplorerPage)));
        NavigationItems.Add(new NavigationItem("Reports", "Sales Summaries", "\uE9F9", typeof(SalesReportsPage)));
        NavigationItems.Add(new NavigationItem("General Settings", "Restaurant Info & Kiosk", "\uE713", typeof(SettingsPage)));
        NavigationItems.Add(new NavigationItem("System & Printers", "Hardware, Config & Backup", "\uE713", typeof(SystemConfigPage)));
        NavigationItems.Add(new NavigationItem("Print Templates", "Receipt & Ticket Layouts", "\uE74C", typeof(Magidesk.Presentation.Views.PrintTemplatesPage)));
    }

    private async Task CaptureBatchAsync()
    {
        var dialog = new Magidesk.Views.AuthorizationCaptureBatchDialog();
        await _navigationService.ShowDialogAsync(dialog);
    }
    
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

    private void Navigate(NavigationItem? item)
    {
        if (item == null) return;
        
        HeaderText = item.Title;
        if (item.PageType != typeof(object))
        {
            CurrentPageType = item.PageType;
        }
    }

    private void GoBack()
    {
        _navigationService.GoBack();
    }
}

public class NavigationItem
{
    public string Title { get; }
    public string Description { get; }
    public string IconData { get; } 
    public Type PageType { get; }

    public NavigationItem(string title, string description, string iconData, Type pageType)
    {
        Title = title;
        Description = description;
        IconData = iconData;
        PageType = pageType;
    }
}
