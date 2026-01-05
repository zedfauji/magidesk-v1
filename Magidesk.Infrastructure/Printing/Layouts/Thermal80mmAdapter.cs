using System;
using System.Text;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;

namespace Magidesk.Infrastructure.Printing.Layouts
{
    public class Thermal80mmAdapter : IPrintLayoutAdapter
    {
        private const int MaxCharsPerLine = 48; // Standard for 80mm

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
            sb.AppendLine(CenterText("City, State 12345"));
            sb.AppendLine(new string('-', MaxCharsPerLine));
            
            sb.AppendLine($"Ticket: #{ticket.TicketNumber}");
            sb.AppendLine($"Date:   {ticket.CreatedAt:MM/dd/yyyy HH:mm}");
            sb.AppendLine($"Server: {ticket.OwnerName}");
            if (!string.IsNullOrEmpty(ticket.TableName))
            {
                sb.AppendLine($"Table:  {ticket.TableName}");
            }
            
            sb.AppendLine(new string('-', MaxCharsPerLine));
            sb.AppendLine("Item                           Qty     Price");
            sb.AppendLine(new string('-', MaxCharsPerLine));

            // Items
            foreach (var line in ticket.OrderLines)
            {
                string name = line.MenuItemName;
                if (name.Length > 28) name = name.Substring(0, 28);
                
                string qty = line.Quantity.ToString("0.##");
                string price = line.TotalAmount.ToString("F2");
                
                // Format: "Name (pad) Qty (pad) Price"
                // Name: 30 chars, Qty: 6 chars, Price: 10 chars approx
                // Using helper to align right
                
                sb.Append(name.PadRight(30));
                sb.Append(qty.PadLeft(6));
                sb.AppendLine(price.PadLeft(12));
                
                // Modifiers
                foreach(var mod in line.Modifiers)
                {
                     sb.AppendLine($"  + {mod.Name}");
                }
            }
            
            sb.AppendLine(new string('-', MaxCharsPerLine));

            // Totals
            sb.AppendLine(FormatLine("Subtotal:", ticket.SubtotalAmount.ToString("C")));
            sb.AppendLine(FormatLine("Tax:", ticket.TaxAmount.ToString("C")));
            if (ticket.DiscountAmount > 0)
            {
                sb.AppendLine(FormatLine("Discount:", $"-{ticket.DiscountAmount:C}"));
            }
            sb.AppendLine(new string('=', MaxCharsPerLine));
            sb.AppendLine(FormatTotal("TOTAL:", ticket.TotalAmount.ToString("C")));
            sb.AppendLine(new string('=', MaxCharsPerLine));
            
            // Footer
            sb.AppendLine();
            sb.AppendLine(CenterText("Thank you for your visit!"));
            sb.AppendLine(CenterText("Powered by MagiDesk"));
            sb.AppendLine();
            sb.AppendLine(); // Margin for cutter

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
            // Bolding simulated by CAPS or just standard alignment for now
            return FormatLine(label, value);
        }
    }
}
