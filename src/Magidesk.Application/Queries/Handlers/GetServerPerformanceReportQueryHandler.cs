using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries.Handlers;

/// <summary>
/// Handler for GetServerPerformanceReportQuery.
/// Generates comprehensive server performance analytics with sales volume, tips, and comparisons.
/// </summary>
public class GetServerPerformanceReportQueryHandler : IQueryHandler<GetServerPerformanceReportQuery, ServerPerformanceReportDto>
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<GetServerPerformanceReportQueryHandler> _logger;

    public GetServerPerformanceReportQueryHandler(
        IAnalyticsRepository repository,
        ILogger<GetServerPerformanceReportQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the server performance report query.
    /// </summary>
    public async Task<ServerPerformanceReportDto> HandleAsync(
        GetServerPerformanceReportQuery query, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating server performance report for {StartDate} to {EndDate}", 
            query.StartDate, query.EndDate);

        try
        {
            // Get server performance data for the date range
            var serverPerformanceData = await _repository.GetServerPerformanceDataAsync(
                query.StartDate, query.EndDate, cancellationToken);

            var serverDataList = serverPerformanceData.ToList();

            if (!serverDataList.Any())
            {
                _logger.LogInformation("No server performance data found for date range {StartDate} to {EndDate}", 
                    query.StartDate, query.EndDate);
                
                return CreateEmptyReport(query.StartDate, query.EndDate);
            }

            // Calculate overall metrics
            var totalSales = new Money(serverDataList.Sum(s => s.TotalSales.Amount), "USD");
            var totalTips = new Money(serverDataList.Sum(s => s.TotalTips.Amount), "USD");
            var overallTipPercentage = totalSales.Amount > 0 ? (totalTips.Amount / totalSales.Amount) * 100 : 0;
            var totalServers = serverDataList.Count;
            var averageSalesPerServer = totalServers > 0 ? totalSales.Amount / totalServers : 0;
            var averageTransactionsPerServer = totalServers > 0 ? 
                (decimal)serverDataList.Sum(s => s.TransactionCount) / totalServers : 0;

            // Create server performance breakdown
            var serverBreakdown = CreateServerBreakdown(serverDataList);

            // Create top performers ranking
            var topPerformers = CreateTopPerformersRanking(serverDataList);

            // Create performance trends (simplified for now)
            var performanceTrends = CreatePerformanceTrends(serverDataList, query.StartDate, query.EndDate);

            var report = new ServerPerformanceReportDto(
                StartDate: query.StartDate,
                EndDate: query.EndDate,
                TotalServers: totalServers,
                TotalSales: totalSales,
                TotalTips: totalTips,
                OverallTipPercentage: Math.Round(overallTipPercentage, 2),
                AverageSalesPerServer: Math.Round(averageSalesPerServer, 2),
                AverageTransactionsPerServer: Math.Round(averageTransactionsPerServer, 2),
                ServerBreakdown: serverBreakdown,
                TopPerformers: topPerformers,
                PerformanceTrends: performanceTrends
            );

            _logger.LogInformation("Generated server performance report: {TotalServers} servers, Total Sales {TotalSales}, Overall Tip % {TipPercentage}", 
                totalServers, totalSales.Amount, overallTipPercentage);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating server performance report for {StartDate} to {EndDate}", 
                query.StartDate, query.EndDate);
            throw new InvalidOperationException($"Failed to generate server performance report for {query.StartDate:yyyy-MM-dd} to {query.EndDate:yyyy-MM-dd}", ex);
        }
    }

    private static ServerPerformanceReportDto CreateEmptyReport(DateTime startDate, DateTime endDate)
    {
        return new ServerPerformanceReportDto(
            StartDate: startDate,
            EndDate: endDate,
            TotalServers: 0,
            TotalSales: new Money(0, "USD"),
            TotalTips: new Money(0, "USD"),
            OverallTipPercentage: 0,
            AverageSalesPerServer: 0,
            AverageTransactionsPerServer: 0,
            ServerBreakdown: Enumerable.Empty<ServerPerformanceDto>(),
            TopPerformers: Enumerable.Empty<ServerComparisonDto>(),
            PerformanceTrends: Enumerable.Empty<ServerTrendDto>()
        );
    }

    private static IEnumerable<ServerPerformanceDto> CreateServerBreakdown(List<ServerPerformanceData> serverDataList)
    {
        var totalSales = serverDataList.Sum(s => s.TotalSales.Amount);
        
        return serverDataList
            .OrderByDescending(s => s.TotalSales.Amount) // Sort first
            .Select((server, index) =>
            {
                var averageTicketSize = server.TransactionCount > 0 ? 
                    server.TotalSales.Amount / server.TransactionCount : 0;
                
                var tipPercentage = server.TotalSales.Amount > 0 ? 
                    (server.TotalTips.Amount / server.TotalSales.Amount) * 100 : 0;
                
                var salesRank = index + 1; // Ranking based on sorted order
                
                var performanceScore = CalculatePerformanceScore(server, totalSales);
                
                var salesPerHour = server.WorkTime.TotalHours > 0 ? 
                    (decimal)(server.TotalSales.Amount / (decimal)server.WorkTime.TotalHours) : 0;

                return new ServerPerformanceDto(
                    ServerId: server.ServerId,
                    ServerName: server.ServerName,
                    TotalSales: server.TotalSales,
                    TransactionCount: server.TransactionCount,
                    AverageTicketSize: Math.Round(averageTicketSize, 2),
                    TotalTips: server.TotalTips,
                    TipPercentage: Math.Round(tipPercentage, 2),
                    SalesRank: salesRank,
                    PerformanceScore: Math.Round(performanceScore, 2),
                    TotalWorkTime: server.WorkTime,
                    SalesPerHour: Math.Round(salesPerHour, 2)
                );
            })
            .ToList(); // Remove the extra OrderByDescending at the end
    }

    private static IEnumerable<ServerComparisonDto> CreateTopPerformersRanking(List<ServerPerformanceData> serverDataList)
    {
        return serverDataList
            .OrderByDescending(s => s.TotalSales.Amount)
            .Take(10) // Top 10 performers
            .Select((server, index) =>
            {
                var tipPercentage = server.TotalSales.Amount > 0 ? 
                    (server.TotalTips.Amount / server.TotalSales.Amount) * 100 : 0;
                
                var performanceCategory = index switch
                {
                    0 => "Top Performer",
                    < 3 => "High Performer",
                    < 7 => "Good Performer",
                    _ => "Average Performer"
                };

                return new ServerComparisonDto(
                    ServerId: server.ServerId,
                    ServerName: server.ServerName,
                    TotalSales: server.TotalSales,
                    TipPercentage: Math.Round(tipPercentage, 2),
                    Rank: index + 1,
                    PerformanceCategory: performanceCategory
                );
            })
            .ToList();
    }

    private static IEnumerable<ServerTrendDto> CreatePerformanceTrends(
        List<ServerPerformanceData> serverDataList, 
        DateTime startDate, 
        DateTime endDate)
    {
        // Simplified trend calculation - in a real implementation, this would analyze daily performance
        var daysDiff = (endDate - startDate).Days;
        
        return serverDataList.Select(server =>
        {
            var dailySales = daysDiff > 0 ? server.TotalSales.Amount / daysDiff : server.TotalSales.Amount;
            var dailyTipPercentage = server.TotalSales.Amount > 0 ? 
                (server.TotalTips.Amount / server.TotalSales.Amount) * 100 : 0;
            
            // Simple growth rate calculation (would be more sophisticated in real implementation)
            var growthRate = 0m; // Placeholder - would calculate based on historical data

            return new ServerTrendDto(
                ServerId: server.ServerId,
                ServerName: server.ServerName,
                Date: endDate,
                DailySales: new Money(dailySales, "USD"),
                DailyTipPercentage: Math.Round(dailyTipPercentage, 2),
                GrowthRate: growthRate
            );
        })
        .ToList();
    }

    private static decimal CalculatePerformanceScore(ServerPerformanceData server, decimal totalSales)
    {
        // Performance score calculation based on multiple factors
        var salesWeight = 0.4m;
        var tipWeight = 0.3m;
        var efficiencyWeight = 0.3m;

        var salesScore = totalSales > 0 ? (server.TotalSales.Amount / totalSales) * 100 : 0;
        
        var tipScore = server.TotalSales.Amount > 0 ? 
            (server.TotalTips.Amount / server.TotalSales.Amount) * 100 : 0;
        
        var efficiencyScore = server.WorkTime.TotalHours > 0 ? 
            (decimal)(server.TotalSales.Amount / (decimal)server.WorkTime.TotalHours) : 0;

        // Normalize efficiency score (assuming $100/hour is excellent)
        efficiencyScore = Math.Min(efficiencyScore / 100, 1) * 100;

        return (salesScore * salesWeight) + (tipScore * tipWeight) + (efficiencyScore * efficiencyWeight);
    }
}