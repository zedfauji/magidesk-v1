using Microsoft.Extensions.DependencyInjection; // Add this
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class PrintTemplatesPage : Page
{
    public PrintTemplatesViewModel ViewModel { get; }

    public PrintTemplatesPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PrintTemplatesViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            App.MessageBox(IntPtr.Zero, $"Failed to load templates: {ex.Message}", "Error", 0x10);
        }
    }
}
