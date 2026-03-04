using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;
using Windows.Foundation;
using MediatR;

namespace Magidesk.Presentation.ViewModels;

public partial class TableDesignerViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly NavigationService _navigationService;
    private readonly ITableRepository _tableRepository;

    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly IDialogService _dialogService;
    private readonly IFloorRepository _floorRepository;

    [ObservableProperty]
    private ObservableCollection<TableDto> _tables = new();

    [ObservableProperty]
    private ObservableCollection<FloorDto> _floors = new();

    [ObservableProperty]
    private FloorDto? _selectedFloor;

    [ObservableProperty]
    private ObservableCollection<TableLayoutDto> _layouts = new();

    [ObservableProperty]
    private TableLayoutDto? _selectedLayout;

    [ObservableProperty]
    private string _layoutStatusBadge = "● DRAFT";

    [ObservableProperty]
    private string _layoutStatusText = "Draft";

    [ObservableProperty]
    private TableShapeType _selectedShape = TableShapeType.Rectangle;

    [ObservableProperty]
    private bool _isDesignMode = true;

    [ObservableProperty]
    private string _layoutName = string.Empty;
    
    // Track the currently edited layout ID
    private Guid? _currentLayoutId;

    [ObservableProperty]
    private bool _isDirty;

    [ObservableProperty]
    private bool _isDraftMode = true;

    [ObservableProperty]
    private TableDto? _selectedTable;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isRendering;

    [ObservableProperty]
    private int _currentFPS;

    [ObservableProperty]
    private DateTime _lastRenderTime;

    [ObservableProperty]
    private int _visibleTableCount;

    [ObservableProperty]
    private int _canvasWidth = 2000;

    [ObservableProperty]
    private int _canvasHeight = 2000;

    [ObservableProperty]
    private string _backgroundColor = "#f8f8f8";

    [ObservableProperty]
    private ObservableCollection<TableDto> _visibleTables = new();

    private Rect _viewport = Rect.Empty;
    private readonly object _renderLock = new object();
    private bool _isVirtualizationEnabled = true;

    public IRelayCommand<Point> AddTableCommand { get; }
    public IRelayCommand<TableDto> DeleteTableCommand { get; }
    public IRelayCommand SaveLayoutCommand { get; }
    public IRelayCommand LoadLayoutCommand { get; }
    public IRelayCommand<TableDto> StartDragCommand { get; }
    public IRelayCommand<TableDto> SelectTableCommand { get; }
    public IRelayCommand ToggleDesignModeCommand { get; }
    public IRelayCommand<TableDto> UpdateTablePositionCommand { get; }
    public IRelayCommand DiscardChangesCommand { get; }

    // New Layout Lifecycle Commands
    public IAsyncRelayCommand NewLayoutCommand { get; }
    public IAsyncRelayCommand CloneLayoutCommand { get; }
    public IAsyncRelayCommand DeleteLayoutCommand { get; }
    public IAsyncRelayCommand PublishLayoutCommand { get; }
    public IAsyncRelayCommand DeleteSelectedTablesCommand { get; }
    public IAsyncRelayCommand RevertChangesCommand { get; }

    public TableDesignerViewModel(
        IMediator mediator,
        NavigationService navigationService,
        ITableRepository tableRepository,

        ITableLayoutRepository tableLayoutRepository,
        IDialogService dialogService,
        IFloorRepository floorRepository)
    {
        _mediator = mediator;
        _navigationService = navigationService;
        _tableRepository = tableRepository;
        _tableLayoutRepository = tableLayoutRepository;
        _dialogService = dialogService;
        _floorRepository = floorRepository;

        AddTableCommand = new AsyncRelayCommand<Point>(AddTableAsync);
        DeleteTableCommand = new AsyncRelayCommand<TableDto>(DeleteTableAsync);
        SaveLayoutCommand = new AsyncRelayCommand(SaveLayoutAsync);
        LoadLayoutCommand = new AsyncRelayCommand(LoadLayoutAsync);
        StartDragCommand = new RelayCommand<TableDto>(StartDrag);
        SelectTableCommand = new RelayCommand<TableDto>(SelectTable);
        ToggleDesignModeCommand = new RelayCommand(ToggleDesignMode);
        UpdateTablePositionCommand = new AsyncRelayCommand<TableDto>(UpdateTablePositionAsync);
        DiscardChangesCommand = new AsyncRelayCommand(DiscardChangesAsync);

        // New Layout Lifecycle Commands
        NewLayoutCommand = new AsyncRelayCommand(NewLayoutAsync);
        CloneLayoutCommand = new AsyncRelayCommand(CloneLayoutAsync);
        DeleteLayoutCommand = new AsyncRelayCommand(DeleteLayoutAsync);
        PublishLayoutCommand = new AsyncRelayCommand(PublishLayoutAsync);
        DeleteSelectedTablesCommand = new AsyncRelayCommand(DeleteSelectedTablesAsync);
        RevertChangesCommand = new AsyncRelayCommand(RevertChangesAsync);

        Title = "Table Designer";
    }

    public async Task<bool> UpdateTablePositionAsync(TableDto table)
    {
        // Save logic to update table position will be implemented here
        return await Task.FromResult(true);
    }
}
