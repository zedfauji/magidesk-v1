using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views.Components;

public sealed partial class TableShapePalette : UserControl
{
    public TableDesignerViewModel? ViewModel { get; set; }

    public TableShapePalette()
    {
        this.InitializeComponent();
    }
}
