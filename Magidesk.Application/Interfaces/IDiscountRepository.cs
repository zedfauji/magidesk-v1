using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IDiscountRepository : IRepository<Discount>
{
    // GetByIdAsync provided by base interface
    Task<Discount?> GetByCouponCodeAsync(string couponCode, CancellationToken cancellationToken = default);
}
