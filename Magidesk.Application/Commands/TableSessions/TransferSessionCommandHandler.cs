using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for transferring sessions between tables with data preservation validation.
/// </summary>
public class TransferSessionCommandHandler : ICommandHandler<TransferSessionCommand, TransferSessionResult>
{
    private readonly ISessionControlService _sessionControlService;
    private readonly ITableSessionRepository _sessionRepository;

    public TransferSessionCommandHandler(
        ISessionControlService sessionControlService,
        ITableSessionRepository sessionRepository)
    {
        _sessionControlService = sessionControlService ?? throw new ArgumentNullException(nameof(sessionControlService));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<TransferSessionResult> HandleAsync(
        TransferSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (command.TargetTableId == Guid.Empty)
        {
            throw new ArgumentException("Target table ID cannot be empty.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for session transfer.", nameof(command));
        }

        if (command.StaffId == Guid.Empty)
        {
            throw new ArgumentException("Staff ID is required for authorization.", nameof(command));
        }

        // Get original session data before transfer
        var originalSession = await _sessionRepository.GetByIdAsync(command.SessionId);
        if (originalSession == null)
        {
            throw new InvalidOperationException($"Session {command.SessionId} not found.");
        }

        var originalTableId = originalSession.TableId;
        var preservedDuration = originalSession.GetBillableTime();
        var preservedCharge = originalSession.TotalCharge.Amount;

        var result = await _sessionControlService.TransferSessionAsync(
            command.SessionId, 
            command.TargetTableId, 
            command.Reason);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to transfer session");
        }

        if (result.Data == null)
        {
            throw new InvalidOperationException("Session control result data is missing");
        }

        return new TransferSessionResult(
            OriginalSessionId: command.SessionId,
            NewSessionId: result.Data.SessionId,
            OriginalTableId: originalTableId,
            NewTableId: command.TargetTableId,
            PreservedCharge: preservedCharge,
            PreservedDuration: preservedDuration,
            TransferredAt: DateTime.UtcNow
        );
    }
}