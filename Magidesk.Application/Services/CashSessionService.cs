using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class CashSessionService : ICashSessionService
{
    private readonly ICashSessionRepository _cashSessionRepository;

    public CashSessionService(ICashSessionRepository cashSessionRepository)
    {
        _cashSessionRepository = cashSessionRepository;
    }

    public async Task<CashSession> StartSessionAsync(UserId userId, Guid terminalId, Guid shiftId, Money openingAmount)
    {
        // 1. One active session per user
        var userSession = await _cashSessionRepository.GetOpenSessionByUserIdAsync(userId.Value);
        if (userSession != null)
        {
            throw new BusinessRuleViolationException($"User already has an open cash session on terminal {userSession.TerminalId}.");
        }

        // 2. One active session per terminal (!)
        // Note: F-0060 specifies "Exclusive drawer locking". 
        // If a terminal is "locked" by a user, no one else can open a drawer there.
        var terminalSession = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
        if (terminalSession != null)
        {
             throw new BusinessRuleViolationException($"Terminal {terminalId} already has an active cash session by user {terminalSession.UserId.Value}.");
        }

        // 3. Create Session
        var session = CashSession.Open(userId, terminalId, shiftId, openingAmount);

        // 4. Persist
        await _cashSessionRepository.AddAsync(session);
        return session;
    }

    public async Task<CashSession> CloseSessionAsync(Guid sessionId, UserId closedBy, Money closingAmount)
    {
        var session = await _cashSessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new Exception($"Cash Session not found: {sessionId}"); // Use NotFound
        }

        session.Close(closedBy, closingAmount);

        await _cashSessionRepository.UpdateAsync(session);
        return session;
    }

    public async Task<CashSession?> GetActiveSessionByUserAsync(UserId userId)
    {
        return await _cashSessionRepository.GetOpenSessionByUserIdAsync(userId.Value);
    }

    public async Task<CashSession?> GetActiveSessionByTerminalAsync(Guid terminalId)
    {
        return await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
    }
}
