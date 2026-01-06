using Microsoft.UI.Xaml.Controls;
using Magidesk.Infrastructure.Services;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Minimal navigation service for WinUI 3.
/// </summary>
public class NavigationService
{
    private Frame? _frame;
    private readonly Application.Interfaces.IUserService _userService;

    public NavigationService(Application.Interfaces.IUserService userService)
    {
        _userService = userService;
    }

    private Microsoft.UI.Dispatching.DispatcherQueue? _dispatcherQueue;

    public void Initialize(Frame frame)
    {
        _frame = frame;
        _dispatcherQueue = frame.DispatcherQueue;
    }

    public bool CanGoBack => _frame?.CanGoBack == true;

    public virtual void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }

    public virtual bool Navigate(Type pageType, object? parameter = null)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("NavigationService is not initialized. Call Initialize(frame) first.");
        }

        // AUTH GUARD
        // Allow public access to LoginPage
        if (pageType == typeof(Views.LoginPage))
        {
             return _frame.Navigate(pageType, parameter);
        }

        // Block all other pages if not authenticated
        if (_userService.CurrentUser == null)
        {
            // Redirect to Login if not already there
            if (_frame.CurrentSourcePageType != typeof(Views.LoginPage))
            {
                System.Diagnostics.Debug.WriteLine($"[NavigationService] Auth Access Denied to {pageType.Name}. Redirecting to Login.");
                _frame.Navigate(typeof(Views.LoginPage));
                
                // T-009: Strict Auth Return + Feedback
                // Return false because we did NOT go to the requested page.
                // Optionally show a message? "Please log in."
                // Since we are redirecting to login, the login page itself is the feedback.
                return false; 
            }
            
            // If we are already on Login page and try to nav elsewhere without user
            System.Diagnostics.Debug.WriteLine($"[NavigationService] Auth Access Denied to {pageType.Name}. Already on Login.");
            return false;
        }

        return _frame.Navigate(pageType, parameter);
    }

    public virtual async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("NavigationService is not initialized. Call Initialize(frame) first.");
        }

        // Ensure we run on UI thread using cached DispatcherQueue (safe from any thread)
        if (_dispatcherQueue != null && !_dispatcherQueue.HasThreadAccess)
        {
            var tcs = new TaskCompletionSource<ContentDialogResult>();
            _dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    var result = await ShowDialogAsync(dialog);
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        var attempts = 0;
        while (_frame.XamlRoot == null && attempts < 40)
        {
            await Task.Delay(50);
            attempts++;
        }

        if (_frame.XamlRoot == null)
        {
            System.Diagnostics.Debug.WriteLine($"[NavigationService] Failed to show dialog '{dialog.Title}'. XamlRoot not found after wait. Falling back.");
            // Default panic fallback...
            return ContentDialogResult.None;
        }

        dialog.XamlRoot = _frame.XamlRoot;
        try 
        {
            return await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NavigationService] ShowAsync Failed: {ex.Message}.");
            return ContentDialogResult.None;
        }
    }

    public virtual async Task ShowErrorAsync(string title, string message)
    {
        var errorDialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK"
        };
        await ShowDialogAsync(errorDialog);
    }

    public virtual async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK"
        };
        await ShowDialogAsync(dialog);
    }

    public Microsoft.UI.Dispatching.DispatcherQueue? DispatcherQueue => _frame?.DispatcherQueue;
}
