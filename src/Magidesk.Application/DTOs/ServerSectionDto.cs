using System;
using System.Collections.Generic;

namespace Magidesk.Application.DTOs;

public class ServerSectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ServerId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public List<Guid> TableIds { get; set; } = new();
    public int TableCount { get; set; }
    public string Color { get; set; } = "#3498db";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}

public class ServerAssignmentDto
{
    public Guid ServerId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public List<ServerSectionDto> Sections { get; set; } = new();
    public int TotalTables { get; set; }
    public bool IsActive { get; set; } = true;
}
