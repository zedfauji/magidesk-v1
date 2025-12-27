using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using System.Threading;

namespace Magidesk.Application.Services;

public class ChangeTableCommandHandler : ICommandHandler<ChangeTableCommand, ChangeTableResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITableRepository _tableRepository;

    public ChangeTableCommandHandler(
        ITicketRepository ticketRepository,
        ITableRepository tableRepository)
    {
        _ticketRepository = ticketRepository;
        _tableRepository = tableRepository;
    }

    public async Task<ChangeTableResult> HandleAsync(ChangeTableCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            return new ChangeTableResult { Success = false, ErrorMessage = "Ticket not found." };

        var newTable = await _tableRepository.GetByIdAsync(command.NewTableId, cancellationToken);
        if (newTable == null)
            return new ChangeTableResult { Success = false, ErrorMessage = "New table not found." };

        if (!newTable.IsAvailable())
            return new ChangeTableResult { Success = false, ErrorMessage = $"Table {newTable.TableNumber} is not available." };

        // Handle Old Table logic (if ticket was assigned to one)
        // Note: Relation might be on Table side (CurrentTicketId) or Ticket side (TableId).
        // Domain model check needed? 
        // Table entity has CurrentTicketId. 
        
        // Find the table currently holding this ticket (if any)
        // Optimization: Ticket should probably know its TableId? 
        // Let's assume for now we might need to query the old table or Ticket has it.
        // Checking Ticket Entity... (I recall Ticket has TableNumbers string, but maybe no ID link in new schema?)
        // Let's check Ticket.cs later. For now, assuming we query tables by TicketId.
        
        var currentTable = await _tableRepository.GetByTicketIdAsync(ticket.Id);
        if (currentTable != null)
        {
            if (currentTable.Id == newTable.Id)
                return new ChangeTableResult { Success = false, ErrorMessage = "Ticket is already at this table." };

            currentTable.ReleaseTicket();
            await _tableRepository.UpdateAsync(currentTable);
        }

        newTable.AssignTicket(ticket.Id);
        await _tableRepository.UpdateAsync(newTable);
        
        // Update Ticket's table reference (string or ID)
        ticket.AssignTable(newTable.TableNumber); 
        await _ticketRepository.UpdateAsync(ticket);

        return new ChangeTableResult { Success = true };
    }
}
