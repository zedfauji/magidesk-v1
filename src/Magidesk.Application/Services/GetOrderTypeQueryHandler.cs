using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetOrderTypeQuery.
/// </summary>
public class GetOrderTypeQueryHandler : IQueryHandler<GetOrderTypeQuery, GetOrderTypeResult>
{
    private readonly IOrderTypeRepository _orderTypeRepository;

    public GetOrderTypeQueryHandler(IOrderTypeRepository orderTypeRepository)
    {
        _orderTypeRepository = orderTypeRepository;
    }

    public async Task<GetOrderTypeResult> HandleAsync(GetOrderTypeQuery query, CancellationToken cancellationToken = default)
    {
        var orderType = await _orderTypeRepository.GetByIdAsync(query.OrderTypeId, cancellationToken);
        
        return new GetOrderTypeResult
        {
            OrderType = orderType != null ? MapToDto(orderType) : null
        };
    }

    private static OrderTypeDto MapToDto(Domain.Entities.OrderType orderType)
    {
        return new OrderTypeDto
        {
            Id = orderType.Id,
            Name = orderType.Name,
            CloseOnPaid = orderType.CloseOnPaid,
            AllowSeatBasedOrder = orderType.AllowSeatBasedOrder,
            AllowToAddTipsLater = orderType.AllowToAddTipsLater,
            IsBarTab = orderType.IsBarTab,
            IsActive = orderType.IsActive,
            Properties = orderType.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }
}

