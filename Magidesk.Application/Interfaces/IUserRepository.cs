using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByPinAsync(string encryptedPin, CancellationToken cancellationToken = default);
}
