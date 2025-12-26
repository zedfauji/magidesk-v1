using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for Table entity.
/// </summary>
public class TableDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public Guid? FloorId { get; set; }
    public int Capacity { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public TableStatus Status { get; set; }
    public Guid? CurrentTicketId { get; set; }
    public bool IsActive { get; set; }
}

