using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands.ManagerOverrides;

/// <summary>
/// Handler for applying pricing overrides with reason code requirements.
/// </summary>
public class ApplyPricingOverrideCommandHandler : ICommandHandler<ApplyPricingOverrideCommand, ApplyPricingOverrideResult>
{
    private readonly IManagerOverrideService _managerOverrideService;
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly Domain.Services.IPricingService _pricingService;

    public ApplyPricingOverrideCommandHandler(
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

    public async Task<ApplyPricingOverrideResult> HandleAsync(
        ApplyPricingOverrideCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (command.OverrideAmount < 0)
        {
            throw new ArgumentException("Override amount cannot be negative.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for pricing override.", nameof(command));
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

        // Calculate original charge
        var tableType = await _tableTypeRepository.GetByIdAsync(session.TableTypeId);
        var originalCharge = tableType != null 
            ? _pricingService.CalculateTimeCharge(session.GetBillableTime(), tableType)
            : Money.Zero();

        // Apply the pricing override
        var overrideAmount = new Money(command.OverrideAmount);
        var result = await _managerOverrideService.ApplyPricingOverrideAsync(
            command.SessionId,
            overrideAmount,
            command.Reason,
            command.ManagerId);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to apply pricing override");
        }

        return new ApplyPricingOverrideResult(
            SessionId: command.SessionId,
            OriginalCharge: originalCharge.Amount,
            NewCharge: command.OverrideAmount,
            OverrideAmount: command.OverrideAmount,
            Reason: command.Reason,
            ManagerId: command.ManagerId,
            AppliedAt: DateTime.UtcNow
        );
    }
}