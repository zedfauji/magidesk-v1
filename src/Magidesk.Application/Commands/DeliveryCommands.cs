using System;

namespace Magidesk.Application.Commands;

public class MarkTicketAsReadyCommand
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; } // Who marked it ready
}

public class DispatchTicketCommand
{
    public Guid TicketId { get; set; }
    public Guid? DriverId { get; set; }
    public Guid UserId { get; set; } // Who dispatched it
}

public class ScheduleTicketCommand
{
    public Guid TicketId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public Guid UserId { get; set; }
}

public class FireScheduledTicketsCommand
{
    // System command, usually doesn't need params, maybe a dry-run flag
    public DateTime DueBy { get; set; } // Process tickets due by this time
}
