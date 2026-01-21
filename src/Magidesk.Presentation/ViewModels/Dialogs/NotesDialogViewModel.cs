using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class NotesDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<UpdateTicketNoteCommand> _updateTicketNoteHandler;
    private readonly ICommandHandler<UpdateOrderLineInstructionCommand> _updateInstructionHandler;
    private readonly ILogger<NotesDialogViewModel> _logger;

    private string? _note;
    public string? Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    private string? _error;
    public string? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    // Context
    private Guid? _ticketId;
    private Guid? _orderLineId;

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    public event EventHandler? RequestClose;

    public NotesDialogViewModel(
        ICommandHandler<UpdateTicketNoteCommand> updateTicketNoteHandler,
        ICommandHandler<UpdateOrderLineInstructionCommand> updateInstructionHandler,
        ILogger<NotesDialogViewModel> logger)
    {
        _updateTicketNoteHandler = updateTicketNoteHandler;
        _updateInstructionHandler = updateInstructionHandler;
        _logger = logger;

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public void InitializeForTicket(Guid ticketId, string? currentNote)
    {
        _ticketId = ticketId;
        _orderLineId = null;
        Note = currentNote;
        Title = "Edit Ticket Note";
        Error = null; // Clear any previous errors
    }

    public void InitializeForOrderLine(Guid ticketId, Guid orderLineId, string? currentInstruction)
    {
        _ticketId = ticketId;
        _orderLineId = orderLineId;
        Note = currentInstruction;
        Title = "Edit Item Instructions";
        Error = null; // Clear any previous errors
    }

    private async void Save()
    {
        if (_ticketId == null) return;

        try
        {
            IsBusy = true;
            Error = null; // Clear previous errors

            if (_orderLineId.HasValue)
            {
                var command = new UpdateOrderLineInstructionCommand(_ticketId.Value, _orderLineId.Value, Note);
                await _updateInstructionHandler.HandleAsync(command);
            }
            else
            {
                var command = new UpdateTicketNoteCommand(_ticketId.Value, Note);
                await _updateTicketNoteHandler.HandleAsync(command);
            }

            // Close dialog on success
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            // TICKET-013: Proper error handling with logging and UI notification
            _logger.LogError(ex, "Failed to save note for ticket {TicketId}", _ticketId);
            Error = $"Failed to save note: {ex.Message}";
            // Keep dialog open so user can retry
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
