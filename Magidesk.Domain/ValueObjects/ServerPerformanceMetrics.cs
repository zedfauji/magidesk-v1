using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Performance metrics for a server over a specified period.
/// </summary>
public record ServerPerformanceMetrics(
    Guid ServerId,
    string ServerName,
    DateTime FromDate,
    DateTime ToDate,
    int TotalSessionsServed,
    TimeSpan TotalServiceTime,
    Money TotalSalesGenerated,
    Money TotalTipsEarned,
    decimal AverageSessionDuration,
    decimal CustomerSatisfactionScore,
    int PrimarySessionCount,
    int SecondarySessionCount,
    Money AverageTipPerSession,
    decimal SalesPerHour
)
{
    /// <summary>
    /// Calculates the average tip percentage based on sales and tips.
    /// </summary>
    public decimal AverageTipPercentage => 
        TotalSalesGenerated.Amount > 0 
            ? (TotalTipsEarned.Amount / TotalSalesGenerated.Amount) * 100 
            : 0;

    /// <summary>
    /// Calculates sessions per hour worked.
    /// </summary>
    public decimal SessionsPerHour => 
        TotalServiceTime.TotalHours > 0 
            ? (decimal)(TotalSessionsServed / TotalServiceTime.TotalHours) 
            : 0;
}