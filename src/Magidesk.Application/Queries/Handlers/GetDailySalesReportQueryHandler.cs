using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries.Handlers;

/// <summary>
/// Handler for GetDailySalesReportQuery.
/// Generates comprehensive daily sales reports with breakdowns.
/// </summary>
public class GetDailySalesReportQueryHandler : IQueryHandler<GetDailySalesReportQuery, DailySalesReportDto>
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<GetDailySalesReportQueryHandler> _logger;

    public GetDailySalesReportQueryHandler(
        IAnalyticsRepository repository,
        ILogger<GetDailySalesReportQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the daily sales report query.
    /// </summary>
    public async Task<DailySalesReportDto> HandleAsync(
        GetDailySalesReportQuery query, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating daily sales report for {Date}", query.Date);

        try
        {
            // Get main daily sales data
            var dailySalesData = await _repository.GetDailySalesDataAsync(query.Date, cancellationToken);

            // Get all breakdown data in parallel for performance
            var hourlySalesTask = _repository.GetHourlySalesDataAsync(query.Date, cancellationToken);
            var categorySalesTask = _repository.GetCategorySalesDataAsync(query.Date, cancellationToken);
            var paymentMethodSalesTask = _repository.GetPaymentMethodSalesDataAsync(query.Date, cancellationToken);
            var tableSalesTask = _repository.GetTableSalesDataAsync(query.Date, cancellationToken);

            await Task.WhenAll(hourlySalesTask, categorySalesTask, paymentMethodSalesTask, tableSalesTask);

            var hourlySales = hourlySalesTask.Result.ToList();
            var categorySales = categorySalesTask.Result.ToList();
            var paymentMethodSales = paymentMethodSalesTask.Result.ToList();
            var tableSales = tableSalesTask.Result.ToList();

            // Calculate average ticket size
            var averageTicketSize = dailySalesData.TransactionCount > 0 
                ? dailySalesData.TotalSales.Amount / dailySalesData.TransactionCount 
                : 0m;

            // Convert hourly sales data to DTOs
            var hourlyBreakdown = hourlySales.Select(h => new HourlySalesDto(
                Hour: h.Hour,
                Sales: h.Sales,
                TransactionCount: h.TransactionCount,
                CustomerCount: h.CustomerCount
            ));

            // Create the daily sales report DTO
            var report = new DailySalesReportDto(
                Date: query.Date,
                TotalSales: dailySalesData.TotalSales,
                TimeBasedSales: dailySalesData.TimeBasedSales,
                ProductSales: dailySalesData.ProductSales,
                TotalTax: dailySalesData.TotalTax,
                TotalGratuity: dailySalesData.TotalGratuity,
                TransactionCount: dailySalesData.TransactionCount,
                CustomerCount: dailySalesData.CustomerCount,
                AverageTicketSize: Math.Round(averageTicketSize, 2),
                HourlyBreakdown: hourlyBreakdown,
                CategoryBreakdown: categorySales,
                PaymentBreakdown: paymentMethodSales,
                TableBreakdown: tableSales
            );

            _logger.LogInformation("Generated daily sales report for {Date}: Total Sales {TotalSales}, Transactions {TransactionCount}", 
                query.Date, report.TotalSales.Amount, report.TransactionCount);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating daily sales report for {Date}", query.Date);
            throw new InvalidOperationException($"Failed to generate daily sales report for {query.Date:yyyy-MM-dd}", ex);
        }
    }
}