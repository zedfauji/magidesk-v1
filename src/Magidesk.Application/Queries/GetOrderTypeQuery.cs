using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get an order type by ID.
/// </summary>
public class GetOrderTypeQuery
{
    public Guid OrderTypeId { get; set; }
}

/// <summary>
/// Result of getting an order type.
/// </summary>
public class GetOrderTypeResult
{
    public OrderTypeDto? OrderType { get; set; }
}

