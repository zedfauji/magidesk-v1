using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for pausing a table session.
/// </summary>
public class PauseTableSessionCommandHandler : ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult>
{
    private readonly ITableSessionRepository _sessionRepository;

    public PauseTableSessionCommandHandler(ITableSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<PauseTableSessionResult> HandleAsync(
        PauseTableSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        var session = await _sessionRepository.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Table session {command.SessionId} not found.");
        }

        // Domain validation happens in Pause()
        session.Pause();

        await _sessionRepository.UpdateAsync(session);

        return new PauseTableSessionResult(
            session.Id,
            session.PausedAt ?? DateTime.UtcNow
        );
    }
}
