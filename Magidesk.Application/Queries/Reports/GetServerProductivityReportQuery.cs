using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

public record GetServerProductivityReportQuery(DateTime StartDate, DateTime EndDate, Guid? UserIdFilter = null);
