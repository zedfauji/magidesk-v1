using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Microsoft.UI.Dispatching; // For DispatcherQueue (Timer replacement for WinUI)
using CommunityToolkit.Mvvm.Input;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;
using Magidesk.Application.Services; // For OrderNotification DTO
using Microsoft.AspNetCore.SignalR.Client;

namespace Magidesk.Presentation.ViewModels;


    public class KitchenDisplayViewModel : ViewModelBase
    {
        private readonly IKitchenOrderRepository _repository;
        private readonly IKitchenStatusService _statusService;

        private readonly IPrinterGroupRepository _printerGroupRepository;
        private readonly IKdsSettingsService _settingsService;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly DispatcherQueueTimer _timer;
        private HubConnection? _hubConnection;

        public ObservableCollection<KitchenOrderViewModel> Orders { get; } = new();
        public ObservableCollection<PrinterGroup> AvailableStations { get; } = new();

        private PrinterGroup? _selectedStation;
        public PrinterGroup? SelectedStation
        {
            get => _selectedStation;
            set
            {
                if (SetProperty(ref _selectedStation, value))
                {
                    _settingsService.SetSelectedStationId(value?.Id);
                    _ = LoadOrdersAsync();
                }
            }
        }
        
        private string _lastUpdated = "Never";
        public string LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }
    
    public ICommand BumpCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ToggleHistoryCommand { get; }
    public ICommand MarkAsDeliveredCommand { get; }

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

    public Magidesk.Presentation.Services.LocalizationService Localization { get; }

    public string ViewTitle => IsHistoryMode ? "KD_HistoryTitle" : "KD_Title";

    public KitchenDisplayViewModel(
        IKitchenOrderRepository repository,
        IKitchenStatusService statusService,

        IPrinterGroupRepository printerGroupRepository,
        IKdsSettingsService settingsService,
        Magidesk.Presentation.Services.LocalizationService localizationService)
    {
        _repository = repository;
        _statusService = statusService;

        _printerGroupRepository = printerGroupRepository;
        _settingsService = settingsService;
        Localization = localizationService;
        
        BumpCommand = new AsyncRelayCommand<KitchenOrderViewModel>(BumpOrderAsync);
        RefreshCommand = new AsyncRelayCommand(LoadOrdersAsync);
        ToggleHistoryCommand = new RelayCommand(() => IsHistoryMode = !IsHistoryMode);
        MarkAsDeliveredCommand = new AsyncRelayCommand<KitchenOrderViewModel>(MarkAsDeliveredAsync);
        
        _lastUpdated = Localization["KD_Never"];

        // Setup Polling Timer (Fallback - 60 seconds)
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (_dispatcherQueue != null)
        {
            _timer = _dispatcherQueue.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(60); 
            _timer.Tick += (s, e) => _ = LoadOrdersAsync();
            // Do NOT start timer here. It will be started by SignalR fallback if needed.
        }

        // Initialize Sequentially to avoid DbContext concurrency issues (NpgsqlOperationInProgressException)
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await LoadStationsAsync(); // 1. Load Stations (Required for filtering)
            await LoadOrdersAsync();   // 2. Initial Order Load (Show data immediately)
            await InitializeSignalRAsync(); // 3. Start Realtime (Might enable polling fallback)
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"KDS Initialization Failed: {ex.Message}");
            // Ensure polling is active if everything else fails
            _dispatcherQueue.TryEnqueue(() => _timer?.Start());
        }
    }

    private async Task InitializeSignalRAsync()
    {
        try
        {
            var baseUrl = _settingsService.GetApiBaseUrl().TrimEnd('/');
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/hubs/kitchen")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<OrderNotification>("OrderUpdated", (notification) =>
            {
                // Dispatch to UI Thread
                _dispatcherQueue.TryEnqueue(() => 
                {
                     // In the future: Check notification.KitchenOrderId or Type to opt-out
                     _ = LoadOrdersAsync(); 
                });
            });

            _hubConnection.Closed += async (error) => 
            {
                System.Diagnostics.Debug.WriteLine($"SignalR Closed: {error?.Message}. Starting Polling.");
                _dispatcherQueue.TryEnqueue(() => _timer.Start());
                await Task.CompletedTask;
            };

            _hubConnection.Reconnecting += (error) =>
            {
                System.Diagnostics.Debug.WriteLine($"SignalR Reconnecting: {error?.Message}. Starting Polling.");
                _dispatcherQueue.TryEnqueue(() => _timer.Start());
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                System.Diagnostics.Debug.WriteLine("SignalR Reconnected. Stopping Polling.");
                _dispatcherQueue.TryEnqueue(() => _timer.Stop());
                return Task.CompletedTask;
            };

            await _hubConnection.StartAsync();
            System.Diagnostics.Debug.WriteLine("KDS Connected to SignalR Hub. Stopping Polling.");
            _dispatcherQueue.TryEnqueue(() => _timer.Stop());
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"SignalR Connection Failed: {ex.Message}");
             // Fallback to polling is already active from Constructor
             _dispatcherQueue.TryEnqueue(() => _timer.Start());
        }
    }

        private readonly System.Threading.SemaphoreSlim _loadingLock = new(1, 1);

    public async Task LoadStationsAsync()
    {
        await _loadingLock.WaitAsync();
        try
        {
            var groups = await _printerGroupRepository.GetAllAsync();
            AvailableStations.Clear();
            foreach (var group in groups)
            {
                AvailableStations.Add(group);
            }

            var savedId = _settingsService.GetSelectedStationId();
            if (savedId.HasValue)
            {
                var station = System.Linq.Enumerable.FirstOrDefault(AvailableStations, s => s.Id == savedId.Value);
                if (SelectedStation != station)
                {
                    // Update field directly to avoid triggering setter re-entry if we wanted, 
                    // but setter triggers LoadOrdersAsync which is now locked too.
                    // Ideally we set backing field and load manually to control flow.
                    if (SetProperty(ref _selectedStation, station, nameof(SelectedStation)))
                    {
                         _settingsService.SetSelectedStationId(station?.Id);
                         // Don't fire LoadOrdersAsync here, we will do it in InitializeAsync anyway
                         // But if user changes it later, we need to.
                         // For now, let the setter fire it, the lock will handle it.
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load stations: {ex.Message}");
        }
        finally
        {
            _loadingLock.Release();
        }
    }
    
    public async Task LoadOrdersAsync()
    {
        // If we can't get the lock immediately (e.g. another load is in progress), 
        // we should wait.
        await _loadingLock.WaitAsync();
        try
        {
            IEnumerable<Magidesk.Domain.Entities.KitchenOrder> fetchedOrders;

            if (IsHistoryMode)
            {
                fetchedOrders = await _repository.GetCompletedOrdersAsync(50);
            }
            else
            {
                fetchedOrders = await _repository.GetActiveOrdersAsync();
            }
            
            // Filter by station if selected
            if (SelectedStation != null)
            {
                fetchedOrders = fetchedOrders.Where(o => o.PrinterGroupId == SelectedStation.Id);
            }
            
            var fetchedList = fetchedOrders.ToList();

            // Sync collection (Smart Merge)
            for (int i = 0; i < fetchedList.Count; i++)
            {
                var newOrder = fetchedList[i];
                var newVM = new KitchenOrderViewModel(newOrder);

                if (i >= Orders.Count)
                {
                    // Append new item
                    Orders.Add(newVM);
                }
                else
                {
                    var existingVM = Orders[i];
                    if (existingVM.Id == newOrder.Id)
                    {
                         // Update in place (Replace to refresh content like Status/TimeAgo)
                         Orders[i] = newVM;
                    }
                    else
                    {
                        // ID Mismatch - Check if existing item is elsewhere in current orders
                        var matchIndex = -1;
                        for (int j = i + 1; j < Orders.Count; j++)
                        {
                            if (Orders[j].Id == newOrder.Id)
                            {
                                matchIndex = j;
                                break;
                            }
                        }

                        if (matchIndex != -1)
                        {
                            // Move to current position (restores order)
                            Orders.Move(matchIndex, i);
                            Orders[i] = newVM; // Update content
                        }
                        else
                        {
                            // New item, insert here
                            Orders.Insert(i, newVM);
                        }
                    }
                }
            }

            // Remove extra items (stale orders)
            while (Orders.Count > fetchedList.Count)
            {
                Orders.RemoveAt(Orders.Count - 1);
            }
            
            LastUpdated = DateTime.Now.ToString("HH:mm:ss");
        }
        catch (Exception ex)
        {
            LastUpdated = "Error connecting";
            System.Diagnostics.Debug.WriteLine($"LoadOrders Error: {ex.Message}");
        }
        finally
        {
            _loadingLock.Release();
        }
    }
    
    // Explicitly expose startup method if Dispatcher isn't ready in ctor
    public void StartPolling()
    {
        if (_timer != null && !_timer.IsRunning) _timer.Start();
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Disconnected)
        {
             _ = _hubConnection.StartAsync();
        }
        _ = LoadOrdersAsync(); // Initial Load
    }

    public void StopPolling()
    {
        if (_timer != null && _timer.IsRunning) _timer.Stop();
        if (_hubConnection != null)
        {
            _ = _hubConnection.StopAsync();
        }
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

    private async Task MarkAsDeliveredAsync(KitchenOrderViewModel? vm)
    {
        if (vm == null) return;
        
        if (IsHistoryMode) return;

        try
        {
            await _statusService.MarkAsDeliveredAsync(vm.Id);
            await LoadOrdersAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Mark As Delivered Error: {ex.Message}");
            
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Error",
                Content = $"Failed to mark order as delivered: {ex.Message}",
                CloseButtonText = "OK"
            };
            
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            }
            
            await dialog.ShowAsync();
        }
    }
}
