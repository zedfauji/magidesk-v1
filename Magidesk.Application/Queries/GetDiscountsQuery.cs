using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;


namespace Magidesk.Application.Queries;

public class GetDiscountsQuery : IQuery<IEnumerable<DiscountDto>>
{
    public bool IncludeInactive { get; set; }
}

public class GetDiscountsQueryHandler : IQueryHandler<GetDiscountsQuery, IEnumerable<DiscountDto>>
{
    private readonly IDiscountRepository _repository;

    public GetDiscountsQueryHandler(IDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DiscountDto>> HandleAsync(GetDiscountsQuery query, CancellationToken cancellationToken = default)
    {
        var discounts = await _repository.GetAllAsync(cancellationToken);

        if (!query.IncludeInactive)
        {
            discounts = discounts.Where(d => d.IsActive);
        }

        return discounts.OrderBy(d => d.Name).Select(d => new DiscountDto
        {
            Id = d.Id,
            Name = d.Name,
            Type = d.Type,
            Value = d.Value,
            MinimumBuy = d.MinimumBuy,
            MinimumQuantity = d.MinimumQuantity,
            AutoApply = d.AutoApply,
            IsActive = d.IsActive,
            CouponCode = d.CouponCode,
            ExpirationDate = d.ExpirationDate
        });
    }
}
