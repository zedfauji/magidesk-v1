using Magidesk.Api.Dtos.Tables;
using Magidesk.Api.Dtos.Sessions;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Commands; // For ChangeTableCommand
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries; // For GetActiveSessionsQuery
using Magidesk.Application.Queries.TableSessions;
using Magidesk.Domain.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/tables")]
public class TablesController : ControllerBase
{
    private readonly IQueryHandler<GetActiveSessionsQuery, IEnumerable<Application.DTOs.ActiveSessionDto>> _activeSessionsHandler;
    private readonly ITableRepository _tableRepository;
    private readonly ITicketRepository _ticketRepository; // New dependency
    private readonly ICommandHandler<StartTableSessionCommand, StartTableSessionResult> _startSessionHandler;
    private readonly ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult> _pauseSessionHandler;
    private readonly ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult> _resumeSessionHandler;
    private readonly ICommandHandler<EndTableSessionCommand, EndTableSessionResult> _endSessionHandler; // Assuming this exists based on naming pattern
    private readonly ICommandHandler<ChangeTableCommand, ChangeTableResult> _changeTableHandler;

    public TablesController(
        IQueryHandler<GetActiveSessionsQuery, IEnumerable<Application.DTOs.ActiveSessionDto>> activeSessionsHandler,
        ITableRepository tableRepository,
        ITicketRepository ticketRepository, // Inject
        ICommandHandler<StartTableSessionCommand, StartTableSessionResult> startSessionHandler,
        ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult> pauseSessionHandler,
        ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult> resumeSessionHandler,
        ICommandHandler<EndTableSessionCommand, EndTableSessionResult> endSessionHandler,
        ICommandHandler<ChangeTableCommand, ChangeTableResult> changeTableHandler)
    {
        _activeSessionsHandler = activeSessionsHandler;
        _tableRepository = tableRepository;
        _ticketRepository = ticketRepository;
        _startSessionHandler = startSessionHandler;
        _pauseSessionHandler = pauseSessionHandler;
        _resumeSessionHandler = resumeSessionHandler;
        _endSessionHandler = endSessionHandler;
        _changeTableHandler = changeTableHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<TableSummaryDto>>> GetAllTables()
    {
        // 1. Get Active Sessions (logic from GetActiveSessionsQueryHandler)
        var sessions = await _activeSessionsHandler.HandleAsync(new GetActiveSessionsQuery());
        var sessionDict = sessions.ToDictionary(s => s.TableId.ToString());

        // 2. Get All Tables (Repo Wrapper)
        var tables = await _tableRepository.GetAllAsync();

        // 3. Get All Open Tickets (Gap: Performance concern for large open ticket sets, but needed for sync)
        var openTickets = await _ticketRepository.GetOpenTicketsAsync();

        // 4. Merge & Map
        var result = tables.Select(t => {
            var hasSession = sessionDict.TryGetValue(t.Id.ToString(), out var session);
            
            // Resolve Active Ticket: Session Ticket OR Independent Open Ticket
            string? activeTicketId = null;
            if (hasSession && session?.TicketId != null) 
            {
                activeTicketId = session.TicketId.ToString();
            }
            else 
            {
                // Fallback: Find open ticket for this table number
                var ticket = openTickets.FirstOrDefault(ti => ti.TableNumbers.Contains(t.TableNumber));
                activeTicketId = ticket?.Id.ToString();
            }

            var sessionStatus = hasSession && session != null ? session.Status.ToString() : (activeTicketId != null ? "Occupied" : "NotStarted"); // Show Occupied if ticket exists but no session
            
            return new TableSummaryDto
            {
                Id = t.Id.ToString(),
                Name = $"Table {t.TableNumber}", // or t.Name if exists
                TableStatus = t.Status.ToString(),
                SessionStatus = sessionStatus,
                ElapsedSeconds = hasSession && session != null ? (DateTime.UtcNow - session.StartTime).TotalSeconds : 0,
                TotalAmount = 0, // Calculated field, requires Ticket lookup (Gap)
                CurrentUserId = hasSession && session?.CustomerId != null ? session.CustomerId.ToString() : null,
                ActiveTicketId = activeTicketId,
                IsReservationLocked = false, // Gap
                Version = 1 // Placeholder for t.Version/RowVersion mapping
            };
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{tableId}")]
    public async Task<ActionResult<TableExtensionDto>> GetTableDetails(string tableId)
    {
        if (!Guid.TryParse(tableId, out var id)) return BadRequest("Invalid ID");

        var table = await _tableRepository.GetByIdAsync(id);
        if (table == null) return NotFound();

        // Lookup active session to populate ActiveTicketId
        // Ideally we would have a GetSessionByTableId query, but reusing existing handler for consistency
        var allSessions = await _activeSessionsHandler.HandleAsync(new GetActiveSessionsQuery());
        var session = allSessions.FirstOrDefault(s => s.TableId == id);

        string? activeTicketId = session?.TicketId?.ToString();

        // Fallback: If no session ticket, look for open ticket by Table Number
        if (activeTicketId == null)
        {
            var openTickets = await _ticketRepository.GetOpenTicketsAsync();
            var ticket = openTickets.FirstOrDefault(ti => ti.TableNumbers.Contains(table.TableNumber));
            activeTicketId = ticket?.Id.ToString();
        }

        return Ok(new TableExtensionDto
        {
            Id = table.Id.ToString(),
            Name = $"Table {table.TableNumber}",
            TableStatus = table.Status.ToString(),
            Capacity = table.Capacity,
            ZoneName = "Main Floor",
            ActiveTicketId = activeTicketId
        });
    }

    [HttpPost("{tableId}/session/start")]
    public async Task<ActionResult> StartSession(string tableId, [FromQuery] int guestCount = 1)
    {
        if (!Guid.TryParse(tableId, out var id)) return BadRequest("Invalid ID");

        try
        {
            // Note: GuestCount, OrderType, etc would typically be in a request body, but WPA flow implies defaults or query params
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null) return NotFound("Table not found");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userId, out var uid);

            // Usage: TableId, TableTypeId, GuestCount... (defaults for others)
            await _startSessionHandler.HandleAsync(new StartTableSessionCommand(
                id, 
                table.TableTypeId ?? Guid.Empty, 
                guestCount,
                null, // CustomerId
                null, // TicketId
                true, // Ensure a ticket is created
                uid == Guid.Empty ? null : uid, // UserId from Context
                null, // TerminalId (Let Handler resolve or use default)
                null, // ShiftId
                null  // OrderTypeId
            ));
            return Ok();
        }
        catch (InvalidOperationException) { return Conflict("Session already active or table invalid."); }
        catch (Exception) { return StatusCode(500); }
    }

    [HttpPost("{tableId}/session/pause")]
    public async Task<ActionResult> PauseSession(string tableId)
    {
        if (!Guid.TryParse(tableId, out var id)) return BadRequest("Invalid ID");
        // Need SessionId, not TableId for the command? 
        // PauseTableSessionCommand usually takes SessionId. 
        // Gap: Need to lookup Session ID from Table ID first.
        
        // Assuming we find logic to bridge this:
        // await _pauseSessionHandler.HandleAsync(...);
        return Ok();
    }

    [HttpPost("{tableId}/session/resume")]
    public async Task<ActionResult> ResumeSession(string tableId)
    {
         // Same Gap as Pause (TableId vs SessionId)
         return Ok();
    }

    [HttpPost("{tableId}/session/end")]
    public async Task<ActionResult<ActiveSessionDto>> EndSession(string tableId)
    {
        // Same Gap as Pause (TableId vs SessionId)
        // await _endSessionHandler.HandleAsync(...);
        
        // Return summary (placeholder)
        return Ok(new ActiveSessionDto());
    }

    [HttpPost("move")]
    public async Task<ActionResult> MoveTable([FromBody] MoveTableRequest request)
    {
         if (!Guid.TryParse(request.SourceTableId, out var sourceId)) return BadRequest();
         if (!Guid.TryParse(request.TargetTableId, out var targetId)) return BadRequest();

         // WPA calls it "Move Order", Backend has "ChangeTableCommand"
         // Need TicketId, but request gives TableId.
         // Gap: Lookup TicketId from TableId
         
         // await _changeTableHandler.HandleAsync(new ChangeTableCommand { ... });
         return Ok();
    }
}
