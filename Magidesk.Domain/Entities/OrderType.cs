using System;
using System.Collections.Generic;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents an order type (e.g., Dine In, Take Out, Delivery, Bar Tab).
/// Reference data entity that affects pricing and ticket behavior.
/// </summary>
public class OrderType
{
    private readonly Dictionary<string, string> _properties = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public bool CloseOnPaid { get; private set; }
    public bool AllowSeatBasedOrder { get; private set; }
    public bool AllowToAddTipsLater { get; private set; }
    public bool IsBarTab { get; private set; }
    public bool IsActive { get; private set; }
    public int Version { get; private set; }
    public IReadOnlyDictionary<string, string> Properties => _properties;

    // Private constructor for EF Core
    private OrderType()
    {
        Name = string.Empty;
    }

    /// <summary>
    /// Creates a new order type.
    /// </summary>
    public static OrderType Create(
        string name,
        bool closeOnPaid = false,
        bool allowSeatBasedOrder = false,
        bool allowToAddTipsLater = false,
        bool isBarTab = false,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Order type name cannot be empty.");
        }

        return new OrderType
        {
            Id = Guid.NewGuid(),
            Name = name,
            CloseOnPaid = closeOnPaid,
            AllowSeatBasedOrder = allowSeatBasedOrder,
            AllowToAddTipsLater = allowToAddTipsLater,
            IsBarTab = isBarTab,
            IsActive = isActive,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the order type name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Order type name cannot be empty.");
        }

        Name = name;
    }

    /// <summary>
    /// Updates the close on paid setting.
    /// </summary>
    public void SetCloseOnPaid(bool closeOnPaid)
    {
        CloseOnPaid = closeOnPaid;
    }

    /// <summary>
    /// Updates the allow seat based order setting.
    /// </summary>
    public void SetAllowSeatBasedOrder(bool allowSeatBasedOrder)
    {
        AllowSeatBasedOrder = allowSeatBasedOrder;
    }

    /// <summary>
    /// Updates the allow to add tips later setting.
    /// </summary>
    public void SetAllowToAddTipsLater(bool allowToAddTipsLater)
    {
        AllowToAddTipsLater = allowToAddTipsLater;
    }

    /// <summary>
    /// Updates the is bar tab setting.
    /// </summary>
    public void SetIsBarTab(bool isBarTab)
    {
        IsBarTab = isBarTab;
    }

    /// <summary>
    /// Activates the order type.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the order type.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Sets a property value.
    /// </summary>
    public void SetProperty(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new Exceptions.BusinessRuleViolationException("Property key cannot be empty.");
        }

        _properties[key] = value;
    }

    /// <summary>
    /// Removes a property.
    /// </summary>
    public void RemoveProperty(string key)
    {
        _properties.Remove(key);
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    public string? GetProperty(string key)
    {
        return _properties.TryGetValue(key, out var value) ? value : null;
    }
}

