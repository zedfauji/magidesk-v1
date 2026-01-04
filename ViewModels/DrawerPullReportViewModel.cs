using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Presentation.Services; // Ensure NavigationService is available
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class DrawerPullReportViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult> _reportQueryHandler;
    private readonly IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult> _currentSessionHandler;
    private readonly NavigationService _navigationService;

    private DrawerPullReportDto? _report;
    private bool _isLoading;

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

    public ICommand PrintCommand { get; }
    public ICommand CloseCommand { get; }

    public DrawerPullReportViewModel(
        IQueryHandler<GetDrawerPullReportQuery, GetDrawerPullReportResult> reportQueryHandler,
        IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult> currentSessionHandler,
        NavigationService navigationService)
    {
        _reportQueryHandler = reportQueryHandler;
        _currentSessionHandler = currentSessionHandler;
        _navigationService = navigationService;

        PrintCommand = new AsyncRelayCommand(PrintAsync);
        CloseCommand = new RelayCommand(Close);
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        try
        {
            var sessionResult = await _currentSessionHandler.HandleAsync(new GetCurrentCashSessionQuery());
            if (sessionResult.CashSession == null)
            {
                // No active session. 
                // In a real scenario, we might want to allow looking up past sessions or report "No active session".
                // For MVP parity F-0012, we likely assume current session context.
                return;
            }

            var result = await _reportQueryHandler.HandleAsync(new GetDrawerPullReportQuery { CashSessionId = sessionResult.CashSession.Id });
            Report = result.Report;
        }
        catch (Exception ex)
        {
             // Log or Handle
             System.Diagnostics.Debug.WriteLine($"Error loading drawer pull report: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task PrintAsync()
    {
        // TODO: Implement Print Service integration (F-0012 Parity Gap: Printing)
        // For now, this is a placeholder.
        await Task.Yield(); 
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
