using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs.Printing;

public class TicketPrintModel
{
    public string TicketNumber { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    
    // Financials (Formatted)
    public string Subtotal { get; set; } = string.Empty;
    public string Tax { get; set; } = string.Empty;
    public string Total { get; set; } = string.Empty;
    public string BalanceDue { get; set; } = string.Empty;

    // Items
    public List<OrderLinePrintModel> Lines { get; set; } = new();

    // Payments
    public List<PaymentPrintModel> Payments { get; set; } = new();

    // Business Info
    public RestaurantPrintModel Restaurant { get; set; } = new();

    // Flags
    public bool IsRefund { get; set; }
    public bool IsReprint { get; set; }
}

public class OrderLinePrintModel
{
    public decimal Quantity { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Total { get; set; } = string.Empty;
    public List<string> Modifiers { get; set; } = new();
    public string Instructions { get; set; } = string.Empty;
}

public class PaymentPrintModel
{
    public string Type { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
}

public class RestaurantPrintModel
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
