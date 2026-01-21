using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using MediatR;

namespace Magidesk.Application.Commands;

public record CreateTableLayoutCommand(
    string Name,
    Guid? FloorId,
    List<TableDto> Tables,
    bool IsDraft = false
) : IRequest<TableLayoutDto>;

public record UpdateTablePositionCommand(
    Guid TableId,
    double X,
    double Y,
    TableShapeType Shape,
    double Width = 100,
    double Height = 100
) : IRequest<TableDto>;

public record SaveTableLayoutCommand(
    Guid LayoutId,
    string Name,
    List<TableDto> Tables,
    bool? IsDraft = null
) : IRequest<TableLayoutDto>;

public record DeleteTableLayoutCommand(
    Guid LayoutId
) : IRequest<bool>;

public record AddTableToLayoutCommand(
    Guid LayoutId,
    int TableNumber,
    int Capacity,
    double X,
    double Y,
    TableShapeType Shape,
    double Width = 100,
    double Height = 100
) : IRequest<TableDto>;

public record RemoveTableFromLayoutCommand(
    Guid LayoutId,
    Guid TableId
) : IRequest<bool>;
