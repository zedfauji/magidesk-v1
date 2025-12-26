using System;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface ICashSessionService
{
    /// <summary>
    /// Starts a new cash session for a user on a terminal.
    /// Enforces: One open session per user, One open session per terminal.
    /// </summary>
    Task<CashSession> StartSessionAsync(UserId userId, Guid terminalId, Guid shiftId, Money openingAmount);

    /// <summary>
    /// Closes an active cash session.
    /// </summary>
    Task<CashSession> CloseSessionAsync(Guid sessionId, UserId closedBy, Money closingAmount);

    /// <summary>
    /// Gets the active session for a specific user.
    /// </summary>
    Task<CashSession?> GetActiveSessionByUserAsync(UserId userId);

    /// <summary>
    /// Gets the active session for a specific terminal.
    /// </summary>
    Task<CashSession?> GetActiveSessionByTerminalAsync(Guid terminalId);
}
