namespace Magidesk.Application.DTOs.Reports;

public class JournalReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<JournalEntryDto> Entries { get; set; } = new();
}

public class JournalEntryDto
{
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty; // UserId or Name if resolved
    public string BeforeState { get; set; } = string.Empty;
    public string AfterState { get; set; } = string.Empty;
}
