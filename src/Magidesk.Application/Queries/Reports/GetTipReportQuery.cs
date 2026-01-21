using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries.Reports;

public record GetTipReportQuery(DateTime StartDate, DateTime EndDate, Guid? UserIdFilter = null);
