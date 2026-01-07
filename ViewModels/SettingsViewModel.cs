using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.SystemConfig;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetRestaurantConfigQuery, GetRestaurantConfigResult> _getQueryHandler;
    private readonly ICommandHandler<UpdateRestaurantConfigCommand, UpdateRestaurantConfigResult> _updateCommandHandler;

    public Services.LocalizationService Localization { get; }

    private bool _isKioskMode;
    private string _restaurantName = "";
    private string _restaurantAddress = "";
    private string _restaurantPhone = "";
    private string _restaurantEmail = "";
    private string _restaurantWebsite = "";
    private string _receiptFooter = "";
    private string _taxId = "";

    public SettingsViewModel(
        IQueryHandler<GetRestaurantConfigQuery, GetRestaurantConfigResult> getQueryHandler,
        ICommandHandler<UpdateRestaurantConfigCommand, UpdateRestaurantConfigResult> updateCommandHandler,
        Services.LocalizationService localizationService)
    {
        Title = "Settings";
        _getQueryHandler = getQueryHandler;
        _updateCommandHandler = updateCommandHandler;
        Localization = localizationService;
        
        LoadSettingsCommand = new AsyncRelayCommand(LoadSettingsAsync);
        SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
        
        LoadSettingsCommand.Execute(null);
    }

    public IAsyncRelayCommand LoadSettingsCommand { get; }
    public IAsyncRelayCommand SaveSettingsCommand { get; }

    public bool IsKioskMode
    {
        get => _isKioskMode;
        set => SetProperty(ref _isKioskMode, value);
    }

    public string RestaurantName { get => _restaurantName; set => SetProperty(ref _restaurantName, value); }
    public string RestaurantAddress { get => _restaurantAddress; set => SetProperty(ref _restaurantAddress, value); }
    public string RestaurantPhone { get => _restaurantPhone; set => SetProperty(ref _restaurantPhone, value); }
    public string RestaurantEmail { get => _restaurantEmail; set => SetProperty(ref _restaurantEmail, value); }
    public string RestaurantWebsite { get => _restaurantWebsite; set => SetProperty(ref _restaurantWebsite, value); }
    public string ReceiptFooter { get => _receiptFooter; set => SetProperty(ref _receiptFooter, value); }
    public string TaxId { get => _taxId; set => SetProperty(ref _taxId, value); }

    private async Task LoadSettingsAsync()
    {
        var result = await _getQueryHandler.HandleAsync(new GetRestaurantConfigQuery());
        if (result?.Configuration != null)
        {
            var c = result.Configuration;
            RestaurantName = c.Name;
            RestaurantAddress = c.Address;
            RestaurantPhone = c.Phone;
            RestaurantEmail = c.Email;
            RestaurantWebsite = c.Website;
            ReceiptFooter = c.ReceiptFooterMessage;
            TaxId = c.TaxId;
            IsKioskMode = c.IsKioskMode;
        }
    }

    private async Task SaveSettingsAsync()
    {
        var dto = new RestaurantConfigurationDto
        {
            Name = RestaurantName,
            Address = RestaurantAddress,
            Phone = RestaurantPhone,
            Email = RestaurantEmail,
            Website = RestaurantWebsite,
            ReceiptFooterMessage = ReceiptFooter,
            TaxId = TaxId,
            IsKioskMode = IsKioskMode
        };

        var result = await _updateCommandHandler.HandleAsync(new UpdateRestaurantConfigCommand(dto));
        if (result.IsSuccess)
        {
            // Show toast or notification? DefaultViewRoutingService might handle errors but not success toasts yet.
        }
    }
}
