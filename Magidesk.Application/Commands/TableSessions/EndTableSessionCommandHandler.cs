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

        // 7. Save session
        await _sessionRepository.UpdateAsync(session);

        // 8. Create or update ticket with time line item
        Guid? ticketId = null;
        if (command.CreateTicket)
        {
            ticketId = await CreateTicketWithTimeChargeAsync(session, totalCharge, billableTime, cancellationToken);
        }
        else
        {
            // TODO: Add to existing ticket logic (requires ticket selection)
            _logger.LogWarning("Add to existing ticket not yet implemented. Creating new ticket instead.");
            ticketId = await CreateTicketWithTimeChargeAsync(session, totalCharge, billableTime, cancellationToken);
        }

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

    private async Task<Guid> CreateTicketWithTimeChargeAsync(
        TableSession session,
        Money totalCharge,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        // TODO: Get proper values from context/session
        // For now, using placeholder values - full implementation needs:
        // - Proper ticket number generation
        // - Current user from context
        // - Terminal ID from context
        // - Shift ID from context
        // - OrderType ID from repository (DineIn)
        
        var ticket = Ticket.Create(
            ticketNumber: 1, // TODO: Get next ticket number from repository
            createdBy: new UserId(Guid.Parse("00000000-0000-0000-0000-000000000001")), // TODO: Get current user
            terminalId: Guid.Parse("00000000-0000-0000-0000-000000000001"), // TODO: Get terminal from context
            shiftId: Guid.Parse("00000000-0000-0000-0000-000000000001"), // TODO: Get shift from context
            orderTypeId: Guid.Parse("00000000-0000-0000-0000-000000000001") // TODO: Get DineIn OrderType from repository
        );

        // Assign table to ticket
        // TODO: Get table number from table entity
        // For now, using placeholder - full implementation needs to get Table entity and use its TableNumber
        ticket.AssignTable(1); // TODO: Get table.TableNumber from table entity

        // Add time-based line item
        // TODO: This is a simplified version. Full implementation should use proper menu item for time charges
        // For now, we'll add a note to the ticket
        ticket.SetNote($"Table Time: {duration.TotalHours:F2} hours @ ${session.HourlyRate:F2}/hr = ${totalCharge.Amount:F2}");

        await _ticketRepository.AddAsync(ticket);

        return ticket.Id;
    }
}
