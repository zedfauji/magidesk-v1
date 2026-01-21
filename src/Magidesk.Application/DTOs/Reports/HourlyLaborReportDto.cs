using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class HourlyLaborReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<HourlyLaborDto> Hours { get; set; } = new();
    public HourlyLaborTotalsDto Totals { get; set; } = new();
}

public class HourlyLaborDto
{
    public int Hour { get; set; }
    public List<EmployeeLaborDto> Employees { get; set; } = new();
    public decimal TotalLaborHours { get; set; }
    public decimal TotalLaborCost { get; set; }
    public decimal TotalSales { get; set; }
    public decimal LaborPercentage { get; set; }
    public bool IsHighLaborPercentage { get; set; }
}

public class EmployeeLaborDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal HoursWorked { get; set; }
    public decimal LaborCost { get; set; }
}

public class HourlyLaborTotalsDto
{
    public decimal TotalLaborHours { get; set; }
    public decimal TotalLaborCost { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AverageLaborPercentage { get; set; }
    public int TotalEmployees { get; set; }
}
