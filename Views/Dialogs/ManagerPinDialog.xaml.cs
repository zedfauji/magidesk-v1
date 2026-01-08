using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Application.DTOs.Security;
using System.Threading.Tasks;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for manager PIN authorization.
/// Used for permission escalation workflow (e.g., void ticket, apply discount, refund).
/// </summary>
public sealed partial class ManagerPinDialog : ContentDialog
{
    public ManagerPinDialogViewModel ViewModel { get; }

    public ManagerPinDialog()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ManagerPinDialogViewModel>();
        DataContext = ViewModel;
    }

    /// <summary>
    /// Shows the dialog and returns the authorization result.
    /// </summary>
    /// <param name="operationType">Type of operation requiring authorization (e.g., "Void Ticket")</param>
    /// <returns>AuthorizationResult if authorized, null if cancelled or failed</returns>
    private AuthorizationResult? _authorizationResult;

    /// <summary>
    /// Shows the dialog and returns the authorization result.
    /// </summary>
    /// <param name="operationType">Type of operation requiring authorization (e.g., "Void Ticket")</param>
    /// <returns>AuthorizationResult if authorized, null if cancelled or failed</returns>
    public async Task<AuthorizationResult?> ShowForOperationAsync(string operationType)
    {
        ViewModel.OperationType = operationType;
        ViewModel.Pin = string.Empty;
        ViewModel.ErrorMessage = string.Empty;
        _authorizationResult = null;

        await ShowAsync();

        return _authorizationResult;
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            var result = await ViewModel.AuthorizeAsync();

            if (result != null && result.Authorized)
            {
                _authorizationResult = result;
            }
            else
            {
                args.Cancel = true; // Keep dialog open to show error
            }
        }
        finally
        {
            deferral.Complete();
        }
    }
}
