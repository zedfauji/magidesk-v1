using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs.Reports;

public class ServerPerformanceReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ServerPerformanceItemDto> Items { get; set; } = new();
}

public class ServerPerformanceItemDto
{
    public Guid UserId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalTips { get; set; }
    public decimal HoursWorked { get; set; }
}
