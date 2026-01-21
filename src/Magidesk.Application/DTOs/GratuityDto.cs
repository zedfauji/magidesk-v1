namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for Gratuity entity.
/// </summary>
public class GratuityDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public decimal Amount { get; set; }
    public bool Paid { get; set; }
    public bool Refunded { get; set; }
    public Guid TerminalId { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}

