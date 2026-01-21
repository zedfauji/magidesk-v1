using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Views.Dialogs;

public sealed partial class CookingInstructionDialog : ContentDialog
{
    public CookingInstructionViewModel ViewModel { get; set; }

    public CookingInstructionDialog()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) => 
        {
             // Ensure DataContext binding for GridView
             if (this.Content is Grid grid) grid.Name = "RootGrid";
             this.DataContext = ViewModel;
        };
    }
}
