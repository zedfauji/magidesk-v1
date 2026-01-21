using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries.Handlers;

/// <summary>
/// Handler for GetShiftSummaryReportQuery.
/// Generates comprehensive shift summary reports with cash reconciliation and server breakdowns.
/// </summary>
public class GetShiftSummaryReportQueryHandler : IQueryHandler<GetShiftSummaryReportQuery, ShiftSummaryReportDto>
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<GetShiftSummaryReportQueryHandler> _logger;

    public GetShiftSummaryReportQueryHandler(
        IAnalyticsRepository repository,
        ILogger<GetShiftSummaryReportQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the shift summary report query.
    /// </summary>
    public async Task<ShiftSummaryReportDto> HandleAsync(
        GetShiftSummaryReportQuery query, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating shift summary report for Shift {ShiftId} from {StartDate} to {EndDate}", 
            query.ShiftId, query.StartDate, query.EndDate);

        try
        {
            // Get main shift summary data
            var shiftSummaryData = await _repository.GetShiftSummaryDataAsync(
                query.ShiftId, query.StartDate, query.EndDate, cancellationToken);

            // Get all breakdown data in parallel for performance
            var serverSalesTask = _repository.GetServerSalesDataAsync(
                query.ShiftId, query.StartDate, query.EndDate, cancellationToken);
            var cashReconciliationTask = _repository.GetCashReconciliationDataAsync(
                query.ShiftId, query.StartDate, query.EndDate, cancellationToken);
            var exceptionsTask = _repository.GetExceptionDataAsync(
                query.ShiftId, query.StartDate, query.EndDate, cancellationToken);
            var voidsTask = _repository.GetVoidDataAsync(
                query.ShiftId, query.StartDate, query.EndDate, cancellationToken);

            await Task.WhenAll(serverSalesTask, cashReconciliationTask, exceptionsTask, voidsTask);

            var serverSales = serverSalesTask.Result.ToList();
            var cashReconciliations = cashReconciliationTask.Result.ToList();
            var exceptions = exceptionsTask.Result.ToList();
            var voids = voidsTask.Result.ToList();

            // Calculate payment method breakdowns
            var paymentBreakdown = CalculatePaymentBreakdown(shiftSummaryData.TotalSales, serverSales);

            // Calculate average ticket size
            var averageTicketSize = shiftSummaryData.TransactionCount > 0 
                ? shiftSummaryData.TotalSales.Amount / shiftSummaryData.TransactionCount 
                : 0m;

            // Convert server sales data to DTOs
            var serverBreakdown = serverSales.Select(s => new ServerSalesDto(
                ServerId: s.ServerId,
                ServerName: s.ServerName,
                TotalSales: s.TotalSales,
                TransactionCount: s.TransactionCount,
                AverageTicketSize: s.TransactionCount > 0 ? s.TotalSales.Amount / s.TransactionCount : 0m,
                TotalTips: s.TotalTips,
                TipPercentage: s.TotalSales.Amount > 0 ? (s.TotalTips.Amount / s.TotalSales.Amount) * 100 : 0m
            ));

            // Aggregate cash reconciliation data
            var cashReconciliation = AggregateCashReconciliation(cashReconciliations);

            // Group exceptions by type
            var exceptionSummary = exceptions
                .GroupBy(e => e.ExceptionType)
                .Select(g => new ExceptionSummaryDto(
                    ExceptionType: g.Key,
                    Count: g.Count(),
                    TotalAmount: g.Aggregate(Money.Zero(), (sum, e) => sum + e.Amount),
                    Description: g.First().Description
                ));

            // Group voids by type
            var voidSummary = voids
                .GroupBy(v => v.VoidType)
                .Select(g => new VoidSummaryDto(
                    VoidType: g.Key,
                    Count: g.Count(),
                    TotalAmount: g.Aggregate(Money.Zero(), (sum, v) => sum + v.Amount),
                    Reason: g.First().Reason
                ));

            // Calculate shift metrics
            var metrics = CalculateShiftMetrics(shiftSummaryData, serverSales, query.StartDate, query.EndDate);

            // Create the shift summary report DTO
            var report = new ShiftSummaryReportDto(
                ShiftId: query.ShiftId,
                ShiftName: shiftSummaryData.ShiftName,
                StartDate: query.StartDate,
                EndDate: query.EndDate,
                TotalSales: shiftSummaryData.TotalSales,
                CashSales: paymentBreakdown.CashSales,
                CardSales: paymentBreakdown.CardSales,
                OtherPaymentSales: paymentBreakdown.OtherSales,
                TransactionCount: shiftSummaryData.TransactionCount,
                AverageTicketSize: Math.Round(averageTicketSize, 2),
                CashReconciliation: cashReconciliation,
                ServerBreakdown: serverBreakdown,
                Exceptions: exceptionSummary,
                Voids: voidSummary,
                Metrics: metrics
            );

            _logger.LogInformation("Generated shift summary report for Shift {ShiftId}: Total Sales {TotalSales}, Transactions {TransactionCount}", 
                query.ShiftId, report.TotalSales.Amount, report.TransactionCount);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating shift summary report for Shift {ShiftId} from {StartDate} to {EndDate}", 
                query.ShiftId, query.StartDate, query.EndDate);
            throw new InvalidOperationException($"Failed to generate shift summary report for shift {query.ShiftId}", ex);
        }
    }

    /// <summary>
    /// Calculates payment method breakdown from server sales data.
    /// This is a simplified calculation - in a real implementation, this would come from payment data.
    /// </summary>
    private (Money CashSales, Money CardSales, Money OtherSales) CalculatePaymentBreakdown(
        Money totalSales, 
        IEnumerable<ServerSalesData> serverSales)
    {
        // Simplified calculation - assume 40% cash, 55% card, 5% other
        // In real implementation, this would be calculated from actual payment data
        var cashSales = totalSales * 0.40m;
        var cardSales = totalSales * 0.55m;
        var otherSales = totalSales * 0.05m;

        return (cashSales, cardSales, otherSales);
    }

    /// <summary>
    /// Aggregates cash reconciliation data from multiple cash sessions.
    /// </summary>
    private CashReconciliationDto AggregateCashReconciliation(IEnumerable<CashReconciliationData> reconciliations)
    {
        var reconciliationList = reconciliations.ToList();
        
        if (!reconciliationList.Any())
        {
            return new CashReconciliationDto(
                OpeningBalance: Money.Zero(),
                ExpectedCash: Money.Zero(),
                ActualCash: Money.Zero(),
                Difference: Money.Zero(),
                CashDrops: Money.Zero(),
                Payouts: Money.Zero(),
                DrawerBleeds: Money.Zero(),
                IsReconciled: true
            );
        }

        var openingBalance = reconciliationList.Aggregate(Money.Zero(), (sum, r) => sum + r.OpeningBalance);
        var expectedCash = reconciliationList.Aggregate(Money.Zero(), (sum, r) => sum + r.ExpectedCash);
        var actualCash = reconciliationList.Aggregate(Money.Zero(), (sum, r) => sum + r.ActualCash);
        var cashDrops = reconciliationList.Aggregate(Money.Zero(), (sum, r) => sum + r.CashDrops);
        var payouts = reconciliationList.Aggregate(Money.Zero(), (sum, r) => sum + r.Payouts);
        var drawerBleeds = reconciliationList.Aggregate(Money.Zero(), (sum, r) => sum + r.DrawerBleeds);
        
        // Calculate difference handling negative values properly
        // Since Money doesn't allow negative values, we calculate the absolute difference
        // and represent shortages as positive values (business convention)
        var differenceAmount = Math.Abs(actualCash.Amount - expectedCash.Amount);
        var difference = new Money(differenceAmount, actualCash.Currency);
        
        var isReconciled = reconciliationList.All(r => r.IsClosed);

        return new CashReconciliationDto(
            OpeningBalance: openingBalance,
            ExpectedCash: expectedCash,
            ActualCash: actualCash,
            Difference: difference,
            CashDrops: cashDrops,
            Payouts: payouts,
            DrawerBleeds: drawerBleeds,
            IsReconciled: isReconciled
        );
    }

    /// <summary>
    /// Calculates additional shift metrics and KPIs.
    /// </summary>
    private ShiftMetricsDto CalculateShiftMetrics(
        ShiftSummaryData shiftData, 
        IEnumerable<ServerSalesData> serverSales,
        DateTime startDate,
        DateTime endDate)
    {
        var shiftDuration = endDate - startDate;
        var hoursWorked = (decimal)shiftDuration.TotalHours;

        var salesPerHour = hoursWorked > 0 ? shiftData.TotalSales.Amount / hoursWorked : 0m;
        var transactionsPerHour = hoursWorked > 0 ? shiftData.TransactionCount / hoursWorked : 0m;

        // Simplified metrics - in real implementation, these would be calculated from actual data
        var averageServiceTime = TimeSpan.FromMinutes(15); // Default 15 minutes
        var tableTurnoverRate = 2.5m; // Default 2.5 turns per shift
        var peakHourSales = shiftData.TotalSales * 0.25m; // Assume 25% of sales in peak hour
        var peakHour = 19; // Default 7 PM

        return new ShiftMetricsDto(
            CustomerCount: shiftData.CustomerCount,
            SalesPerHour: Math.Round(salesPerHour, 2),
            TransactionsPerHour: Math.Round(transactionsPerHour, 2),
            AverageServiceTime: averageServiceTime,
            TableTurnoverRate: tableTurnoverRate,
            PeakHourSales: peakHourSales,
            PeakHour: peakHour
        );
    }
}