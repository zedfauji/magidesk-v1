using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries.Reports;

public record GetCashOutReportQuery(DateTime StartDate, DateTime EndDate, Guid? UserIdFilter = null);
