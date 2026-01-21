using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs.Reports;

/// <summary>
/// Daily Sales Report DTO containing comprehensive sales breakdown for a single day.
/// </summary>
public record DailySalesReportDto(
    DateTime Date,
    Money TotalSales,
    Money TimeBasedSales,
    Money ProductSales,
    Money TotalTax,
    Money TotalGratuity,
    int TransactionCount,
    int CustomerCount,
    decimal AverageTicketSize,
    IEnumerable<HourlySalesDto> HourlyBreakdown,
    IEnumerable<CategorySalesDto> CategoryBreakdown,
    IEnumerable<PaymentMethodSalesDto> PaymentBreakdown,
    IEnumerable<TableSalesDto> TableBreakdown
);

/// <summary>
/// Hourly sales breakdown for daily reports.
/// </summary>
public record HourlySalesDto(
    int Hour,
    Money Sales,
    int TransactionCount,
    int CustomerCount
);

/// <summary>
/// Category sales breakdown for daily reports.
/// </summary>
public record CategorySalesDto(
    string CategoryName,
    Money Sales,
    int ItemCount,
    decimal PercentOfTotal
);

/// <summary>
/// Payment method breakdown for daily reports.
/// </summary>
public record PaymentMethodSalesDto(
    string PaymentMethod,
    Money Amount,
    int TransactionCount,
    decimal PercentOfTotal
);

/// <summary>
/// Table-specific sales breakdown for daily reports.
/// </summary>
public record TableSalesDto(
    int TableNumber,
    string TableType,
    Money TimeSales,
    Money ProductSales,
    Money TotalSales,
    TimeSpan OccupiedTime,
    int SessionCount
);

/// <summary>
/// Raw data for daily sales calculations.
/// </summary>
public record DailySalesData(
    DateTime Date,
    Money TotalSales,
    Money TimeBasedSales,
    Money ProductSales,
    Money TotalTax,
    Money TotalGratuity,
    int TransactionCount,
    int CustomerCount
);

/// <summary>
/// Raw data for hourly sales breakdown.
/// </summary>
public record HourlySalesData(
    DateTime Date,
    int Hour,
    Money Sales,
    int TransactionCount,
    int CustomerCount
);