using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Provides a standard mechanism for showing dialogs and messages to the user,
/// decoupling ViewModels from specific UI implementations.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an error dialog with a standard layout.
    /// </summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="message">The main error message to display.</param>
    /// <param name="exceptionDetails">Optional technical details (stack trace) to show in an expander.</param>
    Task ShowErrorAsync(string title, string message, string? exceptionDetails = null);

    /// <summary>
    /// Shows a warning dialog.
    /// </summary>
    Task ShowWarningAsync(string title, string message);

    /// <summary>
    /// Shows an informational message.
    /// </summary>
    Task ShowMessageAsync(string title, string message);
    
    /// <summary>
    /// Shows a confirmation dialog (Yes/No).
    /// </summary>
    Task<bool> ShowConfirmationAsync(string title, string message, string yesText = "Yes", string noText = "No");
}
