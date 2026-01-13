using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Comprehensive analytics data for a server.
/// </summary>
public record ServerAnalytics(
    Guid ServerId,
    string ServerName,
    DateTime FromDate,
    DateTime ToDate,
    ServerPerformanceMetrics PerformanceMetrics,
    IReadOnlyList<DailyServerMetrics> DailyBreakdown,
    CommissionCalculation CommissionData,
    ServerRanking Ranking
);

/// <summary>
/// Daily performance metrics for a server.
/// </summary>
public record DailyServerMetrics(
    DateTime Date,
    int SessionsServed,
    TimeSpan HoursWorked,
    Money SalesGenerated,
    Money TipsEarned,
    decimal AverageSessionValue
);

/// <summary>
/// Commission calculation data for a server.
/// </summary>
public record CommissionCalculation(
    Money BaseSalary,
    Money CommissionEarned,
    decimal CommissionRate,
    Money TotalCompensation,
    Money BonusEligible
);

/// <summary>
/// Server ranking compared to other servers.
/// </summary>
public record ServerRanking(
    int SalesRank,
    int TipsRank,
    int SessionCountRank,
    int CustomerSatisfactionRank,
    int OverallRank,
    int TotalServers
);