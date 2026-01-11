using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for Member entity.
/// </summary>
public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Member?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Member?> GetByMemberNumberAsync(string memberNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Member member, CancellationToken cancellationToken = default);
    Task UpdateAsync(Member member, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for MembershipTier entity.
/// </summary>
public interface IMembershipTierRepository
{
    Task<MembershipTier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MembershipTier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(MembershipTier tier, CancellationToken cancellationToken = default);
    Task UpdateAsync(MembershipTier tier, CancellationToken cancellationToken = default);
}
