using System.Threading.Tasks;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

public interface IPrintingService
{
    Task PrintTicketAsync(TicketDto ticket, string printerName = null);
    Task PrintKitchenTicketAsync(TicketDto ticket);
    Task PrintReceiptAsync(TicketDto ticket);
}
