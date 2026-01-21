using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

public class PrintTemplate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TemplateType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public bool IsSystem { get; private set; }

    // Concurrency
    public int Version { get; private set; }

    private PrintTemplate() { }

    public static PrintTemplate Create(string name, TemplateType type, string content, bool isSystem = false)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required");

        return new PrintTemplate
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Content = content,
            IsSystem = isSystem,
            Version = 1
        };
    }

    public void UpdateContent(string content)
    {
        if (IsSystem) throw new InvalidOperationException("Cannot modify system templates.");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required");
        
        Content = content;
    }

    public void UpdateName(string name)
    {
        if (IsSystem) throw new InvalidOperationException("Cannot modify system templates.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");

        Name = name;
    }

    public void UpdateIsSystem(bool isSystem)
    {
        IsSystem = isSystem;
    }
}
