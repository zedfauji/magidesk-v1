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

        // TECH-B004: Implement actual assignment logic
        // Since ICustomerRepository is pending, we map GuestName/Phone to Ticket metadata
        
        // Format extra info for guest tracking
        var extraInfo = new
        {
            GuestName = command.GuestName,
            Phone = command.PhoneNumber,
            Timestamp = DateTime.UtcNow
        };
        
        string jsonInfo = JsonSerializer.Serialize(extraInfo);

        // Update the ticket
        // Note: For now we pass null for CustomerId as we don't have the entity key
        // We map the guest name to the DeliveryAddress field if it's not a formal address, 
        // effectively using it as a "Customer Label" for now, or just rely on ExtraDeliveryInfo.
        
        ticket.SetCustomer(
            customerId: null, 
            address: null, // Address would come from a real Customer profile
            extraInfo: jsonInfo
        );

        // If we want to store the name visually on the ticket, we might need a specific field.
        // For now, relying on the 'OwnerName' being the server, and 'ExtraDeliveryInfo' carrying the guest.

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        return new SetCustomerResult { Success = true };
    }
}
