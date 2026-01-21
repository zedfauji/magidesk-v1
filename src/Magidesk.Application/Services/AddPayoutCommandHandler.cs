using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddPayoutCommand.
/// </summary>
public class AddPayoutCommandHandler : ICommandHandler<AddPayoutCommand, AddPayoutResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddPayoutCommandHandler(
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository)
    {
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AddPayoutResult> HandleAsync(AddPayoutCommand command, CancellationToken cancellationToken = default)
    {
        // Get cash session
        var cashSession = await _cashSessionRepository.GetByIdAsync(command.CashSessionId, cancellationToken);
        if (cashSession == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Cash session {command.CashSessionId} not found.");
        }

        // Create payout
        var payout = Payout.Create(
            command.CashSessionId,
            command.Amount,
            command.ProcessedBy,
            command.Reason);

        // Add payout to session
        cashSession.AddPayout(payout);

        // Update cash session
        await _cashSessionRepository.UpdateAsync(cashSession, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(CashSession),
            cashSession.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new
            {
                Action = "PayoutAdded",
                PayoutId = payout.Id,
                Amount = payout.Amount.Amount,
                Reason = payout.Reason
            }),
            $"Payout of {payout.Amount} added to cash session. Reason: {payout.Reason ?? "N/A"}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AddPayoutResult
        {
            PayoutId = payout.Id,
            CashSessionId = cashSession.Id,
            Amount = payout.Amount,
            UpdatedExpectedCash = cashSession.ExpectedCash
        };
    }
}

