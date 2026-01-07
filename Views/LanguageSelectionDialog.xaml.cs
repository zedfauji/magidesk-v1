using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views;

public sealed partial class LanguageSelectionDialog : ContentDialog
{
    public LanguageSelectionViewModel ViewModel { get; }

    public LanguageSelectionDialog(LanguageSelectionViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
        
        this.PrimaryButtonClick += LanguageSelectionDialog_PrimaryButtonClick;
    }

    private async void LanguageSelectionDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Get deferral to allow async save operation
        var deferral = args.GetDeferral();
        try
        {
            await ViewModel.SaveCommand.ExecuteAsync(null);
        }
        finally
        {
            deferral.Complete();
        }
    }
}
