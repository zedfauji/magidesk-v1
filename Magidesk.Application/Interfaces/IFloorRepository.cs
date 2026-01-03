using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IFloorRepository : IRepository<Floor>
{
    Task<Floor?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
