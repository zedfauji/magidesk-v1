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

    public ObservableCollection<KitchenOrderItemViewModel> Items { get; } = new();

    public KitchenOrderViewModel(KitchenOrder order)
    {
        _order = order;
        foreach(var item in order.Items)
        {
            Items.Add(new KitchenOrderItemViewModel(item));
        }
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
