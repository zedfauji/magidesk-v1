namespace Magidesk.Application.DTOs.Reports;

public class TipReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TipReportServerSummaryDto> ServerSummaries { get; set; } = new();
    public List<TipReportDataDto> Details { get; set; } = new();
    
    public int CashTipsCount { get; set; }
    public decimal CashTipsAmount { get; set; }
    public int ChargedTipsCount { get; set; }
    public decimal ChargedTipsAmount { get; set; }
    public decimal TotalTips { get; set; }
    public decimal TotalAutoGratuity { get; set; }
    public decimal AverageTips { get; set; }
    public decimal PaidTips { get; set; }
    public decimal TipsDue { get; set; }
}

public class TipReportDataDto
{
    public string TicketId { get; set; } = string.Empty;
    public string SaleType { get; set; } = string.Empty;
    public decimal TicketTotal { get; set; }
    public decimal CashTips { get; set; }
    public decimal ChargedTips { get; set; }
    public decimal AutoGratuity { get; set; }
    public decimal Tips { get; set; } // Total Tips
    public decimal TipPercentage => TicketTotal > 0 ? (Tips / TicketTotal) * 100 : 0;
    public bool IsPaid { get; set; }
    public string ServerName { get; set; } = string.Empty;
}

public class TipReportServerSummaryDto
{
    public string ServerName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal CashTips { get; set; }
    public decimal ChargedTips { get; set; }
    public decimal AutoGratuity { get; set; }
    public decimal TotalTips { get; set; }
    public decimal TipPercentage => TotalSales > 0 ? (TotalTips / TotalSales) * 100 : 0;
}
