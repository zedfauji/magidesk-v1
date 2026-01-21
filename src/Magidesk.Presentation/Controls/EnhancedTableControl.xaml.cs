using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace Magidesk.Presentation.Controls;

public sealed partial class EnhancedTableControl : UserControl
{
    // Dependency Property for Table
    public static readonly DependencyProperty TableProperty =
        DependencyProperty.Register(
            nameof(Table),
            typeof(TableDto),
            typeof(EnhancedTableControl),
            new PropertyMetadata(null, OnTableChanged));

    public TableDto Table
    {
        get => (TableDto)GetValue(TableProperty);
        set => SetValue(TableProperty, value);
    }

    // Events for table actions
    public event EventHandler<TableActionEventArgs>? TableClicked;
    public event EventHandler<TableActionEventArgs>? TableRightClicked;
    public event EventHandler<TableActionEventArgs>? StartSessionRequested;
    public event EventHandler<TableActionEventArgs>? EndSessionRequested;
    public event EventHandler<TableActionEventArgs>? PauseSessionRequested;
    public event EventHandler<TableActionEventArgs>? ResumeSessionRequested;
    public event EventHandler<TableActionEventArgs>? ViewDetailsRequested;
    public event EventHandler<ServerAssignmentEventArgs>? ServerAssigned;

    private bool _isHovering = false;

    public EnhancedTableControl()
    {
        this.InitializeComponent();
    }

    private static void OnTableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EnhancedTableControl control)
        {
            control.UpdateContextMenu();
            control.UpdateTooltip();
            control.Bindings.Update();
        }
    }

    #region Pointer Events

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _isHovering = true;
        
        // Add hover effect
        if (TableBorder != null)
        {
            TableBorder.Translation = new System.Numerics.Vector3(0, -2, 8);
        }
        
        // Show tooltip
        UpdateTooltip();
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _isHovering = false;
        
        // Remove hover effect
        if (TableBorder != null)
        {
            TableBorder.Translation = new System.Numerics.Vector3(0, 0, 0);
        }
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var pointer = e.GetCurrentPoint(this);
        
        if (pointer.Properties.IsLeftButtonPressed)
        {
            TableClicked?.Invoke(this, new TableActionEventArgs { Table = Table });
        }
    }

    private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        TableRightClicked?.Invoke(this, new TableActionEventArgs { Table = Table });
        ShowContextMenu();
    }

    #endregion

    #region Drag and Drop

    private void OnDragOver(object sender, DragEventArgs e)
    {
        // Check if the dragged data contains server information
        if (e.DataView.Contains("ServerId"))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Assign Server";
            
            // Show drag indicator
            if (DragIndicator != null)
            {
                DragIndicator.Visibility = Visibility.Visible;
            }
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;
        }
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        // Hide drag indicator
        if (DragIndicator != null)
        {
            DragIndicator.Visibility = Visibility.Collapsed;
        }

        if (e.DataView.Contains("ServerId"))
        {
            var serverIdText = await e.DataView.GetTextAsync("ServerId");
            if (Guid.TryParse(serverIdText, out var serverId))
            {
                var serverName = await e.DataView.GetTextAsync("ServerName");
                
                ServerAssigned?.Invoke(this, new ServerAssignmentEventArgs
                {
                    Table = Table,
                    ServerId = serverId,
                    ServerName = serverName
                });
            }
        }
    }

    #endregion

    #region Context Menu

    private void ShowContextMenu()
    {
        UpdateContextMenu();
        
        var flyout = FlyoutBase.GetAttachedFlyout(this);
        if (flyout != null)
        {
            flyout.ShowAt(this);
        }
    }

    private void UpdateContextMenu()
    {
        if (ContextMenu == null || Table == null) return;

        ContextMenu.Items.Clear();

        // Get the TableMapViewModel from the page's DataContext
        var page = FindParentPage();
        if (page?.DataContext is ViewModels.TableMapViewModel viewModel)
        {
            var menuItems = viewModel.GetContextMenuItems(Table);
            foreach (var item in menuItems)
            {
                ContextMenu.Items.Add(item);
            }
        }
        else
        {
            // Fallback to local menu generation if ViewModel not found
            GenerateLocalContextMenu();
        }
    }

    private void GenerateLocalContextMenu()
    {
        if (Table == null || ContextMenu == null) return;

        // Available table actions
        if (Table.Status == TableStatus.Available)
        {
            var startSessionItem = new MenuFlyoutItem
            {
                Text = "Start Session",
                Icon = new SymbolIcon(Symbol.Play)
            };
            startSessionItem.Click += (s, e) => StartSessionRequested?.Invoke(this, new TableActionEventArgs { Table = Table });
            ContextMenu.Items.Add(startSessionItem);
        }

        // Occupied table actions
        if (Table.Status == TableStatus.Seat && Table.SessionId.HasValue)
        {
            var viewDetailsItem = new MenuFlyoutItem
            {
                Text = "View Details",
                Icon = new SymbolIcon(Symbol.View)
            };
            viewDetailsItem.Click += (s, e) => ViewDetailsRequested?.Invoke(this, new TableActionEventArgs { Table = Table });
            ContextMenu.Items.Add(viewDetailsItem);

            ContextMenu.Items.Add(new MenuFlyoutSeparator());

            // Pause/Resume based on session status
            if (Table.SessionStatus == TableSessionStatus.Active)
            {
                var pauseItem = new MenuFlyoutItem
                {
                    Text = "Pause Session",
                    Icon = new SymbolIcon(Symbol.Pause)
                };
                pauseItem.Click += (s, e) => PauseSessionRequested?.Invoke(this, new TableActionEventArgs { Table = Table });
                ContextMenu.Items.Add(pauseItem);
            }
            else if (Table.SessionStatus == TableSessionStatus.Paused)
            {
                var resumeItem = new MenuFlyoutItem
                {
                    Text = "Resume Session",
                    Icon = new SymbolIcon(Symbol.Play)
                };
                resumeItem.Click += (s, e) => ResumeSessionRequested?.Invoke(this, new TableActionEventArgs { Table = Table });
                ContextMenu.Items.Add(resumeItem);
            }

            ContextMenu.Items.Add(new MenuFlyoutSeparator());

            var endSessionItem = new MenuFlyoutItem
            {
                Text = "End Session",
                Icon = new SymbolIcon(Symbol.Stop)
            };
            endSessionItem.Click += (s, e) => EndSessionRequested?.Invoke(this, new TableActionEventArgs { Table = Table });
            ContextMenu.Items.Add(endSessionItem);
        }
    }

    private Page? FindParentPage()
    {
        DependencyObject? parent = this;
        while (parent != null)
        {
            parent = VisualTreeHelper.GetParent(parent);
            if (parent is Page page)
            {
                return page;
            }
        }
        return null;
    }

    #endregion

    #region Tooltip

    private void UpdateTooltip()
    {
        if (Table == null) return;

        var tooltipText = $"Table {Table.TableNumber}\n";
        tooltipText += $"Capacity: {Table.Capacity} guests\n";
        tooltipText += $"Status: {GetStatusText(Table.Status)}";

        if (Table.SessionId.HasValue)
        {
            tooltipText += $"\n\nSession Details:";
            tooltipText += $"\nElapsed Time: {Table.SessionElapsedTimeDisplay ?? "N/A"}";
            tooltipText += $"\nCurrent Charge: {Table.SessionRunningChargeDisplay ?? "N/A"}";
            tooltipText += $"\nStatus: {GetSessionStatusText(Table.SessionStatus)}";
            
            if (Table.SessionHourlyRate.HasValue)
            {
                tooltipText += $"\nHourly Rate: ${Table.SessionHourlyRate.Value:F2}";
            }
        }

        ToolTipService.SetToolTip(this, tooltipText);
    }

    #endregion

    #region Helper Methods

    private CornerRadius GetCornerRadius(TableShapeType shape)
    {
        return shape switch
        {
            TableShapeType.Round => new CornerRadius(75), // Fully rounded
            TableShapeType.Square => new CornerRadius(4),
            TableShapeType.Rectangle => new CornerRadius(4),
            _ => new CornerRadius(4)
        };
    }

    private Brush GetStatusBorderBrush(TableDto? table)
    {
        if (table == null) return new SolidColorBrush(Colors.Gray);

        return table.Status switch
        {
            TableStatus.Available => new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)), // Green
            TableStatus.Seat => new SolidColorBrush(Color.FromArgb(255, 196, 43, 28)), // Red
            TableStatus.Booked => new SolidColorBrush(Color.FromArgb(255, 202, 160, 0)), // Yellow
            TableStatus.Dirty => new SolidColorBrush(Colors.Gray),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    private Brush GetStatusBackgroundBrush(TableDto? table)
    {
        if (table == null) return new SolidColorBrush(Colors.LightGray);

        // Use lighter versions of the border colors for background
        return table.Status switch
        {
            TableStatus.Available => new SolidColorBrush(Color.FromArgb(255, 200, 255, 200)), // Light green
            TableStatus.Seat => new SolidColorBrush(Color.FromArgb(255, 255, 200, 200)), // Light red
            TableStatus.Booked => new SolidColorBrush(Color.FromArgb(255, 255, 255, 200)), // Light yellow
            TableStatus.Dirty => new SolidColorBrush(Color.FromArgb(255, 220, 220, 220)), // Light gray
            _ => new SolidColorBrush(Colors.LightGray)
        };
    }

    private string GetCapacityText(int capacity)
    {
        return $"Seats {capacity}";
    }

    private Visibility GetSessionTimerVisibility(TableDto? table)
    {
        if (table == null) return Visibility.Collapsed;
        
        return table.SessionId.HasValue && table.SessionStatus != TableSessionStatus.Ended
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private bool IsSessionPaused(TableDto? table)
    {
        if (table == null) return false;
        return table.SessionStatus == TableSessionStatus.Paused;
    }

    private string GetStatusIcon(TableDto? table)
    {
        if (table == null) return "";

        if (table.SessionId.HasValue)
        {
            return table.SessionStatus switch
            {
                TableSessionStatus.Active => "\uE768", // Play icon
                TableSessionStatus.Paused => "\uE769", // Pause icon
                _ => ""
            };
        }

        return table.Status switch
        {
            TableStatus.Booked => "\uE787", // Calendar icon
            _ => ""
        };
    }

    private Visibility GetStatusIconVisibility(TableDto? table)
    {
        if (table == null) return Visibility.Collapsed;
        
        var icon = GetStatusIcon(table);
        return string.IsNullOrEmpty(icon) ? Visibility.Collapsed : Visibility.Visible;
    }

    private string GetStatusText(TableStatus status)
    {
        return status switch
        {
            TableStatus.Available => "Available",
            TableStatus.Seat => "Occupied",
            TableStatus.Booked => "Reserved",
            TableStatus.Dirty => "Needs Cleaning",
            _ => "Unknown"
        };
    }

    private string GetSessionStatusText(TableSessionStatus? status)
    {
        if (!status.HasValue) return "N/A";

        return status.Value switch
        {
            TableSessionStatus.Active => "Active",
            TableSessionStatus.Paused => "Paused",
            TableSessionStatus.Ended => "Ended",
            _ => "Unknown"
        };
    }

    #endregion
}

// Event argument classes
public class TableActionEventArgs : EventArgs
{
    public TableDto? Table { get; set; }
}

public class ServerAssignmentEventArgs : EventArgs
{
    public TableDto? Table { get; set; }
    public Guid ServerId { get; set; }
    public string? ServerName { get; set; }
}
