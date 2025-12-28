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
    private readonly GetTicketQueryHandler _getTicketQueryHandler;

    public GetOpenTicketsQueryHandler(
        ITicketRepository ticketRepository,
        GetTicketQueryHandler getTicketQueryHandler)
    {
        _ticketRepository = ticketRepository;
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
                // Populate Table Name
                if (dto.TableNumbers.Any())
                {
                    dto.TableName = $"Table {dto.TableNumbers.First()}";
                }

                // Populate Owner Name (Placeholder until IUserRepository exists)
                // In future: Use IUserRepository.GetByIdAsync(dto.CreatedBy)
                dto.OwnerName = "Server"; 
                
                result.Add(dto);
            }
        }

        return result;
    }
}

