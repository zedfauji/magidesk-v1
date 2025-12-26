using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Presentation.ViewModels;

public sealed class DrawerPullReportViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult> _getReport;

    private string _cashSessionIdText = string.Empty;
    private DrawerPullReportDto? _report;
    private string? _error;

    public DrawerPullReportViewModel(IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult> getReport)
    {
        _getReport = getReport;
        Title = "Drawer Pull Report";

        LoadReportCommand = new AsyncRelayCommand(LoadReportAsync);
    }

    public string CashSessionIdText
    {
        get => _cashSessionIdText;
        set => SetProperty(ref _cashSessionIdText, value);
    }

    public DrawerPullReportDto? Report
    {
        get => _report;
        private set
        {
            if (SetProperty(ref _report, value))
            {
                OnPropertyChanged(nameof(HasReport));
                OnPropertyChanged(nameof(HeaderText));
                OnPropertyChanged(nameof(SummaryText));
                OnPropertyChanged(nameof(Payouts));
                OnPropertyChanged(nameof(CashDrops));
                OnPropertyChanged(nameof(DrawerBleeds));
            }
        }
    }

    public bool HasReport => Report != null;

    public string HeaderText => Report == null
        ? "No report loaded"
        : $"CashSession: {Report.CashSessionId}\nUser: {Report.UserId}\nOpened: {Report.OpenedAt}  Closed: {Report.ClosedAt}";

    public string SummaryText => Report == null
        ? string.Empty
        : $"Opening: {Report.OpeningBalance}  Expected: {Report.ExpectedCash}  Actual: {Report.ActualCash}  Diff: {Report.Difference}\n" +
          $"Cash receipts: {Report.TotalCashReceipts}  Cash refunds: {Report.TotalCashRefunds}  Cash payments: {Report.CashPaymentCount}\n" +
          $"Payouts: {Report.TotalPayouts}  Drops: {Report.TotalCashDrops}  Bleeds: {Report.TotalDrawerBleeds}";

    public IReadOnlyList<PayoutDto> Payouts => Report?.Payouts ?? new List<PayoutDto>();
    public IReadOnlyList<CashDropDto> CashDrops => Report?.CashDrops ?? new List<CashDropDto>();
    public IReadOnlyList<DrawerBleedDto> DrawerBleeds => Report?.DrawerBleeds ?? new List<DrawerBleedDto>();

    public string? Error
    {
        get => _error;
        private set
        {
            if (SetProperty(ref _error, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(Error);

    public AsyncRelayCommand LoadReportCommand { get; }

    private async Task LoadReportAsync()
    {
        Error = null;
        IsBusy = true;

        try
        {
            if (!Guid.TryParse(CashSessionIdText, out var cashSessionId))
            {
                Error = "Invalid CashSessionId.";
                return;
            }

            var result = await _getReport.HandleAsync(new GetDrawerPullReportQuery { CashSessionId = cashSessionId });
            Report = result.Report;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
