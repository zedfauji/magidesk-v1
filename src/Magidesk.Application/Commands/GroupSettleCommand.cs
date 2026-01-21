using Magidesk.Application.DTOs;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Commands
{
    public class GroupSettleCommand
    {
        public List<Guid> TicketIds { get; set; } = new();
        public Money Amount { get; set; } = new(0m, "USD");
        public Money TenderAmount { get; set; } = new(0m, "USD");
        public PaymentType PaymentType { get; set; }
        public UserId ProcessedBy { get; set; } = null!;
        public Guid TerminalId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        
        // Optional: For card payments
        public string? AuthCode { get; set; }
        public string? CardType { get; set; }
        public string? AuthMethod { get; set; }
    }
}
