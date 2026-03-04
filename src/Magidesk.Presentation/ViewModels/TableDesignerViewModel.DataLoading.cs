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

using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for data loading operations.
/// Handles loading floors and tables from repository.
/// </summary>
public partial class TableDesignerViewModel
{
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
        IsBusy = true;
        try
        {
            Floors.Clear();
            var floors = await _floorRepository.GetAllAsync();

            if (!floors.Any())
            {
                // Create default floor if none exist
                var defaultFloor = Magidesk.Domain.Entities.Floor.Create(
                    "Main Floor",
                    "Primary dining area",
                    2000,
                    2000
                );
                
                await _floorRepository.AddAsync(defaultFloor);
                floors = new[] { defaultFloor };
            }

            foreach (var floor in floors)
            {
                Floors.Add(new FloorDto
                {
                    Id = floor.Id,
                    Name = floor.Name,
                    Description = floor.Description,
                    Width = floor.Width,
                    Height = floor.Height,
                    IsActive = floor.IsActive,
                    CreatedAt = floor.CreatedAt,
                    UpdatedAt = floor.UpdatedAt,
                    BackgroundColor = floor.BackgroundColor
                });
            }

            SelectedFloor = Floors.FirstOrDefault();
            _currentLayoutId = null; // Reset current layout when reloading floors
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error loading floors: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadTablesAsync()
    {
        if (SelectedFloor == null) return;

        // Phase 1 Core Integrity: Load ALL active tables, not just available ones.
        // This ensures the designer reflects the true state of the floor.
        var activeTables = await _tableRepository.GetActiveAsync();
        Tables.Clear();

        foreach (var table in activeTables)
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
        IsDirty = false;
    }

}
