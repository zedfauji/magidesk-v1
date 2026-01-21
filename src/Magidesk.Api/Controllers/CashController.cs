using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/cash")]
public class CashController : ControllerBase
{
    private readonly ICashSessionService _cashSessionService;

    public CashController(ICashSessionService cashSessionService)
    {
        _cashSessionService = cashSessionService;
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> StartSession([FromBody] StartSessionRequest request)
    {
        try
        {
            // Assuming UserId is passed in request or derived from Context.
            // For MVP, passing in body.
            var session = await _cashSessionService.StartSessionAsync(
                new UserId(request.UserId), 
                request.TerminalId, 
                request.ShiftId, 
                new Money(request.OpeningAmount, "USD")); // Hardcoded currency for now
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("sessions/{id}/close")]
    public async Task<IActionResult> CloseSession(Guid id, [FromBody] CloseSessionRequest request)
    {
        try
        {
             var session = await _cashSessionService.CloseSessionAsync(
                 id, 
                 new UserId(request.ClosedByUserId), 
                 new Money(request.ClosingAmount, "USD"));
             return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("sessions/active/user/{userId}")]
    public async Task<IActionResult> GetActiveSessionByUser(Guid userId)
    {
        var session = await _cashSessionService.GetActiveSessionByUserAsync(new UserId(userId));
        if (session == null) return NotFound();
        return Ok(session);
    }
}

public class StartSessionRequest
{
    public Guid UserId { get; set; }
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public decimal OpeningAmount { get; set; }
}

public class CloseSessionRequest
{
    public Guid ClosedByUserId { get; set; }
    public decimal ClosingAmount { get; set; }
}
