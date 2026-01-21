namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for Shift entity.
/// </summary>
public class ShiftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for shift report.
/// </summary>
public class ShiftReportDto
{
    public Guid ShiftId { get; set; }
    public string ShiftName { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int TicketCount { get; set; }
    public int ClosedTicketCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCashSales { get; set; }
    public decimal TotalCardSales { get; set; }
    public int CashSessionCount { get; set; }
    public decimal TotalCashReceipts { get; set; }
    public decimal TotalCashRefunds { get; set; }
    public List<TicketDto> Tickets { get; set; } = new();
    public List<CashSessionDto> CashSessions { get; set; } = new();
}

