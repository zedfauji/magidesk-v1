using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .Include(m => m.Customer)
            .Include(m => m.Tier)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Member?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .Include(m => m.Customer)
            .Include(m => m.Tier)
            .FirstOrDefaultAsync(m => m.CustomerId == customerId, cancellationToken);
    }

    public async Task<Member?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .Include(m => m.Customer)
            .Include(m => m.Tier)
            .FirstOrDefaultAsync(m => m.MemberNumber == memberNumber, cancellationToken);
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _context.Members.AddAsync(member, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Member member, CancellationToken cancellationToken = default)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class MembershipTierRepository : IMembershipTierRepository
{
    private readonly ApplicationDbContext _context;

    public MembershipTierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MembershipTier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MembershipTiers
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MembershipTier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MembershipTiers
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MembershipTier tier, CancellationToken cancellationToken = default)
    {
        await _context.MembershipTiers.AddAsync(tier, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MembershipTier tier, CancellationToken cancellationToken = default)
    {
        _context.MembershipTiers.Update(tier);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
