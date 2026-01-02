using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

public record GetCreditCardReportQuery(DateTime StartDate, DateTime EndDate, string? CardTypeFilter = null, string? TransactionTypeFilter = null);
