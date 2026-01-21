using Magidesk.Application.Interfaces;
using System;

namespace Magidesk.Application.Commands;

public class ChangeSeatCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public int? SeatNumber { get; set; }
}
