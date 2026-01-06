using System.Collections.Generic;

using System.Text.Json.Serialization;

namespace Magidesk.Infrastructure.Printing.Models;

public class PrintDocument
{
    public List<PrintElement> Elements { get; set; } = new();
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(TextElement), "Text")]
[JsonDerivedType(typeof(LineBreakElement), "LineBreak")]
[JsonDerivedType(typeof(SeparatorElement), "Separator")]
[JsonDerivedType(typeof(CutElement), "Cut")]
public abstract class PrintElement
{
    public abstract string Type { get; }
}
