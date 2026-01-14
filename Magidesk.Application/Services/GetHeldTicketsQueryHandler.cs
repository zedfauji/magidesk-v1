using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetHeldTicketsQuery.
/// </summary>
public class GetHeldTicketsQueryHandler : IQueryHandler<GetHeldTicketsQuery, IEnumerable<HeldTicketDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public GetHeldTicketsQueryHandler(
        ITicketRepository ticketRepository,
        IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<HeldTicketDto>> HandleAsync(GetHeldTicketsQuery query, CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetHeldTicketsAsync(cancellationToken);
        
        var result = new List<HeldTicketDto>();
        foreach (var ticket in tickets)
        {
            // Get user who held the ticket
            var heldByUser = ticket.HeldBy != null 
                ? await _userRepository.GetByIdAsync(ticket.HeldBy.Value, cancellationToken)
                : null;
            
            var dto = new HeldTicketDto(
                ticket.Id,
                ticket.TicketNumber,
                ticket.HeldAt!.Value,
                ticket.HoldReason!,
                heldByUser != null ? $"{heldByUser.FirstName} {heldByUser.LastName}" : "Unknown",
                ticket.TotalAmount.Amount,
                null, // Customer name - would need to load customer if needed
                ticket.TableNumbers.FirstOrDefault() > 0 ? ticket.TableNumbers.FirstOrDefault() : null
            );
            
            result.Add(dto);
        }

        return result;
    }
}
