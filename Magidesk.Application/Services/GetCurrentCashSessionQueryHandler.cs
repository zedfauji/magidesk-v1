using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetCurrentCashSessionQuery.
/// </summary>
public class GetCurrentCashSessionQueryHandler : IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;

    public GetCurrentCashSessionQueryHandler(ICashSessionRepository cashSessionRepository)
    {
        _cashSessionRepository = cashSessionRepository;
    }

    public async Task<GetCurrentCashSessionResult> HandleAsync(GetCurrentCashSessionQuery query, CancellationToken cancellationToken = default)
    {
        var cashSession = await _cashSessionRepository.GetOpenSessionByUserIdAsync(query.UserId, cancellationToken);
        
        return new GetCurrentCashSessionResult
        {
            CashSession = cashSession != null ? MapToDto(cashSession) : null
        };
    }

    private static CashSessionDto MapToDto(Domain.Entities.CashSession cashSession)
    {
        var cashPayments = cashSession.Payments
            .Where(p => p.PaymentType == PaymentType.Cash && !p.IsVoided)
            .ToList();

        var cashReceipts = cashPayments
            .Where(p => p.TransactionType == TransactionType.Credit)
            .Sum(p => p.Amount.Amount);

        var cashRefunds = cashPayments
            .Where(p => p.TransactionType == TransactionType.Debit)
            .Sum(p => p.Amount.Amount);

        return new CashSessionDto
        {
            Id = cashSession.Id,
            UserId = cashSession.UserId.Value,
            TerminalId = cashSession.TerminalId,
            ShiftId = cashSession.ShiftId,
            OpenedAt = cashSession.OpenedAt,
            ClosedAt = cashSession.ClosedAt,
            ClosedBy = cashSession.ClosedBy?.Value,
            OpeningBalance = cashSession.OpeningBalance.Amount,
            ExpectedCash = cashSession.ExpectedCash.Amount,
            ActualCash = cashSession.ActualCash?.Amount,
            Difference = cashSession.Difference?.Amount,
            Status = cashSession.Status,
            Payouts = cashSession.Payouts.Select(MapPayoutToDto).ToList(),
            CashDrops = cashSession.CashDrops.Select(MapCashDropToDto).ToList(),
            DrawerBleeds = cashSession.DrawerBleeds.Select(MapDrawerBleedToDto).ToList(),
            PaymentCount = cashPayments.Count,
            TotalCashReceipts = cashReceipts,
            TotalCashRefunds = cashRefunds
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

