using System.Text;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Printing.Models;

namespace Magidesk.Infrastructure.Printing.Drivers;

public class PlainTextDriver : IPrintDriver
{
    public byte[] Render(PrintDocument doc, PrinterMapping mapping)
    {
        var sb = new StringBuilder();

        foreach (var element in doc.Elements)
        {
            switch (element)
            {
                case TextElement text:
                    // Simple alignment simulation (imperfect for plain text without fixed width logic, but functional)
                    // For now, just append content. Advanced alignment would require padding calculation.
                    sb.AppendLine(text.Content); 
                    break;

                case LineBreakElement _:
                    sb.AppendLine();
                    break;

                case SeparatorElement sep:
                    // Standard page is usually wider, e.g., 80 chars
                    int width = 80; 
                    sb.AppendLine(new string(sep.Char, width));
                    break;
                
                case CutElement _:
                    // No-op for plain text
                    sb.AppendLine();
                    sb.AppendLine("--- CUT ---");
                    sb.AppendLine();
                    break;
            }
        }

        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}
