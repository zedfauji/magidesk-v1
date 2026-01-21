using System.Threading.Tasks;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Interfaces.Printing
{
    public interface IPrintLayoutEngine
    {
        Task<string> GenerateTicketLayoutAsync(object ticketData, PrinterFormat format);
    }
}
