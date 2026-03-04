using System.Collections.ObjectModel;
using System.Threading;
using Magidesk.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Magidesk.Application.Commands.TableSessions;
using Microsoft.UI.Xaml.Controls;

using System.Collections.ObjectModel;
using System.Threading;
using Magidesk.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Magidesk.Application.Commands.TableSessions;
using Microsoft.UI.Xaml.Controls;

using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for data refresh and table status polling operations.
/// Handles loading, refreshing, and real-time monitoring of table data.
/// </summary>
public partial class TableMapViewModel
{
    private async Task LoadTablesAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _getTableMap.HandleAsync(new GetTableMapQuery());
            Tables.Clear();
            
            double maxX = 2000;
            double maxY = 2000;

            foreach (var table in result.Tables)
            {
                Tables.Add(table);
                
                // Track max extent (+ padding) to resize canvas dynamically
                double tableRight = table.X + (table.Width > 0 ? table.Width : 150);
                double tableBottom = table.Y + (table.Height > 0 ? table.Height : 150);
                
                if (tableRight > maxX) maxX = tableRight;
                if (tableBottom > maxY) maxY = tableBottom;
            }
            
            // Add margin
            CanvasWidth = maxX + 200;
            CanvasHeight = maxY + 200;
        }
        finally
        {
            IsBusy = false;
        }
    }


    private void StartRealTimePolling()
    {
        if (IsRealTimeEnabled) // IsBusy check removed as we use separate scope
        {
            // Initial delay to avoid collision with page load
            _refreshTimer = new Timer(async _ => await RefreshTableStatusAsync(), 
                                     null, TimeSpan.FromMilliseconds(RefreshInterval), TimeSpan.FromMilliseconds(RefreshInterval));
        }
    }

    private void StartUIRefreshTimer()
    {
        // Create a DispatcherTimer for UI updates (1 second interval)
        _uiRefreshTimer = new Microsoft.UI.Xaml.DispatcherTimer();
        _uiRefreshTimer.Interval = TimeSpan.FromSeconds(1);
        _uiRefreshTimer.Tick += (s, e) =>
        {
            // Force UI update for calculated properties (SessionElapsedTime, SessionRunningCharge)
            // This triggers property change notifications for all tables with active sessions
            var tablesWithSessions = Tables.Where(t => t.SessionId.HasValue && t.SessionStatus == TableSessionStatus.Active).ToList();
            
            if (tablesWithSessions.Any())
            {
                // Create a new collection to trigger UI updates for calculated properties
                // This is necessary because TableDto doesn't implement INotifyPropertyChanged
                var updatedTables = new List<TableDto>();
                
                foreach (var table in Tables)
                {
                    if (table.SessionId.HasValue && table.SessionStatus == TableSessionStatus.Active)
                    {
                        // Create a copy with updated calculated values to trigger UI refresh
                        var updatedTable = new TableDto
                        {
                            Id = table.Id,
                            TableNumber = table.TableNumber,
                            Status = table.Status,
                            X = table.X,
                            Y = table.Y,
                            Width = table.Width,
                            Height = table.Height,
                            Shape = table.Shape,
                            CurrentTicketId = table.CurrentTicketId,
                            SessionId = table.SessionId,
                            SessionStartTime = table.SessionStartTime,
                            SessionStatus = table.SessionStatus,
                            SessionHourlyRate = table.SessionHourlyRate,
                            SessionPausedDuration = table.SessionPausedDuration,
                            FloorId = table.FloorId,
                            LayoutId = table.LayoutId,
                            Capacity = table.Capacity,
                            IsActive = table.IsActive,
                            IsSelected = table.IsSelected,
                            IsLocked = table.IsLocked
                        };
                        updatedTables.Add(updatedTable);
                    }
                }
                
                // Update the tables in the collection to trigger UI refresh
                foreach (var updatedTable in updatedTables)
                {
                    var index = Tables.ToList().FindIndex(t => t.Id == updatedTable.Id);
                    if (index >= 0)
                    {
                        Tables[index] = updatedTable;
                    }
                }
            }
        };
        _uiRefreshTimer.Start();
    }


    private void StopUIRefreshTimer()
    {
        _uiRefreshTimer?.Stop();
        _uiRefreshTimer = null;
    }


    private void StopRealTimePolling()
    {
        _refreshTimer?.Dispose();
    }


    private async Task RefreshTableStatusAsync()
    {
        if (!IsRealTimeEnabled) return;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTableMap = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTableMapQuery, GetTableMapResult>>();
                var result = await getTableMap.HandleAsync(new GetTableMapQuery());
            
                // Marshall back to UI thread if needed, or update ObservableCollection carefully.
                // Since this is updating the ObservableCollection properties (Status, CurrentTicketId),
                // we should do this on the UI thread to avoid "The application called an interface that was marshalled for a different thread."
                
                _dispatcherQueue.TryEnqueue(() => 
                {
                    // Update only changed tables for performance
                    foreach (var updatedTable in result.Tables)
                    {
                        var existingTable = Tables.FirstOrDefault(t => t.Id == updatedTable.Id);
                        if (existingTable != null)
                        {
                            // Update table status
                            if (existingTable.Status != updatedTable.Status)
                            {
                                existingTable.Status = updatedTable.Status;
                            }
                            
                            if (existingTable.CurrentTicketId != updatedTable.CurrentTicketId)
                            {
                                existingTable.CurrentTicketId = updatedTable.CurrentTicketId;
                            }
                            
                            // Update session data (for timers and icons)
                            existingTable.SessionId = updatedTable.SessionId;
                            existingTable.SessionStartTime = updatedTable.SessionStartTime;
                            existingTable.SessionStatus = updatedTable.SessionStatus;
                            existingTable.SessionHourlyRate = updatedTable.SessionHourlyRate;
                            existingTable.SessionPausedDuration = updatedTable.SessionPausedDuration;
                        }
                    }
                });
            }
            
            _dispatcherQueue.TryEnqueue(() => LastRefresh = DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            // Log error but don't crash the polling
            System.Diagnostics.Debug.WriteLine($"Error refreshing table status: {ex.Message}");
        }
    }


    private async Task RefreshTablesAsync()
    {
        await LoadTablesAsync();
        LastRefresh = DateTime.UtcNow;
    }


    private async Task ToggleRealTimeAsync()
    {
        IsRealTimeEnabled = !IsRealTimeEnabled;
        
        if (IsRealTimeEnabled)
        {
            StartRealTimePolling();
        }
        else
        {
            StopRealTimePolling();
        }
    }


}
