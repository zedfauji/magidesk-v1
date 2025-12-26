using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KitchenController : ControllerBase
{
    private readonly IKitchenRoutingService _routingService;
    private readonly IKitchenStatusService _statusService;

    public KitchenController(IKitchenRoutingService routingService, IKitchenStatusService statusService)
    {
        _routingService = routingService;
        _statusService = statusService;
    }

    [HttpPost("orders/{id}/bump")]
    public async Task<IActionResult> BumpOrder(Guid id)
    {
        try
        {
            await _statusService.BumpOrderAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("orders/{id}/void")]
    public async Task<IActionResult> VoidOrder(Guid id)
    {
        try
        {
            await _statusService.VoidOrderAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
