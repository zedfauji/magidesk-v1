using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.Services;

/// <summary>
/// WinUI 3 implementation of IDialogService.
/// Handles XamlRoot resolution and provides standard dialog templates.
/// </summary>
public class WindowsDialogService : IDialogService
{
    private readonly NavigationService _navigationService;

    public WindowsDialogService(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async Task ShowErrorAsync(string title, string message, string? exceptionDetails = null)
    {
        await ShowDialogInternalAsync(title, message, "OK", null, ContentDialogButton.Primary, exceptionDetails);
    }

    public async Task ShowWarningAsync(string title, string message)
    {
        await ShowDialogInternalAsync(title, message, "OK", null, ContentDialogButton.Primary);
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        await ShowDialogInternalAsync(title, message, "OK", null, ContentDialogButton.Primary);
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string yesText = "Yes", string noText = "No")
    {
        var result = await ShowDialogInternalAsync(title, message, yesText, noText, ContentDialogButton.Primary);
        return result == ContentDialogResult.Primary;
    }

    private async Task<ContentDialogResult> ShowDialogInternalAsync(
        string title, 
        string message, 
        string primaryBtnText, 
        string? secondaryBtnText, 
        ContentDialogButton defaultBtn,
        string? expandableContent = null)
    {
        // Safety: Ensure we operate on UI thread if this is called from background
        if (_navigationService.DispatcherQueue != null && !_navigationService.DispatcherQueue.HasThreadAccess)
        {
            var tcs = new TaskCompletionSource<ContentDialogResult>();
            _navigationService.DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    var res = await ShowDialogInternalAsync(title, message, primaryBtnText, secondaryBtnText, defaultBtn, expandableContent);
                    tcs.SetResult(res);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        try
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = CreateDialogContent(message, expandableContent),
                PrimaryButtonText = primaryBtnText,
                SecondaryButtonText = secondaryBtnText ?? string.Empty,
                DefaultButton = defaultBtn
            };

            // Use NavigationService's helper to safely finding XamlRoot
            // logic matches NAV-001 requirements (we will harden NavigationService next, 
            // but for now we delegate to it or duplicate safe logic if NavigationService isn't hardened yet).
            // Since NAV-001 is next, we will rely on NavigationService.ShowDialogAsync which currently exists 
            // but needs the fix. Using it consolidates the fix location.
            
            return await _navigationService.ShowDialogAsync(dialog);
        }
        catch (Exception ex)
        {
            // Last line of defense: Log to Debug if dialog fails (e.g. no Window active)
            System.Diagnostics.Debug.WriteLine($"[DialogService] FAILED to show dialog '{title}': {ex}");
            return ContentDialogResult.None;
        }
    }

    private object CreateDialogContent(string message, string? extraDetails)
    {
        if (string.IsNullOrEmpty(extraDetails))
        {
            return new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, MaxWidth = 400 };
        }

        var sp = new StackPanel { Spacing = 10, MaxWidth = 500 };
        sp.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap });
        
        var expander = new Expander
        {
            Header = "Technical Details",
            Content = new ScrollViewer 
            { 
                Content = new TextBlock { Text = extraDetails, TextWrapping = TextWrapping.Wrap, FontSize = 12, FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas") },
                MaxHeight = 200,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            }
        };
        sp.Children.Add(expander);

        return sp;
    }
}
