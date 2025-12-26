using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IDiscountRepository
{
    Task<Discount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Discount?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default);
}
