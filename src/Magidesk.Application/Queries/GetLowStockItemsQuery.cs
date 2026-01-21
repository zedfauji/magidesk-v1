using System.Collections.Generic;
using Magidesk.Application.DTOs;
using MediatR;

namespace Magidesk.Application.Queries;

public record GetLowStockItemsQuery() : IRequest<List<LowStockItemDto>>;
