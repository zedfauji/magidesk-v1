using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using MediatR;

namespace Magidesk.Application.Commands;

public record CreateTableLayoutCommand(
    string Name,
    Guid? FloorId,
    List<TableDto> Tables
) : IRequest<TableLayoutDto>;

public record UpdateTablePositionCommand(
    Guid TableId,
    int X,
    int Y,
    TableShapeType Shape
) : IRequest<TableDto>;

public record SaveTableLayoutCommand(
    Guid LayoutId,
    string Name,
    List<TableDto> Tables
) : IRequest<TableLayoutDto>;

public record DeleteTableLayoutCommand(
    Guid LayoutId
) : IRequest<bool>;

public record AddTableToLayoutCommand(
    Guid LayoutId,
    int TableNumber,
    int Capacity,
    int X,
    int Y,
    TableShapeType Shape,
    int Width = 100,
    int Height = 100
) : IRequest<TableDto>;

public record RemoveTableFromLayoutCommand(
    Guid LayoutId,
    Guid TableId
) : IRequest<bool>;
