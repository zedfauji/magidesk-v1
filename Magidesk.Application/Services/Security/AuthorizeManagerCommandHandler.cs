using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.Security;
using Magidesk.Application.DTOs.Security;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services.Security;

/// <summary>
/// Handles manager authorization requests for privileged operations.
/// Validates PIN, checks manager-level permissions, and logs all attempts.
/// </summary>
public class AuthorizeManagerCommandHandler : ICommandHandler<AuthorizeManagerCommand, DTOs.Security.AuthorizationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly ILogger<AuthorizeManagerCommandHandler> _logger;

    // Manager-level permissions required for authorization
    private static readonly UserPermission[] ManagerPermissions = new[]
    {
        UserPermission.VoidTicket,
        UserPermission.RefundPayment,
        UserPermission.ApplyDiscount,
        UserPermission.OpenDrawer,
        UserPermission.CloseBatch,
        UserPermission.ManageUsers,
        UserPermission.ManageTableLayout,
        UserPermission.ManageMenu,
        UserPermission.ViewReports,
        UserPermission.SystemConfiguration,
        UserPermission.RefundTicket
    };

    public AuthorizeManagerCommandHandler(
        IUserRepository userRepository,
        IAesEncryptionService encryptionService,
        ILogger<AuthorizeManagerCommandHandler> logger)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task<DTOs.Security.AuthorizationResult> HandleAsync(
        AuthorizeManagerCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(command.Pin))
            {
                _logger.LogWarning("Authorization attempt with empty PIN for operation: {OperationType}", command.OperationType);
                return DTOs.Security.AuthorizationResult.Failure("PIN is required.");
            }

            // Encrypt PIN for lookup
            var encryptedPin = _encryptionService.Encrypt(command.Pin);

            // Validate PIN and get user
            var user = await _userRepository.GetByPinAsync(encryptedPin, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Authorization failed: Invalid PIN for operation: {OperationType}", command.OperationType);
                return DTOs.Security.AuthorizationResult.Failure("Invalid PIN.");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Authorization failed: Inactive user {UserId} attempted operation: {OperationType}", 
                    user.Id, command.OperationType);
                return DTOs.Security.AuthorizationResult.Failure("User account is inactive.");
            }

            // Load role to check permissions
            if (user.Role == null)
            {
                _logger.LogError("Authorization failed: User {UserId} has no role assigned for operation: {OperationType}", 
                    user.Id, command.OperationType);
                return DTOs.Security.AuthorizationResult.Failure("User has no role assigned.");
            }

            // Check if user has any manager-level permissions
            var hasManagerPermission = false;
            foreach (var permission in ManagerPermissions)
            {
                if (user.Role.Permissions.HasFlag(permission))
                {
                    hasManagerPermission = true;
                    break;
                }
            }

            if (!hasManagerPermission)
            {
                _logger.LogWarning("Authorization failed: User {UserId} ({UserName}) lacks manager permissions for operation: {OperationType}", 
                    user.Id, user.Username, command.OperationType);
                return DTOs.Security.AuthorizationResult.Failure("Insufficient permissions. Manager authorization required.");
            }

            // Success - log and return authorization
            _logger.LogInformation("Authorization successful: User {UserId} ({UserName}) authorized operation: {OperationType}", 
                user.Id, user.Username, command.OperationType);

            return DTOs.Security.AuthorizationResult.Success(user.Id, $"{user.FirstName} {user.LastName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authorization error for operation: {OperationType}", command.OperationType);
            return DTOs.Security.AuthorizationResult.Failure($"System error: {ex.Message}");
        }
    }
}
