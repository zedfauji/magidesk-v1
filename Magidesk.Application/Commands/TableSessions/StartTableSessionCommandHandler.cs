using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for starting a new table session.
/// </summary>
public class StartTableSessionCommandHandler : ICommandHandler<StartTableSessionCommand, StartTableSessionResult>
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ITableTypeRepository _tableTypeRepository;

    public StartTableSessionCommandHandler(
        ITableSessionRepository sessionRepository,
        ITableRepository tableRepository,
        ITableTypeRepository tableTypeRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _tableRepository = tableRepository ?? throw new ArgumentNullException(nameof(tableRepository));
        _tableTypeRepository = tableTypeRepository ?? throw new ArgumentNullException(nameof(tableTypeRepository));
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

        // 5. Create table session
        var session = TableSession.Start(
            command.TableId,
            tableType.Id,
            tableType.HourlyRate,
            command.GuestCount,
            command.CustomerId,
            command.TicketId
        );

        await _sessionRepository.AddAsync(session);

        // 6. Update table status to InUse
        table.MarkInUse();
        await _tableRepository.UpdateAsync(table, cancellationToken);

        // 7. Return result
        return new StartTableSessionResult(
            session.Id,
            session.StartTime,
            tableType.HourlyRate,
            tableType.Name
        );
    }
}
