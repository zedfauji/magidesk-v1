using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views
{
    public sealed partial class ManagerFunctionsDialog : ContentDialog
    {
        public ManagerFunctionsViewModel ViewModel { get; }

        public ManagerFunctionsDialog()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ManagerFunctionsViewModel>();
            ViewModel.CloseAction = () => this.Hide();
            this.DataContext = ViewModel;
        }
    }
}
