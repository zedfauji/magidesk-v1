using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Magidesk.Application.DTOs;
using Magidesk.Presentation.ViewModels;
using Windows.Foundation;
using Windows.ApplicationModel.DataTransfer;

namespace Magidesk.Presentation.Views.Components;

public sealed partial class VirtualizedTableCanvas : UserControl
{
    private TableDesignerViewModel? _viewModel;
    private DispatcherTimer? _fpsTimer;
    private DateTime _lastFrameTime = DateTime.UtcNow;
    private int _frameCount = 0;
    private const int FPS_UPDATE_INTERVAL = 1000; // 1 second

    public VirtualizedTableCanvas()
    {
        this.InitializeComponent();
        
        // Initialize FPS timer
        _fpsTimer = new DispatcherTimer();
        _fpsTimer.Interval = TimeSpan.FromMilliseconds(FPS_UPDATE_INTERVAL);
        _fpsTimer.Tick += UpdateFPS;
        _fpsTimer.Start();

        this.Unloaded += OnUnloaded;
    }

    public TableDesignerViewModel ViewModel
    {
        get => _viewModel!;
        set
        {
            _viewModel = value;
            DataContext = value;
        }
    }

    private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        UpdateVisibleTables();
    }

    private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateVisibleTables();
    }

    private void UpdateVisibleTables()
    {
        if (_viewModel == null || MainScrollViewer == null) return;

        var viewport = new Rect(
            MainScrollViewer.HorizontalOffset,
            MainScrollViewer.VerticalOffset,
            MainScrollViewer.ViewportWidth,
            MainScrollViewer.ViewportHeight
        );

        // Update visible tables based on viewport
        _viewModel.UpdateVisibleTables(viewport);
    }

    private void Table_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        if (_viewModel?.IsDesignMode != true) return;

        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            args.Data.SetText($"Table:{table.Id}");
        }
    }

    private void Table_Drop(object sender, DragEventArgs e)
    {
        if (_viewModel?.IsDesignMode != true) return;

        var position = e.GetPosition(null);
        // Handle table position update
    }

    private void Table_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            _viewModel?.SelectTableCommand.Execute(table);
        }
    }

    private void Table_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (_viewModel?.IsDesignMode != true) return;

        if (sender is FrameworkElement element && element.DataContext is TableDto table)
        {
            // Start drag operation
        }
    }

    private void Table_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (_viewModel?.IsDesignMode != true) return;

        // Handle drag delta
    }

    private void Table_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        // Complete drag operation
    }

    private void UpdateFPS(object? sender, object e)
    {
        var now = DateTime.UtcNow;
        var elapsed = (now - _lastFrameTime).TotalMilliseconds;
        
        if (elapsed > 0)
        {
            _viewModel.CurrentFPS = (int)(_frameCount * 1000 / elapsed);
        }

        _frameCount = 0;
        _lastFrameTime = now;
        _viewModel.LastRenderTime = now;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _fpsTimer?.Stop();
        _fpsTimer = null;
    }
}
