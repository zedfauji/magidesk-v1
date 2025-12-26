using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("sales-balance")]
    public async Task<ActionResult<SalesBalanceReportDto>> GetSalesBalance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetSalesBalanceQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
