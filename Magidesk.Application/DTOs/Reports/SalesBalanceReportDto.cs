using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs.Reports;

public class SalesBalanceReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public SalesSummaryDto Sales { get; set; } = new();
    public PaymentSummaryDto Payments { get; set; } = new();
    
    public decimal Variance => Payments.NetCollected - Sales.TotalGrossSales;
}

public class SalesSummaryDto 
{
    public int TicketCount { get; set; }
    public decimal TotalGrossSales { get; set; }
    public decimal NetSales { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GratuityAmount { get; set; }
    public decimal ServiceChargeAmount { get; set; }
    public decimal DeliveryChargeAmount { get; set; }
}

public class PaymentSummaryDto
{
    public decimal TotalCollected { get; set; }
    public decimal TotalRefunded { get; set; }
    public decimal NetCollected => TotalCollected - TotalRefunded;
    
    public List<PaymentTypeSummaryDto> ByType { get; set; } = new();
}

public class PaymentTypeSummaryDto
{
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
}
