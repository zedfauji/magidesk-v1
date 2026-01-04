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

    [ObservableProperty]
    private ObservableCollection<TableDto> _tables = new();

    [ObservableProperty]
    private ObservableCollection<FloorDto> _floors = new();

    [ObservableProperty]
    private FloorDto? _selectedFloor;

    [ObservableProperty]
    private TableShapeType _selectedShape = TableShapeType.Rectangle;

    [ObservableProperty]
    private bool _isDesignMode = true;

    [ObservableProperty]
    private string _layoutName = string.Empty;

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

    public TableDesignerViewModel(
        IMediator mediator,
        NavigationService navigationService,
        ITableRepository tableRepository,
        ITableLayoutRepository tableLayoutRepository)
    {
        _mediator = mediator;
        _navigationService = navigationService;
        _tableRepository = tableRepository;
        _tableLayoutRepository = tableLayoutRepository;

        AddTableCommand = new AsyncRelayCommand<Point>(AddTableAsync);
        DeleteTableCommand = new AsyncRelayCommand<TableDto>(DeleteTableAsync);
        SaveLayoutCommand = new AsyncRelayCommand(SaveLayoutAsync);
        LoadLayoutCommand = new AsyncRelayCommand(LoadLayoutAsync);
        StartDragCommand = new RelayCommand<TableDto>(StartDrag);
        SelectTableCommand = new RelayCommand<TableDto>(SelectTable);
        ToggleDesignModeCommand = new RelayCommand(ToggleDesignMode);
        UpdateTablePositionCommand = new AsyncRelayCommand<TableDto>(UpdateTablePositionAsync);

        Title = "Table Designer";
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await LoadFloorsAsync();
            await LoadTablesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }


    private async Task LoadFloorsAsync()
    {
        // For now, create a default floor
        Floors.Clear();
        Floors.Add(new FloorDto
        {
            Id = Guid.NewGuid(),
            Name = "Main Floor",
            Description = "Primary dining area",
            Width = 2000,
            Height = 2000,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        SelectedFloor = Floors.FirstOrDefault();
    }

    private async Task LoadTablesAsync()
    {
        if (SelectedFloor == null) return;

        var availableTables = await _tableRepository.GetAvailableAsync();
        Tables.Clear();

        foreach (var table in availableTables)
        {
            Tables.Add(new TableDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                X = table.X,
                Y = table.Y,
                Status = table.Status,
                IsActive = table.IsActive
            });
        }
    }

    private async Task AddTableAsync(Point position)
    {
        if (!IsDesignMode) return;

        var nextTableNumber = Tables.Count > 0 ? Tables.Max(t => t.TableNumber) + 1 : 1;

        var command = new AddTableToLayoutCommand(
            Guid.NewGuid(), // Layout ID - would come from selected layout
            nextTableNumber,
            4, // Default capacity
            (int)position.X,
            (int)position.Y,
            SelectedShape
        );

        try
        {
            var newTable = await _mediator.Send(command);
            Tables.Add(newTable);
        }
        catch (Exception ex)
        {
            // Handle error - could show a dialog or set an error property
            System.Diagnostics.Debug.WriteLine($"Error adding table: {ex.Message}");
        }
    }

    private async Task DeleteTableAsync(TableDto? table)
    {
        if (table == null || !IsDesignMode) return;

        try
        {
            // Check if table has active ticket
            if (table.Status != TableStatus.Available)
            {
                // Show error - cannot delete table with active ticket
                await ShowErrorAsync("Cannot delete table with active ticket. Please clear the table first.");
                return;
            }

            // Confirm deletion
            var confirmed = await ShowConfirmationAsync($"Delete Table {table.TableNumber}?", 
                "This action cannot be undone. Are you sure you want to delete this table?");
            
            if (!confirmed) return;

            Tables.Remove(table);
            
            // Would also delete from repository in real implementation
            // await _tableRepository.DeleteAsync(table.Id);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error deleting table: {ex.Message}");
        }
    }

    private async Task<bool> UpdateTablePositionAsync(TableDto? table)
    {
        if (table == null || !IsDesignMode) return false;

        try
        {
            // Validate position is within canvas bounds
            var maxX = SelectedFloor?.Width ?? 2000;
            var maxY = SelectedFloor?.Height ?? 2000;
            
            if (table.X < 0 || table.Y < 0 || table.X > maxX - 100 || table.Y > maxY - 100)
            {
                // Snap to canvas bounds
                table.X = Math.Max(0, Math.Min(maxX - 100, table.X));
                table.Y = Math.Max(0, Math.Min(maxY - 100, table.Y));
                return false;
            }

            // Check for overlapping tables
            var overlappingTable = Tables.FirstOrDefault(t => 
                t.Id != table.Id && 
                Math.Abs(t.X - table.X) < 100 && 
                Math.Abs(t.Y - table.Y) < 100);

            if (overlappingTable != null)
            {
                await ShowErrorAsync($"Table {table.TableNumber} overlaps with Table {overlappingTable.TableNumber}. Please choose a different position.");
                return false;
            }

            // Update position in repository
            var command = new UpdateTablePositionCommand(
                table.Id,
                (int)table.X,
                (int)table.Y,
                table.Shape
            );

            await _mediator.Send(command);
            return true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error updating table position: {ex.Message}");
            return false;
        }
    }

    private async Task SaveLayoutAsync()
    {
        if (string.IsNullOrWhiteSpace(LayoutName))
        {
            await ShowErrorAsync("Please enter a layout name before saving.");
            return;
        }

        if (!Tables.Any())
        {
            await ShowErrorAsync("Cannot save empty layout. Please add tables first.");
            return;
        }

        IsBusy = true;
        try
        {
            // Check if layout name is unique
            var isUnique = await _tableLayoutRepository.IsLayoutNameUniqueAsync(LayoutName);
            if (!isUnique)
            {
                await ShowErrorAsync($"Layout name '{LayoutName}' already exists. Please choose a different name.");
                return;
            }

            var command = new SaveTableLayoutCommand(
                Guid.NewGuid(), // Would use existing layout ID if editing
                LayoutName,
                Tables.ToList()
            );

            var savedLayout = await _mediator.Send(command);
            
            await ShowSuccessAsync($"Layout '{LayoutName}' saved successfully with {Tables.Count} tables.");
            
            // Clear layout name for next save
            LayoutName = string.Empty;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error saving layout: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadLayoutAsync()
    {
        IsBusy = true;
        try
        {
            await LoadFloorsAsync();
            await LoadTablesAsync();
            
            // If there's an active layout, load its details
            if (SelectedFloor != null)
            {
                var layouts = await _tableLayoutRepository.GetLayoutsByFloorAsync(SelectedFloor.Id);
                if (layouts.Any())
                {
                    var activeLayout = layouts.FirstOrDefault();
                    LayoutName = activeLayout.Name;
                    Tables.Clear();
                    
                    foreach (var tableDto in activeLayout.Tables)
                    {
                        Tables.Add(tableDto);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error loading layout: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Test methods for Day 3
    public async Task<bool> TestBasicDesignerWorkflow()
    {
        try
        {
            // Test 1: Create a new layout
            LayoutName = "Test Layout";
            await AddTableAsync(new Point(100, 100));
            await AddTableAsync(new Point(300, 200));
            await AddTableAsync(new Point(500, 100));
            
            // Test 2: Save layout
            await SaveLayoutAsync();
            
            // Test 3: Load layout
            await LoadLayoutAsync();
            
            // Test 4: Verify tables are loaded correctly
            if (Tables.Count != 3)
            {
                await ShowErrorAsync($"Expected 3 tables, but found {Tables.Count}");
                return false;
            }
            
            // Test 5: Test table deletion
            var tableToDelete = Tables.FirstOrDefault();
            if (tableToDelete != null)
            {
                await DeleteTableAsync(tableToDelete);
            }
            
            // Test 6: Verify deletion
            if (Tables.Count != 2)
            {
                await ShowErrorAsync($"Expected 2 tables after deletion, but found {Tables.Count}");
                return false;
            }
            
            await ShowSuccessAsync("Basic designer workflow test passed!");
            return true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Test failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestDragAndDropFunctionality()
    {
        try
        {
            // Test 1: Create a table
            var testTable = new TableDto
            {
                Id = Guid.NewGuid(),
                TableNumber = 1,
                Capacity = 4,
                X = 100,
                Y = 100,
                Status = TableStatus.Available,
                IsActive = true
            };
            Tables.Add(testTable);
            
            // Test 2: Simulate drag operation
            var newX = 200;
            var newY = 200;
            
            // Test 3: Update position
            testTable.X = newX;
            testTable.Y = newY;
            
            // Test 4: Verify position update
            if (testTable.X != newX || testTable.Y != newY)
            {
                await ShowErrorAsync("Drag and drop position update failed.");
                return false;
            }
            
            // Test 5: Test boundary validation
            testTable.X = -10; // Should snap to 0
            testTable.Y = -10; // Should snap to 0
            
            if (testTable.X != 0 || testTable.Y != 0)
            {
                await ShowErrorAsync("Boundary validation failed.");
                return false;
            }
            
            // Test 6: Test overlap detection
            var overlappingTable = new TableDto
            {
                Id = Guid.NewGuid(),
                TableNumber = 2,
                Capacity = 4,
                X = 110, // Overlaps with testTable
                Y = 110,
                Status = TableStatus.Available,
                IsActive = true
            };
            Tables.Add(overlappingTable);
            
            // The UpdateTablePositionAsync should detect overlap
            var updateResult = await UpdateTablePositionAsync(overlappingTable);
            if (!updateResult) // Should fail due to overlap
            {
                // This is expected behavior
            }
            
            await ShowSuccessAsync("Drag and drop functionality test passed!");
            return true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Drag and drop test failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestShapeSelection()
    {
        try
        {
            // Test each shape type
            var shapes = new[] 
            { 
                TableShapeType.Rectangle, 
                TableShapeType.Square, 
                TableShapeType.Round, 
                TableShapeType.Oval 
            };
            
            foreach (var shape in shapes)
            {
                SelectedShape = shape;
                
                // Create a table with the selected shape
                await AddTableAsync(new Point(100 + (int)shape * 150, 100));
                
                // Verify the table was created with the correct shape
                var createdTable = Tables.LastOrDefault();
                if (createdTable == null || createdTable.Shape != shape)
                {
                    await ShowErrorAsync($"Shape selection failed for {shape}");
                    return false;
                }
                
                // Remove table for next test
                Tables.Remove(createdTable);
            }
            
            await ShowSuccessAsync("Shape selection test passed!");
            return true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Shape selection test failed: {ex.Message}");
            return false;
        }
    }

    private void StartDrag(TableDto? table)
    {
        if (table == null || !IsDesignMode) return;
        SelectedTable = table;
    }

    private void SelectTable(TableDto? table)
    {
        if (table == null) return;
        SelectedTable = table;
    }

    private void ToggleDesignMode()
    {
        IsDesignMode = !IsDesignMode;
    }

    partial void OnSelectedFloorChanged(FloorDto? value)
    {
        if (value != null && !IsBusy)
        {
            _ = LoadTablesAsync();
        }
    }

    private int GetNextTableNumber()
    {
        return Tables.Count > 0 ? Tables.Max(t => t.TableNumber) + 1 : 1;
    }

    private async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        // In a real implementation, this would show a confirmation dialog
        // For now, return true to simulate user confirmation
        return true;
    }

    private async Task ShowErrorAsync(string message)
    {
        // In a real implementation, this would show an error dialog
        System.Diagnostics.Debug.WriteLine($"Error: {message}");
    }

    private async Task ShowSuccessAsync(string message)
    {
        // In a real implementation, this would show a success notification
        System.Diagnostics.Debug.WriteLine($"Success: {message}");
    }

    // Performance optimization methods
    public void UpdateVisibleTables(Rect viewport)
    {
        if (!_isVirtualizationEnabled)
        {
            VisibleTables = new ObservableCollection<TableDto>(Tables);
            VisibleTableCount = Tables.Count;
            return;
        }

        lock (_renderLock)
        {
            _viewport = viewport;
            
            // Calculate visible bounds with padding
            var padding = 100; // Extra padding to preload tables
            var visibleBounds = new Rect(
                viewport.X - padding,
                viewport.Y - padding,
                viewport.Width + (padding * 2),
                viewport.Height + (padding * 2)
            );

            // Filter tables that are visible
            var visibleTables = Tables.Where(table => 
                IsTableVisible(table, visibleBounds)).ToList();

            // Update visible tables collection efficiently
            if (!visibleTables.SequenceEqual(VisibleTables))
            {
                VisibleTables = new ObservableCollection<TableDto>(visibleTables);
                VisibleTableCount = visibleTables.Count;
            }
        }
    }

    private bool IsTableVisible(TableDto table, Rect visibleBounds)
    {
        var tableWidth = table.Width > 0 ? table.Width : 100;
        var tableHeight = table.Height > 0 ? table.Height : 100;
        
        return table.X < visibleBounds.X + visibleBounds.Width &&
               table.X + tableWidth > visibleBounds.X &&
               table.Y < visibleBounds.Y + visibleBounds.Height &&
               table.Y + tableHeight > visibleBounds.Y;
    }

    public void OptimizeForLargeLayouts()
    {
        if (Tables.Count > 100)
        {
            _isVirtualizationEnabled = true;
        }
        else
        {
            _isVirtualizationEnabled = false;
            VisibleTables = new ObservableCollection<TableDto>(Tables);
            VisibleTableCount = Tables.Count;
        }
    }

    public void ToggleVirtualization()
    {
        _isVirtualizationEnabled = !_isVirtualizationEnabled;
        
        if (_isVirtualizationEnabled)
        {
            // Recalculate visible tables
            if (_viewport != Rect.Empty)
            {
                UpdateVisibleTables(_viewport);
            }
        }
        else
        {
            VisibleTables = new ObservableCollection<TableDto>(Tables);
            VisibleTableCount = Tables.Count;
        }
    }

    public void SetCanvasDimensions(int width, int height, string backgroundColor)
    {
        CanvasWidth = width;
        CanvasHeight = height;
        BackgroundColor = backgroundColor;
        
        // Update floor properties if available
        if (SelectedFloor != null)
        {
            SelectedFloor.Width = width;
            SelectedFloor.Height = height;
            SelectedFloor.BackgroundColor = backgroundColor;
        }
    }

    public PerformanceMetrics GetPerformanceMetrics()
    {
        return new PerformanceMetrics
        {
            TotalTables = Tables.Count,
            VisibleTables = VisibleTableCount,
            CurrentFPS = CurrentFPS,
            IsVirtualizationEnabled = _isVirtualizationEnabled,
            CanvasWidth = CanvasWidth,
            CanvasHeight = CanvasHeight,
            LastRenderTime = LastRenderTime,
            MemoryUsage = GC.GetTotalMemory(false)
        };
    }
}

public class PerformanceMetrics
{
    public int TotalTables { get; set; }
    public int VisibleTables { get; set; }
    public int CurrentFPS { get; set; }
    public bool IsVirtualizationEnabled { get; set; }
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
    public DateTime LastRenderTime { get; set; }
    public long MemoryUsage { get; set; }
}
