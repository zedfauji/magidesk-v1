using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class PaymentPage : Page
{
    public PaymentViewModel ViewModel { get; }

    public PaymentPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PaymentViewModel>();
        DataContext = ViewModel;
    }
}
