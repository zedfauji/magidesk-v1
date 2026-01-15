using System.ComponentModel;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for displaying active table sessions.
/// </summary>
public class ActiveSessionDto : INotifyPropertyChanged
{
    public Guid SessionId { get; set; }
    public Guid TableId { get; set; }
    public Guid? TicketId { get; set; } // Added for frontend navigation
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

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies UI that elapsed time and charge properties have changed.
    /// Called by the monitoring dashboard to trigger UI updates.
    /// </summary>
    public void NotifyElapsedTimeChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTime)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElapsedTimeDisplay)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RunningCharge)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RunningChargeDisplay)));
    }

    // Additional properties for enhanced monitoring
    public decimal CurrentCharge => RunningCharge;
    public string StatusDisplay => Status switch
    {
        TableSessionStatus.Active => "Active",
        TableSessionStatus.Paused => "Paused",
        TableSessionStatus.Ended => "Ended",
        _ => "Unknown"
    };

    public bool IsLongRunning => ElapsedTime > TimeSpan.FromHours(3);
    public bool IsHighValue => CurrentCharge > 50m;
}
