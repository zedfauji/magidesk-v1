using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for ending a table session and calculating charges.
/// </summary>
public class EndTableSessionCommandHandler : ICommandHandler<EndTableSessionCommand, EndTableSessionResult>
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IPricingService _pricingService;
    private readonly ILogger<EndTableSessionCommandHandler> _logger;

    public EndTableSessionCommandHandler(
        ITableSessionRepository sessionRepository,
        ITableRepository tableRepository,
        ITicketRepository ticketRepository,
        IPricingService pricingService,
        ILogger<EndTableSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _tableRepository = tableRepository ?? throw new ArgumentNullException(nameof(tableRepository));
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EndTableSessionResult> HandleAsync(
        EndTableSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Get session
        var session = await _sessionRepository.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session {command.SessionId} not found.");
        }

        // 2. Validate session not already ended
        if (session.Status == TableSessionStatus.Ended)
        {
            throw new InvalidOperationException($"Session {command.SessionId} has already ended.");
        }

        // 3. Get table
        var table = await _tableRepository.GetByIdAsync(session.TableId);
        if (table == null)
        {
            throw new InvalidOperationException($"Table {session.TableId} not found.");
        }

        // 4. Calculate billable time
        var billableTime = session.GetBillableTime();

        // 5. Calculate charge using PricingService
        var totalCharge = _pricingService.CalculateTimeCharge(billableTime, session.HourlyRate);

        // 6. End session (domain method enforces invariants)
        session.End(totalCharge);

        // 7. Create or update ticket with time line item
        Guid? ticketId = null;
        
        // Check if session is already linked to a ticket (Feature C.1)
        if (session.TicketId.HasValue)
        {
             await AddTimeChargeToTicketAsync(session.TicketId.Value, session, totalCharge, billableTime, cancellationToken);
             ticketId = session.TicketId.Value;
        }
        else if (command.CreateTicket)
        {
            ticketId = await CreateTicketWithTimeChargeAsync(session, totalCharge, billableTime, command, cancellationToken);
            // Ensure session is linked to the new ticket
            session.LinkToTicket(ticketId.Value);
        }
        else
        {
             _logger.LogWarning("Session ended without ticket creation (CreateTicket=false and no linked ticket).");
        }

        // 8. Save session (persists End state and Ticket Link)
        await _sessionRepository.UpdateAsync(session);

        // 9. Update table status to Available
        table.MarkAvailable();
        await _tableRepository.UpdateAsync(table);

        _logger.LogInformation(
            "Ended session {SessionId} for table {TableId}. Duration: {Duration}, Charge: {Charge}",
            session.Id, session.TableId, billableTime, totalCharge);

        // 10. Return result
        return new EndTableSessionResult(
            session.Id,
            ticketId,
            billableTime,
            totalCharge,
            session.EndTime!.Value
        );
    }

    private async Task AddTimeChargeToTicketAsync(
        Guid ticketId,
        TableSession session,
        Money totalCharge,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
        {
            _logger.LogWarning("Linked ticket {TicketId} not found. Cannot add time charge.", ticketId);
            return;
        }

        var timeChargeLine = OrderLine.CreateTimeCharge(
            ticket.Id,
            duration,
            session.HourlyRate,
            totalCharge
        );

        ticket.AddOrderLine(timeChargeLine);
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }

    private async Task<Guid> CreateTicketWithTimeChargeAsync(
        TableSession session,
        Money totalCharge,
        TimeSpan duration,
        EndTableSessionCommand command, // Pass full command for context
        CancellationToken cancellationToken)
    {
        // Validate required context for ticket creation
        if (!command.UserId.HasValue || !command.TerminalId.HasValue || !command.ShiftId.HasValue || !command.OrderTypeId.HasValue)
        {
            throw new InvalidOperationException("Cannot create ticket: Missing required context (UserId, TerminalId, ShiftId, OrderTypeId).");
        }

        // Get next ticket number
        var ticketNumber = await _ticketRepository.GetNextTicketNumberAsync(cancellationToken);

        var ticket = Ticket.Create(
            ticketNumber: ticketNumber,
            createdBy: new UserId(command.UserId.Value),
            terminalId: command.TerminalId.Value,
            shiftId: command.ShiftId.Value,
            orderTypeId: command.OrderTypeId.Value
        );

        // Assign table to ticket
        var table = await _tableRepository.GetByIdAsync(session.TableId);
        if (table != null)
        {
            ticket.AssignTable(table.TableNumber);
        }

        // Link session
        ticket.SetSession(session.Id);
        
        // Auto-assign customer from session
        if (session.CustomerId.HasValue)
        {
            ticket.SetCustomer(session.CustomerId.Value);
        }

        // Add time-based line item
        // F-C.2: Using dedicated factory method
        var timeChargeLine = OrderLine.CreateTimeCharge(
            ticket.Id,
            duration,
            session.HourlyRate,
            totalCharge
        );

        ticket.AddOrderLine(timeChargeLine);

        await _ticketRepository.AddAsync(ticket);

        return ticket.Id;
    }
}
