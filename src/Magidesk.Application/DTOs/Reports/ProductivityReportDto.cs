namespace Magidesk.Application.DTOs.Reports;

public class ProductivityReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ServerProductivityDto> ServerStats { get; set; } = new();
}
