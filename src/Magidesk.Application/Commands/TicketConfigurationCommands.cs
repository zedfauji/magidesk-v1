using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

public class ChangeTicketOrderTypeCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderTypeId { get; set; }
    public UserId ChangedBy { get; set; } = null!;
}

public class SetTicketCustomerCommand
{
    public Guid TicketId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? ExtraDeliveryInfo { get; set; }
    public UserId ChangedBy { get; set; } = null!;
}
