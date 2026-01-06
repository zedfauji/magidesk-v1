using System.Threading.Tasks;

namespace Magidesk.Services
{
    /// <summary>
    /// Central error handling service for surfacing all failures to appropriate UI elements.
    /// </summary>
    public interface IErrorService
    {
        /// <summary>
        /// Shows a fatal error dialog - app cannot continue safely.
        /// </summary>
        Task ShowFatalAsync(string title, string message, string? details = null);

        /// <summary>
        /// Shows an error dialog - action failed, operator must act.
        /// </summary>
        Task ShowErrorAsync(string title, string message, string? details = null);

        /// <summary>
        /// Shows a warning banner - system degraded but usable.
        /// </summary>
        Task ShowWarningAsync(string title, string message, string? details = null);

        /// <summary>
        /// Shows an info toast - non-blocking status notification.
        /// </summary>
        Task ShowInfoAsync(string title, string message, string? details = null);
    }

    /// <summary>
    /// Error severity levels for appropriate UI surfacing.
    /// </summary>
    public enum ErrorSeverity
    {
        Fatal,    // App cannot continue - Fatal Dialog
        Error,    // Action failed, operator must act - Error Dialog
        Warning,  // System degraded but usable - Warning Banner
        Info       // Non-blocking status - Info Toast
    }
}