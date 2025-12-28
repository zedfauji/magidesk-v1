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
        ViewModel = App.Services.GetService<BackOfficeViewModel>();
        
        // Handle Navigation
        var navView = this.Content as Grid; 
        if (navView?.Children[1] is NavigationView nv)
        {
            nv.SelectionChanged += Nv_SelectionChanged;
        }
    }

    private void Nv_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationItem item)
        {
            ViewModel.NavigateCommand.Execute(item);
            ContentFrame.Navigate(item.PageType);
        }
    }
}
