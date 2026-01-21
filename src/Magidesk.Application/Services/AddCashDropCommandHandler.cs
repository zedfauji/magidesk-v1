using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddCashDropCommand.
/// </summary>
public class AddCashDropCommandHandler : ICommandHandler<AddCashDropCommand, AddCashDropResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddCashDropCommandHandler(
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository)
    {
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AddCashDropResult> HandleAsync(AddCashDropCommand command, CancellationToken cancellationToken = default)
    {
        // Get cash session
        var cashSession = await _cashSessionRepository.GetByIdAsync(command.CashSessionId, cancellationToken);
        if (cashSession == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Cash session {command.CashSessionId} not found.");
        }

        // Create cash drop
        var cashDrop = CashDrop.Create(
            command.CashSessionId,
            command.Amount,
            command.ProcessedBy,
            command.Reason);

        // Add cash drop to session
        cashSession.AddCashDrop(cashDrop);

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
                Action = "CashDropAdded",
                CashDropId = cashDrop.Id,
                Amount = cashDrop.Amount.Amount,
                Reason = cashDrop.Reason
            }),
            $"Cash drop of {cashDrop.Amount} added to cash session. Reason: {cashDrop.Reason ?? "N/A"}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AddCashDropResult
        {
            CashDropId = cashDrop.Id,
            CashSessionId = cashSession.Id,
            Amount = cashDrop.Amount,
            UpdatedExpectedCash = cashSession.ExpectedCash
        };
    }
}

