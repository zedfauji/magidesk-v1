using Magidesk.Application.DTOs.Reports;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public partial class SalesReportRepository
{
    public async Task<CreditCardReportDto> GetCreditCardReportAsync(DateTime startDate, DateTime endDate, string? cardTypeFilter = null, string? transactionTypeFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new CreditCardReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for credit card payments
        var query = _context.Payments
            .OfType<CreditCardPayment>()
            .AsNoTracking()
            .Where(p => p.TransactionTime >= startDate && p.TransactionTime <= endDate)
            .Join(_context.Tickets,
                  p => p.TicketId,
                  t => t.Id,
                  (p, t) => new { p, t });

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(cardTypeFilter))
        {
            query = query.Where(x => x.p.CardType != null && x.p.CardType.Contains(cardTypeFilter));
        }

        if (!string.IsNullOrWhiteSpace(transactionTypeFilter))
        {
            query = query.Where(x => x.p.TransactionType.ToString().Contains(transactionTypeFilter));
        }

        // Execute query and project to DTO
        var transactions = await query
            .Select(x => new CreditCardTransactionDto
            {
                TransactionTime = x.p.TransactionTime,
                TicketNumber = x.t.TicketNumber.ToString(),
                CardType = x.p.CardType ?? "Unknown",
                CardLast4 = x.p.CardNumber != null && x.p.CardNumber.Length >= 4 ? "**** " + x.p.CardNumber.Substring(x.p.CardNumber.Length - 4) : "****",
                AuthorizationCode = x.p.AuthorizationCode ?? string.Empty,
                Amount = x.p.Amount.Amount,
                TipAmount = x.p.TipsAmount.Amount,
                TransactionType = x.p.TransactionType.ToString(),
                TransactionStatus = x.p.IsVoided ? "Voided" : "Approved",
                TerminalId = x.p.TerminalId.ToString(),
                MerchantId = string.Empty
            })
            .OrderByDescending(x => x.TransactionTime)
            .ToListAsync(cancellationToken);

        report.Transactions = transactions;

        // Calculate totals
        report.Totals.TotalTransactions = transactions.Count;
        report.Totals.TotalSales = transactions.Where(t => t.TransactionType == "Credit").Sum(t => t.Amount);
        report.Totals.TotalTips = transactions.Sum(t => t.TipAmount);
        report.Totals.TotalVoids = transactions.Where(t => t.TransactionStatus == "Voided").Sum(t => t.Amount);
        report.Totals.TotalRefunds = transactions.Where(t => t.TransactionType == "Debit" && t.TransactionStatus != "Voided").Sum(t => t.Amount);

        // Group by card type
        var cardTypeGroups = transactions
            .GroupBy(t => t.CardType)
            .Select(g => new CardTypeTotalDto
            {
                CardType = g.Key,
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(t => t.Amount),
                TipAmount = g.Sum(t => t.TipAmount)
            })
            .OrderByDescending(g => g.TotalAmount)
            .ToList();

        report.Totals.ByCardType = cardTypeGroups;

        return report;
    }
    private static string MaskCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
            return "****";

        // Return only last 4 digits with masking for PCI compliance
        return "**** **** **** " + cardNumber.Substring(Math.Max(0, cardNumber.Length - 4));
    }
    public async Task<PaymentReportDto> GetPaymentReportAsync(DateTime startDate, DateTime endDate, string? terminalFilter = null, CancellationToken cancellationToken = default)
    {
        startDate = ToUtc(startDate);
        endDate = ToUtc(endDate);
        var report = new PaymentReportDto
        {
            StartDate = ToSafeDisplayDate(startDate),
            EndDate = ToSafeDisplayDate(endDate)
        };

        // Build base query for all payments in date range
        var query = _context.Payments
            .AsNoTracking()
            .Where(p => p.TransactionTime >= startDate && p.TransactionTime <= endDate);

        // Apply terminal filter if provided
        if (!string.IsNullOrWhiteSpace(terminalFilter))
        {
            query = query.Where(p => p.TerminalId.ToString().Contains(terminalFilter));
        }

        // Execute query and group by payment type
        var payments = await query
            .Select(p => new
            {
                p.PaymentType,
                CardType = p is CreditCardPayment ? ((CreditCardPayment)p).CardType : 
                           p is DebitCardPayment ? ((DebitCardPayment)p).CardType : null,
                p.Amount.Amount,
                TipAmount = p.TipsAmount.Amount,
                p.TransactionType
            })
            .ToListAsync(cancellationToken);

        // Group payments by type and subtype
        var groupedPayments = payments
            .GroupBy(p => new { p.PaymentType, SubType = p.CardType ?? string.Empty })
            .Select(g => new PaymentTypeTotalDto
            {
                PaymentType = GetPaymentTypeDisplayName(g.Key.PaymentType),
                SubType = g.Key.SubType,
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(p => p.Amount),
                TipAmount = g.Sum(p => p.TipAmount)
            })
            .OrderBy(g => g.PaymentType)
            .ThenBy(g => g.SubType)
            .ToList();

        report.PaymentTypes = groupedPayments;

        // Calculate totals by payment type
        report.Totals.TotalTransactions = payments.Count;
        report.Totals.TotalCash = payments.Where(p => p.PaymentType == PaymentType.Cash).Sum(p => p.Amount);
        report.Totals.TotalCreditCards = payments.Where(p => p.PaymentType == PaymentType.CreditCard || 
                                                           p.PaymentType == PaymentType.CreditVisa || 
                                                           p.PaymentType == PaymentType.CreditMasterCard || 
                                                           p.PaymentType == PaymentType.CreditAmex || 
                                                           p.PaymentType == PaymentType.CreditDiscover).Sum(p => p.Amount);
        report.Totals.TotalDebitCards = payments.Where(p => p.PaymentType == PaymentType.DebitCard || 
                                                          p.PaymentType == PaymentType.DebitVisa || 
                                                          p.PaymentType == PaymentType.DebitMasterCard).Sum(p => p.Amount);
        report.Totals.TotalGiftCertificates = payments.Where(p => p.PaymentType == PaymentType.GiftCertificate).Sum(p => p.Amount);
        report.Totals.TotalHouseAccounts = 0; // Not supported in current model
        report.Totals.TotalChecks = 0; // Not supported in current model
        report.Totals.TotalOther = payments.Where(p => p.PaymentType == PaymentType.CustomPayment).Sum(p => p.Amount);
        report.Totals.TotalTips = payments.Sum(p => p.TipAmount);

        return report;
    }
    private static string GetPaymentTypeDisplayName(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => "Cash",
            PaymentType.CreditCard => "Credit Card",
            PaymentType.DebitCard => "Debit Card",
            PaymentType.GiftCertificate => "Gift Certificate",
            PaymentType.CustomPayment => "Custom Payment",
            _ => "Unknown"
        };
    }
}
