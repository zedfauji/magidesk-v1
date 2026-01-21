using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

public record GetHourlyLaborReportQuery(DateTime StartDate, DateTime EndDate, Guid? EmployeeIdFilter = null);
