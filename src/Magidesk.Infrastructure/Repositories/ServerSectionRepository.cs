using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class ServerSectionRepository : IServerSectionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ServerSectionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // IRepository implementation

    public async Task<ServerSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServerSections
            .Include(s => s.Server)
            .Include(s => s.Tables)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ServerSection>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServerSections
            .Include(s => s.Server)
            .Include(s => s.Tables)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ServerSection entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.ServerSections.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ServerSection entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ServerSections.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ServerSection entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ServerSections.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // IServerSectionRepository implementation

    public async Task<List<ServerSection>> GetSectionsAsync(Guid? serverId = null, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ServerSections
            .Include(s => s.Server)
            .Include(s => s.Tables)
            .AsQueryable();

        if (serverId.HasValue)
        {
            query = query.Where(s => s.ServerId == serverId.Value);
        }

        if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ServerSection?> GetSectionWithTablesAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(sectionId, cancellationToken);
    }

    public async Task<List<ServerSection>> GetSectionsByTableAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServerSections
            .Include(s => s.Tables)
            .Include(s => s.Server)
            .Where(s => s.IsActive && s.Tables.Any(t => t.Id == tableId))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetServerByIdAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == serverId, cancellationToken);
    }

    public async Task<bool> IsSectionNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ServerSections.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return !await query.AnyAsync(s => s.Name.ToLower() == name.ToLower(), cancellationToken);
    }
}
