using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.Reports;
using System.Collections.ObjectModel;


namespace Magidesk.Presentation.ViewModels;

public enum ReportProperties
{
    SalesSummary,
    Labor,
    Delivery
}

public partial class SalesReportsViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> _getSalesSummary;
    private readonly IQueryHandler<GetLaborReportQuery, LaborReportDto> _getLaborReport;
    private readonly IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> _getDeliveryReport;

    private DateTimeOffset _startDate = DateTime.Today;
    private DateTimeOffset _endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
    private ReportProperties _selectedReportType;
    private string _error = string.Empty;

    // Report Data Containers
    private SalesSummaryReportDto? _salesSummary;
    private LaborReportDto? _laborReport;
    private DeliveryReportDto? _deliveryReport;

    public SalesReportsViewModel(
        IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto> getSalesSummary,
        IQueryHandler<GetLaborReportQuery, LaborReportDto> getLaborReport,
        IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto> getDeliveryReport)
    {
        _getSalesSummary = getSalesSummary;
        _getLaborReport = getLaborReport;
        _getDeliveryReport = getDeliveryReport;

        Title = "Reporting";
        LoadReportCommand = new AsyncRelayCommand(LoadReportAsync);
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
                OnPropertyChanged(nameof(IsLaborReportVisible));
                OnPropertyChanged(nameof(IsDeliveryReportVisible));
            }
        }
    }

    public ObservableCollection<ReportProperties> ReportTypes { get; } = new(Enum.GetValues<ReportProperties>());

    public bool IsSalesSummaryVisible => SelectedReportType == ReportProperties.SalesSummary;
    public bool IsLaborReportVisible => SelectedReportType == ReportProperties.Labor;
    public bool IsDeliveryReportVisible => SelectedReportType == ReportProperties.Delivery;

    public SalesSummaryReportDto? SalesSummary
    {
        get => _salesSummary;
        private set => SetProperty(ref _salesSummary, value);
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
