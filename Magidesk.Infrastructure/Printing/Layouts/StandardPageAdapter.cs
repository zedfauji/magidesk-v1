using System.Threading.Tasks;
using System.Text;

namespace Magidesk.Infrastructure.Printing.Layouts
{
    public class StandardPageAdapter : IPrintLayoutAdapter
    {
        public Task<string> GenerateLayoutAsync(object data)
        {
            // TODO: Implement PDF or HTML generation for standard printers
            var sb = new StringBuilder();
            sb.AppendLine("<html><body>");
            sb.AppendLine("<h1>Standard Page Layout</h1>");
            sb.AppendLine("</body></html>");
            
            return Task.FromResult(sb.ToString());
        }
    }
}
