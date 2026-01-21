using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IDiscountRepository : IRepository<Discount>
{
    // GetByIdAsync provided by base interface
    Task<Discount?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active discounts.
    /// Task 2.1.8: Get active discounts for discount selection
    /// </summary>
    Task<IReadOnlyList<Discount>> GetActiveDiscountsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the member discount for a specific customer if they are an active member.
    /// Task 2.1.8: Support member discount auto-application
    /// </summary>
    Task<Discount?> GetMemberDiscountAsync(Guid customerId, CancellationToken cancellationToken = default);
}
