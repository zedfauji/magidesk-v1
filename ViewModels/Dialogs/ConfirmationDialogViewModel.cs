using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// Represents a detail item for the confirmation dialog detail card.
/// </summary>
public class DetailItem
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

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
    private string _primaryButtonText = "Confirm";

    [ObservableProperty]
    private string _secondaryButtonText = "Cancel";

    [ObservableProperty]
    private string _icon = "⚠️";

    [ObservableProperty]
    private string _severity = "Warning"; // Warning, Error, Info

    [ObservableProperty]
    private string _details = string.Empty;

    [ObservableProperty]
    private string _warningMessage = "This action cannot be undone. Please confirm you want to proceed.";

    [ObservableProperty]
    private ObservableCollection<DetailItem> _detailItems = new();

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
    public void Initialize(string title, string message, string primaryButtonText = "Confirm", 
                          string secondaryButtonText = "Cancel", string icon = "⚠️", 
                          string severity = "Warning", string details = "", 
                          string warningMessage = "This action cannot be undone. Please confirm you want to proceed.",
                          Dictionary<string, string>? detailItems = null)
    {
        Title = title;
        Message = message;
        PrimaryButtonText = primaryButtonText;
        SecondaryButtonText = secondaryButtonText;
        Icon = icon;
        Severity = severity;
        Details = details;
        WarningMessage = warningMessage;

        // Populate detail items
        DetailItems.Clear();
        if (detailItems != null)
        {
            foreach (var item in detailItems)
            {
                DetailItems.Add(new DetailItem { Label = item.Key, Value = item.Value });
            }
        }
    }

    /// <summary>
    /// Gets whether details are available.
    /// </summary>
    public bool HasDetails => !string.IsNullOrEmpty(Details);

    /// <summary>
    /// Gets the visibility for the details section.
    /// </summary>
    public Visibility DetailsVisibility => HasDetails ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets whether detail items are available.
    /// </summary>
    public bool HasDetailItems => DetailItems.Count > 0;

    /// <summary>
    /// Gets the visibility for the detail card.
    /// </summary>
    public Visibility DetailCardVisibility => HasDetailItems ? Visibility.Visible : Visibility.Collapsed;
}