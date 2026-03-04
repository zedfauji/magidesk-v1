using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        // 1. Fetch Closed Tickets in Range
        // We project to a lightweight anonymous type or fetch required fields to minimize data transfer if needed.
        // For now, fetching entities is acceptable if volume is not massive.
        
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed)
            .Select(t => new 
            {
                t.Id,
                TotalAmount = t.TotalAmount.Amount,
                SubtotalAmount = t.SubtotalAmount.Amount,
                TaxAmount = t.TaxAmount.Amount,
                DiscountAmount = t.DiscountAmount.Amount,
                ServiceChargeAmount = t.ServiceChargeAmount.Amount,
                DeliveryChargeAmount = t.DeliveryChargeAmount.Amount,
                GratuityAmount = t.Gratuity != null ? t.Gratuity.Amount.Amount : 0m
            })
            .ToListAsync(cancellationToken);

        // 2. Fetch Payments in Range
        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.TransactionTime >= startDate && p.TransactionTime <= endDate)
            .Where(p => !p.IsVoided)
            .Select(p => new
            {
                p.Amount.Amount,
                p.TransactionType,
                p.PaymentType
            })
            .ToListAsync(cancellationToken);

        // 3. Build Sales Summary
        var salesSummary = new SalesSummaryDto
        {
            TicketCount = tickets.Count,
            TotalGrossSales = tickets.Sum(t => t.TotalAmount),
            NetSales = tickets.Sum(t => t.SubtotalAmount),
            TaxAmount = tickets.Sum(t => t.TaxAmount),
            DiscountAmount = tickets.Sum(t => t.DiscountAmount),
            ServiceChargeAmount = tickets.Sum(t => t.ServiceChargeAmount),
            DeliveryChargeAmount = tickets.Sum(t => t.DeliveryChargeAmount),
            GratuityAmount = tickets.Sum(t => t.GratuityAmount)
        };
        
        // 4. Build Payment Summary
        var paymentSummary = new PaymentSummaryDto
        {
             TotalCollected = payments.Where(p => p.TransactionType == TransactionType.Credit).Sum(p => p.Amount),
             TotalRefunded = payments.Where(p => p.TransactionType == TransactionType.Debit).Sum(p => p.Amount)
        };
        
        // Group by type
        var paymentsByType = payments
            .GroupBy(p => p.PaymentType)
            .Select(g => new PaymentTypeSummaryDto
            {
                PaymentType = g.Key.ToString(),
                Count = g.Count(),
                // Net Amount for type = Credits - Debits
                Amount = g.Where(p => p.TransactionType == TransactionType.Credit).Sum(p => p.Amount) 
                         - g.Where(p => p.TransactionType == TransactionType.Debit).Sum(p => p.Amount)
            })
            .ToList();
            
        paymentSummary.ByType = paymentsByType;

        return new SalesBalanceReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate),
            Sales = salesSummary,
            Payments = paymentSummary
        };
    }
    public async Task<SalesSummaryReportDto> GetSalesSummaryAsync(DateTime startDate, DateTime endDate, bool includeGroups, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);

        var report = new SalesSummaryReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Use a projected join to avoid materializing problematic entity fields
        var allLines = await (from ol in _context.OrderLines.AsNoTracking()
                             join t in _context.Tickets.AsNoTracking() on ol.TicketId equals t.Id
                             where t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed
                             select new 
                             {
                                 ol.ItemCount,
                                 SubtotalAmount = ol.SubtotalAmount.Amount,
                                 TotalAmount = ol.TotalAmount.Amount,
                                 TaxAmount = ol.TaxAmount.Amount,
                                 CategoryName = ol.CategoryName ?? "Uncategorized",
                                 GroupName = ol.GroupName ?? "No Group"
                             })
                             .ToListAsync(cancellationToken);

        if (!allLines.Any()) return report;

        // Calculate Totals - Using SubtotalAmount as Gross based on business logic (Net = Total - Tax)
        report.Totals.TotalItemCount = allLines.Sum(l => l.ItemCount);
        report.Totals.TotalGrossSales = allLines.Sum(l => l.SubtotalAmount); 
        report.Totals.TotalNetSales = allLines.Sum(l => l.TotalAmount - l.TaxAmount);
        report.Totals.TotalTax = allLines.Sum(l => l.TaxAmount);

        // Group by Category
        var categoryGroups = allLines.GroupBy(l => l.CategoryName);

        foreach (var catGroup in categoryGroups)
        {
            var catDto = new SalesCategoryDto
            {
                Name = catGroup.Key,
                MainItemCount = catGroup.Sum(l => l.ItemCount),
                GrossSales = catGroup.Sum(l => l.SubtotalAmount),
                NetSales = catGroup.Sum(l => l.TotalAmount - l.TaxAmount),
                TaxAmount = catGroup.Sum(l => l.TaxAmount)
            };

            if (includeGroups)
            {
                var subGroups = catGroup.GroupBy(l => l.GroupName);
                foreach (var subGroup in subGroups)
                {
                    catDto.Groups.Add(new SalesGroupDto
                    {
                        Name = subGroup.Key,
                        ItemCount = subGroup.Sum(l => l.ItemCount),
                        GrossSales = subGroup.Sum(l => l.SubtotalAmount)
                    });
                }
            }

            report.Categories.Add(catDto);
        }

        return report;
    }
    public async Task<ExceptionsReportDto> GetExceptionsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new ExceptionsReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // 1. Voids
        // Fetch using projection to avoid full entity tracking
        var voids = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.Status == TicketStatus.Voided && t.ActiveDate >= startDate && t.ActiveDate <= endDate)
            .Select(t => new VoidItemDto
            {
                Date = t.ActiveDate,
                TicketNumber = t.TicketNumber,
                Amount = t.TotalAmount.Amount,
                VoidedBy = t.VoidedBy != null ? t.VoidedBy.Value.ToString() : "Unknown", // Handle nullable UserId
                Reason = "Voided" // Placeholder
            })
            .ToListAsync(cancellationToken);
        
        foreach(var v in voids) v.Date = ToSafeDisplayDate(v.Date);
        report.Voids = voids;

        // 2. Refunds
        var refunds = await _context.Payments
            .AsNoTracking()
            .Where(p => p.TransactionType == TransactionType.Debit && p.TransactionTime >= startDate && p.TransactionTime <= endDate)
            .Join(_context.Tickets, 
                  p => p.TicketId, 
                  t => t.Id, 
                  (p, t) => new { p, t.TicketNumber })
            .Select(x => new RefundItemDto
            {
                Date = x.p.TransactionTime,
                TicketNumber = x.TicketNumber,
                Amount = x.p.Amount.Amount,
                PaymentType = x.p.PaymentType.ToString(),
                Reason = x.p.Note ?? "Refund"
            })
            .ToListAsync(cancellationToken);

        foreach(var r in refunds) r.Date = ToSafeDisplayDate(r.Date);
        report.Refunds = refunds;

        // 3. Discounts
        var discounts = await _context.TicketDiscounts
            .AsNoTracking()
            .Where(d => d.AppliedAt >= startDate && d.AppliedAt <= endDate)
            .Join(_context.Tickets,
                  d => d.TicketId,
                  t => t.Id,
                  (d, t) => new { d, t.TicketNumber })
            .Select(x => new DiscountItemDto
            {
                Date = x.d.AppliedAt,
                TicketNumber = x.TicketNumber,
                Name = x.d.Name,
                Amount = x.d.Amount.Amount
            })
            .ToListAsync(cancellationToken);

        foreach(var d in discounts) d.Date = ToSafeDisplayDate(d.Date);
        report.Discounts = discounts;

        return report;
    }
    public async Task<JournalReportDto> GetJournalReportAsync(DateTime startDate, DateTime endDate, string? entityType, Guid? userId, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var query = _context.AuditEvents
            .AsNoTracking()
            .Where(e => e.Timestamp >= startDate && e.Timestamp <= endDate);

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(e => e.EntityType == entityType);
        }

        if (userId.HasValue)
        {
            query = query.Where(e => e.UserId == userId.Value);
        }

        var entries = await query
            .Join(_context.Users,
                  e => e.UserId,
                  u => u.Id,
                  (e, u) => new { Event = e, UserName = u.FirstName + " " + u.LastName })
            .OrderByDescending(x => x.Event.Timestamp)
            .Select(x => new JournalEntryDto
            {
                Timestamp = x.Event.Timestamp,
                EventType = x.Event.EventType.ToString(),
                Description = x.Event.Description,
                User = x.UserName,
                BeforeState = x.Event.BeforeState ?? string.Empty,
                AfterState = x.Event.AfterState
            })
            .ToListAsync(cancellationToken);

        foreach (var entry in entries) entry.Timestamp = ToSafeDisplayDate(entry.Timestamp);

        return new JournalReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate),
            Entries = entries
        };
    }
}
