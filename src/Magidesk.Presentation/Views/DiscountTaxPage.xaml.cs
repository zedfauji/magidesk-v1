using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class DiscountTaxPage : Page
{
    public DiscountTaxViewModel ViewModel { get; }

    public DiscountTaxPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<DiscountTaxViewModel>();
        DataContext = ViewModel;
    }
}
