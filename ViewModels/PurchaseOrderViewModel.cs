using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

public class PurchaseOrderViewModel : ViewModelBase
{
    private readonly IPurchaseOrderRepository _poRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IInventoryAdjustmentRepository _adjustmentRepository;

    public ObservableCollection<PurchaseOrder> PurchaseOrders { get; } = new();
    public ObservableCollection<Vendor> AvailableVendors { get; } = new();
    public ObservableCollection<InventoryItem> AvailableItems { get; } = new();

    private PurchaseOrder? _selectedPO;
    public PurchaseOrder? SelectedPO
    {
        get => _selectedPO;
        set
        {
            if (SetProperty(ref _selectedPO, value))
            {
                OnPropertyChanged(nameof(CanOrder));
                OnPropertyChanged(nameof(CanReceive));
            }
        }
    }

    private Vendor? _selectedVendor;
    public Vendor? SelectedVendor { get => _selectedVendor; set => SetProperty(ref _selectedVendor, value); }

    public bool CanOrder => SelectedPO?.Status == PurchaseOrderStatus.Draft;
    public bool CanReceive => SelectedPO?.Status == PurchaseOrderStatus.Ordered;

    public ICommand LoadDataCommand { get; }
    public ICommand CreatePOCommand { get; }
    public ICommand OrderPOCommand { get; }
    public ICommand ReceivePOCommand { get; }
    public ICommand AddLineCommand { get; }

    public PurchaseOrderViewModel(
        IPurchaseOrderRepository poRepository,
        IVendorRepository vendorRepository,
        IInventoryItemRepository inventoryRepository,
        IInventoryAdjustmentRepository adjustmentRepository)
    {
        _poRepository = poRepository;
        _vendorRepository = vendorRepository;
        _inventoryRepository = inventoryRepository;
        _adjustmentRepository = adjustmentRepository;

        Title = "Purchase Orders";

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        CreatePOCommand = new AsyncRelayCommand(CreatePOAsync);
        OrderPOCommand = new AsyncRelayCommand(OrderPOAsync);
        ReceivePOCommand = new AsyncRelayCommand(ReceivePOAsync);
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var pos = await _poRepository.GetAllAsync();
            PurchaseOrders.Clear();
            foreach (var po in pos) PurchaseOrders.Add(po);

            var vendors = await _vendorRepository.GetAllAsync();
            AvailableVendors.Clear();
            foreach (var v in vendors) AvailableVendors.Add(v);

            var items = await _inventoryRepository.GetAllAsync();
            AvailableItems.Clear();
            foreach (var i in items) AvailableItems.Add(i);
        }
        finally { IsBusy = false; }
    }

    private async Task CreatePOAsync()
    {
        if (SelectedVendor == null) return;

        var poNumber = $"PO-{DateTime.Now:yyyyMMdd}-{PurchaseOrders.Count + 1:D3}";
        var po = PurchaseOrder.Create(poNumber, SelectedVendor.Id);
        await _poRepository.AddAsync(po);
        PurchaseOrders.Insert(0, po);
        SelectedPO = po;
    }

    private async Task OrderPOAsync()
    {
        if (SelectedPO == null) return;
        SelectedPO.MarkAsOrdered();
        await _poRepository.UpdateAsync(SelectedPO);
        OnPropertyChanged(nameof(CanOrder));
        OnPropertyChanged(nameof(CanReceive));
    }

    private async Task ReceivePOAsync()
    {
        if (SelectedPO == null) return;

        IsBusy = true;
        try
        {
            SelectedPO.MarkAsReceived();
            
            // Update Inventory levels
            foreach (var line in SelectedPO.Lines)
            {
                var item = await _inventoryRepository.GetByIdAsync(line.InventoryItemId);
                if (item != null)
                {
                    item.AdjustStock(line.QuantityExpected);
                    await _inventoryRepository.UpdateAsync(item);
                    
                    // Audit adjustment
                    var adj = InventoryAdjustment.Create(item.Id, line.QuantityExpected, $"Received on PO {SelectedPO.PONumber}");
                    await _adjustmentRepository.AddAsync(adj);
                }
            }

            await _poRepository.UpdateAsync(SelectedPO);
            OnPropertyChanged(nameof(CanOrder));
            OnPropertyChanged(nameof(CanReceive));
        }
        finally { IsBusy = false; }
    }
}
