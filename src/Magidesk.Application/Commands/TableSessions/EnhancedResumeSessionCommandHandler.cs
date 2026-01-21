using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for enhanced resume session command with validation.
/// </summary>
public class EnhancedResumeSessionCommandHandler : ICommandHandler<EnhancedResumeSessionCommand, EnhancedResumeSessionResult>
{
    private readonly ISessionControlService _sessionControlService;

    public EnhancedResumeSessionCommandHandler(ISessionControlService sessionControlService)
    {
        _sessionControlService = sessionControlService ?? throw new ArgumentNullException(nameof(sessionControlService));
    }

    public async Task<EnhancedResumeSessionResult> HandleAsync(
        EnhancedResumeSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        var result = await _sessionControlService.ResumeSessionAsync(command.SessionId);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to resume session");
        }

        if (result.Data == null)
        {
            throw new InvalidOperationException("Session control result data is missing");
        }

        return new EnhancedResumeSessionResult(
            SessionId: result.Data.SessionId,
            ResumedAt: DateTime.UtcNow,
            TotalPausedDuration: result.Data.TotalPausedDuration,
            CurrentCharge: result.Data.CurrentCharge.Amount,
            Status: result.Data.Status.ToString()
        );
    }
}