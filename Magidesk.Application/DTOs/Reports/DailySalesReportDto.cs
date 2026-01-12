using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs.Reports;

public class DailySalesReportDto
{
    public DateTime Date { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalGratuity { get; set; }
    public int TotalTransactions { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalTimeSales { get; set; }
    public decimal TotalProductSales { get; set; }

    public List<HourlySalesDto> HourlyBreakdown { get; set; } = new();
    public List<CategorySalesDto> CategoryBreakdown { get; set; } = new();
    public List<PaymentMethodSalesDto> PaymentBreakdown { get; set; } = new();
    public List<TableSalesDto> TableBreakdown { get; set; } = new();
}

public class HourlySalesDto
{
    public int Hour { get; set; }
    public decimal Sales { get; set; }
    public int TransactionCount { get; set; }
}

public class CategorySalesDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int ItemCount { get; set; }
}

public class PaymentMethodSalesDto
{
    public string MethodName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
}

public class TableSalesDto
{
    public string TableName { get; set; } = string.Empty;
    public decimal TimeRevenue { get; set; }
    public TimeSpan Duration { get; set; }
}
