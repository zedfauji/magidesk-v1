using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;

namespace Magidesk.Application.Commands.ManagerOverrides;

/// <summary>
/// Handler for forcing session end in emergency situations.
/// </summary>
public class ForceEndSessionCommandHandler : ICommandHandler<ForceEndSessionCommand, ForceEndSessionResult>
{
    private readonly IManagerOverrideService _managerOverrideService;
    private readonly ITableSessionRepository _sessionRepository;

    public ForceEndSessionCommandHandler(
        IManagerOverrideService managerOverrideService,
        ITableSessionRepository sessionRepository)
    {
        _managerOverrideService = managerOverrideService ?? throw new ArgumentNullException(nameof(managerOverrideService));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<ForceEndSessionResult> HandleAsync(
        ForceEndSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for force ending session.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.ManagerPin))
        {
            throw new ArgumentException("Manager PIN is required.", nameof(command));
        }

        if (command.ManagerId == Guid.Empty)
        {
            throw new ArgumentException("Manager ID cannot be empty.", nameof(command));
        }

        // Validate manager authorization
        var authResult = await _managerOverrideService.ValidateManagerAuthorizationAsync(
            command.ManagerPin, 
            command.ManagerId);

        if (!authResult.IsSuccessful)
        {
            throw new UnauthorizedAccessException(authResult.ErrorMessage ?? "Manager authorization failed");
        }

        // Get original session data
        var session = await _sessionRepository.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session {command.SessionId} not found.");
        }

        var originalStatus = session.Status.ToString();
        var totalDuration = session.GetBillableTime();
        var finalCharge = session.TotalCharge.Amount;

        // Force end the session
        var result = await _managerOverrideService.ForceEndSessionAsync(
            command.SessionId,
            command.Reason,
            command.ManagerId);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to force end session");
        }

        return new ForceEndSessionResult(
            SessionId: command.SessionId,
            OriginalStatus: originalStatus,
            FinalCharge: finalCharge,
            TotalDuration: totalDuration,
            Reason: command.Reason,
            ManagerId: command.ManagerId,
            EndedAt: DateTime.UtcNow
        );
    }
}