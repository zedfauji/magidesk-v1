using Microsoft.UI.Xaml.Controls;

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

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public bool CanGoBack => _frame?.CanGoBack == true;

    public void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }

    public bool Navigate(Type pageType, object? parameter = null)
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
                return _frame.Navigate(typeof(Views.LoginPage));
            }
            return false;
        }

        return _frame.Navigate(pageType, parameter);
    }

    public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("NavigationService is not initialized. Call Initialize(frame) first.");
        }

        var attempts = 0;
        while (_frame.XamlRoot == null && attempts < 40)
        {
            await Task.Delay(50);
            attempts++;
        }

        if (_frame.XamlRoot == null)
        {
            throw new InvalidOperationException("Cannot show dialog: Frame's XamlRoot is null.");
        }

        dialog.XamlRoot = _frame.XamlRoot;
        return await dialog.ShowAsync();
    }
    public Microsoft.UI.Dispatching.DispatcherQueue? DispatcherQueue => _frame?.DispatcherQueue;
}
