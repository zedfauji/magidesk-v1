using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;

using Magidesk.Application.Interfaces;

namespace Magidesk.Infrastructure.Security;

public class SecurityService : ISecurityService
{
    private readonly ApplicationDbContext _dbContext;

    public SecurityService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasPermissionAsync(UserId userId, UserPermission permission, CancellationToken cancellationToken = default)
    {
        // Optimization: Could use cache here
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user == null || !user.IsActive)
            return false;

        var role = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == user.RoleId, cancellationToken);

        if (role == null)
            return false;

        return role.Permissions.HasFlag(permission);
    }

    public async Task<User?> GetUserByPinAsync(string encryptedPin, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EncryptedPin == encryptedPin && u.IsActive, cancellationToken);
    }
}
