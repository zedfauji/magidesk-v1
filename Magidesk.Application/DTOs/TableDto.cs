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
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public TableShapeType Shape { get; set; }
    public TableStatus Status { get; set; }
    public Guid? CurrentTicketId { get; set; }
    public bool IsActive { get; set; }
    public bool IsSelected { get; set; }
}

