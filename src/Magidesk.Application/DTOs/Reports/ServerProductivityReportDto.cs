using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class ServerProductivityReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ServerProductivityDto> Servers { get; set; } = new();
    public ServerProductivityTotalsDto Totals { get; set; } = new();
}

public class ServerProductivityDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal NetSales { get; set; }
    public decimal TipsCollected { get; set; }
    public decimal AverageTicketSize { get; set; }
    public decimal AverageTipPercentage { get; set; }
    public decimal SalesPerHour { get; set; }
    public double TotalHours { get; set; }
}

public class ServerProductivityTotalsDto
{
    public int TotalServers { get; set; }
    public int TotalTickets { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalNetSales { get; set; }
    public decimal TotalTips { get; set; }
    public decimal AverageTicketSize { get; set; }
    public decimal AverageTipPercentage { get; set; }
    public decimal AverageSalesPerHour { get; set; }
}
