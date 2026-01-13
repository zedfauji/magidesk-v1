using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for confirmation dialogs with customizable content.
/// </summary>
public partial class ConfirmationDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Confirm Action";

    [ObservableProperty]
    private string _message = "Are you sure you want to proceed?";

    [ObservableProperty]
    private string _primaryButtonText = "Yes";

    [ObservableProperty]
    private string _secondaryButtonText = "No";

    [ObservableProperty]
    private string _icon = "⚠️";

    [ObservableProperty]
    private string _severity = "Warning"; // Warning, Error, Info

    [ObservableProperty]
    private string _details = string.Empty;

    /// <summary>
    /// Gets the InfoBar severity enum value.
    /// </summary>
    public InfoBarSeverity InfoBarSeverity => _severity switch
    {
        "Error" => InfoBarSeverity.Error,
        "Info" => InfoBarSeverity.Informational,
        "Success" => InfoBarSeverity.Success,
        _ => InfoBarSeverity.Warning
    };

    /// <summary>
    /// Initializes the confirmation dialog with the specified parameters.
    /// </summary>
    public void Initialize(string title, string message, string primaryButtonText = "Yes", 
                          string secondaryButtonText = "No", string icon = "⚠️", 
                          string severity = "Warning", string details = "")
    {
        Title = title;
        Message = message;
        PrimaryButtonText = primaryButtonText;
        SecondaryButtonText = secondaryButtonText;
        Icon = icon;
        Severity = severity;
        Details = details;
    }

    /// <summary>
    /// Gets whether details are available.
    /// </summary>
    public bool HasDetails => !string.IsNullOrEmpty(Details);

    /// <summary>
    /// Gets the visibility for the details section.
    /// </summary>
    public Visibility DetailsVisibility => HasDetails ? Visibility.Visible : Visibility.Collapsed;
}