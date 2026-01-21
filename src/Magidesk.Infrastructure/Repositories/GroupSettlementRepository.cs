using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class GroupSettlementRepository : IGroupSettlementRepository
{
    private readonly ApplicationDbContext _context;

    public GroupSettlementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(GroupSettlement groupSettlement)
    {
        await _context.GroupSettlements.AddAsync(groupSettlement);
        await _context.SaveChangesAsync();
    }

    public async Task<GroupSettlement?> GetByIdAsync(Guid id)
    {
        return await _context.GroupSettlements
            .FirstOrDefaultAsync(gs => gs.Id == id);
    }
}
