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
using Windows.Foundation;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for table operation commands.
/// Handles adding, deleting, and positioning tables.
/// </summary>
public partial class TableDesignerViewModel
{
    private async Task AddTableAsync(Point position)
    {
        if (!IsDesignMode) return;

        try
        {
            // CRITICAL FIX: Ensure layout exists in database before adding tables
            if (_currentLayoutId == null || _currentLayoutId == Guid.Empty)
            {
                // Auto-save the layout first
                await SaveLayoutAsync();
                
                // If save failed or was cancelled, don't add table
                if (_currentLayoutId == null || _currentLayoutId == Guid.Empty)
                {
                    await ShowErrorAsync("Please save the layout before adding tables.");
                    return;
                }
            }

            var nextTableNumber = Tables.Count > 0 ? Tables.Max(t => t.TableNumber) + 1 : 1;

            var command = new AddTableToLayoutCommand(
                _currentLayoutId.Value,
                nextTableNumber,
                4, // Default capacity
                position.X,
                position.Y,
                SelectedShape
            );

            var newTable = await _mediator.Send(command);
            Tables.Add(newTable);
            IsDirty = true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error adding table: {ex.Message}");
        }
    }

    private async Task DeleteTableAsync(TableDto? table)
    {
        if (table == null || !IsDesignMode) return;

        try
        {
            // T-DR-005: Live Table Lock
            if (table.Status != TableStatus.Available)
            {
                await ShowErrorAsync($"Cannot delete Table {table.TableNumber} because it is currently {table.Status}. Please clear the table first.");
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

}
