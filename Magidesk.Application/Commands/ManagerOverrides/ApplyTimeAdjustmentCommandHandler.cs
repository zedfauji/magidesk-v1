using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands.ManagerOverrides;

/// <summary>
/// Handler for applying time adjustment overrides with authorization.
/// </summary>
public class ApplyTimeAdjustmentCommandHandler : ICommandHandler<ApplyTimeAdjustmentCommand, ApplyTimeAdjustmentResult>
{
    private readonly IManagerOverrideService _managerOverrideService;
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly Domain.Services.IPricingService _pricingService;

    public ApplyTimeAdjustmentCommandHandler(
        IManagerOverrideService managerOverrideService,
        ITableSessionRepository sessionRepository,
        ITableTypeRepository tableTypeRepository,
        Domain.Services.IPricingService pricingService)
    {
        _managerOverrideService = managerOverrideService ?? throw new ArgumentNullException(nameof(managerOverrideService));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _tableTypeRepository = tableTypeRepository ?? throw new ArgumentNullException(nameof(tableTypeRepository));
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
    }

    public async Task<ApplyTimeAdjustmentResult> HandleAsync(
        ApplyTimeAdjustmentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for time adjustment.", nameof(command));
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

        var originalBillableTime = session.GetBillableTime();
        
        // Calculate original charge
        var tableType = await _tableTypeRepository.GetByIdAsync(session.TableTypeId);
        var originalCharge = tableType != null 
            ? _pricingService.CalculateTimeCharge(originalBillableTime, tableType)
            : Money.Zero();

        // Apply the time adjustment
        var result = await _managerOverrideService.ApplyTimeAdjustmentAsync(
            command.SessionId,
            command.AdjustmentAmount,
            command.Reason,
            command.ManagerId);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to apply time adjustment");
        }

        // Get updated session data
        var updatedSession = await _sessionRepository.GetByIdAsync(command.SessionId);
        var newBillableTime = updatedSession?.GetBillableTime() ?? originalBillableTime;
        
        // Calculate new charge
        var newCharge = tableType != null 
            ? _pricingService.CalculateTimeCharge(newBillableTime, tableType)
            : Money.Zero();

        return new ApplyTimeAdjustmentResult(
            SessionId: command.SessionId,
            OriginalBillableTime: originalBillableTime,
            NewBillableTime: newBillableTime,
            AdjustmentApplied: command.AdjustmentAmount,
            OriginalCharge: originalCharge.Amount,
            NewCharge: newCharge.Amount,
            ManagerId: command.ManagerId,
            AppliedAt: DateTime.UtcNow
        );
    }
}