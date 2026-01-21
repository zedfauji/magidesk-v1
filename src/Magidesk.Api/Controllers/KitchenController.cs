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
    private readonly IKitchenOrderRepository _orderRepository;

    public KitchenController(
        IKitchenRoutingService routingService, 
        IKitchenStatusService statusService,
        IKitchenOrderRepository orderRepository)
    {
        _routingService = routingService;
        _statusService = statusService;
        _orderRepository = orderRepository;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        try
        {
            var orders = await _orderRepository.GetActiveOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
