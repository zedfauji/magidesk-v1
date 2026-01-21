using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class StockMovementRepository : EfRepository<StockMovement>
{
    public StockMovementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
