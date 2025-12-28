using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views;

public sealed partial class MenuEditorPage : Page
{
    public MenuEditorViewModel ViewModel { get; }

    public MenuEditorPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetService<MenuEditorViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (ViewModel.LoadDataCommand.CanExecute(null))
        {
            ViewModel.LoadDataCommand.Execute(null);
        }
    }
}
