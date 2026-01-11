using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Application.Commands;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for starting a new table session.
/// </summary>
public class StartTableSessionCommandHandler : ICommandHandler<StartTableSessionCommand, StartTableSessionResult>
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicketHandler;

    public StartTableSessionCommandHandler(
        ITableSessionRepository sessionRepository,
        ITableRepository tableRepository,
        ITableTypeRepository tableTypeRepository,
        ITicketRepository ticketRepository,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _tableRepository = tableRepository ?? throw new ArgumentNullException(nameof(tableRepository));
        _tableTypeRepository = tableTypeRepository ?? throw new ArgumentNullException(nameof(tableTypeRepository));
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _createTicketHandler = createTicketHandler ?? throw new ArgumentNullException(nameof(createTicketHandler));
    }

    public async Task<StartTableSessionResult> HandleAsync(
        StartTableSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate table exists
        var table = await _tableRepository.GetByIdAsync(command.TableId, cancellationToken);
        if (table == null)
        {
            throw new InvalidOperationException($"Table {command.TableId} not found.");
        }

        // 2. Validate table is available for sessions
        // Allow both Available and Seat status (Seat = table has ticket but no session yet)
        if (table.Status != TableStatus.Available && table.Status != TableStatus.Seat)
        {
            throw new InvalidOperationException($"Table {table.TableNumber} is not available for sessions. Current status: {table.Status}");
        }

        // 3. Check for existing active session
        var existingSession = await _sessionRepository.GetActiveSessionByTableIdAsync(command.TableId);
        if (existingSession != null)
        {
            throw new InvalidOperationException($"Table {table.TableNumber} already has an active session.");
        }

        // 4. Get table type to retrieve hourly rate
        // NOTE: Since Table doesn't have TableTypeId, we need to get it from command
        // This will be passed from the UI which knows the table type
        var tableType = await _tableTypeRepository.GetByIdAsync(command.TableTypeId);
        if (tableType == null)
        {
            throw new InvalidOperationException($"Table type {command.TableTypeId} not found.");
        }

        if (!tableType.IsActive)
        {
            throw new InvalidOperationException($"Table type '{tableType.Name}' is not active.");
        }

        // 5. Determine TicketId
        Guid? ticketId = command.TicketId;
        
        if (command.CreateTicket && !ticketId.HasValue)
        {
            // Create a new ticket for this session
            var createTicketCommand = new CreateTicketCommand
            {
                TableId = command.TableId,
                TableNumbers = new List<int> { table.TableNumber },
                CustomerId = command.CustomerId,
                NumberOfGuests = command.GuestCount,
                CreatedBy = new UserId((command.UserId ?? Guid.Empty) == Guid.Empty ? Guid.Parse("00000000-0000-0000-0000-000000000001") : command.UserId.Value),
                TerminalId = command.TerminalId ?? Guid.Empty,
                ShiftId = command.ShiftId ?? Guid.Empty,
                OrderTypeId = command.OrderTypeId ?? Guid.Empty,
                Note = $"Session for Table {table.TableNumber}"
            };

            var ticketResult = await _createTicketHandler.HandleAsync(createTicketCommand, cancellationToken);
            ticketId = ticketResult.TicketId;
        }

        // 6. Create table session
        var session = TableSession.Start(
            command.TableId,
            tableType.Id,
            tableType.HourlyRate,
            command.GuestCount,
            command.CustomerId,
            ticketId
        );

        await _sessionRepository.AddAsync(session);

        // 7. Update table status to InUse
        table.MarkInUse();
        await _tableRepository.UpdateAsync(table, cancellationToken);

        // 8. If a ticket was created/linked, ensure it knows about the session
        // (This is redundant if CreateTicketCommandHandler already handles it, but good for safety)
        if (ticketId.HasValue)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId.Value, cancellationToken);
            if (ticket != null && ticket.SessionId != session.Id)
            {
                ticket.SetSession(session.Id);
                await _ticketRepository.UpdateAsync(ticket, cancellationToken);
            }
        }

        // 9. Return result
        return new StartTableSessionResult(
            session.Id,
            session.StartTime,
            tableType.HourlyRate,
            tableType.Name,
            ticketId
        );
    }
}
