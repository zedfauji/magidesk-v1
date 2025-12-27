namespace Magidesk.Application.DTOs.Reports;

public class ProductivityReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ServerProductivityDto> ServerStats { get; set; } = new();
}

public class ServerProductivityDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalTips { get; set; }
    public double TotalHours { get; set; }
    public decimal SalesPerHour { get; set; }
    public int TicketCount { get; set; }
}
