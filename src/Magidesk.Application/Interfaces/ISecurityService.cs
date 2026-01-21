using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface ISecurityService
{
    Task<bool> HasPermissionAsync(UserId userId, UserPermission permission, CancellationToken cancellationToken = default);
    Task<User?> GetUserByPinAsync(string encryptedPin, CancellationToken cancellationToken = default);
}
