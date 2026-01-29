using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

public class KitchenOrderViewModel : ViewModelBase
{
    private readonly KitchenOrder _order;
    
    public Guid Id => _order.Id;
    public string TableNumber => _order.TableNumber;
    public string ServerName => _order.ServerName;
    public KitchenStatus Status => _order.Status;
    
    public DateTime Timestamp => _order.Timestamp;

    public string TimeAgo
    {
        get
        {
            var span = DateTime.UtcNow - _order.Timestamp;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalHours < 1) return $"{span.Minutes}m ago";
            return $"{span.Hours}h {span.Minutes}m ago";
        }
    }
    
    public bool IsLate => (DateTime.UtcNow - _order.Timestamp).TotalMinutes > 20;
    
    public bool IsDoneStatus => _order.Status == KitchenStatus.Done;

    /// <summary>
    /// Gets the preparation time text for display.
    /// Shows elapsed time if order is in progress, or total time if delivered.
    /// </summary>
    public string PreparationTimeText
    {
        get
        {
            var prepTime = _order.PreparationTime;
            
            if (prepTime.HasValue)
            {
                // Order is delivered, show total prep time
                return FormatTimeSpan(prepTime.Value);
            }
            
            // Order is in progress, show elapsed time
            var elapsed = DateTime.UtcNow - _order.SentToKitchenAt;
            return FormatTimeSpan(elapsed);
        }
    }

    /// <summary>
    /// Gets the color for preparation time display based on elapsed time.
    /// Green (< 15m), Yellow (15-30m), Red (> 30m)
    /// </summary>
    public string PreparationTimeColor
    {
        get
        {
            var elapsed = _order.PreparationTime ?? (DateTime.UtcNow - _order.SentToKitchenAt);
            var minutes = elapsed.TotalMinutes;
            
            if (minutes < 15) return "#28A745"; // Green
            if (minutes < 30) return "#FFC107"; // Yellow
            return "#DC3545"; // Red
        }
    }

    public ObservableCollection<KitchenOrderItemViewModel> Items { get; } = new();

    public KitchenOrderViewModel(KitchenOrder order)
    {
        _order = order;
        foreach(var item in order.Items)
        {
            Items.Add(new KitchenOrderItemViewModel(item));
        }
    }

    /// <summary>
    /// Formats a TimeSpan for display.
    /// Examples: "5m", "23m", "1h 15m"
    /// </summary>
    private string FormatTimeSpan(TimeSpan span)
    {
        if (span.TotalHours >= 1)
        {
            return $"{(int)span.TotalHours}h {span.Minutes}m";
        }
        return $"{(int)span.TotalMinutes}m";
    }
}

public class KitchenOrderItemViewModel
{
    private readonly KitchenOrderItem _item;
    
    public string Quantity => _item.Quantity.ToString();
    public string Name => _item.ItemName; // Assuming ItemName property exists on KitchenOrderItem
    public string Modifiers { get; }

    public KitchenOrderItemViewModel(KitchenOrderItem item)
    {
        _item = item;
        // Modifiers is List<string>, join them
        Modifiers = _item.Modifiers != null && _item.Modifiers.Any() 
            ? string.Join(", ", _item.Modifiers) 
            : string.Empty;
    }
}
