using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels.Dialogs.TableSessions;

public class AdjustSessionTimeDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<AdjustSessionTimeCommand, AdjustSessionTimeResult> _adjustHandler;
    private readonly Guid _sessionId;

    private int _adjustmentMinutes;
    public int AdjustmentMinutes
    {
        get => _adjustmentMinutes;
        set
        {
            if (SetProperty(ref _adjustmentMinutes, value))
            {
                NotifyCanExecuteChanged();
            }
        }
    }

    private string _reason = string.Empty;
    public string Reason
    {
        get => _reason;
        set
        {
            if (SetProperty(ref _reason, value))
            {
                NotifyCanExecuteChanged();
            }
        }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public ICommand ConfirmCommand { get; }

    public AdjustSessionTimeDialogViewModel(
        ICommandHandler<AdjustSessionTimeCommand, AdjustSessionTimeResult> adjustHandler,
        Guid sessionId)
    {
        _adjustHandler = adjustHandler;
        _sessionId = sessionId;

        ConfirmCommand = new AsyncRelayCommand<ContentDialog>(ConfirmAsync, CanConfirm);
    }

    private async Task ConfirmAsync(ContentDialog? dialog)
    {
        if (dialog == null) return;

        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var command = new AdjustSessionTimeCommand(
                _sessionId, 
                TimeSpan.FromMinutes(AdjustmentMinutes), 
                Reason
            );

            await _adjustHandler.HandleAsync(command);
            dialog.Hide();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
    }

    private bool CanConfirm(ContentDialog? dialog)
    {
        return AdjustmentMinutes != 0 && !string.IsNullOrWhiteSpace(Reason);
    }

    private void NotifyCanExecuteChanged()
    {
        ((AsyncRelayCommand<ContentDialog>)ConfirmCommand).NotifyCanExecuteChanged();
    }
}
