using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Interfaces.Persistence;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Implementation of IPriceLevelRepository.
/// </summary>
public class PriceLevelRepository : EfRepository<PriceLevel>, IPriceLevelRepository
{
    public PriceLevelRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task DeleteAsync(PriceLevel entity, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(entity, cancellationToken);
    }
}
