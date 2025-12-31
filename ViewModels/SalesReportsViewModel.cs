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
    SalesBalance,
    Exceptions,
    Journal,
    Productivity,
    Labor,
    Delivery
}

public partial class SalesReportsViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> _getSalesSummary;
    private readonly IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto> _getSalesBalance;
    private readonly IQueryHandler<GetExceptionsReportQuery, ExceptionsReportDto> _getExceptionsReport;
    private readonly IQueryHandler<GetJournalReportQuery, JournalReportDto> _getJournalReport;
    private readonly IQueryHandler<GetProductivityReportQuery, ProductivityReportDto> _getProductivityReport;
    private readonly IQueryHandler<GetLaborReportQuery, LaborReportDto> _getLaborReport;
    private readonly IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> _getDeliveryReport;

    private DateTimeOffset _startDate = DateTime.Today;
    private DateTimeOffset _endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
    private ReportProperties _selectedReportType;
    private string _error = string.Empty;
    
    // Quick Date Commands
    public System.Windows.Input.ICommand SetDateRangeCommand { get; }


    // Report Data Containers
    private SalesSummaryReportDto? _salesSummary;
    private SalesBalanceReportDto? _salesBalance;
    private ExceptionsReportDto? _exceptionsReport;
    private JournalReportDto? _journalReport;
    private ProductivityReportDto? _productivityReport;
    private LaborReportDto? _laborReport;
    private DeliveryReportDto? _deliveryReport;

    public SalesReportsViewModel(
        IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> getSalesSummary,
        IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto> getSalesBalance,
        IQueryHandler<GetExceptionsReportQuery, ExceptionsReportDto> getExceptionsReport,
        IQueryHandler<GetJournalReportQuery, JournalReportDto> getJournalReport,
        IQueryHandler<GetProductivityReportQuery, ProductivityReportDto> getProductivityReport,
        IQueryHandler<GetLaborReportQuery, LaborReportDto> getLaborReport,
        IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> getDeliveryReport)
    {
        _getSalesSummary = getSalesSummary;
        _getSalesBalance = getSalesBalance;
        _getExceptionsReport = getExceptionsReport;
        _getJournalReport = getJournalReport;
        _getProductivityReport = getProductivityReport;
        _getLaborReport = getLaborReport;
        _getDeliveryReport = getDeliveryReport;

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
                OnPropertyChanged(nameof(IsSalesBalanceVisible));
                OnPropertyChanged(nameof(IsExceptionsReportVisible));
                OnPropertyChanged(nameof(IsJournalReportVisible));
                OnPropertyChanged(nameof(IsProductivityReportVisible));
                OnPropertyChanged(nameof(IsLaborReportVisible));
                OnPropertyChanged(nameof(IsDeliveryReportVisible));

                OnPropertyChanged(nameof(HasSalesSummary));
                OnPropertyChanged(nameof(HasSalesBalance));
                OnPropertyChanged(nameof(HasExceptionsReport));
                OnPropertyChanged(nameof(HasJournalReport));
                OnPropertyChanged(nameof(HasProductivityReport));
                OnPropertyChanged(nameof(HasLaborReport));
                OnPropertyChanged(nameof(HasDeliveryReport));
            }
        }
    }

    public ObservableCollection<ReportProperties> ReportTypes { get; } = new(Enum.GetValues<ReportProperties>());

    public bool IsSalesSummaryVisible => SelectedReportType == ReportProperties.SalesSummary;
    public bool IsSalesBalanceVisible => SelectedReportType == ReportProperties.SalesBalance;
    public bool IsExceptionsReportVisible => SelectedReportType == ReportProperties.Exceptions;
    public bool IsJournalReportVisible => SelectedReportType == ReportProperties.Journal;
    public bool IsProductivityReportVisible => SelectedReportType == ReportProperties.Productivity;
    public bool IsLaborReportVisible => SelectedReportType == ReportProperties.Labor;
    public bool IsDeliveryReportVisible => SelectedReportType == ReportProperties.Delivery;

    public SalesSummaryReportDto? SalesSummary
    {
        get => _salesSummary;
        private set => SetProperty(ref _salesSummary, value);
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

    public bool HasSalesSummary => SalesSummary != null;
    public bool HasSalesBalance => SalesBalance != null;
    public bool HasExceptionsReport => ExceptionsReport != null;
    public bool HasJournalReport => JournalReport != null;
    public bool HasProductivityReport => ProductivityReport != null;
    public bool HasLaborReport => LaborReport != null;
    public bool HasDeliveryReport => DeliveryReport != null;

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

                case ReportProperties.SalesBalance:
                    var salesBalance = await _getSalesBalance.HandleAsync(new GetSalesBalanceQuery(start, end));
                    SalesBalance = salesBalance;
                    break;

                case ReportProperties.Exceptions:
                    var exceptions = await _getExceptionsReport.HandleAsync(new GetExceptionsReportQuery(start, end));
                    ExceptionsReport = exceptions;
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
