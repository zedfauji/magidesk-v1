namespace Magidesk.Application.DTOs.Reports;

public class DeliveryReportDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Aggregates
    public int TotalDeliveries { get; set; }
    public decimal TotalDeliverySales { get; set; }
    public double AverageDeliveryTimeMinutes { get; set; }

    public List<DriverPerformanceDto> DriverStats { get; set; } = new();
}

public class DriverPerformanceDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int DeliveryCount { get; set; }
    public decimal TotalSales { get; set; } // Settled Amount
    public double AverageTimeMinutes { get; set; } // Dispatch to Close
    public decimal TipsAmount { get; set; }
}
