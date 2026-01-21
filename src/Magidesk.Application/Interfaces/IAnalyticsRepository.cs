using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for analytics data access.
/// Provides optimized queries for analytics calculations.
/// </summary>
public interface IAnalyticsRepository
{
    /// <summary>
    /// Gets table session data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of table session data</returns>
    Task<IEnumerable<TableSessionData>> GetTableSessionDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets table occupancy data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of table occupancy data</returns>
    Task<IEnumerable<TableOccupancyData>> GetTableOccupancyDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets time-based revenue data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of time revenue data</returns>
    Task<IEnumerable<TimeRevenueData>> GetTimeRevenueDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product revenue data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of product revenue data</returns>
    Task<IEnumerable<ProductRevenueData>> GetProductRevenueDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets member activity data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of member activity data</returns>
    Task<IEnumerable<MemberActivityData>> GetMemberActivityDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets member visit data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of member visit data</returns>
    Task<IEnumerable<MemberVisitData>> GetMemberVisitDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets operating hours for tables on the specified date.
    /// </summary>
    /// <param name="date">Date to get operating hours for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total operating hours for the date</returns>
    Task<TimeSpan> GetOperatingHoursAsync(
        DateTime date, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets historical data for trend analysis.
    /// </summary>
    /// <param name="metricType">Type of metric</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of trend data points</returns>
    Task<IEnumerable<TrendDataPoint>> GetTrendDataAsync(
        string metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets daily sales data for the specified date.
    /// </summary>
    /// <param name="date">Date to get sales data for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Daily sales data</returns>
    Task<DailySalesData> GetDailySalesDataAsync(
        DateTime date, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets hourly sales breakdown for the specified date.
    /// </summary>
    /// <param name="date">Date to get hourly breakdown for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of hourly sales data</returns>
    Task<IEnumerable<HourlySalesData>> GetHourlySalesDataAsync(
        DateTime date, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets category sales breakdown for the specified date.
    /// </summary>
    /// <param name="date">Date to get category breakdown for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of category sales data</returns>
    Task<IEnumerable<CategorySalesDto>> GetCategorySalesDataAsync(
        DateTime date, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment method breakdown for the specified date.
    /// </summary>
    /// <param name="date">Date to get payment method breakdown for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of payment method sales data</returns>
    Task<IEnumerable<PaymentMethodSalesDto>> GetPaymentMethodSalesDataAsync(
        DateTime date, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets table-specific sales breakdown for the specified date.
    /// </summary>
    /// <param name="date">Date to get table breakdown for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of table sales data</returns>
    Task<IEnumerable<TableSalesDto>> GetTableSalesDataAsync(
        DateTime date, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed table utilization data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of detailed table utilization data</returns>
    Task<IEnumerable<DetailedTableUtilizationData>> GetDetailedTableUtilizationDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets hourly occupancy data for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of hourly occupancy data</returns>
    Task<IEnumerable<HourlyOccupancyData>> GetHourlyOccupancyDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets day of week occupancy patterns for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of day of week occupancy data</returns>
    Task<IEnumerable<DayOfWeekOccupancyData>> GetDayOfWeekOccupancyDataAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets shift summary data for the specified shift and date range.
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Shift summary data</returns>
    Task<ShiftSummaryData> GetShiftSummaryDataAsync(
        Guid shiftId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets server sales breakdown for the specified shift and date range.
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of server sales data</returns>
    Task<IEnumerable<ServerSalesData>> GetServerSalesDataAsync(
        Guid shiftId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cash reconciliation data for the specified shift and date range.
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of cash reconciliation data</returns>
    Task<IEnumerable<CashReconciliationData>> GetCashReconciliationDataAsync(
        Guid shiftId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets exception data for the specified shift and date range.
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of exception data</returns>
    Task<IEnumerable<ExceptionData>> GetExceptionDataAsync(
        Guid shiftId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets void data for the specified shift and date range.
    /// </summary>
    /// <param name="shiftId">Shift ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of void data</returns>
    Task<IEnumerable<VoidData>> GetVoidDataAsync(
        Guid shiftId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets server performance data for all servers across all shifts in the specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of server performance data</returns>
    Task<IEnumerable<ServerPerformanceData>> GetServerPerformanceDataAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}