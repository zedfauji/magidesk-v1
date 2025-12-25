using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for OpenCashSessionCommand.
/// </summary>
public class OpenCashSessionCommandHandler : ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult>
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.CashSessionDomainService _cashSessionDomainService;

    public OpenCashSessionCommandHandler(
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.CashSessionDomainService cashSessionDomainService)
    {
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
        _cashSessionDomainService = cashSessionDomainService;
    }

    public async Task<OpenCashSessionResult> HandleAsync(OpenCashSessionCommand command, CancellationToken cancellationToken = default)
    {
        // Check if user already has an open session
        var existingSession = await _cashSessionRepository.GetOpenSessionByUserIdAsync(
            command.UserId.Value,
            cancellationToken);

        if (existingSession != null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                $"User {command.UserId.Value} already has an open cash session ({existingSession.Id}).");
        }

        // Create new cash session
        var cashSession = CashSession.Open(
            command.UserId,
            command.TerminalId,
            command.ShiftId,
            command.OpeningBalance);

        // Save cash session
        await _cashSessionRepository.AddAsync(cashSession, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Created,
            nameof(CashSession),
            cashSession.Id,
            command.UserId.Value,
            System.Text.Json.JsonSerializer.Serialize(new
            {
                TerminalId = command.TerminalId,
                ShiftId = command.ShiftId,
                OpeningBalance = command.OpeningBalance.Amount
            }),
            $"Cash session opened with opening balance {command.OpeningBalance}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new OpenCashSessionResult
        {
            CashSessionId = cashSession.Id,
            OpenedAt = cashSession.OpenedAt
        };
    }
}

