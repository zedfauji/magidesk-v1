using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs.Reports;

public class AttendanceReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<AttendanceReportItemDto> Items { get; set; } = new();
    
    // Aggregates
    public double TotalHours { get; set; }
    public int TotalShifts { get; set; }
}

public class AttendanceReportItemDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ClockInTime { get; set; }
    public DateTime? ClockOutTime { get; set; }
    public double HoursWorked { get; set; } // Duration in hours
    public string ShiftName { get; set; } = string.Empty; // Optional, if using Shift entity
}
