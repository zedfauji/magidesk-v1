using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries;

public class GetInventoryItemsPagedQueryHandler : ICommandHandler<GetInventoryItemsPagedQuery, InventoryItemPagedResultDto>
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryItemsPagedQueryHandler(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryItemPagedResultDto> HandleAsync(
        GetInventoryItemsPagedQuery query,
        CancellationToken cancellationToken)
    {
        int skip = query.Page * query.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            query.SearchTerm,
            query.Filter,
            query.CategoryId,
            skip,
            query.PageSize,
            cancellationToken);

        var dtos = items.Select(item => new InventoryItemDto(
            item.Id,
            item.Name,
            item.Unit,
            item.SkuCode,
            item.StockQuantity,
            item.ReorderPoint,
            item.CategoryId,
            null, // CategoryName will be null since we're not joining in the repository
            item.CreatedAt,
            item.IsActive
        )).ToList();

        return new InventoryItemPagedResultDto(
            dtos,
            totalCount,
            query.Page,
            query.PageSize);
    }
}
