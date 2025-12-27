using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

public class ChangeTableCommand
{
    public Guid TicketId { get; set; }
    public Guid NewTableId { get; set; }
    public UserId UserId { get; set; }
}

public class ChangeTableResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
