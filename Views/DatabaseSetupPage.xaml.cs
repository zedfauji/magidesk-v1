using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class DatabaseSetupPage : Page
{
    public DatabaseSetupViewModel ViewModel { get; }

    public DatabaseSetupPage()
    {
        // Get ViewModel from DI
        ViewModel = App.Services.GetRequiredService<DatabaseSetupViewModel>();
        this.DataContext = ViewModel;
        
        this.InitializeComponent();
    }
}
