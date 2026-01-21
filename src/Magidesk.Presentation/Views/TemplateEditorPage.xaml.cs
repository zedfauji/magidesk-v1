using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation; // Added
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class TemplateEditorPage : Page
{
    public TemplateEditorViewModel ViewModel { get; }

    private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    public TemplateEditorPage()
    {
        InitializeComponent();
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread(); 
        ViewModel = App.Services.GetRequiredService<TemplateEditorViewModel>();
        DataContext = ViewModel;

        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }
    
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            if (e.Parameter is Guid id)
            {
                await ViewModel.InitializeAsync(id);
            }
            else
            {
                await ViewModel.InitializeAsync(null);
            }
        }
        catch (Exception ex)
        {
            App.MessageBox(IntPtr.Zero, $"Failed to load template editor: {ex.Message}", "Error", 0x10);
        }
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is TemplateEditorViewModel vm)
        {
            vm.PropertyChanged += ViewModel_PropertyChanged;
            // Initial load if any
            if (!string.IsNullOrEmpty(vm.PreviewHtml))
            {
                try
                {
                    await PreviewWebView.EnsureCoreWebView2Async();
                    PreviewWebView.NavigateToString(vm.PreviewHtml);
                }
                catch (Exception ex)
                {
                    // If WebView2 fails to init (e.g. missing runtime), we shouldn't crash.
                    System.Diagnostics.Debug.WriteLine($"WebView2 Init Failed: {ex.Message}");
                }
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is TemplateEditorViewModel vm)
        {
            vm.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TemplateEditorViewModel.PreviewHtml))
        {
            // Use sender to avoid touching UI properties (DataContext) on bg thread
            if (sender is TemplateEditorViewModel vm)
            {
                _dispatcherQueue.TryEnqueue(() => 
                {
                    try 
                    {
                        PreviewWebView.NavigateToString(vm.PreviewHtml);
                    }
                    catch { /* WebView might not be ready */ }
                });
            }
        }
    }
}
