using System;

namespace Magidesk.Application.DTOs.Security;

/// <summary>
/// Result of a manager authorization attempt.
/// Contains authorization status, user details, and expiration time.
/// </summary>
public record AuthorizationResult(
    bool Authorized,
    Guid? AuthorizingUserId,
    string? AuthorizingUserName,
    DateTime? ExpiresAt,  // Short-lived (5 minutes) for specific operation
    string? FailureReason
)
{
    /// <summary>
    /// Creates a successful authorization result.
    /// </summary>
    public static AuthorizationResult Success(Guid userId, string userName)
    {
        return new AuthorizationResult(
            Authorized: true,
            AuthorizingUserId: userId,
            AuthorizingUserName: userName,
            ExpiresAt: DateTime.UtcNow.AddMinutes(5),
            FailureReason: null
        );
    }

    /// <summary>
    /// Creates a failed authorization result.
    /// </summary>
    public static AuthorizationResult Failure(string reason)
    {
        return new AuthorizationResult(
            Authorized: false,
            AuthorizingUserId: null,
            AuthorizingUserName: null,
            ExpiresAt: null,
            FailureReason: reason
        );
    }
}
