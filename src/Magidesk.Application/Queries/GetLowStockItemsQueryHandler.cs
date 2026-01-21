using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using MediatR;

namespace Magidesk.Application.Queries;

public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, List<LowStockItemDto>>
{
    private readonly IMenuRepository _menuRepository;

    public GetLowStockItemsQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<LowStockItemDto>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        var allItems = await _menuRepository.GetAllAsync(cancellationToken);

        return allItems
            .Where(i => i.TrackStock && i.StockQuantity <= i.MinimumStockLevel)
            .Select(i => new LowStockItemDto(
                i.Id,
                i.Name,
                i.StockQuantity,
                i.MinimumStockLevel,
                i.MinimumStockLevel - i.StockQuantity
            ))
            .ToList();
    }
}
