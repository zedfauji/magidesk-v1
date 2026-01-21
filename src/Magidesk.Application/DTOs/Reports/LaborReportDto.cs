using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs.Reports;

public class LaborReportDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<LaborCostItemDto> StaffLabor { get; set; } = new();
    public decimal TotalLaborCost { get; set; }
    public decimal TotalNetSales { get; set; } // To calculate Labor %
    public decimal LaborCostPercentage => TotalNetSales > 0 ? (TotalLaborCost / TotalNetSales) * 100 : 0;
    public double TotalHours { get; set; }
    public decimal SalesPerLaborHour => TotalHours > 0 ? TotalNetSales / (decimal)TotalHours : 0;
}

public class LaborCostItemDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public double TotalHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalCost { get; set; } // Hours * Rate
    public decimal TotalSales { get; set; } // Attributed Sales (Productivity)
    public decimal SalesPerLaborHour => TotalHours > 0 ? TotalSales / (decimal)TotalHours : 0;
}
