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
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicketHandler;
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicketHandler;
    private readonly ITableRepository _tableRepository; // New dependency
    private readonly Magidesk.Infrastructure.Data.ApplicationDbContext _dbContext;

    public OrdersController(
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLineHandler,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler,
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        ITableRepository tableRepository, // Inject
        Magidesk.Infrastructure.Data.ApplicationDbContext dbContext)
    {
        _addOrderLineHandler = addOrderLineHandler;
        _createTicketHandler = createTicketHandler;
        _getTicketHandler = getTicketHandler;
        _tableRepository = tableRepository;
        _dbContext = dbContext;
    }

    [HttpPost("{ticketId}/lines")]
    public async Task<ActionResult<TicketResultDto>> SendOrderToKitchen(string ticketId, [FromBody] AddLinesRequest request)
    {
        // ... (existing implementation)
        if (!Guid.TryParse(ticketId, out var tId)) return BadRequest("Invalid Ticket ID");

        // Transaction Strategy: Wrap loop in explicit transaction to prevent partial commits
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try 
        {
            foreach (var item in request.Items)
            {
                if (!Guid.TryParse(item.MenuItemId, out var mId)) continue;
                
                await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
                {
                    TicketId = tId,
                    MenuItemId = mId,
                    Quantity = item.Quantity,
                    UnitPrice = new Magidesk.Domain.ValueObjects.Money(item.UnitPrice), 
                });
            }

            await transaction.CommitAsync();
            
            return Ok(new TicketResultDto
            {
                Success = true,
                TicketId = ticketId,
                UpdatedVersion = 2 
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
            return StatusCode(500, "Error adding item batch.");
        }
    }

    [HttpPost("tickets")]
    public async Task<ActionResult<TicketResultDto>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        // 1. Resolve effective User ID
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(userIdString, out var userId);
        if (userId == Guid.Empty) userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Fallback for dev

        // 2. Validate Table and Get Table Number
        if (!Guid.TryParse(request.TableId, out var tableId)) return BadRequest("Invalid Table ID");
        
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return BadRequest("Table not found");

        // 3. Create Ticket Command
        var command = new CreateTicketCommand
        {
            TableId = tableId,
            TableNumbers = new List<int> { table.TableNumber }, // Resolved Table Number
            CustomerId = null, // Or from request
            NumberOfGuests = request.GuestCount,
            CreatedBy = new Magidesk.Domain.ValueObjects.UserId(userId),
            TerminalId = Guid.Empty, // Let Handler resolve default
            ShiftId = Guid.Empty,    // Let Handler resolve default
            OrderTypeId = Guid.Empty, // Let Handler resolve default
            Note = "Quick Order Ticket"
        };
        
        try 
        {
            // 4. Handle
            var result = await _createTicketHandler.HandleAsync(command);
            
            return Ok(new TicketResultDto
            {
                Success = true,
                TicketId = result.TicketId.ToString(),
                UpdatedVersion = 1
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("tickets/{ticketId}")]
    public async Task<ActionResult<Magidesk.Api.Dtos.Sessions.ActiveSessionDto>> GetTicket(string ticketId)
    {
        if (!Guid.TryParse(ticketId, out var id)) return BadRequest("Invalid ID");

        var ticketDto = await _getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = id });

        if (ticketDto == null) return NotFound();

        // Map TicketDto (Backend) to ActiveSessionDto (Frontend Contract)
        return Ok(new Magidesk.Api.Dtos.Sessions.ActiveSessionDto
        {
            TicketId = ticketDto.Id.ToString(),
            TicketNumber = ticketDto.TicketNumber.ToString(),
            StartTime = ticketDto.CreatedAt.ToString("O"), 
            TableId = ticketDto.TableNumbers.FirstOrDefault().ToString(), 
            IsPaused = ticketDto.SessionStatus == Magidesk.Domain.Enumerations.TableSessionStatus.Paused,
            HourlyRate = ticketDto.SessionHourlyRate ?? 0,
            
            CommittedItems = ticketDto.OrderLines.Select(ol => new Magidesk.Api.Dtos.Sessions.CommittedOrderLineDto
            {
                Id = ol.Id.ToString(),
                MenuItemId = ol.MenuItemId.ToString(),
                Name = ol.MenuItemName,
                Quantity = ol.Quantity,
                UnitPrice = ol.UnitPrice,
                Total = ol.TotalAmount,
                Modifiers = ol.Modifiers.Select(m => new Magidesk.Api.Dtos.Sessions.SelectedModifierDto
                {
                    Name = m.Name,
                    PriceDelta = m.UnitPrice,
                    GroupId = m.SectionName ?? "", // Helper to group if SectionName provided
                    OptionId = m.ModifierId?.ToString() ?? Guid.Empty.ToString()
                }).ToList()
            }).ToList(),

            Totals = new SessionTotalsDto
            {
                SessionTimeAmount = ticketDto.SessionRunningCharge ?? 0,
                FnBSubtotal = ticketDto.SubtotalAmount, // Assuming this serves as F&B subtotal
                Tax = ticketDto.TaxAmount,
                GrandTotal = ticketDto.TotalAmount
            }
        });
    }
}
