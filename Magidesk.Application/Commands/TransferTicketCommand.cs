using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

public class TransferTicketCommand
{
    public Guid TicketId { get; set; }
    public UserId NewOwnerId { get; set; } = null!;
    public UserId TransferredBy { get; set; } = null!; // Who initiated the transfer (Manager)
}
