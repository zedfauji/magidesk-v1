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
using System.Collections.Generic;
using System.Linq;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for layout management operations.
/// Handles saving and loading table layouts.
/// </summary>
public partial class TableDesignerViewModel
{
    public async Task SaveLayoutAsync()
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

        // Validate all tables before saving
        if (!ValidateAllTables())
        {
            return; // Validation errors shown in ValidateAllTables
        }

        IsBusy = true;
        try
        {
            // Check if layout name is unique (exclude current layout if updating)
            var isUnique = await _tableLayoutRepository.IsLayoutNameUniqueAsync(LayoutName, _currentLayoutId);
            if (!isUnique)
            {
                await ShowErrorAsync($"Layout name '{LayoutName}' already exists. Please choose a different name.");
                return;
            }

            if (_currentLayoutId.HasValue)
            {
                // UPDATE existing layout
                var command = new SaveTableLayoutCommand(
                    _currentLayoutId.Value,
                    LayoutName,
                    Tables.ToList(),
                    IsDraftMode
                );
                await _mediator.Send(command);
                await ShowSuccessAsync($"Layout '{LayoutName}' updated successfully.");
            }
            else
            {
                // CREATE new layout
                var newLayoutId = Guid.NewGuid();
                var command = new CreateTableLayoutCommand(
                    LayoutName,
                    SelectedFloor?.Id ?? Guid.NewGuid(),
                    Tables.ToList(),
                    IsDraftMode
                );
                
                var createdLayout = await _mediator.Send(command);
                _currentLayoutId = createdLayout.Id;
                await ShowSuccessAsync($"Layout '{LayoutName}' created successfully.");
            }

            IsDirty = false;
            
            // Clear layout name for next save? No, keep it as we might keep editing.
            // LayoutName = string.Empty; 
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            // Handle database constraint violations with user-friendly messages
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            
            if (innerMessage.Contains("IX_Tables_TableNumber") || innerMessage.Contains("duplicate key"))
            {
                await ShowErrorAsync("Cannot save layout: Duplicate table numbers detected. Please ensure all table numbers are unique.");
            }
            else if (innerMessage.Contains("FK_") || innerMessage.Contains("foreign key"))
            {
                await ShowErrorAsync("Cannot save layout: Invalid floor or layout reference. Please try reloading the page.");
            }
            else
            {
                await ShowErrorAsync($"Database error while saving layout: {innerMessage}");
            }
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
                // Prefer IsActive = true, or just first one
                var activeLayout = layouts.FirstOrDefault(l => l.IsActive) ?? layouts.FirstOrDefault();
                
                if (activeLayout != null)
                {
                    LayoutName = activeLayout.Name;
                    _currentLayoutId = activeLayout.Id;
                    _isDraftMode = activeLayout.IsDraft;
                    OnPropertyChanged(nameof(IsDraftMode));

                    Tables.Clear();
                    
                    foreach (var tableDto in activeLayout.Tables)
                    {
                        Tables.Add(tableDto);
                    }
                }
                else
                {
                    // No layout found, reset ID
                    _currentLayoutId = null;
                    LayoutName = string.Empty;
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
}
