using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to add a modifier to an order line.
/// </summary>
public class AddOrderLineModifierCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public Guid ModifierId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public string? SectionName { get; set; }
    public Guid? ParentOrderLineModifierId { get; set; }
    public UserId? AddedBy { get; set; }
}

/// <summary>
/// Command to remove a modifier from an order line.
/// </summary>
public class RemoveOrderLineModifierCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public Guid OrderLineModifierId { get; set; }
    public UserId? RemovedBy { get; set; }
}

/// <summary>
/// Command to add a cooking instruction (text only) to an order line.
/// </summary>
public class AddOrderLineInstructionCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public UserId? AddedBy { get; set; }
}


