using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Presentation.Services;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class DrawerPullReportViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult> _reportQueryHandler;
    private readonly IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult> _currentSessionHandler;
    private readonly IReportPrintService _reportPrintService;
    private readonly IUserService _userService;
    private readonly NavigationService _navigationService;

    private DrawerPullReportDto? _report;
    private bool _isLoading;
    private bool _isPrinting;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public DrawerPullReportDto? Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsPrinting
    {
        get => _isPrinting;
        set => SetProperty(ref _isPrinting, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    // Calculated properties for cash reconciliation
    public decimal ExpectedCashBalance => Report?.ExpectedCash ?? 0m;
    public decimal ActualCashBalance { get; set; } = 0m;
    public decimal CashVariance => ActualCashBalance - ExpectedCashBalance;
    public bool HasVariance => Math.Abs(CashVariance) > 0.01m;
    public string VarianceStatus => CashVariance switch
    {
        > 0.01m => "OVERAGE",
        < -0.01m => "SHORTAGE",
        _ => "BALANCED"
    };

    public ICommand PrintCommand { get; }
    public ICommand PrintReconciliationCommand { get; }
    public ICommand CountCashCommand { get; }
    public ICommand CloseCommand { get; }

    public DrawerPullReportViewModel(
        IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult> reportQueryHandler,
        IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult> currentSessionHandler,
        IReportPrintService reportPrintService,
        IUserService userService,
        NavigationService navigationService)
    {
        _reportQueryHandler = reportQueryHandler;
        _currentSessionHandler = currentSessionHandler;
        _reportPrintService = reportPrintService;
        _userService = userService;
        _navigationService = navigationService;

        PrintCommand = new AsyncRelayCommand(PrintAsync, CanPrint);
        PrintReconciliationCommand = new AsyncRelayCommand(PrintReconciliationAsync, CanPrintReconciliation);
        CountCashCommand = new AsyncRelayCommand(CountCashAsync);
        CloseCommand = new RelayCommand(Close);
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        HasError = false;
        ErrorMessage = string.Empty;
        
        try
        {
            var sessionResult = await _currentSessionHandler.HandleAsync(new GetCurrentCashSessionQuery());
            if (sessionResult.CashSession == null)
            {
                ErrorMessage = "No active cash session found. Please start a cash session to generate a drawer pull report.";
                HasError = true;
                return;
            }

            var result = await _reportQueryHandler.HandleAsync(new GetDrawerPullReportQuery { CashSessionId = sessionResult.CashSession.Id });
            Report = result.Report;
            
            // Initialize actual cash balance with expected balance as starting point
            ActualCashBalance = ExpectedCashBalance;
            OnPropertyChanged(nameof(ActualCashBalance));
            OnPropertyChanged(nameof(CashVariance));
            OnPropertyChanged(nameof(HasVariance));
            OnPropertyChanged(nameof(VarianceStatus));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading drawer pull report: {ex.Message}";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"Error loading drawer pull report: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanPrint() => Report != null && !IsPrinting;

    private bool CanPrintReconciliation() => Report != null && !IsPrinting;

    private async Task PrintAsync()
    {
        if (Report == null) return;

        IsPrinting = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var userId = _userService.CurrentUser?.Id;
            var success = await _reportPrintService.PrintDrawerPullReportAsync(Report, userId);
            
            if (!success)
            {
                ErrorMessage = "Failed to print drawer pull report. Please check printer connection and try again.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error printing report: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsPrinting = false;
            ((AsyncRelayCommand)PrintCommand).NotifyCanExecuteChanged();
            ((AsyncRelayCommand)PrintReconciliationCommand).NotifyCanExecuteChanged();
        }
    }

    private async Task PrintReconciliationAsync()
    {
        if (Report == null) return;

        IsPrinting = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var userId = _userService.CurrentUser?.Id;
            var success = await _reportPrintService.PrintCashReconciliationReportAsync(
                Report.CashSessionId, 
                ExpectedCashBalance, 
                ActualCashBalance, 
                CashVariance, 
                userId);
            
            if (!success)
            {
                ErrorMessage = "Failed to print cash reconciliation report. Please check printer connection and try again.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error printing reconciliation report: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsPrinting = false;
            ((AsyncRelayCommand)PrintCommand).NotifyCanExecuteChanged();
            ((AsyncRelayCommand)PrintReconciliationCommand).NotifyCanExecuteChanged();
        }
    }

    private async Task CountCashAsync()
    {
        try
        {
            var dialog = new Views.Dialogs.CashEntryDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            var (result, amount, reason) = await dialog.ShowCashEntryAsync(
                "Cash Count", 
                "Count the actual cash in the drawer and enter the total amount:", 
                true, // Show denomination breakdown
                false); // Don't require reason for counting

            if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                UpdateActualCashBalance(amount);
                
                // Show reconciliation result
                var resultDialog = new Views.Dialogs.ConfirmationDialog();
                resultDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                var varianceMessage = HasVariance 
                    ? $"Variance detected: {VarianceStatus} of {Math.Abs(CashVariance):C}"
                    : "Cash drawer is balanced - no variance detected.";
                
                var icon = HasVariance ? "⚠️" : "✅";
                var severity = HasVariance ? "Warning" : "Success";
                
                await resultDialog.ShowConfirmationAsync(
                    "Cash Count Complete",
                    $"Actual cash amount recorded: {ActualCashBalance:C}",
                    "OK",
                    "",
                    icon,
                    severity,
                    $"Expected: {ExpectedCashBalance:C}\nActual: {ActualCashBalance:C}\n{varianceMessage}");
            }
        }
        catch (Exception ex)
        {
            var errorDialog = new Views.Dialogs.ConfirmationDialog();
            errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            await errorDialog.ShowConfirmationAsync(
                "Error",
                "Failed to record cash count.",
                "OK",
                "",
                "❌",
                "Error",
                $"Error details: {ex.Message}");
        }
    }

    public void UpdateActualCashBalance(decimal actualAmount)
    {
        ActualCashBalance = actualAmount;
        OnPropertyChanged(nameof(ActualCashBalance));
        OnPropertyChanged(nameof(CashVariance));
        OnPropertyChanged(nameof(HasVariance));
        OnPropertyChanged(nameof(VarianceStatus));
    }

    private void Close()
    {
        // View logic handles dialog closure via binding or code-behind interaction
        // Typically dialog ViewModels might expose a 'RequestClose' event or similar if strictly MVVM,
        // or the View just binds the Close button to `DialogResult = Cancel`.
        // Here, we'll assume the View's Close button handles the ContentDialog result directly for simplicity,
        // OR we can use the NavigationService to close if it supports it.
        // For ContentDialogs, usually the command is just bound to the dialog's Primary/Secondary/Close button logic.
        // We'll leave this empty as the XAML Close Button usually handles this.
    }
}
