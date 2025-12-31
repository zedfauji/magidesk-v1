using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetOpenTicketsQuery.
/// </summary>
public class GetOpenTicketsQueryHandler : IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly GetTicketQueryHandler _getTicketQueryHandler;

    public GetOpenTicketsQueryHandler(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        GetTicketQueryHandler getTicketQueryHandler)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _getTicketQueryHandler = getTicketQueryHandler;
    }

    public async Task<IEnumerable<TicketDto>> HandleAsync(GetOpenTicketsQuery query, CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetOpenTicketsAsync(cancellationToken);
        
        var result = new List<TicketDto>();
        foreach (var ticket in tickets)
        {
            var getTicketQuery = new GetTicketQuery { TicketId = ticket.Id };
            var dto = await _getTicketQueryHandler.HandleAsync(getTicketQuery, cancellationToken);
            
            if (dto != null)
            {
                // Populate Owner Name
                var owner = await _userRepository.GetByIdAsync(ticket.CreatedBy.Value, cancellationToken);
                dto.OwnerName = owner != null ? $"{owner.FirstName} {owner.LastName}" : "Unknown";
                
                result.Add(dto);
            }
        }

        return result;
    }
}

