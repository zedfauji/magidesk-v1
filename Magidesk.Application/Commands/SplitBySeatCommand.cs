using Magidesk.Application.DTOs;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands
{
    public class SplitBySeatCommand
    {
        public Guid OriginalTicketId { get; set; }
        public UserId ProcessedBy { get; set; } = null!;
        public Guid TerminalId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
    }

    public class SplitBySeatResult
    {
        public bool Success { get; set; }
        public Guid OriginalTicketId { get; set; }
        public List<Guid> NewTicketIds { get; set; } = new();
        public Dictionary<int, Guid> SeatToTicketMapping { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
