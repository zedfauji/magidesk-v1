using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels.Dialogs;
using System.Threading.Tasks;

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
    /// <param name="primaryButtonText">Primary button text (default: "Yes")</param>
    /// <param name="secondaryButtonText">Secondary button text (default: "No")</param>
    /// <param name="icon">Icon to display (default: warning)</param>
    /// <param name="severity">Severity level (Warning, Error, Info)</param>
    /// <param name="details">Additional details</param>
    /// <returns>True if user confirmed, false otherwise</returns>
    public async Task<bool> ShowConfirmationAsync(string title, string message, 
                                                 string primaryButtonText = "Yes", 
                                                 string secondaryButtonText = "No", 
                                                 string icon = "⚠️", 
                                                 string severity = "Warning", 
                                                 string details = "")
    {
        ViewModel.Initialize(title, message, primaryButtonText, secondaryButtonText, icon, severity, details);
        
        var result = await ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}