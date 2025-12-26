using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class SalesReportRepository : ISalesReportRepository
{
    private readonly ApplicationDbContext _context;

    public SalesReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
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
            StartDate = startDate,
            EndDate = endDate,
            Sales = salesSummary,
            Payments = paymentSummary
        };
    }
}
