using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for resuming a table session.
/// </summary>
public class ResumeTableSessionCommandHandler : ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult>
{
    private readonly ITableSessionRepository _sessionRepository;

    public ResumeTableSessionCommandHandler(ITableSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<ResumeTableSessionResult> HandleAsync(
        ResumeTableSessionCommand command,
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

        // Domain validation happens in Resume()
        session.Resume();

        await _sessionRepository.UpdateAsync(session);

        return new ResumeTableSessionResult(
            session.Id,
            DateTime.UtcNow,
            session.TotalPausedDuration
        );
    }
}
