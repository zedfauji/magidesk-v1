using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class DiscountRepository : EfRepository<Discount>, IDiscountRepository
{
    public DiscountRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Discount?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Discount>()
            .FirstOrDefaultAsync(d => d.CouponCode == couponCode, cancellationToken);
    }

    /// <summary>
    /// Gets all active discounts.
    /// Task 2.1.9: Get active discounts for discount selection
    /// </summary>
    public async Task<IReadOnlyList<Discount>> GetActiveDiscountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Discount>()
            .Where(d => d.IsActive)
            .Where(d => d.ExpirationDate == null || d.ExpirationDate > DateTime.UtcNow)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the member discount for a specific customer if they are an active member.
    /// Task 2.1.9: Support member discount auto-application
    /// </summary>
    public async Task<Discount?> GetMemberDiscountAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        // 1. Find the member record for this customer
        var member = await _dbContext.Set<Member>()
            .Include(m => m.Tier)
            .FirstOrDefaultAsync(m => m.CustomerId == customerId, cancellationToken);

        // 2. If not a member or not active, return null
        if (member == null || !member.IsActive)
        {
            return null;
        }

        // 3. Get the membership tier to determine discount percentage
        if (member.Tier == null)
        {
            return null;
        }

        var discountPercent = member.Tier.DiscountPercent;

        // 4. If no discount on this tier, return null
        if (discountPercent <= 0)
        {
            return null;
        }

        // 5. Create a member discount object
        // This is a virtual discount that represents the member's tier discount
        var memberDiscount = Discount.Create(
            name: $"Member Discount - {member.Tier.Name}",
            type: DiscountType.Percentage,
            value: discountPercent,
            qualificationType: QualificationType.Order,
            applicationType: ApplicationType.PercentagePerOrder,
            autoApply: true,
            requiresAuthorization: false
        );

        return memberDiscount;
    }
}
