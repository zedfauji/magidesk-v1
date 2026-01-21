using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

public record GetMenuUsageReportQuery(DateTime StartDate, DateTime EndDate, string? CategoryFilter = null, string? OrderTypeFilter = null);
