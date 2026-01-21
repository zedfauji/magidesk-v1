using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels;

public partial class AuthorizationCaptureBatchViewModel : ViewModelBase
{
    private bool _isProcessing;
    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }

    private bool _isFinished;
    public bool IsFinished
    {
        get => _isFinished;
        set 
        {
            if (SetProperty(ref _isFinished, value))
            {
                OnPropertyChanged(nameof(CanClose));
                CloseCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private string _statusMessage = "Ready to start capture batch.";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private double _progressValue;
    public double ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    private bool _isIndeterminate = false;
    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        set => SetProperty(ref _isIndeterminate, value);
    }

    public ObservableCollection<string> LogEntries { get; } = new ObservableCollection<string>();

    public AuthorizationCaptureBatchViewModel()
    {
        Title = "Batch Authorization Capture";
    }

    [RelayCommand]
    private async Task StartBatchAsync()
    {
        if (IsProcessing || IsFinished) return;

        IsProcessing = true;
        IsFinished = false;
        ProgressValue = 0;
        LogEntries.Clear();
        StatusMessage = "Initializing Batch...";

        AddLog("Starting Batch Capture...");

        // Simulate fetching transactions
        await Task.Delay(1000); 
        var transactionsToCapture = new[] { "TX-1001", "TX-1002", "TX-1003", "TX-1004", "TX-1005" };
        
        IsIndeterminate = false;
        double step = 100.0 / transactionsToCapture.Length;

        foreach (var txId in transactionsToCapture)
        {
            StatusMessage = $"Processing {txId}...";
            AddLog($"Capturing Auth for {txId}...");
            
            // Simulate Gateway Latency
            await Task.Delay(1500);

            AddLog($"SUCCESS: {txId} captured.");
            ProgressValue += step;
        }

        StatusMessage = "Batch Complete.";
        AddLog("Batch Capture Finished.");
        IsProcessing = false;
        IsFinished = true;
        ProgressValue = 100;
    }

    [RelayCommand(CanExecute = nameof(CanClose))]
    private void Close()
    {
        // View will bind to this command to close the dialog
        // This command exists primarily for binding state; actual close action 
        // will be handled by the dialog listening to this or just binding the button result.
        // For ContentDialog, the Close button usually sets the DialogResult.
    }

    private bool CanClose() => IsFinished;

    private void AddLog(string message)
    {
        LogEntries.Add($"[{System.DateTime.Now:HH:mm:ss}] {message}");
    }
}
