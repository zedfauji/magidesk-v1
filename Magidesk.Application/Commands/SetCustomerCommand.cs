using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

public class SetCustomerCommand
{
    public Guid TicketId { get; set; }
    public string? GuestName { get; set; }
    public string? PhoneNumber { get; set; }
    public UserId ModifiedBy { get; set; }
}

public class SetCustomerResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
