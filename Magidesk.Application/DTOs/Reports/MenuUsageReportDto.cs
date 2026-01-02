using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class MenuUsageReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<MenuUsageItemDto> Items { get; set; } = new();
    public MenuUsageTotalsDto Totals { get; set; } = new();
}

public class MenuUsageItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal PercentageOfTotal { get; set; }
    public decimal AveragePrice { get; set; }
    public int TicketCount { get; set; }
    public bool IsTopPerformer => QuantitySold > 0 && PercentageOfTotal >= 5.0m; // Top 5% threshold
    public bool IsBottomPerformer => QuantitySold > 0 && PercentageOfTotal <= 0.5m; // Bottom 0.5% threshold
}

public class MenuUsageTotalsDto
{
    public int TotalItems { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int AverageQuantityPerItem { get; set; }
    public decimal AverageRevenuePerItem { get; set; }
}
