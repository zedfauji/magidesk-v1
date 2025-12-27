using Magidesk.Application.DTOs;

namespace Magidesk.Application.DTOs.Reports;

public class SalesSummaryReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<SalesCategoryDto> Categories { get; set; } = new();
    public SalesTotalsDto Totals { get; set; } = new();
}

public class SalesCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int MainItemCount { get; set; }
    public decimal GrossSales { get; set; } // Subtotal w/ modifiers + Tax? Or just Price?
                                           // Typically "Gross Sales" = Subtotal (Item + Mods)
    public decimal NetSales { get; set; }  // Subtotal - Discount
    public decimal TaxAmount { get; set; }
    public List<SalesGroupDto> Groups { get; set; } = new();
}

public class SalesGroupDto
{
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal GrossSales { get; set; }
}

public class SalesTotalsDto
{
    public int TotalItemCount { get; set; }
    public decimal TotalGrossSales { get; set; }
    public decimal TotalNetSales { get; set; }
    public decimal TotalTax { get; set; }
}
