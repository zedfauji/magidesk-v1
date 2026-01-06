using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using Magidesk.Presentation.Services;
using System.Collections.Concurrent;

namespace Magidesk.Services
{
    /// <summary>
    /// Central error handling service implementation with standardized UI surfacing.
    /// </summary>
    public class ErrorService : IErrorService
    {
        private readonly NavigationService _navigationService;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly ConcurrentQueue<ErrorRequest> _errorQueue = new();

        public ErrorService(NavigationService navigationService)
        {
            _navigationService = navigationService;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public async Task ShowFatalAsync(string title, string message, string? details = null)
        {
            await ShowErrorDialogAsync(title, message, details, "Fatal Error");
        }

        public async Task ShowErrorAsync(string title, string message, string? details = null)
        {
            await ShowErrorDialogAsync(title, message, details, "Error");
        }

        public async Task ShowWarningAsync(string title, string message, string? details = null)
        {
            await ShowWarningBannerAsync(title, message, details);
        }

        public async Task ShowInfoAsync(string title, string message, string? details = null)
        {
            await ShowInfoToastAsync(title, message, details);
        }

        private async Task ShowErrorDialogAsync(string title, string message, string? details, string dialogType)
        {
            await _dispatcherQueue.EnqueueAsync(async () =>
            {
                try
                {
                    var dialog = new ContentDialog
                    {
                        Title = dialogType,
                        Content = CreateDialogContent(title, message, details),
                        CloseButtonText = "OK",
                        XamlRoot = GetXamlRoot()
                    };

                    await _navigationService.ShowDialogAsync(dialog);
                }
                catch (Exception ex)
                {
                    // Last resort - if dialog system fails, log to debug
                    System.Diagnostics.Debug.WriteLine($"[ErrorService] Failed to show {dialogType} dialog: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[ErrorService] Original error: {title} - {message}");
                }
            });
        }

        private async Task ShowWarningBannerAsync(string title, string message, string? details)
        {
            await _dispatcherQueue.EnqueueAsync(async () =>
            {
                try
                {
                    // For now, use dialog as banner fallback
                    // TODO: Implement proper banner system
                    var dialog = new ContentDialog
                    {
                        Title = "Warning",
                        Content = CreateDialogContent(title, message, details),
                        CloseButtonText = "OK",
                        XamlRoot = GetXamlRoot()
                    };

                    await _navigationService.ShowDialogAsync(dialog);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ErrorService] Failed to show warning banner: {ex.Message}");
                }
            });
        }

        private async Task ShowInfoToastAsync(string title, string message, string? details)
        {
            await _dispatcherQueue.EnqueueAsync(async () =>
            {
                try
                {
                    // For now, use dialog as toast fallback
                    // TODO: Implement proper toast system
                    var dialog = new ContentDialog
                    {
                        Title = title,
                        Content = CreateDialogContent(title, message, details),
                        CloseButtonText = "OK",
                        XamlRoot = GetXamlRoot()
                    };

                    await _navigationService.ShowDialogAsync(dialog);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ErrorService] Failed to show info toast: {ex.Message}");
                }
            });
        }

        private object CreateDialogContent(string title, string message, string? details)
        {
            var panel = new StackPanel { Spacing = 8 };
            
            panel.Children.Add(new TextBlock 
            { 
                Text = title, 
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            });

            panel.Children.Add(new TextBlock 
            { 
                Text = message, 
                TextWrapping = TextWrapping.Wrap
            });

            if (!string.IsNullOrEmpty(details))
            {
                panel.Children.Add(new TextBlock 
                { 
                    Text = $"Details: {details}", 
                    TextWrapping = TextWrapping.Wrap,
                    FontStyle = Microsoft.UI.Text.FontStyle.Italic
                });
            }

            return panel;
        }

        private object? GetXamlRoot()
        {
            try
            {
                return App.MainWindowInstance?.Content?.XamlRoot;
            }
            catch
            {
                return null;
            }
        }

        private record ErrorRequest(
            ErrorSeverity Severity,
            string Title,
            string Message,
            string? Details = null
        );
    }
}