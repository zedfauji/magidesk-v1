using System.Text;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Printing.Models;

namespace Magidesk.Infrastructure.Printing.Drivers;

public class HtmlPreviewDriver
{
    public string Render(PrintDocument doc)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html><html><head><style>");
        sb.AppendLine("body { font-family: 'Courier New', monospace; font-size: 14px; width: 300px; margin: 0 auto; background: white; padding: 20px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }");
        sb.AppendLine(".line { margin: 0; white-space: pre-wrap; line-height: 1.2; }");
        sb.AppendLine(".bold { font-weight: bold; }");
        sb.AppendLine(".double-height { font-size: 1.5em; line-height: 1.5; }");
        sb.AppendLine(".center { text-align: center; }");
        sb.AppendLine(".right { text-align: right; }");
        sb.AppendLine(".separator { border-top: 1px dashed black; margin: 5px 0; }");
        sb.AppendLine("</style></head><body>");

        foreach (var element in doc.Elements)
        {
            switch (element)
            {
                case TextElement text:
                    var classes = new List<string> { "line" };
                    if (text.Bold) classes.Add("bold");
                    if (text.DoubleHeight) classes.Add("double-height");
                    
                    if (text.Align == "Center") classes.Add("center");
                    else if (text.Align == "Right") classes.Add("right");

                    sb.AppendLine($"<div class='{string.Join(" ", classes)}'>{System.Net.WebUtility.HtmlEncode(text.Content)}</div>");
                    break;

                case LineBreakElement _:
                    sb.AppendLine("<div class='line'>&nbsp;</div>");
                    break;

                case SeparatorElement sep:
                    sb.AppendLine("<div class='separator'></div>");
                    break;
                
                case CutElement _:
                    sb.AppendLine("<div class='line' style='border-bottom: 2px dotted red; text-align: center; margin-top: 10px; color: red;'>--- CUT ---</div>");
                    break;
            }
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}
