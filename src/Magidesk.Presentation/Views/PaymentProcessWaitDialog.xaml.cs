using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views;

public sealed partial class PaymentProcessWaitDialog : ContentDialog
{
    public PaymentProcessWaitViewModel ViewModel { get; }

    public PaymentProcessWaitDialog()
    {
        this.InitializeComponent();
        ViewModel = Magidesk.Presentation.App.Services.GetRequiredService<PaymentProcessWaitViewModel>();
        this.DataContext = ViewModel;
    }
    
    // Prevent user from closing via Esc or clicking outside
    private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        // Require explicit programmatic closure (ViewModel or Service triggering Hide)
        // However, ContentDialog.Hide() bypasses this check or we can set args.Cancel = true
        // If the result is 'None', it implies cancellation/dismissal. 
        // We only allow it if explicitly handled. 
        
        // For simple blocking, we can set Cancel = true unless we have a specific flag.
        // Actually, WinUI ContentDialog Hide() is programmatic. User actions trigger closing with Result=None.
        
        // Let's implement a simple lock.
        if (!ViewModel.CanClose)
        {
            args.Cancel = true;
        }
    }
}
