namespace Magidesk.Application.Commands;

/// <summary>
/// Command to update a table.
/// </summary>
public class UpdateTableCommand
{
    public Guid TableId { get; set; }
    public int? TableNumber { get; set; }
    public int? Capacity { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }
    public Guid? FloorId { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Result of updating a table.
/// </summary>
public class UpdateTableResult
{
    public Guid TableId { get; set; }
    public int TableNumber { get; set; }
}

