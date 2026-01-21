using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs.Printing;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IPrintContextBuilder
{
    Task<TicketPrintModel> BuildTicketContextAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<TicketPrintModel> BuildKitchenContextAsync(Ticket ticket, IEnumerable<OrderLine> lines, CancellationToken cancellationToken);
}
