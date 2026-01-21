using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views;

public sealed partial class ActiveSessionsPanel : UserControl
{
    public ActiveSessionsPanelViewModel ViewModel { get; }

    public ActiveSessionsPanel(ActiveSessionsPanelViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        
        this.InitializeComponent();

        // Initialize on load
        this.Loaded += async (s, e) => await ViewModel.InitializeAsync();
        this.Unloaded += (s, e) => ViewModel.Dispose();
    }
}
