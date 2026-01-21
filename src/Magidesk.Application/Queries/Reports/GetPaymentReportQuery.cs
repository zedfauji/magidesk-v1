using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

public record GetPaymentReportQuery(DateTime StartDate, DateTime EndDate, string? TerminalFilter = null);
