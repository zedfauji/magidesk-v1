using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for updating guest count with authorization checking.
/// </summary>
public class UpdateGuestCountCommandHandler : ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult>
{
    private readonly ISessionControlService _sessionControlService;
    private readonly ITableSessionRepository _sessionRepository;

    public UpdateGuestCountCommandHandler(
        ISessionControlService sessionControlService,
        ITableSessionRepository sessionRepository)
    {
        _sessionControlService = sessionControlService ?? throw new ArgumentNullException(nameof(sessionControlService));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<UpdateGuestCountResult> HandleAsync(
        UpdateGuestCountCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (command.StaffId == null || command.StaffId == Guid.Empty)
        {
            throw new ArgumentException("Staff ID is required for authorization.", nameof(command));
        }

        if (command.NewGuestCount < 1 || command.NewGuestCount > 20)
        {
            throw new ArgumentException("Guest count must be between 1 and 20.", nameof(command));
        }

        // Get current session to capture previous guest count
        var session = await _sessionRepository.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session {command.SessionId} not found.");
        }

        var previousGuestCount = session.GuestCount;

        var result = await _sessionControlService.UpdateGuestCountAsync(
            command.SessionId, 
            command.NewGuestCount, 
            command.StaffId.GetValueOrDefault());

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to update guest count");
        }

        if (result.Data == null)
        {
            throw new InvalidOperationException("Session control result data is missing");
        }

        return new UpdateGuestCountResult(
            SessionId: result.Data.SessionId,
            PreviousGuestCount: previousGuestCount,
            NewGuestCount: command.NewGuestCount,
            CurrentCharge: result.Data.CurrentCharge.Amount,
            UpdatedAt: DateTime.UtcNow,
            StaffId: command.StaffId
        );
    }
}