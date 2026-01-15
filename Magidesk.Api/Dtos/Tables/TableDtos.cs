namespace Magidesk.Api.Dtos.Tables;

public class TableSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TableStatus { get; set; } = "Available"; // Available, Occupied, Dirty, Disabled
    public string? SessionStatus { get; set; } // NotStarted, Running, Paused, Ended
    public double? ElapsedSeconds { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? CurrentUserId { get; set; }
    public string? ActiveTicketId { get; set; }
    public bool? IsReservationLocked { get; set; }
    public int Version { get; set; }
}

public class TableExtensionDto : TableSummaryDto
{
    public int Capacity { get; set; }
    public string ZoneName { get; set; } = string.Empty;
}

public class MoveTableRequest
{
    public string SourceTableId { get; set; } = string.Empty;
    public string TargetTableId { get; set; } = string.Empty;
}
