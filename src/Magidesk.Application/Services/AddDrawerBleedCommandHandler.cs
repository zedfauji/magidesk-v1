using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddDrawerBleedCommand.
/// </summary>
public class AddDrawerBleedCommandHandler : ICommandHandler<AddDrawerBleedCommand, AddDrawerBleedResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddDrawerBleedCommandHandler(
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository)
    {
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AddDrawerBleedResult> HandleAsync(AddDrawerBleedCommand command, CancellationToken cancellationToken = default)
    {
        // Get cash session
        var cashSession = await _cashSessionRepository.GetByIdAsync(command.CashSessionId, cancellationToken);
        if (cashSession == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Cash session {command.CashSessionId} not found.");
        }

        // Create drawer bleed
        var drawerBleed = DrawerBleed.Create(
            command.CashSessionId,
            command.Amount,
            command.ProcessedBy,
            command.Reason);

        // Add drawer bleed to session
        cashSession.AddDrawerBleed(drawerBleed);

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
                Action = "DrawerBleedAdded",
                DrawerBleedId = drawerBleed.Id,
                Amount = drawerBleed.Amount.Amount,
                Reason = drawerBleed.Reason
            }),
            $"Drawer bleed of {drawerBleed.Amount} added to cash session. Reason: {drawerBleed.Reason ?? "N/A"}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AddDrawerBleedResult
        {
            DrawerBleedId = drawerBleed.Id,
            CashSessionId = cashSession.Id,
            Amount = drawerBleed.Amount,
            UpdatedExpectedCash = cashSession.ExpectedCash
        };
    }
}

