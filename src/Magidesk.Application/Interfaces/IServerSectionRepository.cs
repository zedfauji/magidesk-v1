using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IServerSectionRepository : IRepository<ServerSection>
{
    Task<List<ServerSection>> GetSectionsAsync(Guid? serverId = null, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<ServerSection?> GetSectionWithTablesAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<List<ServerSection>> GetSectionsByTableAsync(Guid tableId, CancellationToken cancellationToken = default);
    Task<User?> GetServerByIdAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<bool> IsSectionNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
