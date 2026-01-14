using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Service for handling member discount auto-application.
/// Task 2.1.7: Implement member discount auto-application
/// </summary>
public class MemberDiscountService
{
    private readonly IRepository<Member> _memberRepository;
    private readonly IDiscountRepository _discountRepository;

    public MemberDiscountService(
        IRepository<Member> memberRepository,
        IDiscountRepository discountRepository)
    {
        _memberRepository = memberRepository;
        _discountRepository = discountRepository;
    }

    /// <summary>
    /// Gets the member discount for a customer if they are an active member.
    /// </summary>
    /// <param name="customerId">The customer ID to check for membership</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Discount object representing the member discount, or null if not a member</returns>
    public async Task<Discount?> GetMemberDiscountAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        // Delegate to the repository which will handle the logic
        return await _discountRepository.GetMemberDiscountAsync(customerId, cancellationToken);
    }

    /// <summary>
    /// Checks if a customer is an active member.
    /// </summary>
    /// <param name="customerId">The customer ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the customer is an active member, false otherwise</returns>
    public async Task<bool> IsActiveMemberAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var members = await _memberRepository.GetAllAsync(cancellationToken);
        var member = members.FirstOrDefault(m => m.CustomerId == customerId);

        return member != null && member.IsActive;
    }

    /// <summary>
    /// Gets the member record for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The Member entity, or null if not found</returns>
    public async Task<Member?> GetMemberAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var members = await _memberRepository.GetAllAsync(cancellationToken);
        return members.FirstOrDefault(m => m.CustomerId == customerId);
    }
}
