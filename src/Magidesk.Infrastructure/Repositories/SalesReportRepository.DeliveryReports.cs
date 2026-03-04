using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<DeliveryReportDto> GetDeliveryReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        // 1. Fetch Closed or Paid Tickets in Range that are Type "Delivery" (or just by AssginedDriverId?)
        // Better: Fetch ticket where AssignedDriverId is not null or Order Type is Delivery.
        // Let's filter by range first.
        
        var query = _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed && t.AssignedDriverId != null);
            // We use AssignedDriverId != null as primary signal for "Delivery that involves a driver".
            // If we filter by OrderType Name containing 'Delivery', we skip orders that might have been delivered but type wasn't exact match?
            // "AssignedDriverId" is the strongest signal for "Driver Performance".
            
        var tickets = await query
            .Select(t => new 
            {
                t.Id,
                t.AssignedDriverId,
                Subtotal = t.SubtotalAmount.Amount,
                Tax = t.TaxAmount.Amount,
                Total = t.TotalAmount.Amount,
                t.ClosedAt,
                t.DispatchedTime, // Needed for Time Calculation
                t.DeliveryAddress,
                t.Properties // Check for gratuity? Or use Gratuity entity?
                // Gratuity is navigation property.
            })
            .ToListAsync(cancellationToken);

        // Fetch Driver Names
        var driverIds = tickets.Select(t => t.AssignedDriverId).Distinct().Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var drivers = await _context.Users
            .AsNoTracking()
            .Where(u => driverIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username, cancellationToken); // Or FirstName

        // Fetch Tips Separately (if gratuity is not easily selected in anonymous projection without Include)
        // Or we can load tips by TicketIds.
        var ticketIds = tickets.Select(t => t.Id).ToList();
        var tips = await _context.Gratuities
            .AsNoTracking()
            .Where(g => ticketIds.Contains(g.TicketId))
            .Select(g => new { g.TicketId, Amount = g.Amount.Amount })
            .ToDictionaryAsync(g => g.TicketId, g => g.Amount, cancellationToken);

        var report = new DeliveryReportDto
        {
            PeriodStart = ToSafeDisplayDate(startDate),
            PeriodEnd = ToSafeDisplayDate(endDate)
        };

        // Aggregation
        var driverStats = tickets
            .GroupBy(t => t.AssignedDriverId)
            .Select(g => 
            {
                var driverId = g.Key!.Value;
                var driverName = drivers.ContainsKey(driverId) ? drivers[driverId] : "Unknown";
                
                var count = g.Count();
                var totalSales = g.Sum(x => x.Total); // Use TotalAmount (Gross) for settlement/cashiering, or Subtotal for performance?
                // Usually Driver "Sales" refers to money handled or value delivered. Let's use TotalAmount.
                
                // Tips
                var driverTips = g.Sum(x => tips.ContainsKey(x.Id) ? tips[x.Id] : 0m);

                // Time (Close - Dispatch)
                // DispatchedTime might be null if not tracked properly? Default to OpenedAt? 
                // Let's protect against null DispatchedTime.
                var timeSum = g.Sum(x => 
                {
                   if (x.DispatchedTime.HasValue && x.ClosedAt.HasValue)
                   {
                       return (x.ClosedAt.Value - x.DispatchedTime.Value).TotalMinutes;
                   }
                   return 0;
                });
                
                var validTimeCount = g.Count(x => x.DispatchedTime.HasValue && x.ClosedAt.HasValue);
                var avgTime = validTimeCount > 0 ? timeSum / validTimeCount : 0;

                return new DriverPerformanceDto
                {
                    DriverId = driverId,
                    DriverName = driverName,
                    DeliveryCount = count,
                    TotalSales = totalSales,
                    TipsAmount = driverTips,
                    AverageTimeMinutes = Math.Round(avgTime, 1)
                };
            })
            .OrderByDescending(d => d.DeliveryCount)
            .ToList();

        report.DriverStats = driverStats;
        report.TotalDeliveries = driverStats.Sum(d => d.DeliveryCount);
        report.TotalDeliverySales = driverStats.Sum(d => d.TotalSales);
        
        if (report.TotalDeliveries > 0)
        {
             // Weighted average for overall time? Or simple average of averages?
             // Simple average of averages is incorrect.
             // Re-calculate global average.
             var globalTimeSum = tickets
                 .Where(x => x.DispatchedTime.HasValue && x.ClosedAt.HasValue)
                 .Sum(x => (x.ClosedAt!.Value - x.DispatchedTime!.Value).TotalMinutes);
                 
             var globalValidCount = tickets.Count(x => x.DispatchedTime.HasValue && x.ClosedAt.HasValue);
             report.AverageDeliveryTimeMinutes = globalValidCount > 0 ? Math.Round(globalTimeSum / globalValidCount, 1) : 0;
        }

        return report;
    }
}
