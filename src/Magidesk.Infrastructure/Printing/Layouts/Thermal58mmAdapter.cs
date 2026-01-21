using System;
using System.Text;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;

namespace Magidesk.Infrastructure.Printing.Layouts
{
    public class Thermal58mmAdapter : IPrintLayoutAdapter
    {
        private const int MaxCharsPerLine = 32;

        public Task<string> GenerateLayoutAsync(object data)
        {
            if (data is not TicketDto ticket)
            {
                return Task.FromResult("Invalid Data Type");
            }

            var sb = new StringBuilder();
            
            // Header
            sb.AppendLine(CenterText("MAGIDESK POS"));
            sb.AppendLine(CenterText("123 Restaurant Way"));
            sb.AppendLine(new string('-', MaxCharsPerLine));
            
            sb.AppendLine($"#{ticket.TicketNumber} {ticket.CreatedAt:MM/dd HH:mm}");
            sb.AppendLine(new string('-', MaxCharsPerLine));
            sb.AppendLine("Item              Qty    Total"); // 32 chars
            sb.AppendLine(new string('-', MaxCharsPerLine));

            // Items
            foreach (var line in ticket.OrderLines)
            {
                string name = line.MenuItemName;
                if (name.Length > 16) name = name.Substring(0, 16);
                
                string qty = line.Quantity.ToString("0.#");
                string price = line.TotalAmount.ToString("0.00");
                
                // Format: "Name (16) | Qty (5) | Total (remaining)"
                
                sb.Append(name.PadRight(17));
                sb.Append(qty.PadLeft(5));
                sb.AppendLine(price.PadLeft(10));
                
                 foreach(var mod in line.Modifiers)
                {
                     sb.AppendLine($" + {mod.Name}");
                }
            }
            
            sb.AppendLine(new string('-', MaxCharsPerLine));

            // Totals
            sb.AppendLine(FormatLine("Subtotal:", ticket.SubtotalAmount.ToString("0.00")));
            sb.AppendLine(FormatLine("Tax:", ticket.TaxAmount.ToString("0.00")));
            sb.AppendLine(new string('=', MaxCharsPerLine));
            sb.AppendLine(FormatTotal("TOTAL:", ticket.TotalAmount.ToString("0.00")));
            sb.AppendLine(new string('=', MaxCharsPerLine));
            
            sb.AppendLine();
            sb.AppendLine(CenterText("Thank You!"));
            sb.AppendLine();

            return Task.FromResult(sb.ToString());
        }

        private string CenterText(string text)
        {
            if (text.Length >= MaxCharsPerLine) return text;
            int totalSpaces = MaxCharsPerLine - text.Length;
            int padLeft = totalSpaces / 2;
            return text.PadLeft(padLeft + text.Length);
        }

        private string FormatLine(string label, string value)
        {
            int labelLen = label.Length;
            int valueLen = value.Length;
            int spaces = MaxCharsPerLine - labelLen - valueLen;
            if (spaces < 1) spaces = 1;
            return label + new string(' ', spaces) + value;
        }

        private string FormatTotal(string label, string value)
        {
             return FormatLine(label, value);
        }
    }
}
