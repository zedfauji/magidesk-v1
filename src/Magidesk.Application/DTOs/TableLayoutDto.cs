using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs;

public class TableLayoutDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? FloorId { get; set; }
    public string FloorName { get; set; } = string.Empty;
    public List<TableDto> Tables { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDraft { get; set; }
    public int Version { get; set; }
}

public class FloorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Width { get; set; } = 2000;
    public int Height { get; set; } = 2000;
    public string BackgroundColor { get; set; } = "#f3f3f3";
    public List<TableDto> Tables { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
