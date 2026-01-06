using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Mapping;

/// <summary>
/// Centralized mapper for Table entity to TableDto.
/// Enforces type safety and precision rules.
/// </summary>
public static class TableMapper
{
    public static TableDto ToDto(Table table)
    {
        if (table == null) return null;

        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            FloorId = table.FloorId,
            LayoutId = table.LayoutId,
            Capacity = table.Capacity,
            X = table.X,
            Y = table.Y,
            Width = table.Width,
            Height = table.Height,
            Shape = table.Shape,
            Status = table.Status,
            CurrentTicketId = table.CurrentTicketId,
            IsActive = table.IsActive,
            IsLocked = table.Status != TableStatus.Available
        };
    }

    public static List<TableDto> ToDtoList(IEnumerable<Table> tables)
    {
        return tables?.Select(ToDto).ToList() ?? new List<TableDto>();
    }

    public static TableLayoutDto ToLayoutDto(TableLayout layout)
    {
        if (layout == null) return null;

        return new TableLayoutDto
        {
            Id = layout.Id,
            Name = layout.Name,
            FloorId = layout.FloorId,
            IsDraft = layout.IsDraft,
            IsActive = layout.IsActive,
            Version = layout.Version,
            CreatedAt = layout.CreatedAt,
            UpdatedAt = layout.UpdatedAt,
            Tables = ToDtoList(layout.Tables)
        };
    }
}
