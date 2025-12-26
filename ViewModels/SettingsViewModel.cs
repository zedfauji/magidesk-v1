namespace Magidesk.Presentation.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private bool _isKioskMode;
    private string _apiBaseUrl = "(not configured)";

    public SettingsViewModel()
    {
        Title = "Settings";
    }

    public bool IsKioskMode
    {
        get => _isKioskMode;
        set => SetProperty(ref _isKioskMode, value);
    }

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set => SetProperty(ref _apiBaseUrl, value);
    }
}
