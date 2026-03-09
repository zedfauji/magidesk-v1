using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries;

public class GetInventoryCategoriesQueryHandler : ICommandHandler<GetInventoryCategoriesQuery, IReadOnlyList<InventoryCategoryDto>>
{
    private readonly IInventoryCategoryRepository _repository;

    public GetInventoryCategoriesQueryHandler(IInventoryCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryCategoryDto>> HandleAsync(
        GetInventoryCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllActiveAsync(cancellationToken);

        var dtos = categories
            .Select(c => new InventoryCategoryDto(
                c.Id,
                c.Name,
                c.SortOrder,
                c.ParentCategoryId))
            .ToList();

        return dtos;
    }
}
