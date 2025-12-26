using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Interfaces;

public interface ISalesReportRepository
{
    Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
