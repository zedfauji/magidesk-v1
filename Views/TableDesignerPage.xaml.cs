using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using Magidesk.Application.DTOs;
using Magidesk.Presentation.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace Magidesk.Presentation.Views;

public sealed partial class TableDesignerPage : Page
{
    private TableDto? _draggedTable;
    private Point _dragStartPosition;
    private bool _isDragging = false;

    public TableDesignerViewModel ViewModel { get; }

    public TableDesignerPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TableDesignerViewModel>();
        ShapePalette.ViewModel = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // FEH-004: Async Void Barrier
        try
        {
            await ViewModel.LoadDataAsync();
        }
        catch (Exception ex)
        {
             // We can't easily show a dialog here since the page might not be fully loaded,
             // but IDialogService logic via NavigationService is robust now (NAV-001).
             try
             {
                 var dialogService = App.Services.GetRequiredService<Magidesk.Application.Interfaces.IDialogService>();
                 await dialogService.ShowErrorAsync("Designer Error", "Failed to load table designer.", ex.ToString());
             }
             catch { /* Last resort silence */ }
        }
    }

    private void Canvas_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (!ViewModel.IsDesignMode) return;

        var position = e.GetPosition(null);
        var point = new Point((int)position.X, (int)position.Y);
        ViewModel.AddTableCommand.Execute(point);
    }

    private void Table_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        if (!ViewModel.IsDesignMode) return;

        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            _draggedTable = table;
            _dragStartPosition = new Point(0, 0); // Simplified for WinUI 3
            _isDragging = true;

            args.Data.SetText($"Table:{table.Id}");
            
            // Note: SetContentFromDataPackage is not available in WinUI 3. 
            // We use simple dragging or custom bitmap if needed.
        }
    }

    private void Table_Drop(object sender, DragEventArgs e)
    {
        if (!ViewModel.IsDesignMode || _draggedTable == null) return;

        var position = e.GetPosition(null);
        var newX = (int)(position.X - 50); // Center the table on drop point
        var newY = (int)(position.Y - 50);

        // Update table position
        _draggedTable.X = newX;
        _draggedTable.Y = newY;

        // Reset drag state
        _draggedTable = null;
        _isDragging = false;
    }

    private void Table_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            ViewModel.SelectTableCommand.Execute(table);
        }
    }

    private void Table_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (!ViewModel.IsDesignMode) return;

        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            _draggedTable = table;
            _dragStartPosition = e.Position;
            _isDragging = true;
        }
    }

    private void Table_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (!_isDragging || _draggedTable == null) return;

        var delta = e.Delta;
        _draggedTable.X = Math.Max(0, (int)(_draggedTable.X + delta.Translation.X));
        _draggedTable.Y = Math.Max(0, (int)(_draggedTable.Y + delta.Translation.Y));
    }

    private void Table_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        _isDragging = false;
        _draggedTable = null;
    }
}
