using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
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
}
