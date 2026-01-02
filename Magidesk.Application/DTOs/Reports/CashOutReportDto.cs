namespace Magidesk.Application.DTOs.Reports;

public class CashOutReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CashOutReportItemDto> Items { get; set; } = new();
    
    // Aggregates
    public decimal TotalCashSales { get; set; }
    public decimal TotalChargedTips { get; set; }
    public decimal TotalNetDue { get; set; } // Positive = Server Owes House, Negative = House Owes Server
}

public class CashOutReportItemDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal CashSales { get; set; } // Cash Payments collected by server
    public decimal ChargedTips { get; set; } // Tips included in card payments (owed to server)
    
    // Formula: Cash Collected - Tips Owed
    // Example: Collected $100 Cash. Owed $20 Card Tips. Net Due = $80. (Server Pays House $80)
    // Example: Collected $0 Cash. Owed $50 Card Tips. Net Due = -$50. (House Pays Server $50)
    public decimal NetDue => CashSales - ChargedTips;
}
