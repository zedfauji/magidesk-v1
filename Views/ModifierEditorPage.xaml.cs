using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class ModifierEditorPage : Page
{
    public ModifierEditorViewModel ViewModel { get; }

    public ModifierEditorPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetService<ModifierEditorViewModel>()!;
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
