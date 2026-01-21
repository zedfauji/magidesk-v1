namespace Magidesk.Infrastructure.Printing.Models;

public class TextElement : PrintElement
{
    public override string Type => "Text";
    public string Content { get; set; } = string.Empty;
    public string Align { get; set; } = "Left"; // Left, Center, Right
    public bool Bold { get; set; }
    public bool DoubleHeight { get; set; }
}

public class LineBreakElement : PrintElement
{
    public override string Type => "LineBreak";
}

public class SeparatorElement : PrintElement
{
    public override string Type => "Separator";
    public char Char { get; set; } = '-';
}

public class CutElement : PrintElement
{
    public override string Type => "Cut";
}
