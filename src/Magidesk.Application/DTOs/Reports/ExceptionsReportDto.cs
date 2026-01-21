using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class ExceptionsReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public List<VoidItemDto> Voids { get; set; } = new();
    public List<RefundItemDto> Refunds { get; set; } = new();
    public List<DiscountItemDto> Discounts { get; set; } = new();
}

public class VoidItemDto
{
    public DateTime Date { get; set; }
    public int TicketNumber { get; set; }
    public decimal Amount { get; set; }
    public string VoidedBy { get; set; } = string.Empty; // Staff Name or ID
    public string Reason { get; set; } = string.Empty;
}

public class RefundItemDto
{
    public DateTime Date { get; set; }
    public int TicketNumber { get; set; }
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class DiscountItemDto
{
    public DateTime Date { get; set; }
    public int TicketNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
