using System;
using System.Text;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;

namespace Magidesk.Infrastructure.Printing.Layouts
{
    public class StandardPageAdapter : IPrintLayoutAdapter
    {
        public Task<string> GenerateLayoutAsync(object data)
        {
            if (data is not TicketDto ticket)
            {
                return Task.FromResult("<html><body><h1>Invalid Data</h1></body></html>");
            }

            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head><style>body { font-family: sans-serif; } table { width: 100%; border-collapse: collapse; } th, td { padding: 8px; border-bottom: 1px solid #ddd; } .total { font-weight: bold; font-size: 1.2em; }</style></head>");
            sb.AppendLine("<body>");
            
            sb.AppendLine($"<h1>Ticket #{ticket.TicketNumber}</h1>");
            sb.AppendLine($"<p>Date: {ticket.CreatedAt:g}</p>");
            sb.AppendLine($"<p>Server: {ticket.OwnerName}</p>");
            
            sb.AppendLine("<table>");
            sb.AppendLine("<thead><tr><th>Item</th><th>Qty</th><th>Price</th><th>Total</th></tr></thead>");
            sb.AppendLine("<tbody>");
            
            foreach (var line in ticket.OrderLines)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{line.MenuItemName}</td>");
                sb.AppendLine($"<td>{line.Quantity}</td>");
                sb.AppendLine($"<td>{line.UnitPrice:C}</td>");
                sb.AppendLine($"<td>{line.TotalAmount:C}</td>");
                sb.AppendLine("</tr>");
            }
            
            sb.AppendLine("</tbody></table>");
            
            sb.AppendLine("<div style='margin-top: 20px; text-align: right;'>");
            sb.AppendLine($"<p>Subtotal: {ticket.SubtotalAmount:C}</p>");
            sb.AppendLine($"<p>Tax: {ticket.TaxAmount:C}</p>");
            sb.AppendLine($"<p class='total'>Total: {ticket.TotalAmount:C}</p>");
            sb.AppendLine("</div>");
            
            sb.AppendLine("</body></html>");
            
            return Task.FromResult(sb.ToString());
        }
    }
}
