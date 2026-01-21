using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Microsoft.UI.Dispatching; // For DispatcherQueue (Timer replacement for WinUI)
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;


    public class KitchenDisplayViewModel : ViewModelBase
    {
        private readonly IKitchenOrderRepository _repository;
        private readonly IKitchenStatusService _statusService;
        private readonly IOrderNotificationService _notificationService;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly DispatcherQueueTimer _timer;

        public ObservableCollection<KitchenOrderViewModel> Orders { get; } = new();
        
        private string _lastUpdated = "Never";
        public string LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }
    
    public ICommand BumpCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ToggleHistoryCommand { get; }

    private bool _isHistoryMode;
    public bool IsHistoryMode
    {
        get => _isHistoryMode;
        set
        {
            if (SetProperty(ref _isHistoryMode, value))
            {
                OnPropertyChanged(nameof(ViewTitle));
                _ = LoadOrdersAsync();
            }
        }
    }

    public Services.LocalizationService Localization { get; }

    public string ViewTitle => IsHistoryMode ? "KD_HistoryTitle" : "KD_Title";

    public KitchenDisplayViewModel(
        IKitchenOrderRepository repository,
        IKitchenStatusService statusService,
        IOrderNotificationService notificationService,
        Services.LocalizationService localizationService)
    {
        _repository = repository;
        _statusService = statusService;
        _notificationService = notificationService;
        Localization = localizationService;
        
        BumpCommand = new AsyncRelayCommand<KitchenOrderViewModel>(BumpOrderAsync);
        RefreshCommand = new AsyncRelayCommand(LoadOrdersAsync);
        ToggleHistoryCommand = new RelayCommand(() => IsHistoryMode = !IsHistoryMode);
        
        _lastUpdated = Localization["KD_Never"];

        // Setup Polling Timer (every 10 seconds)
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (_dispatcherQueue != null)
        {
            _timer = _dispatcherQueue.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(10);
            _timer.Tick += (s, e) => _ = LoadOrdersAsync();
            _timer.Start();
        }
    }
    
    public async Task LoadOrdersAsync()
    {
        try
        {
            System.Collections.Generic.IEnumerable<Magidesk.Domain.Entities.KitchenOrder> fetchedOrders;

            if (IsHistoryMode)
            {
                fetchedOrders = await _repository.GetCompletedOrdersAsync(50);
            }
            else
            {
                fetchedOrders = await _repository.GetActiveOrdersAsync();
            }
            
            // Simple approach: Clear and Add. 
            // Better approach for UI stability: Merge/Update, but MVP first.
            Orders.Clear();
            foreach (var order in fetchedOrders)
            {
                Orders.Add(new KitchenOrderViewModel(order));
            }
            
            LastUpdated = DateTime.Now.ToString("HH:mm:ss");
        }
        catch (Exception)
        {
            // Handle error (maybe set status property)
            LastUpdated = "Error connecting";
        }
    }
    
    // Explicitly expose startup method if Dispatcher isn't ready in ctor
    public void StartPolling()
    {
        if (_timer != null && !_timer.IsRunning) _timer.Start();
        _ = LoadOrdersAsync(); // Initial Load
    }

    public void StopPolling()
    {
        if (_timer != null && _timer.IsRunning) _timer.Stop();
    }

    private async Task BumpOrderAsync(KitchenOrderViewModel? vm)
    {
        if (vm == null) return;
        
        // If in history mode, maybe we want to "Restore" (Undo)?
        // For now, disable Bump in History or ensure it doesn't break.
        if (IsHistoryMode) return; 

        try
        {
            // Use the enhanced KitchenStatusService which now sends notifications
            await _statusService.BumpOrderAsync(vm.Id);
            
            // Refresh the orders list to show updated status
            await LoadOrdersAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Bump Order Error: {ex.Message}");
            
            // Show error dialog
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Error",
                Content = $"Failed to update order status: {ex.Message}",
                CloseButtonText = "OK"
            };
            
            // Set XamlRoot if available
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            }
            
            await dialog.ShowAsync();
        }
    }
}
