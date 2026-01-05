using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetDrawerPullReportQuery.
/// </summary>
public class GetDrawerPullReportQueryHandler : IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly ISalesReportRepository _salesReportRepository;

    public GetDrawerPullReportQueryHandler(ICashSessionRepository cashSessionRepository, ISalesReportRepository salesReportRepository)
    {
        _cashSessionRepository = cashSessionRepository;
        _salesReportRepository = salesReportRepository;
    }

    public async Task<GetDrawerPullReportResult> HandleAsync(GetDrawerPullReportQuery query, CancellationToken cancellationToken = default)
    {
        var cashSession = await _cashSessionRepository.GetByIdAsync(query.CashSessionId, cancellationToken);
        if (cashSession == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Cash session {query.CashSessionId} not found.");
        }

        // Fetch Sales Data for the session duration
        var startDate = cashSession.OpenedAt;
        var endDate = cashSession.ClosedAt ?? DateTime.UtcNow;

        // TECH-U004: Fetch Sales and Tips
        // Note: Filters by null (all categories/users) to match the session. 
        // Ideally we should filter by cashSession.UserId if this is a "User" drawer pull. 
        // CashSessions are per-user/per-terminal usually. 
        // Assuming this is a USER drawer pull, we verify strictly by User.
        
        var salesSummary = await _salesReportRepository.GetSalesSummaryAsync(startDate, endDate, false, cancellationToken);
        var tipReport = await _salesReportRepository.GetTipReportAsync(startDate, endDate, cashSession.UserId, cancellationToken);

        var cashPayments = cashSession.Payments
            .Where(p => p.PaymentType == PaymentType.Cash && !p.IsVoided)
            .ToList();

        var cashReceipts = cashPayments
            .Where(p => p.TransactionType == TransactionType.Credit)
            .Sum(p => p.Amount.Amount);

        var cashRefunds = cashPayments
            .Where(p => p.TransactionType == TransactionType.Debit)
            .Sum(p => p.Amount.Amount);

        var report = new DrawerPullReportDto
        {
            CashSessionId = cashSession.Id,
            UserId = cashSession.UserId.Value,
            OpenedAt = cashSession.OpenedAt,
            ClosedAt = cashSession.ClosedAt,
            OpeningBalance = cashSession.OpeningBalance.Amount,
            ExpectedCash = cashSession.ExpectedCash.Amount,
            ActualCash = cashSession.ActualCash?.Amount,
            Difference = cashSession.Difference?.Amount,
            TotalCashReceipts = cashReceipts,
            TotalCashRefunds = cashRefunds,
            TotalPayouts = cashSession.Payouts.Sum(p => p.Amount.Amount),
            TotalCashDrops = cashSession.CashDrops.Sum(d => d.Amount.Amount),
            TotalDrawerBleeds = cashSession.DrawerBleeds.Sum(b => b.Amount.Amount),
            
            // Populated from Repositories
            NetSales = salesSummary.Totals.TotalNetSales,
            Tax = salesSummary.Totals.TotalTax,
            TotalTips = tipReport.TotalTips,

            Payouts = cashSession.Payouts.Select(MapPayoutToDto).ToList(),
            CashDrops = cashSession.CashDrops.Select(MapCashDropToDto).ToList(),
            DrawerBleeds = cashSession.DrawerBleeds.Select(MapDrawerBleedToDto).ToList(),
            CashPaymentCount = cashPayments.Count
        };

        return new GetDrawerPullReportResult
        {
            Report = report
        };
    }

    private static PayoutDto MapPayoutToDto(Domain.Entities.Payout payout)
    {
        return new PayoutDto
        {
            Id = payout.Id,
            CashSessionId = payout.CashSessionId,
            Amount = payout.Amount.Amount,
            Reason = payout.Reason,
            ProcessedBy = payout.ProcessedBy.Value,
            ProcessedAt = payout.ProcessedAt
        };
    }

    private static CashDropDto MapCashDropToDto(Domain.Entities.CashDrop cashDrop)
    {
        return new CashDropDto
        {
            Id = cashDrop.Id,
            CashSessionId = cashDrop.CashSessionId,
            Amount = cashDrop.Amount.Amount,
            Reason = cashDrop.Reason,
            ProcessedBy = cashDrop.ProcessedBy.Value,
            ProcessedAt = cashDrop.ProcessedAt
        };
    }

    private static DrawerBleedDto MapDrawerBleedToDto(Domain.Entities.DrawerBleed drawerBleed)
    {
        return new DrawerBleedDto
        {
            Id = drawerBleed.Id,
            CashSessionId = drawerBleed.CashSessionId,
            Amount = drawerBleed.Amount.Amount,
            Reason = drawerBleed.Reason,
            ProcessedBy = drawerBleed.ProcessedBy.Value,
            ProcessedAt = drawerBleed.ProcessedAt
        };
    }
}

