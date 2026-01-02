using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class NotesDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<UpdateTicketNoteCommand> _updateTicketNoteHandler;
    private readonly ICommandHandler<UpdateOrderLineInstructionCommand> _updateInstructionHandler;

    private string? _note;
    public string? Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    // Context
    private Guid? _ticketId;
    private Guid? _orderLineId;

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    public event EventHandler? RequestClose;

    public NotesDialogViewModel(
        ICommandHandler<UpdateTicketNoteCommand> updateTicketNoteHandler,
        ICommandHandler<UpdateOrderLineInstructionCommand> updateInstructionHandler)
    {
        _updateTicketNoteHandler = updateTicketNoteHandler;
        _updateInstructionHandler = updateInstructionHandler;

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public void InitializeForTicket(Guid ticketId, string? currentNote)
    {
        _ticketId = ticketId;
        _orderLineId = null;
        Note = currentNote;
        Title = "Edit Ticket Note";
    }

    public void InitializeForOrderLine(Guid ticketId, Guid orderLineId, string? currentInstruction)
    {
        _ticketId = ticketId;
        _orderLineId = orderLineId;
        Note = currentInstruction;
        Title = "Edit Item Instructions";
    }

    private async void Save()
    {
        if (_ticketId == null) return;

        try
        {
            IsBusy = true;

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

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            // Simple failure handling for now
            // Cannot use HasError/ErrorMessage as they are not in ViewModelBase
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
