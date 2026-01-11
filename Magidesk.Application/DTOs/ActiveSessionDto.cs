using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for displaying active table sessions.
/// </summary>
public class ActiveSessionDto
{
    public Guid SessionId { get; set; }
    public Guid TableId { get; set; }
    public int TableNumber { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime StartTime { get; set; }
    public TableSessionStatus Status { get; set; }
    public decimal HourlyRate { get; set; }
    public TimeSpan PausedDuration { get; set; }

    // Calculated properties
    public TimeSpan ElapsedTime
    {
        get
        {
            var elapsed = DateTime.UtcNow - StartTime;
            return elapsed - PausedDuration;
        }
    }

    public string ElapsedTimeDisplay
    {
        get
        {
            var time = ElapsedTime;
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }

    public decimal RunningCharge
    {
        get
        {
            var hours = (decimal)ElapsedTime.TotalHours;
            return hours * HourlyRate;
        }
    }

    public string RunningChargeDisplay
    {
        get
        {
            return $"${RunningCharge:F2}";
        }
    }

    public bool IsPaused => Status == TableSessionStatus.Paused;
}
