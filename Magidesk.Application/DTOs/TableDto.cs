using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for Table entity.
/// </summary>
public class TableDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public Guid? FloorId { get; set; }
    public Guid? LayoutId { get; set; }
    public int Capacity { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public TableShapeType Shape { get; set; }
    public TableStatus Status { get; set; }
    public Guid? CurrentTicketId { get; set; }
    public bool IsActive { get; set; }
    public bool IsSelected { get; set; }
    public bool IsLocked { get; set; }

    // Session data (for timer display)
    public Guid? SessionId { get; set; }
    public DateTime? SessionStartTime { get; set; }
    public TableSessionStatus? SessionStatus { get; set; }
    public decimal? SessionHourlyRate { get; set; }
    public TimeSpan? SessionPausedDuration { get; set; }

    // Calculated properties for UI binding
    public TimeSpan? SessionElapsedTime
    {
        get
        {
            if (!SessionStartTime.HasValue || SessionStatus == TableSessionStatus.Ended)
                return null;

            var elapsed = DateTime.UtcNow - SessionStartTime.Value;
            var pausedDuration = SessionPausedDuration ?? TimeSpan.Zero;

            // If currently paused, don't add current pause time (it's already in TotalPausedDuration)
            return elapsed - pausedDuration;
        }
    }

    public decimal? SessionRunningCharge
    {
        get
        {
            if (!SessionElapsedTime.HasValue || !SessionHourlyRate.HasValue)
                return null;

            return (decimal)SessionElapsedTime.Value.TotalHours * SessionHourlyRate.Value;
        }
    }

    public string? SessionElapsedTimeDisplay
    {
        get
        {
            if (!SessionElapsedTime.HasValue)
                return null;

            var time = SessionElapsedTime.Value;
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }

    public string? SessionRunningChargeDisplay
    {
        get
        {
            if (!SessionRunningCharge.HasValue)
                return null;

            return $"${SessionRunningCharge.Value:F2}";
        }
    }
}

