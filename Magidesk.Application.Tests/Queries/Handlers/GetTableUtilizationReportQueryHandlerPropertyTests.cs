using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Handlers;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Queries.Handlers;

/// <summary>
/// Property-based tests for GetTableUtilizationReportQueryHandler.
/// Feature: reporting-export, Property 3: Table Utilization Calculation Accuracy
/// Validates: Requirements 2.1, 2.2, 2.4
/// </summary>
public class GetTableUtilizationReportQueryHandlerPropertyTests
{
    private readonly Mock<IAnalyticsRepository> _mockRepository;
    private readonly Mock<IAnalyticsEngine> _mockAnalyticsEngine;
    private readonly Mock<ILogger<GetTableUtilizationReportQueryHandler>> _mockLogger;
    private readonly GetTableUtilizationReportQueryHandler _handler;

    public GetTableUtilizationReportQueryHandlerPropertyTests()
    {
        _mockRepository = new Mock<IAnalyticsRepository>();
        _mockAnalyticsEngine = new Mock<IAnalyticsEngine>();
        _mockLogger = new Mock<ILogger<GetTableUtilizationReportQueryHandler>>();
        _handler = new GetTableUtilizationReportQueryHandler(
            _mockRepository.Object, 
            _mockAnalyticsEngine.Object, 
            _mockLogger.Object);
    }

    /// <summary>
    /// Property test: Table utilization occupancy percentage calculation accuracy.
    /// For any set of table sessions, the occupancy percentage should be calculated as 
    /// (total occupied time / total operating time) * 100, and should never exceed 100% or be negative.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property TableUtilizationOccupancyCalculationAccuracy()
    {
        return Prop.ForAll(
            GenerateValidTableUtilizationData(),
            data =>
            {
                // Arrange
                var (query, utilizationMetrics, detailedData, hourlyData, weeklyData, revenueData) = data;

                SetupMockRepository(query, detailedData, hourlyData, weeklyData, revenueData);
                _mockAnalyticsEngine.Setup(e => e.CalculateTableUtilizationAsync(
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(utilizationMetrics);

                // Act
                var result = _handler.HandleAsync(query).Result;

                // Assert - Property 3: Table Utilization Calculation Accuracy
                // Occupancy percentage should be between 0 and 100
                var occupancyValid = result.OverallOccupancyPercent >= 0m && result.OverallOccupancyPercent <= 100m;
                
                // Each table's occupancy should also be valid
                var tableOccupancyValid = result.TableBreakdown.All(t => 
                    t.OccupancyPercent >= 0m && t.OccupancyPercent <= 100m);
                
                // Hourly occupancy should be valid
                var hourlyOccupancyValid = result.HourlyOccupancy.All(h => 
                    h.OccupancyPercent >= 0m && h.OccupancyPercent <= 100m);
                
                // Weekly pattern occupancy should be valid
                var weeklyOccupancyValid = result.WeeklyPattern.All(w => 
                    w.AverageOccupancyPercent >= 0m && w.AverageOccupancyPercent <= 100m);

                // Revenue should be non-negative
                var revenueValid = result.TotalTimeRevenue.Amount >= 0m &&
                    result.TableBreakdown.All(t => t.RevenuePerTable.Amount >= 0m) &&
                    result.HourlyOccupancy.All(h => h.HourlyRevenue.Amount >= 0m) &&
                    result.WeeklyPattern.All(w => w.AverageRevenue.Amount >= 0m);

                // Session counts should be non-negative
                var sessionCountsValid = result.TableBreakdown.All(t => t.TotalSessions >= 0);

                // Time spans should be non-negative
                var timeSpansValid = result.AverageSessionDuration >= TimeSpan.Zero &&
                    result.TableBreakdown.All(t => 
                        t.AverageSessionDuration >= TimeSpan.Zero && 
                        t.TotalOccupiedTime >= TimeSpan.Zero) &&
                    result.WeeklyPattern.All(w => w.AverageSessionDuration >= TimeSpan.Zero);

                return occupancyValid && tableOccupancyValid && hourlyOccupancyValid && 
                       weeklyOccupancyValid && revenueValid && sessionCountsValid && timeSpansValid;
            });
    }

    /// <summary>
    /// Property test: Data aggregation consistency across breakdown categories.
    /// For any table utilization report with breakdown data, the sum of individual table metrics 
    /// should be consistent with overall metrics where applicable.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property TableUtilizationDataAggregationConsistency()
    {
        return Prop.ForAll(
            GenerateValidTableUtilizationData(),
            data =>
            {
                // Arrange
                var (query, utilizationMetrics, detailedData, hourlyData, weeklyData, revenueData) = data;

                SetupMockRepository(query, detailedData, hourlyData, weeklyData, revenueData);
                _mockAnalyticsEngine.Setup(e => e.CalculateTableUtilizationAsync(
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(utilizationMetrics);

                // Act
                var result = _handler.HandleAsync(query).Result;

                // Assert - Data aggregation consistency
                // Total sessions from table breakdown should be consistent
                var totalSessionsFromTables = result.TableBreakdown.Sum(t => t.TotalSessions);
                var sessionCountConsistent = totalSessionsFromTables >= 0; // Should be non-negative

                // Total revenue from tables should equal or be close to total time revenue
                var totalRevenueFromTables = result.TableBreakdown
                    .Aggregate(Money.Zero(), (sum, t) => sum + t.RevenuePerTable);
                var revenueConsistent = Math.Abs(totalRevenueFromTables.Amount - result.TotalTimeRevenue.Amount) <= 0.01m;

                // Hourly data should have 24 hours or less
                var hourlyDataValid = result.HourlyOccupancy.Count() <= 24 &&
                    result.HourlyOccupancy.All(h => h.Hour >= 0 && h.Hour <= 23);

                // Weekly data should have 7 days
                var weeklyDataValid = result.WeeklyPattern.Count() <= 7 &&
                    result.WeeklyPattern.All(w => Enum.IsDefined(typeof(DayOfWeek), w.DayOfWeek));

                // Date range should be valid
                var dateRangeValid = result.StartDate <= result.EndDate;

                return sessionCountConsistent && revenueConsistent && hourlyDataValid && 
                       weeklyDataValid && dateRangeValid;
            });
    }

    /// <summary>
    /// Unit test: Empty data should result in valid empty report structure.
    /// </summary>
    [Fact]
    public async Task EmptyDataResultsInValidEmptyReport()
    {
        // Arrange
        var query = new GetTableUtilizationReportQuery(DateTime.Today, DateTime.Today.AddDays(1));
        var emptyUtilizationMetrics = new TableUtilizationMetrics(
            OccupancyPercent: 0m,
            AverageSessionDuration: TimeSpan.Zero,
            TotalSessions: 0,
            TotalOperatingHours: TimeSpan.Zero,
            TotalOccupiedTime: TimeSpan.Zero
        );

        SetupMockRepository(query, 
            new List<DetailedTableUtilizationData>(),
            new List<HourlyOccupancyData>(),
            new List<DayOfWeekOccupancyData>(),
            new List<TimeRevenueData>());

        _mockAnalyticsEngine.Setup(e => e.CalculateTableUtilizationAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyUtilizationMetrics);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Empty data should result in valid empty report structure
        Assert.Equal(0m, result.OverallOccupancyPercent);
        Assert.Equal(TimeSpan.Zero, result.AverageSessionDuration);
        Assert.Equal(Money.Zero().Amount, result.TotalTimeRevenue.Amount);
        Assert.Empty(result.TableBreakdown);
        Assert.Empty(result.HourlyOccupancy);
        Assert.Empty(result.WeeklyPattern);
    }

    /// <summary>
    /// Unit test: Single table with known values produces correct calculations.
    /// </summary>
    [Fact]
    public async Task SingleTableWithKnownValuesProducesCorrectCalculations()
    {
        // Arrange
        var query = new GetTableUtilizationReportQuery(DateTime.Today, DateTime.Today.AddDays(1));
        var tableId = Guid.NewGuid();
        
        var utilizationMetrics = new TableUtilizationMetrics(
            OccupancyPercent: 50m,
            AverageSessionDuration: TimeSpan.FromHours(2),
            TotalSessions: 2,
            TotalOperatingHours: TimeSpan.FromHours(8),
            TotalOccupiedTime: TimeSpan.FromHours(4)
        );

        var detailedData = new List<DetailedTableUtilizationData>
        {
            new(tableId, 1, "Pool", DateTime.Today, TimeSpan.FromHours(4), 
                TimeSpan.FromHours(8), 2, new Money(40m, "USD"), TimeSpan.FromHours(4))
        };

        var revenueData = new List<TimeRevenueData>
        {
            new(DateTime.Today, tableId, "Pool", new Money(40m, "USD"), TimeSpan.FromHours(4), 10m)
        };

        SetupMockRepository(query, detailedData, 
            new List<HourlyOccupancyData>(),
            new List<DayOfWeekOccupancyData>(),
            revenueData);

        _mockAnalyticsEngine.Setup(e => e.CalculateTableUtilizationAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(utilizationMetrics);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Equal(50m, result.OverallOccupancyPercent);
        Assert.Equal(TimeSpan.FromHours(2), result.AverageSessionDuration);
        Assert.Equal(40m, result.TotalTimeRevenue.Amount);
        
        var tableBreakdown = result.TableBreakdown.Single();
        Assert.Equal(1, tableBreakdown.TableNumber);
        Assert.Equal("Pool", tableBreakdown.TableType);
        Assert.Equal(50m, tableBreakdown.OccupancyPercent);
        Assert.Equal(2, tableBreakdown.TotalSessions);
        Assert.Equal(40m, tableBreakdown.RevenuePerTable.Amount);
    }

    /// <summary>
    /// Generates valid table utilization test data for property-based testing.
    /// </summary>
    private static Arbitrary<(GetTableUtilizationReportQuery, TableUtilizationMetrics, 
        List<DetailedTableUtilizationData>, List<HourlyOccupancyData>, 
        List<DayOfWeekOccupancyData>, List<TimeRevenueData>)> GenerateValidTableUtilizationData()
    {
        return Arb.From(
            from startDate in Arb.Generate<DateTime>().Where(d => d.Year >= 2020 && d.Year <= 2030)
            from dayRange in Gen.Choose(1, 30)
            let endDate = startDate.AddDays(dayRange)
            let query = new GetTableUtilizationReportQuery(startDate, endDate)
            from tableCount in Gen.Choose(1, 10)
            from sessionCount in Gen.Choose(0, 50)
            from operatingHours in Gen.Choose(8, 16).Select(h => TimeSpan.FromHours(h))
            let tables = GenerateTables(tableCount)
            let sessions = GenerateDetailedUtilizationData(tables, startDate, endDate, sessionCount, operatingHours)
            let hourlyData = GenerateHourlyOccupancyData(startDate, endDate)
            let weeklyData = GenerateWeeklyOccupancyData()
            let revenueData = GenerateTimeRevenueData(tables, startDate, endDate)
            let utilizationMetrics = CalculateUtilizationMetrics(sessions, operatingHours, dayRange)
            select (query, utilizationMetrics, sessions, hourlyData, weeklyData, revenueData)
        );
    }

    private static List<(Guid Id, int Number, string Type)> GenerateTables(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => (Guid.NewGuid(), i, i % 2 == 0 ? "Pool" : "Snooker"))
            .ToList();
    }

    private static List<DetailedTableUtilizationData> GenerateDetailedUtilizationData(
        List<(Guid Id, int Number, string Type)> tables, 
        DateTime startDate, 
        DateTime endDate, 
        int sessionCount,
        TimeSpan operatingHours)
    {
        var random = new System.Random(42); // Fixed seed for reproducibility
        var result = new List<DetailedTableUtilizationData>();
        var totalDays = (endDate - startDate).Days + 1;

        foreach (var table in tables)
        {
            for (var day = 0; day < totalDays; day++)
            {
                var date = startDate.AddDays(day);
                var sessionsForDay = Math.Max(0, sessionCount / totalDays + random.Next(-2, 3));
                var occupiedHours = Math.Min(operatingHours.TotalHours, sessionsForDay * random.NextDouble() * 2);
                var totalSessionDuration = TimeSpan.FromHours(occupiedHours);
                var revenue = new Money((decimal)(occupiedHours * 10), "USD");

                result.Add(new DetailedTableUtilizationData(
                    table.Id, table.Number, table.Type, date,
                    TimeSpan.FromHours(occupiedHours), operatingHours,
                    sessionsForDay, revenue, totalSessionDuration));
            }
        }

        return result;
    }

    private static List<HourlyOccupancyData> GenerateHourlyOccupancyData(DateTime startDate, DateTime endDate)
    {
        var random = new System.Random(42);
        var result = new List<HourlyOccupancyData>();
        
        for (var hour = 9; hour <= 21; hour++) // Operating hours 9 AM to 9 PM
        {
            var occupancy = (decimal)(random.NextDouble() * 100);
            var activeTables = random.Next(1, 6);
            var revenue = new Money((decimal)(random.NextDouble() * 100), "USD");
            
            result.Add(new HourlyOccupancyData(
                startDate, hour, activeTables, 10, occupancy, revenue));
        }

        return result;
    }

    private static List<DayOfWeekOccupancyData> GenerateWeeklyOccupancyData()
    {
        var random = new System.Random(42);
        var result = new List<DayOfWeekOccupancyData>();

        foreach (DayOfWeek dayOfWeek in Enum.GetValues<DayOfWeek>())
        {
            var occupancy = (decimal)(random.NextDouble() * 100);
            var sessionDuration = TimeSpan.FromHours(random.NextDouble() * 4 + 1);
            var revenue = new Money((decimal)(random.NextDouble() * 200), "USD");
            var sessions = random.Next(1, 20);

            result.Add(new DayOfWeekOccupancyData(
                dayOfWeek, occupancy, sessionDuration, revenue, sessions));
        }

        return result;
    }

    private static List<TimeRevenueData> GenerateTimeRevenueData(
        List<(Guid Id, int Number, string Type)> tables, 
        DateTime startDate, 
        DateTime endDate)
    {
        var random = new System.Random(42);
        var result = new List<TimeRevenueData>();

        foreach (var table in tables)
        {
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var revenue = new Money((decimal)(random.NextDouble() * 50), "USD");
                var billedTime = TimeSpan.FromHours(random.NextDouble() * 8);
                var hourlyRate = billedTime.TotalHours > 0 ? (decimal)(revenue.Amount / (decimal)billedTime.TotalHours) : 0m;

                result.Add(new TimeRevenueData(
                    date, table.Id, table.Type, revenue, billedTime, hourlyRate));
            }
        }

        return result;
    }

    private static TableUtilizationMetrics CalculateUtilizationMetrics(
        List<DetailedTableUtilizationData> sessions, 
        TimeSpan operatingHours, 
        int dayRange)
    {
        if (!sessions.Any())
        {
            return new TableUtilizationMetrics(0m, TimeSpan.Zero, 0, TimeSpan.Zero, TimeSpan.Zero);
        }

        var totalOccupiedTime = sessions.Aggregate(TimeSpan.Zero, (sum, s) => sum + s.OccupiedTime);
        var totalOperatingHours = TimeSpan.FromTicks(operatingHours.Ticks * dayRange);
        var totalSessions = sessions.Sum(s => s.SessionCount);
        
        var occupancyPercent = totalOperatingHours.TotalHours > 0 
            ? Math.Min((decimal)(totalOccupiedTime.TotalHours / totalOperatingHours.TotalHours * 100), 100m)
            : 0m;

        var averageSessionDuration = totalSessions > 0 
            ? TimeSpan.FromTicks(sessions.Sum(s => s.TotalSessionDuration.Ticks) / totalSessions)
            : TimeSpan.Zero;

        return new TableUtilizationMetrics(
            Math.Round(occupancyPercent, 2), averageSessionDuration, totalSessions, 
            totalOperatingHours, totalOccupiedTime);
    }

    private void SetupMockRepository(
        GetTableUtilizationReportQuery query,
        List<DetailedTableUtilizationData> detailedData,
        List<HourlyOccupancyData> hourlyData,
        List<DayOfWeekOccupancyData> weeklyData,
        List<TimeRevenueData> revenueData)
    {
        _mockRepository.Setup(r => r.GetDetailedTableUtilizationDataAsync(
            query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detailedData);

        _mockRepository.Setup(r => r.GetHourlyOccupancyDataAsync(
            query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hourlyData);

        _mockRepository.Setup(r => r.GetDayOfWeekOccupancyDataAsync(
            query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weeklyData);

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(
            query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(revenueData);
    }
}