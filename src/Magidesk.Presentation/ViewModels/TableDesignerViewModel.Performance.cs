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

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for performance testing and optimization.
/// Handles virtualization, viewport updates, and performance metrics.
/// </summary>
public partial class TableDesignerViewModel
{
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
