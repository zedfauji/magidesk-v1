using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface ITableLayoutRepository : IRepository<TableLayout>
{
    Task<TableLayoutDto?> GetLayoutByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<TableLayoutDto>> GetLayoutsByFloorAsync(Guid floorId, CancellationToken cancellationToken = default);
    Task<TableLayoutDto?> GetLayoutWithTablesAsync(Guid layoutId, CancellationToken cancellationToken = default);
    Task<bool> IsLayoutNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<TableLayoutDto?> GetActiveLayoutAsync(Guid floorId, CancellationToken cancellationToken = default);
    Task DeactivateOtherLayoutsAsync(Guid floorId, Guid activeLayoutId, CancellationToken cancellationToken = default);

}
