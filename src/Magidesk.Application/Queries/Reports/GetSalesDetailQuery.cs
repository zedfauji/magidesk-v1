using Magidesk.Application.DTOs.Reports;
namespace Magidesk.Application.Queries.Reports;
 
public record GetSalesDetailQuery(DateTime StartDate, DateTime EndDate, string? CategoryFilter = null, string? GroupFilter = null, string? ItemFilter = null);
