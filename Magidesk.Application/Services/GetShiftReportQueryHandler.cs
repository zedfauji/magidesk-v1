using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetShiftReportQuery.
/// </summary>
public class GetShiftReportQueryHandler : IQueryHandler<GetShiftReportQuery, GetShiftReportResult>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ICashSessionRepository _cashSessionRepository;

    public GetShiftReportQueryHandler(
        IShiftRepository shiftRepository,
        ITicketRepository ticketRepository,
        ICashSessionRepository cashSessionRepository)
    {
        _shiftRepository = shiftRepository;
        _ticketRepository = ticketRepository;
        _cashSessionRepository = cashSessionRepository;
    }

    public async Task<GetShiftReportResult> HandleAsync(GetShiftReportQuery query, CancellationToken cancellationToken = default)
    {
        // Get shift
        var shift = await _shiftRepository.GetByIdAsync(query.ShiftId, cancellationToken);
        if (shift == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Shift {query.ShiftId} not found.");
        }

        // Get tickets for this shift
        var tickets = await _ticketRepository.GetByShiftIdAsync(query.ShiftId, cancellationToken);
        
        // Filter by date range if provided
        if (query.StartDate.HasValue)
        {
            tickets = tickets.Where(t => t.CreatedAt >= query.StartDate.Value);
        }
        if (query.EndDate.HasValue)
        {
            tickets = tickets.Where(t => t.CreatedAt <= query.EndDate.Value);
        }

        var ticketList = tickets.ToList();
        var closedTickets = ticketList.Where(t => t.Status == TicketStatus.Closed).ToList();

        // Get cash sessions for this shift
        var cashSessions = await _cashSessionRepository.GetByShiftIdAsync(query.ShiftId, cancellationToken);
        var cashSessionList = cashSessions.ToList();

        // Calculate totals
        var totalSales = ticketList.Sum(t => t.TotalAmount.Amount);
        var totalCashSales = ticketList
            .SelectMany(t => t.Payments)
            .Where(p => p.PaymentType == PaymentType.Cash && !p.IsVoided && p.TransactionType == TransactionType.Credit)
            .Sum(p => p.Amount.Amount);
        var totalCardSales = ticketList
            .SelectMany(t => t.Payments)
            .Where(p => (p.PaymentType == PaymentType.CreditCard || p.PaymentType == PaymentType.DebitCard) && !p.IsVoided)
            .Sum(p => p.Amount.Amount);

        var totalCashReceipts = cashSessionList
            .SelectMany(cs => cs.Payments)
            .Where(p => p.PaymentType == PaymentType.Cash && !p.IsVoided && p.TransactionType == TransactionType.Credit)
            .Sum(p => p.Amount.Amount);
        var totalCashRefunds = cashSessionList
            .SelectMany(cs => cs.Payments)
            .Where(p => p.PaymentType == PaymentType.Cash && !p.IsVoided && p.TransactionType == TransactionType.Debit)
            .Sum(p => p.Amount.Amount);

        // Map tickets to DTOs (simplified - would need full mapping in real implementation)
        var ticketDtos = ticketList.Select(t => new TicketDto
        {
            Id = t.Id,
            TicketNumber = t.TicketNumber,
            Status = t.Status,
            TotalAmount = t.TotalAmount.Amount,
            CreatedAt = t.CreatedAt,
            ClosedAt = t.ClosedAt
        }).ToList();

        // Map cash sessions to DTOs (simplified)
        var cashSessionDtos = cashSessionList.Select(cs => new CashSessionDto
        {
            Id = cs.Id,
            UserId = cs.UserId.Value,
            OpenedAt = cs.OpenedAt,
            ClosedAt = cs.ClosedAt,
            ExpectedCash = cs.ExpectedCash.Amount,
            ActualCash = cs.ActualCash?.Amount,
            Status = cs.Status
        }).ToList();

        var report = new ShiftReportDto
        {
            ShiftId = shift.Id,
            ShiftName = shift.Name,
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TicketCount = ticketList.Count,
            ClosedTicketCount = closedTickets.Count,
            TotalSales = totalSales,
            TotalCashSales = totalCashSales,
            TotalCardSales = totalCardSales,
            CashSessionCount = cashSessionList.Count,
            TotalCashReceipts = totalCashReceipts,
            TotalCashRefunds = totalCashRefunds,
            Tickets = ticketDtos,
            CashSessions = cashSessionDtos
        };

        return new GetShiftReportResult
        {
            Report = report
        };
    }
}

