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
/// Property-based tests for GetServerPerformanceReportQueryHandler.
/// Feature: reporting-export, Property 8: Server Performance Attribution
/// Validates: Requirements 7.1, 7.2, 7.4, 7.5
/// </summary>
public class GetServerPerformanceReportQueryHandlerPropertyTests
{
    private readonly Mock<IAnalyticsRepository> _mockRepository;
    private readonly Mock<ILogger<GetServerPerformanceReportQueryHandler>> _mockLogger;
    private readonly GetServerPerformanceReportQueryHandler _handler;

    public GetServerPerformanceReportQueryHandlerPropertyTests()
    {
        _mockRepository = new Mock<IAnalyticsRepository>();
        _mockLogger = new Mock<ILogger<GetServerPerformanceReportQueryHandler>>();
        _handler = new GetServerPerformanceReportQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Unit test: Basic server performance report generation with known values.
    /// </summary>
    [Fact]
    public async Task ServerPerformanceReport_WithKnownValues_ReturnsCorrectAttribution()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today;
        var query = new GetServerPerformanceReportQuery(startDate, endDate);

        var serverId1 = Guid.NewGuid();
        var serverId2 = Guid.NewGuid();

        var serverPerformanceData = new List<ServerPerformanceData>
        {
            new(serverId1, "John Doe", new Money(1000m, "USD"), 10, new Money(150m, "USD"), 
                TimeSpan.FromHours(8), DateTime.Today.AddHours(-8), DateTime.Today, 1),
            new(serverId2, "Jane Smith", new Money(800m, "USD"), 8, new Money(120m, "USD"), 
                TimeSpan.FromHours(6), DateTime.Today.AddHours(-6), DateTime.Today, 1)
        };

        _mockRepository.Setup(r => r.GetServerPerformanceDataAsync(
            startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serverPerformanceData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Verify server performance attribution
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Equal(2, result.TotalServers);
        Assert.Equal(1800m, result.TotalSales.Amount); // 1000 + 800
        Assert.Equal(270m, result.TotalTips.Amount); // 150 + 120
        
        // Verify tip percentage calculation
        var expectedTipPercentage = (270m / 1800m) * 100;
        Assert.Equal(Math.Round(expectedTipPercentage, 2), result.OverallTipPercentage);
        
        // Verify server breakdown
        Assert.Equal(2, result.ServerBreakdown.Count());
        
        var johnPerformance = result.ServerBreakdown.First(s => s.ServerId == serverId1);
        Assert.Equal("John Doe", johnPerformance.ServerName);
        Assert.Equal(1000m, johnPerformance.TotalSales.Amount);
        Assert.Equal(10, johnPerformance.TransactionCount);
        Assert.Equal(100m, johnPerformance.AverageTicketSize); // 1000 / 10
        Assert.Equal(15m, johnPerformance.TipPercentage); // (150 / 1000) * 100
        Assert.Equal(125m, johnPerformance.SalesPerHour); // 1000 / 8
        
        // Verify top performers ranking
        Assert.True(result.TopPerformers.Any());
        var topPerformer = result.TopPerformers.First();
        Assert.Equal(serverId1, topPerformer.ServerId); // John has higher sales
        Assert.Equal(1, topPerformer.Rank);
    }

    /// <summary>
    /// Unit test: Empty server data should result in zero values but valid structure.
    /// </summary>
    [Fact]
    public async Task EmptyServerDataResultsInZeroValuesWithValidStructure()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today;
        var query = new GetServerPerformanceReportQuery(startDate, endDate);

        _mockRepository.Setup(r => r.GetServerPerformanceDataAsync(
            startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServerPerformanceData>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Empty data should result in zero values
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Equal(0, result.TotalServers);
        Assert.Equal(0m, result.TotalSales.Amount);
        Assert.Equal(0m, result.TotalTips.Amount);
        Assert.Equal(0m, result.OverallTipPercentage);
        Assert.Equal(0m, result.AverageSalesPerServer);
        Assert.Equal(0m, result.AverageTransactionsPerServer);
        
        // Verify collections are empty but not null
        Assert.NotNull(result.ServerBreakdown);
        Assert.NotNull(result.TopPerformers);
        Assert.NotNull(result.PerformanceTrends);
        Assert.Empty(result.ServerBreakdown);
        Assert.Empty(result.TopPerformers);
        Assert.Empty(result.PerformanceTrends);
    }

    /// <summary>
    /// Property test: Server performance attribution accuracy.
    /// For any server performance calculation, sales should be attributed only to the assigned server, 
    /// tip calculations should be accurate, and performance rankings should be consistent.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property ServerPerformanceAttributionAccuracy()
    {
        return Prop.ForAll(
            GenerateValidServerPerformanceData(),
            data =>
            {
                // Arrange
                var (query, serverPerformanceData) = data;

                _mockRepository.Setup(r => r.GetServerPerformanceDataAsync(
                    query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(serverPerformanceData);

                // Act
                var result = _handler.HandleAsync(query).Result;

                // Assert - Property 8: Server Performance Attribution
                
                // Total sales should equal sum of individual server sales
                var expectedTotalSales = serverPerformanceData.Sum(s => s.TotalSales.Amount);
                var salesAttributionCorrect = Math.Abs(result.TotalSales.Amount - expectedTotalSales) < 0.01m;
                
                // Total tips should equal sum of individual server tips
                var expectedTotalTips = serverPerformanceData.Sum(s => s.TotalTips.Amount);
                var tipsAttributionCorrect = Math.Abs(result.TotalTips.Amount - expectedTotalTips) < 0.01m;
                
                // Server count should match input data
                var serverCountCorrect = result.TotalServers == serverPerformanceData.Count;
                
                // Each server's tip percentage should be calculated correctly
                var tipPercentagesCorrect = result.ServerBreakdown.All(server =>
                {
                    var originalServer = serverPerformanceData.First(s => s.ServerId == server.ServerId);
                    var expectedTipPercentage = originalServer.TotalSales.Amount > 0 
                        ? (originalServer.TotalTips.Amount / originalServer.TotalSales.Amount) * 100 
                        : 0;
                    return Math.Abs(server.TipPercentage - expectedTipPercentage) < 0.01m;
                });
                
                // Average ticket size should be calculated correctly for each server
                var averageTicketSizesCorrect = result.ServerBreakdown.All(server =>
                {
                    var originalServer = serverPerformanceData.First(s => s.ServerId == server.ServerId);
                    var expectedAverage = originalServer.TransactionCount > 0 
                        ? originalServer.TotalSales.Amount / originalServer.TransactionCount 
                        : 0;
                    return Math.Abs(server.AverageTicketSize - expectedAverage) < 0.01m;
                });
                
                // Sales per hour should be calculated correctly for each server
                var salesPerHourCorrect = result.ServerBreakdown.All(server =>
                {
                    var originalServer = serverPerformanceData.First(s => s.ServerId == server.ServerId);
                    var expectedSalesPerHour = originalServer.WorkTime.TotalHours > 0 
                        ? (decimal)(originalServer.TotalSales.Amount / (decimal)originalServer.WorkTime.TotalHours)
                        : 0;
                    return Math.Abs(server.SalesPerHour - expectedSalesPerHour) < 0.01m;
                });
                
                // Rankings should be consistent (highest sales gets rank 1)
                var rankingsConsistent = true;
                if (result.ServerBreakdown.Count() > 1)
                {
                    var sortedByRank = result.ServerBreakdown.OrderBy(s => s.SalesRank).ToList();
                    var sortedBySales = result.ServerBreakdown.OrderByDescending(s => s.TotalSales.Amount).ToList();
                    
                    for (int i = 0; i < sortedByRank.Count; i++)
                    {
                        if (sortedByRank[i].ServerId != sortedBySales[i].ServerId)
                        {
                            rankingsConsistent = false;
                            break;
                        }
                    }
                }
                
                // All monetary values should be non-negative
                var monetaryValuesValid = result.TotalSales.Amount >= 0 &&
                    result.TotalTips.Amount >= 0 &&
                    result.ServerBreakdown.All(s => 
                        s.TotalSales.Amount >= 0 && 
                        s.TotalTips.Amount >= 0 &&
                        s.AverageTicketSize >= 0 &&
                        s.SalesPerHour >= 0);
                
                // Percentages should be valid (0-100 for tip percentages)
                var percentagesValid = result.OverallTipPercentage >= 0 &&
                    result.ServerBreakdown.All(s => s.TipPercentage >= 0);
                
                // Transaction counts should be non-negative
                var transactionCountsValid = result.ServerBreakdown.All(s => s.TransactionCount >= 0);

                return salesAttributionCorrect && tipsAttributionCorrect && serverCountCorrect &&
                       tipPercentagesCorrect && averageTicketSizesCorrect && salesPerHourCorrect &&
                       rankingsConsistent && monetaryValuesValid && percentagesValid && transactionCountsValid;
            });
    }

    /// <summary>
    /// Property test: Performance ranking consistency.
    /// For any server performance data, rankings should be consistent with sales amounts,
    /// and top performers should be correctly identified.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property PerformanceRankingConsistency()
    {
        return Prop.ForAll(
            GenerateValidServerPerformanceData(),
            data =>
            {
                // Arrange
                var (query, serverPerformanceData) = data;

                // Skip if no servers
                if (!serverPerformanceData.Any())
                    return true;

                _mockRepository.Setup(r => r.GetServerPerformanceDataAsync(
                    query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(serverPerformanceData);

                // Act
                var result = _handler.HandleAsync(query).Result;

                // Assert - Performance ranking consistency
                
                // Top performers should be ordered by sales (descending)
                var topPerformersOrdered = true;
                var topPerformersList = result.TopPerformers.ToList();
                for (int i = 0; i < topPerformersList.Count - 1; i++)
                {
                    if (topPerformersList[i].TotalSales.Amount < topPerformersList[i + 1].TotalSales.Amount)
                    {
                        topPerformersOrdered = false;
                        break;
                    }
                }
                
                // Ranks should be sequential starting from 1
                var ranksSequential = true;
                for (int i = 0; i < topPerformersList.Count; i++)
                {
                    if (topPerformersList[i].Rank != i + 1)
                    {
                        ranksSequential = false;
                        break;
                    }
                }
                
                // Server breakdown should also be ordered by sales (descending)
                var serverBreakdownOrdered = true;
                var serverBreakdownList = result.ServerBreakdown.ToList();
                for (int i = 0; i < serverBreakdownList.Count - 1; i++)
                {
                    if (serverBreakdownList[i].TotalSales.Amount < serverBreakdownList[i + 1].TotalSales.Amount)
                    {
                        serverBreakdownOrdered = false;
                        break;
                    }
                }
                
                // Performance categories should be assigned correctly
                var categoriesCorrect = result.TopPerformers.All(tp =>
                {
                    return tp.Rank switch
                    {
                        1 => tp.PerformanceCategory == "Top Performer",
                        <= 3 => tp.PerformanceCategory == "High Performer",
                        <= 7 => tp.PerformanceCategory == "Good Performer",
                        _ => tp.PerformanceCategory == "Average Performer"
                    };
                });

                return topPerformersOrdered && ranksSequential && serverBreakdownOrdered && categoriesCorrect;
            });
    }

    /// <summary>
    /// Generates valid server performance test data for property-based testing.
    /// </summary>
    private static Arbitrary<(GetServerPerformanceReportQuery, List<ServerPerformanceData>)> GenerateValidServerPerformanceData()
    {
        return Arb.From(
            from startDateDays in Gen.Choose(-365, -1)
            let startDate = DateTime.Today.AddDays(startDateDays)
            from dayRange in Gen.Choose(1, 30)
            let endDate = startDate.AddDays(dayRange)
            let query = new GetServerPerformanceReportQuery(startDate, endDate)
            from serverCount in Gen.Choose(0, 10)
            let serverData = GenerateServerPerformanceDataList(serverCount)
            select (query, serverData)
        );
    }

    private static List<ServerPerformanceData> GenerateServerPerformanceDataList(int serverCount)
    {
        var random = new System.Random(42);
        var result = new List<ServerPerformanceData>();
        var serverNames = new[] { "John Doe", "Jane Smith", "Bob Johnson", "Alice Brown", "Charlie Wilson", 
                                 "Diana Prince", "Clark Kent", "Bruce Wayne", "Peter Parker", "Tony Stark" };

        for (int i = 0; i < serverCount; i++)
        {
            var salesAmount = (decimal)(random.NextDouble() * 2000); // $0 - $2000
            var transactionCount = random.Next(0, 50);
            var tipAmount = salesAmount * (decimal)(random.NextDouble() * 0.25); // 0-25% tips
            var workHours = random.Next(1, 12);
            var workTime = TimeSpan.FromHours(workHours);
            var shiftCount = random.Next(1, 5);

            result.Add(new ServerPerformanceData(
                ServerId: Guid.NewGuid(),
                ServerName: serverNames[i % serverNames.Length],
                TotalSales: new Money(Math.Round(salesAmount, 2), "USD"),
                TransactionCount: transactionCount,
                TotalTips: new Money(Math.Round(tipAmount, 2), "USD"),
                WorkTime: workTime,
                FirstTransaction: DateTime.Today.AddHours(-workHours),
                LastTransaction: DateTime.Today,
                ShiftCount: shiftCount
            ));
        }

        return result;
    }
}