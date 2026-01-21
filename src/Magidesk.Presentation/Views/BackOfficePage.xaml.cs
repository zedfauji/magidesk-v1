using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection; // Ensure this is available

namespace Magidesk.Presentation.Views;

public sealed partial class BackOfficePage : Page
{
    public BackOfficeViewModel ViewModel { get; }

    public BackOfficePage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<BackOfficeViewModel>();
        DataContext = ViewModel;
        
        // Handle Navigation
        var navView = this.Content as Grid; 
        if (navView?.Children[1] is NavigationView nv)
        {
            nv.SelectionChanged += Nv_SelectionChanged;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Type targetType)
        {
             // Find item matching type
             var navView = (this.Content as Grid)?.Children[1] as NavigationView;
             if (navView != null)
             {
                 var item = ViewModel.NavigationItems.FirstOrDefault(i => i.PageType == targetType);
                 if (item != null)
                 {
                     navView.SelectedItem = item;
                     // SelectionChanged will fire and navigate
                 }
             }
        }
    }

    private void Nv_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationItem item)
        {
            ViewModel.NavigateCommand.Execute(item);

            if (item.PageType == typeof(object))
            {
                _ = ShowNotImplementedAsync(item.Title);
                return;
            }

            ContentFrame.Navigate(item.PageType);
        }
    }

    private async Task ShowNotImplementedAsync(string title)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = "Not implemented yet.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
