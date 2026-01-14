using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Magidesk.Presentation.ViewModels;
using System.Linq;

namespace Magidesk.Presentation.Views;

public sealed partial class SwitchboardPage : Page
{
    public SwitchboardViewModel ViewModel { get; }
    private IServiceScope? _scope;

    public SwitchboardPage()
    {
        InitializeComponent();
        _scope = App.Services.CreateScope();
        ViewModel = _scope.ServiceProvider.GetRequiredService<SwitchboardViewModel>();
        DataContext = ViewModel;

        // Add keyboard shortcut handling
        this.KeyDown += Page_KeyDown;

        this.Unloaded += (s, e) =>
        {
             _scope?.Dispose();
             _scope = null;
        };
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Load open tickets when page is displayed
        await ViewModel.LoadTicketsAsync();
        
        // Refresh live counts
        await ViewModel.RefreshLiveCountsAsync();
    }

    private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        // Handle F1-F12 keyboard shortcuts
        var button = e.Key switch
        {
            VirtualKey.F1 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F1"),
            VirtualKey.F2 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F2"),
            VirtualKey.F3 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F3"),
            VirtualKey.F4 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F4"),
            VirtualKey.F5 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F5"),
            VirtualKey.F6 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F6"),
            VirtualKey.F7 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F7"),
            VirtualKey.F8 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F8"),
            VirtualKey.F9 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F9"),
            VirtualKey.F10 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F10"),
            VirtualKey.F11 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F11"),
            VirtualKey.F12 => ViewModel.NavigationButtons.FirstOrDefault(b => b.KeyboardShortcut == "F12"),
            _ => null
        };

        if (button != null && button.IsEnabled)
        {
            ViewModel.NavigateCommand.Execute(button);
            e.Handled = true;
        }
    }
}
