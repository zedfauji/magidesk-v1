using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly ApplicationDbContext _context;

    public DiscountRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Discount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Discount>()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Discount?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Discount>()
            .FirstOrDefaultAsync(d => d.CouponCode == couponCode, cancellationToken);
    }
}
