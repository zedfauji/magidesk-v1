using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetTicketByNumberQuery.
/// </summary>
public class GetTicketByNumberQueryHandler : IQueryHandler<GetTicketByNumberQuery, TicketDto?>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly GetTicketQueryHandler _getTicketQueryHandler;

    public GetTicketByNumberQueryHandler(
        ITicketRepository ticketRepository,
        GetTicketQueryHandler getTicketQueryHandler)
    {
        _ticketRepository = ticketRepository;
        _getTicketQueryHandler = getTicketQueryHandler;
    }

    public async Task<TicketDto?> HandleAsync(GetTicketByNumberQuery query, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByTicketNumberAsync(query.TicketNumber, cancellationToken);
        if (ticket == null)
        {
            return null;
        }

        // Use the mapping logic from GetTicketQueryHandler
        var getTicketQuery = new GetTicketQuery { TicketId = ticket.Id };
        return await _getTicketQueryHandler.HandleAsync(getTicketQuery, cancellationToken);
    }
}

