using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class PurchaseOrder
{
    public Guid Id { get; private set; }
    public string PONumber { get; private set; } = null!;
    public Guid VendorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? OrderedAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<PurchaseOrderLine> _lines = new();
    public IReadOnlyCollection<PurchaseOrderLine> Lines => _lines.AsReadOnly();

    public virtual Vendor Vendor { get; private set; } = null!;

    private PurchaseOrder() { }

    public static PurchaseOrder Create(string poNumber, Guid vendorId, string? notes = null)
    {
        return new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            PONumber = poNumber,
            VendorId = vendorId,
            CreatedAt = DateTime.UtcNow,
            Status = PurchaseOrderStatus.Draft,
            Notes = notes
        };
    }

    public void AddLine(Guid inventoryItemId, decimal quantity, decimal unitCost)
    {
        if (Status != PurchaseOrderStatus.Draft) throw new InvalidOperationException("Can only add lines to draft orders");
        
        var existing = _lines.FirstOrDefault(l => l.InventoryItemId == inventoryItemId);
        if (existing != null)
        {
            _lines.Remove(existing);
        }

        _lines.Add(new PurchaseOrderLine(Id, inventoryItemId, quantity, unitCost));
        CalculateTotal();
    }

    public void MarkAsOrdered()
    {
        if (Status != PurchaseOrderStatus.Draft) throw new InvalidOperationException("Can only order draft POs");
        Status = PurchaseOrderStatus.Ordered;
        OrderedAt = DateTime.UtcNow;
    }

    public void MarkAsReceived()
    {
        if (Status != PurchaseOrderStatus.Ordered) throw new InvalidOperationException("Can only receive ordered POs");
        Status = PurchaseOrderStatus.Received;
        ReceivedAt = DateTime.UtcNow;

        foreach (var line in _lines)
        {
            line.MarkAsReceived();
        }
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Received) throw new InvalidOperationException("Cannot cancel received orders");
        Status = PurchaseOrderStatus.Cancelled;
    }

    private void CalculateTotal()
    {
        TotalAmount = _lines.Sum(l => l.Subtotal);
    }
}

public class PurchaseOrderLine
{
    public Guid Id { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public decimal QuantityExpected { get; private set; }
    public decimal QuantityReceived { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal Subtotal => QuantityExpected * UnitCost;
    public bool IsReceived { get; private set; }

    public virtual InventoryItem InventoryItem { get; private set; } = null!;

    private PurchaseOrderLine() { }

    internal PurchaseOrderLine(Guid poId, Guid itemId, decimal quantity, decimal cost)
    {
        Id = Guid.NewGuid();
        PurchaseOrderId = poId;
        InventoryItemId = itemId;
        QuantityExpected = quantity;
        UnitCost = cost;
    }

    internal void MarkAsReceived()
    {
        QuantityReceived = QuantityExpected;
        IsReceived = true;
    }
}
