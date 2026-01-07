using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.Reports;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;


namespace Magidesk.Presentation.ViewModels;

public enum ReportProperties
{
    SalesSummary,
    SalesDetail,
    SalesBalance,
    Exceptions,
    CreditCardReport,
    PaymentReport,
    MenuUsageReport,
    ServerProductivityReport,
    HourlyLaborReport,
    Journal,
    Productivity,
    Labor,
    Delivery,
    Tips,
    Attendance,
    CashOut
}

public partial class SalesReportsViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> _getSalesSummary;
    private readonly IQueryHandler<GetSalesDetailQuery, SalesDetailReportDto> _getSalesDetail;
    private readonly IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto> _getSalesBalance;
    private readonly IQueryHandler<GetExceptionsReportQuery, ExceptionsReportDto> _getExceptionsReport;
    private readonly IQueryHandler<GetCreditCardReportQuery, CreditCardReportDto> _getCreditCardReport;
    private readonly IQueryHandler<GetPaymentReportQuery, PaymentReportDto> _getPaymentReport;
    private readonly IQueryHandler<GetMenuUsageReportQuery, MenuUsageReportDto> _getMenuUsageReport;
    private readonly IQueryHandler<GetServerProductivityReportQuery, ServerProductivityReportDto> _getServerProductivityReport;
    private readonly IQueryHandler<GetHourlyLaborReportQuery, HourlyLaborReportDto> _getHourlyLaborReport;
    private readonly IQueryHandler<GetJournalReportQuery, JournalReportDto> _getJournalReport;
    private readonly IQueryHandler<GetProductivityReportQuery, ProductivityReportDto> _getProductivityReport;
    private readonly IQueryHandler<GetLaborReportQuery, LaborReportDto> _getLaborReport;
    private readonly IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> _getDeliveryReport;
    private readonly IQueryHandler<GetTipReportQuery, TipReportDto> _getTipReport;
    private readonly IQueryHandler<GetAttendanceReportQuery, AttendanceReportDto> _getAttendanceReport;
    private readonly IQueryHandler<GetCashOutReportQuery, CashOutReportDto> _getCashOutReport;

    public Services.LocalizationService Localization { get; }

    private DateTimeOffset _startDate = DateTime.Today;
    private DateTimeOffset _endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
    private ReportProperties _selectedReportType;
    private string _error = string.Empty;
    
    // Quick Date Commands
    public System.Windows.Input.ICommand SetDateRangeCommand { get; }


    // Report Data Containers
    private SalesSummaryReportDto? _salesSummary;
    private SalesDetailReportDto? _salesDetail;
    private SalesBalanceReportDto? _salesBalance;
    private ExceptionsReportDto? _exceptionsReport;
    private CreditCardReportDto? _creditCardReport;
    private PaymentReportDto? _paymentReport;
    private MenuUsageReportDto? _menuUsageReport;
    private ServerProductivityReportDto? _serverProductivityReport;
    private HourlyLaborReportDto? _hourlyLaborReport;
    private JournalReportDto? _journalReport;
    private ProductivityReportDto? _productivityReport;
    private LaborReportDto? _laborReport;
    private DeliveryReportDto? _deliveryReport;
    private TipReportDto? _tipReport;
    private AttendanceReportDto? _attendanceReport;
    private CashOutReportDto? _cashOutReport;

    public SalesReportsViewModel(
        IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> getSalesSummary,
        IQueryHandler<GetSalesDetailQuery, SalesDetailReportDto> getSalesDetail,
        IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto> getSalesBalance,
        IQueryHandler<GetExceptionsReportQuery, ExceptionsReportDto> getExceptionsReport,
        IQueryHandler<GetCreditCardReportQuery, CreditCardReportDto> getCreditCardReport,
        IQueryHandler<GetPaymentReportQuery, PaymentReportDto> getPaymentReport,
        IQueryHandler<GetMenuUsageReportQuery, MenuUsageReportDto> getMenuUsageReport,
        IQueryHandler<GetServerProductivityReportQuery, ServerProductivityReportDto> getServerProductivityReport,
        IQueryHandler<GetHourlyLaborReportQuery, HourlyLaborReportDto> getHourlyLaborReport,
        IQueryHandler<GetJournalReportQuery, JournalReportDto> getJournalReport,
        IQueryHandler<GetProductivityReportQuery, ProductivityReportDto> getProductivityReport,
        IQueryHandler<GetLaborReportQuery, LaborReportDto> getLaborReport,
        IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> getDeliveryReport,
        IQueryHandler<GetTipReportQuery, TipReportDto> getTipReport,
        IQueryHandler<GetAttendanceReportQuery, AttendanceReportDto> getAttendanceReport,
        IQueryHandler<GetCashOutReportQuery, CashOutReportDto> getCashOutReport,
        Services.LocalizationService localizationService)
    {
        _getSalesSummary = getSalesSummary;
        _getSalesDetail = getSalesDetail;
        _getSalesBalance = getSalesBalance;
        _getExceptionsReport = getExceptionsReport;
        _getCreditCardReport = getCreditCardReport;
        _getPaymentReport = getPaymentReport;
        _getMenuUsageReport = getMenuUsageReport;
        _getServerProductivityReport = getServerProductivityReport;
        _getHourlyLaborReport = getHourlyLaborReport;
        _getJournalReport = getJournalReport;
        _getProductivityReport = getProductivityReport;
        _getLaborReport = getLaborReport;
        _getDeliveryReport = getDeliveryReport;
        _getTipReport = getTipReport;
        _getAttendanceReport = getAttendanceReport;
        _getCashOutReport = getCashOutReport;
        Localization = localizationService;

        Title = "Reporting";
        LoadReportCommand = new AsyncRelayCommand(LoadReportAsync);
        SetDateRangeCommand = new RelayCommand<string>(SetDateRange);
    }

    private void SetDateRange(string range)
    {
        var now = DateTimeOffset.Now;
        switch (range?.ToLower())
        {
            case "today":
                StartDate = now.Date;
                EndDate = now.Date.AddDays(1).AddSeconds(-1);
                break;
            case "yesterday":
                StartDate = now.Date.AddDays(-1);
                EndDate = now.Date.AddSeconds(-1);
                break;
            case "week":
                // Start of week (Monday)
                var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                StartDate = now.Date.AddDays(-1 * diff);
                EndDate = now.Date.AddDays(1).AddSeconds(-1);
                break;
            case "month":
                 StartDate = new DateTime(now.Year, now.Month, 1);
                 EndDate = now.Date.AddDays(1).AddSeconds(-1);
                 break;
        }
        // Auto-load when range changes? Maybe not, sticking to manual "Run" is safer.
    }

    public DateTimeOffset StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTimeOffset EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public ReportProperties SelectedReportType
    {
        get => _selectedReportType;
        set
        {
            if (SetProperty(ref _selectedReportType, value))
            {
                // Clear previous data or auto-load?
                // Auto-load might be nice but let's stick to explicit "Run" for now to avoid accidental heavy queries
                OnPropertyChanged(nameof(IsSalesSummaryVisible));
                OnPropertyChanged(nameof(IsSalesDetailVisible));
                OnPropertyChanged(nameof(IsSalesBalanceVisible));
                OnPropertyChanged(nameof(IsExceptionsReportVisible));
                OnPropertyChanged(nameof(IsCreditCardReportVisible));
                OnPropertyChanged(nameof(IsPaymentReportVisible));
                OnPropertyChanged(nameof(IsMenuUsageReportVisible));
                OnPropertyChanged(nameof(IsServerProductivityReportVisible));
                OnPropertyChanged(nameof(IsHourlyLaborReportVisible));
                OnPropertyChanged(nameof(IsJournalReportVisible));
                OnPropertyChanged(nameof(IsProductivityReportVisible));
                OnPropertyChanged(nameof(IsLaborReportVisible));
                OnPropertyChanged(nameof(IsDeliveryReportVisible));
                OnPropertyChanged(nameof(IsTipReportVisible));

                OnPropertyChanged(nameof(HasSalesSummary));
                OnPropertyChanged(nameof(HasSalesDetail));
                OnPropertyChanged(nameof(HasSalesBalance));
                OnPropertyChanged(nameof(HasExceptionsReport));
                OnPropertyChanged(nameof(HasCreditCardReport));
                OnPropertyChanged(nameof(HasPaymentReport));
                OnPropertyChanged(nameof(HasMenuUsageReport));
                OnPropertyChanged(nameof(HasServerProductivityReport));
                OnPropertyChanged(nameof(HasHourlyLaborReport));
                OnPropertyChanged(nameof(HasJournalReport));
                OnPropertyChanged(nameof(HasProductivityReport));
                OnPropertyChanged(nameof(HasLaborReport));
                OnPropertyChanged(nameof(HasDeliveryReport));
                OnPropertyChanged(nameof(HasTipReport));
                OnPropertyChanged(nameof(HasAttendanceReport));
                OnPropertyChanged(nameof(HasCashOutReport));
                OnPropertyChanged(nameof(IsCashOutReportVisible));
            }
        }
    }

    public ObservableCollection<ReportProperties> ReportTypes { get; } = new(Enum.GetValues<ReportProperties>());

    public bool IsSalesSummaryVisible => SelectedReportType == ReportProperties.SalesSummary;
    public bool IsSalesDetailVisible => SelectedReportType == ReportProperties.SalesDetail;
    public bool IsSalesBalanceVisible => SelectedReportType == ReportProperties.SalesBalance;
    public bool IsExceptionsReportVisible => SelectedReportType == ReportProperties.Exceptions;
    public bool IsCreditCardReportVisible => SelectedReportType == ReportProperties.CreditCardReport;
    public bool IsPaymentReportVisible => SelectedReportType == ReportProperties.PaymentReport;
    public bool IsMenuUsageReportVisible => SelectedReportType == ReportProperties.MenuUsageReport;
    public bool IsServerProductivityReportVisible => SelectedReportType == ReportProperties.ServerProductivityReport;
    public bool IsHourlyLaborReportVisible => SelectedReportType == ReportProperties.HourlyLaborReport;
    public bool IsJournalReportVisible => SelectedReportType == ReportProperties.Journal;
    public bool IsProductivityReportVisible => SelectedReportType == ReportProperties.Productivity;
    public bool IsLaborReportVisible => SelectedReportType == ReportProperties.Labor;
    public bool IsDeliveryReportVisible => SelectedReportType == ReportProperties.Delivery;
    public bool IsTipReportVisible => SelectedReportType == ReportProperties.Tips;
    public bool IsAttendanceReportVisible => SelectedReportType == ReportProperties.Attendance;
    public bool IsCashOutReportVisible => SelectedReportType == ReportProperties.CashOut;

    public SalesSummaryReportDto? SalesSummary
    {
        get => _salesSummary;
        private set => SetProperty(ref _salesSummary, value);
    }

    public SalesDetailReportDto? SalesDetail
    {
        get => _salesDetail;
        private set => SetProperty(ref _salesDetail, value);
    }

    public SalesBalanceReportDto? SalesBalance
    {
        get => _salesBalance;
        private set => SetProperty(ref _salesBalance, value);
    }

    public ExceptionsReportDto? ExceptionsReport
    {
        get => _exceptionsReport;
        private set => SetProperty(ref _exceptionsReport, value);
    }

    public CreditCardReportDto? CreditCardReport
    {
        get => _creditCardReport;
        private set => SetProperty(ref _creditCardReport, value);
    }

    public PaymentReportDto? PaymentReport
    {
        get => _paymentReport;
        private set => SetProperty(ref _paymentReport, value);
    }

    public MenuUsageReportDto? MenuUsageReport
    {
        get => _menuUsageReport;
        private set => SetProperty(ref _menuUsageReport, value);
    }

    public ServerProductivityReportDto? ServerProductivityReport
    {
        get => _serverProductivityReport;
        private set => SetProperty(ref _serverProductivityReport, value);
    }

    public HourlyLaborReportDto? HourlyLaborReport
    {
        get => _hourlyLaborReport;
        private set => SetProperty(ref _hourlyLaborReport, value);
    }

    public JournalReportDto? JournalReport
    {
        get => _journalReport;
        private set => SetProperty(ref _journalReport, value);
    }

    public ProductivityReportDto? ProductivityReport
    {
        get => _productivityReport;
        private set => SetProperty(ref _productivityReport, value);
    }

    public LaborReportDto? LaborReport
    {
        get => _laborReport;
        private set => SetProperty(ref _laborReport, value);
    }

    public DeliveryReportDto? DeliveryReport
    {
        get => _deliveryReport;
        private set => SetProperty(ref _deliveryReport, value);
    }

    public TipReportDto? TipReport
    {
        get => _tipReport;
        private set => SetProperty(ref _tipReport, value);
    }

    public AttendanceReportDto? AttendanceReport
    {
        get => _attendanceReport;
        private set => SetProperty(ref _attendanceReport, value);
    }

    public CashOutReportDto? CashOutReport
    {
        get => _cashOutReport;
        private set => SetProperty(ref _cashOutReport, value);
    }

    public bool HasSalesSummary => SalesSummary != null;
    public bool HasSalesDetail => SalesDetail != null;
    public bool HasSalesBalance => SalesBalance != null;
    public bool HasExceptionsReport => ExceptionsReport != null;
    public bool HasCreditCardReport => CreditCardReport != null;
    public bool HasPaymentReport => PaymentReport != null;
    public bool HasMenuUsageReport => MenuUsageReport != null;
    public bool HasServerProductivityReport => ServerProductivityReport != null;
    public bool HasHourlyLaborReport => HourlyLaborReport != null;
    public bool HasJournalReport => JournalReport != null;
    public bool HasProductivityReport => ProductivityReport != null;
    public bool HasLaborReport => LaborReport != null;
    public bool HasDeliveryReport => DeliveryReport != null;
    public bool HasTipReport => TipReport != null;
    public bool HasAttendanceReport => AttendanceReport != null;
    public bool HasCashOutReport => CashOutReport != null;

    public string Error
    {
        get => _error;
        set
        {
            if (SetProperty(ref _error, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(Error);

    public AsyncRelayCommand LoadReportCommand { get; }

    private async Task LoadReportAsync()
    {
        IsBusy = true;
        Error = string.Empty;

        try
        {
            var start = StartDate.LocalDateTime;
            var end = EndDate.LocalDateTime;

            // Ensure end is end of day if user picked same day visually
            if (end.TimeOfDay == TimeSpan.Zero && end == start)
            {
                end = end.AddDays(1).AddSeconds(-1);
            }

            switch (SelectedReportType)
            {
                case ReportProperties.SalesSummary:
                    var salesReport = await _getSalesSummary.HandleAsync(new GetSalesSummaryQuery(start, end));
                    SalesSummary = salesReport;
                    break;

                case ReportProperties.SalesDetail:
                    var salesDetail = await _getSalesDetail.HandleAsync(new GetSalesDetailQuery(start, end));
                    SalesDetail = salesDetail;
                    break;

                case ReportProperties.SalesBalance:
                    var salesBalance = await _getSalesBalance.HandleAsync(new GetSalesBalanceQuery(start, end));
                    SalesBalance = salesBalance;
                    break;

                case ReportProperties.Exceptions:
                    var exceptions = await _getExceptionsReport.HandleAsync(new GetExceptionsReportQuery(start, end));
                    ExceptionsReport = exceptions;
                    break;

                case ReportProperties.CreditCardReport:
                    var creditCardReport = await _getCreditCardReport.HandleAsync(new GetCreditCardReportQuery(start, end));
                    CreditCardReport = creditCardReport;
                    break;

                case ReportProperties.PaymentReport:
                    var paymentReport = await _getPaymentReport.HandleAsync(new GetPaymentReportQuery(start, end));
                    PaymentReport = paymentReport;
                    break;

                case ReportProperties.MenuUsageReport:
                    var menuUsageReport = await _getMenuUsageReport.HandleAsync(new GetMenuUsageReportQuery(start, end));
                    MenuUsageReport = menuUsageReport;
                    break;

                case ReportProperties.ServerProductivityReport:
                    var serverProductivityReport = await _getServerProductivityReport.HandleAsync(new GetServerProductivityReportQuery(start, end));
                    ServerProductivityReport = serverProductivityReport;
                    break;

                case ReportProperties.HourlyLaborReport:
                    var hourlyLaborReport = await _getHourlyLaborReport.HandleAsync(new GetHourlyLaborReportQuery(start, end));
                    HourlyLaborReport = hourlyLaborReport;
                    break;

                case ReportProperties.Journal:
                    var journal = await _getJournalReport.HandleAsync(new GetJournalReportQuery(start, end, null, null));
                    JournalReport = journal;
                    break;

                case ReportProperties.Productivity:
                    var productivity = await _getProductivityReport.HandleAsync(new GetProductivityReportQuery(start, end, null));
                    ProductivityReport = productivity;
                    break;

                case ReportProperties.Labor:
                    var laborReport = await _getLaborReport.HandleAsync(new GetLaborReportQuery(start, end));
                    LaborReport = laborReport;
                    break;

                case ReportProperties.Delivery:
                    var deliveryReport = await _getDeliveryReport.HandleAsync(new GetDeliveryReportQuery(start, end));
                    DeliveryReport = deliveryReport;
                    break;

                case ReportProperties.Tips:
                    var tipsReport = await _getTipReport.HandleAsync(new GetTipReportQuery(start, end));
                    TipReport = tipsReport;
                    break;
                
                case ReportProperties.Attendance:
                    var attendanceReport = await _getAttendanceReport.HandleAsync(new GetAttendanceReportQuery(start, end));
                    AttendanceReport = attendanceReport;
                    break;
                
                case ReportProperties.CashOut:
                    var cashOutReport = await _getCashOutReport.HandleAsync(new GetCashOutReportQuery(start, end));
                    CashOutReport = cashOutReport;
                    break;
            }
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
