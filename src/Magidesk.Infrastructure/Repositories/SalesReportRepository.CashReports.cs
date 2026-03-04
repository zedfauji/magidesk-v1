using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<CashOutReportDto> GetCashOutReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);

        var report = new CashOutReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // 1. Fetch relevant tickets (closed in range)
        var ticketsQuery = _context.Tickets
            .AsNoTracking()
            .Include(t => t.Payments) // Include payments to calculate cash/tips
            .Include(t => t.Gratuity) // Include gratuity for auto-grats
            .Where(t => t.ClosedAt >= startDate && t.ClosedAt <= endDate && t.Status == TicketStatus.Closed);

        if (userIdFilter.HasValue)
        {
             var uid = new UserId(userIdFilter.Value);
             // Should we filter by Ticket Creator? Or Server? Often they are the same.
             // Usually reports are "By Server".
             // Ticket.CreatedBy or Ticket.ServerId (if it exists)
             // Using CreatedBy as primary server
             ticketsQuery = ticketsQuery.Where(t => t.CreatedBy == uid);
        }

        var tickets = await ticketsQuery.ToListAsync(cancellationToken);

        // 2. Aggregate per User
        // Need to group by User.
        // If filtering by user, we still group (will be 1 group).
        
        var userIds = tickets.Select(t => t.CreatedBy.Value).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FirstName + " " + u.LastName, cancellationToken);

        var reportItems = new List<CashOutReportItemDto>();

        foreach (var userId in userIds)
        {
            var userTickets = tickets.Where(t => t.CreatedBy.Value == userId).ToList();
            
            decimal cashSales = 0;
            decimal chargedTips = 0;
            
            foreach (var t in userTickets)
            {
                // A. Cash Sales: Sum of Cash Payments collected
                // IMPORTANT: Refunds (Negative) should reduce cash collected if refunded in cash
                // But usually we track "Cash in Hand".
                // If I refunded cash, I gave out cash.
                // Cash Payment (Credit) = Money In.
                // Cash Payment (Debit) = Money Out.
                // So Sum(Amount) where paymentType=Cash is correct net cash flow.
                
                var cashInHand = t.Payments
                    .Where(p => p.PaymentType == PaymentType.Cash)
                    .Sum(p => p.TransactionType == TransactionType.Credit ? p.Amount.Amount : -p.Amount.Amount);
                
                cashSales += cashInHand;

                // B. Charged Tips: Tips on Non-Cash payments
                // Also AutoGratuity on Non-Cash payments?
                // Floreant usually treats "Tips" as money owed to server.
                // If paid by Credit Card, house collected it, owes it to server.
                // If paid by Cash, server already has it (part of Gross Receipt), but usually we separate "Tip" from "Sales".
                // Let's assume Standard Model:
                // Net Due = (Cash Sales + Cash Tips) - (Credit Card Tips + Auto Grat on Card) -> Wait.
                // Simpler:
                // Server has wallet.
                // Wallet increases by Cash Payments (Sales + Cash Tips).
                // Wallet does NOT increase by CC Payments.
                // Server OWES House for Sales.
                // House OWES Server for CC Tips.
                // Net Due (Server -> House) = Cash In Wallet - Tips Earned (that house holds).
                // Cash In Wallet = Total Cash Payments.
                // Tips Earned (Held by House) = CC Tips.
                // Net Due = Cash Payments - CC Tips.
                
                var ccTips = t.Payments
                    .Where(p => p.PaymentType != PaymentType.Cash)
                    .Sum(p => p.TipsAmount.Amount);
                   
                // What about AutoGratuity?
                // If AutoGrat is on a CC ticket, it's like a Tip.
                // If AutoGrat is on a Cash ticket, it's collected in Cash.
                // Magidesk "Payments" might include tip/grat in Amount or separate?
                // Usually Payment.Amount includes Tip.
                // So Cash Payment = Subtotal + Tax + Tip.
                // So "Cash In Wallet" already includes Cash Tips.
                // "CC Tips" is just the tip part.
                // AutoGratuity logic in Magidesk might be separate entity 'Gratuity'.
                // If Gratuity exists, it is expected to be paid.
                // If Paid by Cash, it's in Cash Payment.
                // If Paid by CC, it's in CC Payment.
                // We need to know if the House owes the server the AutoGrat.
                // Yes, if collected via CC.
                // How do we know if AutoGrat was paid by CC?
                // We look at payments.
                // Proportionally? Or just sum all "Non-Cash Tips/Grat"?
                
                // Refinment: AutoGratuity is distinct from "TipsAmount" on Payment in some models.
                // In Magidesk, Ticket.Gratuity is independent.
                // Let's assume standard CC Tips for now as primary "Owed to Server".
                // If we need strict parity with Floreant's complex auto-grat, we might need deeper inspection.
                // Use Standard: Charged Tips = Sum of Tips on Non-Cash Payments.
                
                chargedTips += ccTips;
            }

            var item = new CashOutReportItemDto
            {
                UserId = userId,
                UserName = users.GetValueOrDefault(userId, "Unknown"),
                TicketCount = userTickets.Count,
                CashSales = cashSales,
                ChargedTips = chargedTips
            };
            reportItems.Add(item);
        }

        report.Items = reportItems;
        report.TotalCashSales = reportItems.Sum(i => i.CashSales);
        report.TotalChargedTips = reportItems.Sum(i => i.ChargedTips);
        report.TotalNetDue = reportItems.Sum(i => i.NetDue);

        return report;
    }
}
