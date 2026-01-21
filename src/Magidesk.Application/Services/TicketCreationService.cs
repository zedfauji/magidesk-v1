using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Application.Services;

public class TicketCreationService : ITicketCreationService
{
    private readonly ITerminalContext _terminalContext;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicketHandler;

    public TicketCreationService(
        ITerminalContext terminalContext,
        ICashSessionRepository cashSessionRepository,
        IOrderTypeRepository orderTypeRepository,
        ITableRepository tableRepository,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler)
    {
        _terminalContext = terminalContext;
        _cashSessionRepository = cashSessionRepository;
        _orderTypeRepository = orderTypeRepository;
        _tableRepository = tableRepository;
        _createTicketHandler = createTicketHandler;
    }

    public async Task<Guid> CreateTicketForTableAsync(Guid tableId, Guid userId)
    {
        // 1. Get Table to retrieve Table Number
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null)
            throw new NotFoundException($"Table with ID {tableId} not found.");

        if (table.Status != Domain.Enumerations.TableStatus.Available)
            throw new BusinessRuleViolationException($"Table {table.TableNumber} is not available (Status: {table.Status}).");

        // 2. Terminal Context
        if (_terminalContext.TerminalId == null)
            throw new BusinessRuleViolationException("No active terminal context.");
        
        var terminalId = _terminalContext.TerminalId.Value;

        // 3. Session Validation
        var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
        if (session == null)
            throw new BusinessRuleViolationException("No open cash session found. Please start a shift.");
        
        var shiftId = session.Id;

        // 4. Order Type (DINE IN)
        var orderTypes = await _orderTypeRepository.GetActiveAsync();
        var dineIn = orderTypes.FirstOrDefault(ot => ot.Name.ToUpper().Contains("DINE IN")) 
                     ?? orderTypes.FirstOrDefault();

        if (dineIn == null)
            throw new BusinessRuleViolationException("No valid Order Types found.");

        // 5. Create Ticket
        var command = new CreateTicketCommand
        {
            CreatedBy = new Domain.ValueObjects.UserId(userId),
            TerminalId = terminalId,
            ShiftId = shiftId,
            OrderTypeId = dineIn.Id,
            TableNumbers = new List<int> { table.TableNumber },
            NumberOfGuests = table.Capacity > 0 ? table.Capacity : 1 // Logic adjustment: default guests to table capacity or 1
        };

        var result = await _createTicketHandler.HandleAsync(command);
        return result.TicketId;
    }
}
