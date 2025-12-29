using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents an attendance record for a user.
/// Tracks Clock In and Clock Out times.
/// </summary>
public class AttendanceHistory
{
    public Guid Id { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime ClockInTime { get; private set; }
    public DateTime? ClockOutTime { get; private set; }
    public Guid? ShiftId { get; private set; }
    
    // Private constructor for EF Core
    private AttendanceHistory() 
    { 
        UserId = null!;
    }

    private AttendanceHistory(UserId userId, DateTime clockInTime, Guid? shiftId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ClockInTime = clockInTime;
        ShiftId = shiftId;
    }

    public static AttendanceHistory Create(UserId userId, Guid? shiftId = null)
    {
        return new AttendanceHistory(userId, DateTime.UtcNow, shiftId);
    }

    public void ClockOut()
    {
        if (ClockOutTime.HasValue)
        {
            throw new Exceptions.InvalidOperationException("User is already clocked out.");
        }
        
        ClockOutTime = DateTime.UtcNow;
    }
}
