using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for enhanced pause session command with validation and reason tracking.
/// </summary>
public class EnhancedPauseSessionCommandHandler : ICommandHandler<EnhancedPauseSessionCommand, EnhancedPauseSessionResult>
{
    private readonly ISessionControlService _sessionControlService;

    public EnhancedPauseSessionCommandHandler(ISessionControlService sessionControlService)
    {
        _sessionControlService = sessionControlService ?? throw new ArgumentNullException(nameof(sessionControlService));
    }

    public async Task<EnhancedPauseSessionResult> HandleAsync(
        EnhancedPauseSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for pausing session.", nameof(command));
        }

        var result = await _sessionControlService.PauseSessionAsync(command.SessionId, command.Reason);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to pause session");
        }

        if (result.Data == null)
        {
            throw new InvalidOperationException("Session control result data is missing");
        }

        return new EnhancedPauseSessionResult(
            SessionId: result.Data.SessionId,
            PausedAt: result.Data.PausedAt ?? DateTime.UtcNow,
            TotalPausedDuration: result.Data.TotalPausedDuration,
            CurrentCharge: result.Data.CurrentCharge.Amount,
            Status: result.Data.Status.ToString()
        );
    }
}