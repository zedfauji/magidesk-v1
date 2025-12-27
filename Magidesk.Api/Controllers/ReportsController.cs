using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.Reports;

using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly Application.Interfaces.IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto> _salesBalanceHandler;

    public ReportsController(Application.Interfaces.IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto> salesBalanceHandler)
    {
        _salesBalanceHandler = salesBalanceHandler;
    }

    [HttpGet("sales-balance")]
    public async Task<ActionResult<SalesBalanceReportDto>> GetSalesBalance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetSalesBalanceQuery(startDate, endDate);

        var result = await _salesBalanceHandler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("sales-summary")]
    public async Task<ActionResult<SalesSummaryReportDto>> GetSalesSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] bool includeGroups = true, [FromServices] Application.Interfaces.IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> handler = null!)
    {
        var query = new GetSalesSummaryQuery(startDate, endDate, includeGroups);
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("exceptions")]
    public async Task<ActionResult<ExceptionsReportDto>> GetExceptionsReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromServices] Application.Interfaces.IQueryHandler<GetExceptionsReportQuery, ExceptionsReportDto> handler = null!)
    {
        var query = new GetExceptionsReportQuery(startDate, endDate);
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("journal")]
    public async Task<ActionResult<JournalReportDto>> GetJournalReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        [FromQuery] string? entityType, 
        [FromQuery] Guid? userId,
        [FromServices] Application.Interfaces.IQueryHandler<GetJournalReportQuery, JournalReportDto> handler = null!)
    {
        var query = new GetJournalReportQuery(startDate, endDate, entityType, userId);
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("productivity")]
    public async Task<ActionResult<ProductivityReportDto>> GetProductivityReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        [FromQuery] Guid? userId,
        [FromServices] Application.Interfaces.IQueryHandler<GetProductivityReportQuery, ProductivityReportDto> handler = null!)
    {
        var query = new GetProductivityReportQuery(startDate, endDate, userId);
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }
    [HttpGet("labor")]
    public async Task<ActionResult<LaborReportDto>> GetLaborReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate,
        [FromServices] Application.Interfaces.IQueryHandler<GetLaborReportQuery, LaborReportDto> handler)
    {
        var query = new GetLaborReportQuery(startDate, endDate);
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("delivery")]
    public async Task<ActionResult<DeliveryReportDto>> GetDeliveryReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate,
        [FromServices] Application.Interfaces.IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> handler)
    {
        var query = new GetDeliveryReportQuery(startDate, endDate);
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }
}
