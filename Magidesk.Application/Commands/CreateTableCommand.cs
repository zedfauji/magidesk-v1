namespace Magidesk.Application.Commands;

/// <summary>
/// Command to create a new table.
/// </summary>
public class CreateTableCommand
{
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Guid? FloorId { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Result of creating a table.
/// </summary>
public class CreateTableResult
{
    public Guid TableId { get; set; }
    public int TableNumber { get; set; }
}

