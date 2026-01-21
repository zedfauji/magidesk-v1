using System;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to update the instructions on an order line.
/// </summary>
public class UpdateOrderLineInstructionCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public string? Instruction { get; set; }

    public UpdateOrderLineInstructionCommand(Guid ticketId, Guid orderLineId, string? instruction)
    {
        TicketId = ticketId;
        OrderLineId = orderLineId;
        Instruction = instruction;
    }
}
