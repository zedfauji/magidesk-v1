using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Interfaces;

public interface ISalesReportRepository
{
    Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<SalesSummaryReportDto> GetSalesSummaryAsync(DateTime startDate, DateTime endDate, bool includeGroups, CancellationToken cancellationToken = default);
    Task<ExceptionsReportDto> GetExceptionsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<JournalReportDto> GetJournalReportAsync(DateTime startDate, DateTime endDate, string? entityType, Guid? userId, CancellationToken cancellationToken = default);
    Task<ProductivityReportDto> GetServerProductivityAsync(DateTime startDate, DateTime endDate, Guid? userId, CancellationToken cancellationToken = default);
    Task<LaborReportDto> GetLaborReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<DeliveryReportDto> GetDeliveryReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
