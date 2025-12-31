using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using System.Text.Json;

namespace Magidesk.Application.Services;

public class SetCustomerCommandHandler : ICommandHandler<SetCustomerCommand, SetCustomerResult>
{
    private readonly ITicketRepository _ticketRepository;

    public SetCustomerCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<SetCustomerResult> HandleAsync(SetCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            return new SetCustomerResult { Success = false, ErrorMessage = "Ticket not found." };

        // For now, we store Name/Phone in ExtraDeliveryInfo JSON or formatting, 
        // since we don't have a Customer Entity or dedicated GuestName field yet.
        // This is a Slice 2 Gap Fill stub.
        
        var info = $"Guest: {command.GuestName} | Phone: {command.PhoneNumber}";
        
        // Pass null for CustomerId and Address as this is just "Guest Assignment"
        ticket.SetCustomer(null, null, info);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        return new SetCustomerResult { Success = true };
    }
}
