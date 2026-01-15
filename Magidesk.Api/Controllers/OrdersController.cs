using Magidesk.Api.Dtos.Orders;
using Magidesk.Api.Dtos.Sessions;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ICommandHandler<AddOrderLineCommand, AddOrderLineResult> _addOrderLineHandler;
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicketHandler;
    private readonly Magidesk.Infrastructure.Data.ApplicationDbContext _dbContext;

    public OrdersController(
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLineHandler,
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        Magidesk.Infrastructure.Data.ApplicationDbContext dbContext)
    {
        _addOrderLineHandler = addOrderLineHandler;
        _getTicketHandler = getTicketHandler;
        _dbContext = dbContext;
    }

    [HttpPost("{ticketId}/lines")]
    public async Task<ActionResult<TicketResultDto>> SendOrderToKitchen(string ticketId, [FromBody] AddLinesRequest request)
    {
        if (!Guid.TryParse(ticketId, out var tId)) return BadRequest("Invalid Ticket ID");

        // Transaction Strategy: Wrap loop in explicit transaction to prevent partial commits
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try 
        {
            // Loop strategy to handle Batch vs Single handler gap
            foreach (var item in request.Items)
            {
                if (!Guid.TryParse(item.MenuItemId, out var mId)) continue;
                
                await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
                {
                    TicketId = tId,
                    MenuItemId = mId,
                    Quantity = item.Quantity,
                    // Gap: UnitPrice should come from Backend lookup, not trust Client. 
                    // However, Command expects Money. Using client value or default for plumbing.
                    UnitPrice = new Magidesk.Domain.ValueObjects.Money(item.UnitPrice), 
                    
                    // Gap: Modifiers mapping
                    // Modifiers = ... (Need to map DTO modifiers to Command modifiers)
                    
                    // Context User? Provided by DI IUserService scope
                    // Context Terminal? Provided by DI ITerminalContext scope
                });
            }

            // Commit if all items succeed
            await transaction.CommitAsync();
            
            return Ok(new TicketResultDto
            {
                Success = true,
                TicketId = ticketId,
                UpdatedVersion = 2 // Placeholder, normally fetch from updated ticket
            });
        }
        catch (Magidesk.Domain.Exceptions.BusinessRuleViolationException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest($"Logic Error: {ex.Message}");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            // In a real loop, might want partial success or atomic transaction.
            // Returning 500 for now.
            return StatusCode(500, "Error adding item batch.");
        }
    }

    [HttpGet("tickets/{ticketId}")]
    public async Task<ActionResult<ActiveSessionDto>> GetTicket(string ticketId)
    {
        if (!Guid.TryParse(ticketId, out var id)) return BadRequest("Invalid ID");

        var ticketDto = await _getTicketHandler.HandleAsync(new GetTicketQuery(id));

        if (ticketDto == null) return NotFound();

        // Map TicketDto (Backend) to ActiveSessionDto (Frontend Contract)
        return Ok(new ActiveSessionDto
        {
            TicketId = ticketDto.Id.ToString(),
            TicketNumber = ticketDto.TicketNumber.ToString(),
            StartTime = ticketDto.CreatedAt.ToString("O"), // or SessionStartTime
            // ... Mappings for totals, items, etc.
            Totals = new SessionTotalsDto
            {
                GrandTotal = ticketDto.TotalAmount
            }
        });
    }
}
