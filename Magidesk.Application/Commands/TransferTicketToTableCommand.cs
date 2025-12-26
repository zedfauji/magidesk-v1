using System;

namespace Magidesk.Application.Commands;

public class TransferTicketToTableCommand
{
    public Guid TicketId { get; set; }
    public Guid SourceTableId { get; set; }
    public Guid TargetTableId { get; set; }
    public Guid UserId { get; set; }
}

public class TransferTicketToTableResult
{
    public Guid TicketId { get; set; }
    public Guid SourceTableId { get; set; }
    public Guid TargetTableId { get; set; }
    public bool Success { get; set; }
}
