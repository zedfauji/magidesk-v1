using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class SalesDetailReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<SalesDetailItemDto> Items { get; set; } = new();
    public SalesDetailTotalsDto Totals { get; set; } = new();
}

public class SalesDetailItemDto
{
    public DateTime TicketTime { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class SalesDetailTotalsDto
{
    public int TotalItems { get; set; }
    public decimal TotalGrossSales { get; set; }
    public decimal TotalDiscounts { get; set; }
    public decimal TotalNetSales { get; set; }
    public decimal TotalTax { get; set; }
}
