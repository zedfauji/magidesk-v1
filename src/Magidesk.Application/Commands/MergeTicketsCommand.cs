using Magidesk.Application.Interfaces;
using System;

namespace Magidesk.Application.Commands;

public class MergeTicketsCommand
{
    public Guid SourceTicketId { get; set; }
    public Guid TargetTicketId { get; set; }
    public Guid ProcessedBy { get; set; }
}
