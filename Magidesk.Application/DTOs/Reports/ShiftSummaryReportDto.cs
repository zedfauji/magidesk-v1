using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs.Reports;

public class ShiftSummaryReportDto
{
    public DateTime Date { get; set; }
    public string ShiftName { get; set; } = string.Empty;
    
    public List<DrawerSummaryDto> Drawers { get; set; } = new();
    
    public decimal TotalSales { get; set; }
    public decimal TotalTips { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCard { get; set; }
    public decimal TotalVariance { get; set; }

    public List<PaymentMethodSalesDto> PaymentBreakdown { get; set; } = new();
    public List<ServerSalesDto> ServerSales { get; set; } = new();
}

public class DrawerSummaryDto
{
    public Guid CashSessionId { get; set; }
    public string TerminalName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
    public decimal ExpectedCash { get; set; }
    public decimal ActualCash { get; set; }
    public decimal Difference { get; set; }
}

public class ServerSalesDto
{
    public Guid UserId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TipAmount { get; set; }
}
