using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels.Dialogs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Confirmation dialog for critical operations.
/// </summary>
public sealed partial class ConfirmationDialog : ContentDialog
{
    public ConfirmationDialogViewModel ViewModel { get; }

    public ConfirmationDialog()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ConfirmationDialogViewModel>();
        DataContext = ViewModel;
    }

    /// <summary>
    /// Shows a confirmation dialog with the specified parameters.
    /// </summary>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Main message</param>
    /// <param name="primaryButtonText">Primary button text (default: "Confirm")</param>
    /// <param name="secondaryButtonText">Secondary button text (default: "Cancel")</param>
    /// <param name="icon">Icon to display (default: warning)</param>
    /// <param name="severity">Severity level (Warning, Error, Info)</param>
    /// <param name="details">Additional details</param>
    /// <param name="warningMessage">Warning message for InfoBar</param>
    /// <param name="detailItems">Dictionary of detail items to display in the detail card</param>
    /// <returns>True if user confirmed, false otherwise</returns>
    public async Task<bool> ShowConfirmationAsync(
        string title, 
        string message, 
        string primaryButtonText = "Confirm", 
        string secondaryButtonText = "Cancel", 
        string icon = "⚠️", 
        string severity = "Warning", 
        string details = "",
        string warningMessage = "This action cannot be undone. Please confirm you want to proceed.",
        Dictionary<string, string>? detailItems = null)
    {
        ViewModel.Initialize(title, message, primaryButtonText, secondaryButtonText, icon, severity, details, warningMessage, detailItems);
        
        var result = await ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    /// Static helper method to show a confirmation dialog.
    /// </summary>
    /// <param name="title">Dialog title</param>
    /// <param name="warning">Warning message for InfoBar</param>
    /// <param name="detail">Detail message</param>
    /// <param name="details">Dictionary of detail items to display in the detail card</param>
    /// <param name="severity">Severity level (Warning, Error, Info)</param>
    /// <returns>True if user confirmed, false otherwise</returns>
    public static async Task<bool> ShowAsync(
        string title,
        string warning,
        string detail,
        Dictionary<string, string>? details = null,
        string severity = "Warning")
    {
        var dialog = App.Services.GetRequiredService<ConfirmationDialog>();
        
        // Set XamlRoot for the dialog
        if (App.MainWindowInstance?.Content?.XamlRoot != null)
        {
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        }
        
        return await dialog.ShowConfirmationAsync(
            title: title,
            message: detail,
            warningMessage: warning,
            detailItems: details,
            severity: severity);
    }
}