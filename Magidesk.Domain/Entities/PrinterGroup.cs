using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a virtual printer destination (e.g., "Kitchen", "Receipt").
/// Menu items are routed to Printer Groups rather than physical devices directly.
/// </summary>
public class PrinterGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public PrinterType Type { get; private set; }

    // Private constructor for EF Core
    private PrinterGroup()
    {
    }

    public static PrinterGroup Create(string name, PrinterType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Printer group name is required.");

        return new PrinterGroup
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type
        };
    }

    public void Update(string name, PrinterType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Printer group name is required.");

        Name = name;
        Type = type;
    }
}
