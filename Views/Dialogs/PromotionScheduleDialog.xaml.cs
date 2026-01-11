using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class PromotionScheduleDialog : ContentDialog
{
    public PromotionScheduleDialogViewModel ViewModel { get; }

    public PromotionScheduleDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PromotionScheduleDialogViewModel>();
        this.Name = "RootDialog"; // For ElementName binding
    }
}
