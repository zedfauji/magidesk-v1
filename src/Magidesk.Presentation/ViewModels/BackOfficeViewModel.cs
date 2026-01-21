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
    private string _headerText = "BO_Title"; // Default Key
    private Type _currentPageType;

    public Services.LocalizationService Localization { get; }

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
        ISecurityService securityService,
        Services.LocalizationService localizationService)
    {
        _navigationService = navigationService;
        _userService = userService;
        _securityService = securityService;
        Localization = localizationService;
        
        Title = "Back Office";
        
        NavigateCommand = new RelayCommand<NavigationItem>(Navigate);
        GoBackCommand = new RelayCommand(GoBack);
        CaptureBatchCommand = new AsyncRelayCommand(CaptureBatchAsync);
        
        _ = LoadNavigationItemsAsync();
    }

    private async Task LoadNavigationItemsAsync()
    {
        NavigationItems.Clear();
        // Storing KEYS in Title and Description instead of raw text
        NavigationItems.Add(new NavigationItem("BO_Nav_MenuEditor", "BO_Nav_MenuEditor_Desc", "\uE70F", typeof(MenuEditorPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Modifiers", "BO_Nav_Modifiers_Desc", "\uE74C", typeof(ModifierEditorPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Inventory", "BO_Nav_Inventory_Desc", "\uE8F2", typeof(InventoryPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Vendors", "BO_Nav_Vendors_Desc", "\uE716", typeof(VendorsPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_PurchaseOrders", "BO_Nav_PurchaseOrders_Desc", "\uEA37", typeof(PurchaseOrdersPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_TableMap", "BO_Nav_TableMap_Desc", "\uE8F1", typeof(TableMapPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_TableExplorer", "BO_Nav_TableExplorer_Desc", "\uE179", typeof(TableExplorerPage), Localization));
        
        if (_userService.CurrentUser != null)
        {
            var hasDesignerPermission = await _securityService.HasPermissionAsync(
                new UserId(_userService.CurrentUser.Id), 
                UserPermission.ManageTableLayout);

            if (hasDesignerPermission)
            {
                NavigationItems.Add(new NavigationItem("BO_Nav_TableDesigner", "BO_Nav_TableDesigner_Desc", "\uE70F", typeof(Magidesk.Presentation.Views.TableDesignerPage), Localization));
            }
        }

        NavigationItems.Add(new NavigationItem("BO_Nav_Users", "BO_Nav_Users_Desc", "\uE77B", typeof(Magidesk.Presentation.Views.UserManagementPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Roles", "BO_Nav_Roles_Desc", "\uE716", typeof(Magidesk.Presentation.Views.RoleManagementPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_TaxDiscount", "BO_Nav_TaxDiscount_Desc", "\uE8D7", typeof(Magidesk.Presentation.Views.DiscountTaxPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_DiscountRules", "BO_Nav_DiscountRules_Desc", "\uE71A", typeof(Magidesk.Presentation.Views.DiscountManagementPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_OrderTypes", "BO_Nav_OrderTypes_Desc", "\uE8A1", typeof(OrderTypeExplorerPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Shifts", "BO_Nav_Shifts_Desc", "\uE823", typeof(ShiftExplorerPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Reports", "BO_Nav_Reports_Desc", "\uE9F9", typeof(SalesReportsPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_Settings", "BO_Nav_Settings_Desc", "\uE713", typeof(SettingsPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_System", "BO_Nav_System_Desc", "\uE713", typeof(SystemConfigPage), Localization));
        NavigationItems.Add(new NavigationItem("BO_Nav_PrintTemplates", "BO_Nav_PrintTemplates_Desc", "\uE74C", typeof(Magidesk.Presentation.Views.PrintTemplatesPage), Localization));
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
        
        HeaderText = item.Title; // Now setting it to the KEY
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

public partial class NavigationItem : ObservableObject
{
    private readonly Services.LocalizationService? _localizationService;

    public string TitleKey { get; }
    public string DescriptionKey { get; }
    public string IconData { get; } 
    public Type PageType { get; }
    
    // Kept for compatibility if accessed directly, but returns Key now.
    public string Title => TitleKey; 

    public string LocalizedTitle => _localizationService?[TitleKey] ?? TitleKey;
    public string LocalizedDescription => _localizationService?[DescriptionKey] ?? DescriptionKey;

    public NavigationItem(string titleKey, string descriptionKey, string iconData, Type pageType, Services.LocalizationService? localizationService = null)
    {
        TitleKey = titleKey;
        DescriptionKey = descriptionKey;
        IconData = iconData;
        PageType = pageType;
        _localizationService = localizationService;

        if (_localizationService != null)
        {
            _localizationService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Item[]")
                {
                    OnPropertyChanged(nameof(LocalizedTitle));
                    OnPropertyChanged(nameof(LocalizedDescription));
                }
            };
        }
    }
}
