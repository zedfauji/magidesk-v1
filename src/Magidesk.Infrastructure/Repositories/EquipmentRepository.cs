using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Equipment entity.
/// </summary>
public class EquipmentRepository : EfRepository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentByTableIdAsync(Guid tableId)
    {
        return await _dbContext.Set<Equipment>()
            .Where(e => e.AssignedTableId == tableId && e.IsActive)
            .OrderBy(e => e.Type)
            .ThenBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentByTypeAsync(EquipmentType equipmentType)
    {
        return await _dbContext.Set<Equipment>()
            .Where(e => e.Type == equipmentType && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentByStatusAsync(EquipmentStatus status)
    {
        return await _dbContext.Set<Equipment>()
            .Where(e => e.Status == status && e.IsActive)
            .OrderBy(e => e.Type)
            .ThenBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetAvailableEquipmentByTypeAsync(EquipmentType equipmentType)
    {
        return await _dbContext.Set<Equipment>()
            .Where(e => e.Type == equipmentType && 
                       e.Status == EquipmentStatus.Available && 
                       e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentRequiringMaintenanceAsync(int daysAhead = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
        
        return await _dbContext.Set<Equipment>()
            .Where(e => e.IsActive && 
                       (e.Status == EquipmentStatus.MaintenanceRequired ||
                        (e.NextMaintenanceDate.HasValue && e.NextMaintenanceDate <= cutoffDate)))
            .OrderBy(e => e.NextMaintenanceDate)
            .ThenBy(e => e.Type)
            .ThenBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipment>> GetActiveEquipmentAsync()
    {
        return await _dbContext.Set<Equipment>()
            .Where(e => e.IsActive)
            .OrderBy(e => e.Type)
            .ThenBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<bool> IsEquipmentAvailableAsync(Guid equipmentId)
    {
        var equipment = await _dbContext.Set<Equipment>()
            .FirstOrDefaultAsync(e => e.Id == equipmentId);

        return equipment != null && 
               equipment.IsActive && 
               equipment.Status == EquipmentStatus.Available;
    }

    public async Task<IEnumerable<EquipmentUtilizationData>> GetEquipmentUtilizationAsync(DateTime fromDate, DateTime toDate)
    {
        // This would typically involve joining with session data to calculate utilization
        // For now, we'll return basic equipment data with placeholder calculations
        var equipment = await _dbContext.Set<Equipment>()
            .Where(e => e.IsActive)
            .ToListAsync();

        // In a real implementation, this would join with TableSessions and ServerAssignments
        // to calculate actual utilization metrics
        return equipment.Select(e => new EquipmentUtilizationData(
            e.Id,
            e.Name,
            e.Type,
            TimeSpan.Zero, // Would be calculated from actual usage data
            0, // Would be calculated from assignment history
            0.0m, // Would be calculated based on usage vs. available time
            e.UpdatedAt // Would be last actual usage time
        ));
    }

    public override async Task<Equipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Equipment>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Equipment>()
            .Where(e => e.IsActive)
            .OrderBy(e => e.Type)
            .ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }
}