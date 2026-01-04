using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CloseCashSessionCommand.
/// </summary>
public class CloseCashSessionCommandHandler : ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly Domain.DomainServices.CashSessionDomainService _cashSessionDomainService;

    public CloseCashSessionCommandHandler(
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository,
        ITicketRepository ticketRepository,
        Domain.DomainServices.CashSessionDomainService cashSessionDomainService)
    {
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
        _ticketRepository = ticketRepository;
        _cashSessionDomainService = cashSessionDomainService;
    }

    public async Task<CloseCashSessionResult> HandleAsync(CloseCashSessionCommand command, CancellationToken cancellationToken = default)
    {
        // Get cash session
        var cashSession = await _cashSessionRepository.GetByIdAsync(command.CashSessionId, cancellationToken);
        if (cashSession == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Cash session {command.CashSessionId} not found.");
        }

        // Validate can close (internal state)
        if (!_cashSessionDomainService.CanCloseSession(cashSession))
        {
            throw new Domain.Exceptions.InvalidOperationException($"Cash session {command.CashSessionId} cannot be closed.");
        }

        // Validate External Dependencies (Settlement Enforcement)
        var openTickets = await _ticketRepository.GetOpenTicketsAsync(cancellationToken);
        if (openTickets.Any())
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                $"Cannot close shift: {openTickets.Count()} tickets are still OPEN or UNPAID. verify all tables are settled.");
        }

        // Close cash session
        cashSession.Close(command.ClosedBy, command.ActualCash);

        // Update cash session
        await _cashSessionRepository.UpdateAsync(cashSession, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.StatusChanged,
            nameof(CashSession),
            cashSession.Id,
            command.ClosedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new
            {
                Status = cashSession.Status,
                ExpectedCash = cashSession.ExpectedCash.Amount,
                ActualCash = cashSession.ActualCash!.Amount,
                Difference = cashSession.Difference!.Amount
            }),
            $"Cash session closed. Expected: {cashSession.ExpectedCash}, Actual: {cashSession.ActualCash}, Difference: {cashSession.Difference}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new CloseCashSessionResult
        {
            CashSessionId = cashSession.Id,
            ExpectedCash = cashSession.ExpectedCash,
            ActualCash = cashSession.ActualCash!,
            Difference = cashSession.Difference!,
            ClosedAt = cashSession.ClosedAt!.Value
        };
    }
}

