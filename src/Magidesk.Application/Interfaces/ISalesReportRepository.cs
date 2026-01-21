using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Interfaces;

public interface ISalesReportRepository
{
    Task<SalesBalanceReportDto> GetSalesBalanceAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<SalesSummaryReportDto> GetSalesSummaryAsync(DateTime startDate, DateTime endDate, bool includeGroups, CancellationToken cancellationToken = default);
    Task<SalesDetailReportDto> GetSalesDetailAsync(DateTime startDate, DateTime endDate, string? categoryFilter = null, string? groupFilter = null, string? itemFilter = null, CancellationToken cancellationToken = default);
    Task<ExceptionsReportDto> GetExceptionsReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<JournalReportDto> GetJournalReportAsync(DateTime startDate, DateTime endDate, string? entityType, Guid? userId, CancellationToken cancellationToken = default);
    Task<ProductivityReportDto> GetServerProductivityAsync(DateTime startDate, DateTime endDate, Guid? userId, CancellationToken cancellationToken = default);
    Task<LaborReportDto> GetLaborReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<DeliveryReportDto> GetDeliveryReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<CreditCardReportDto> GetCreditCardReportAsync(DateTime startDate, DateTime endDate, string? cardTypeFilter = null, string? transactionTypeFilter = null, CancellationToken cancellationToken = default);
    Task<PaymentReportDto> GetPaymentReportAsync(DateTime startDate, DateTime endDate, string? terminalFilter = null, CancellationToken cancellationToken = default);
    Task<MenuUsageReportDto> GetMenuUsageReportAsync(DateTime startDate, DateTime endDate, string? categoryFilter = null, string? orderTypeFilter = null, CancellationToken cancellationToken = default);
    Task<ServerProductivityReportDto> GetServerProductivityReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default);
    Task<HourlyLaborReportDto> GetHourlyLaborReportAsync(DateTime startDate, DateTime endDate, Guid? employeeIdFilter = null, CancellationToken cancellationToken = default);
    Task<TipReportDto> GetTipReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default);
    Task<AttendanceReportDto> GetAttendanceReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default);
    Task<CashOutReportDto> GetCashOutReportAsync(DateTime startDate, DateTime endDate, Guid? userIdFilter = null, CancellationToken cancellationToken = default);
}
