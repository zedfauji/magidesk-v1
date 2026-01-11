using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.ValueObjects;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Presentation.Views;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Views;
using Magidesk.Application.Commands.Security; // For AuthorizeManagerCommand
using AuthorizationResult = Magidesk.Application.DTOs.Security.AuthorizationResult;

namespace Magidesk.Presentation.ViewModels;

public sealed class TicketManagementViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> _getOpenTickets;
    private readonly ICommandHandler<VoidTicketCommand> _voidTicket;
    private readonly ICommandHandler<RefundTicketCommand, RefundTicketResult> _refundTicket;
    private readonly ICommandHandler<SplitTicketCommand, SplitTicketResult> _splitTicket;
    private readonly ICommandHandler<PrintReceiptCommand, PrintReceiptResult> _printReceipt;
    private readonly ICommandHandler<AuthorizeManagerCommand, AuthorizationResult> _authHandler;
    private readonly IQueryHandler<CalculateRefundPreviewQuery, RefundPreviewDto> _refundPreviewQuery;

    private List<TicketDto> _openTickets = new();
    private TicketDto? _selectedTicket;





    private string? _error;
    private string? _lastResult;

    public TicketManagementViewModel(
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTickets,
        ICommandHandler<VoidTicketCommand> voidTicket,
        ICommandHandler<RefundTicketCommand, RefundTicketResult> refundTicket,
        ICommandHandler<SplitTicketCommand, SplitTicketResult> splitTicket,
        ICommandHandler<PrintReceiptCommand, PrintReceiptResult> printReceipt,
        ICommandHandler<AuthorizeManagerCommand, AuthorizationResult> authHandler,
        IQueryHandler<CalculateRefundPreviewQuery, RefundPreviewDto> refundPreviewQuery)
    {
        _getOpenTickets = getOpenTickets;
        _voidTicket = voidTicket;
        _refundTicket = refundTicket;
        _splitTicket = splitTicket;
        _printReceipt = printReceipt;
        _authHandler = authHandler;
        _refundPreviewQuery = refundPreviewQuery;

        Title = "Ticket Management";

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        VoidSelectedCommand = new AsyncRelayCommand(VoidSelectedAsync, CanVoidSelected);
        RefundSelectedCommand = new AsyncRelayCommand(RefundSelectedAsync, CanRefundSelected);
        ReprintReceiptCommand = new AsyncRelayCommand(ReprintSelectedAsync, CanReprintSelected);
    }

    public IReadOnlyList<TicketDto> OpenTickets
    {
        get => _openTickets;
        private set
        {
            if (SetProperty(ref _openTickets, value.ToList()))
            {
                OnPropertyChanged(nameof(ActiveTickets));
                OnPropertyChanged(nameof(CompletedTickets));
                OnPropertyChanged(nameof(HasActiveTickets));
                OnPropertyChanged(nameof(HasNoActiveTickets));
                OnPropertyChanged(nameof(HasCompletedTickets));
                OnPropertyChanged(nameof(HasNoCompletedTickets));
            }
        }
    }

    public IReadOnlyList<TicketDto> ActiveTickets => OpenTickets
        .Where(t => t.Status == Domain.Enumerations.TicketStatus.Draft 
                 || t.Status == Domain.Enumerations.TicketStatus.Open)
        .ToList();

    public IReadOnlyList<TicketDto> CompletedTickets => OpenTickets
        .Where(t => t.Status == Domain.Enumerations.TicketStatus.Closed 
                 || t.Status == Domain.Enumerations.TicketStatus.Refunded 
                 || t.Status == Domain.Enumerations.TicketStatus.Voided)
        .ToList();

    public bool HasCompletedTickets => CompletedTickets.Count > 0;
    public bool HasNoCompletedTickets => !HasCompletedTickets;
    public bool HasActiveTickets => ActiveTickets.Count > 0;
    public bool HasNoActiveTickets => !HasActiveTickets;

    public TicketDto? SelectedTicket
    {
        get => _selectedTicket;
        set
        {
            if (SetProperty(ref _selectedTicket, value))
            {
                OnPropertyChanged(nameof(SelectedSummaryText));
                OnPropertyChanged(nameof(CanVoidTicket));
                OnPropertyChanged(nameof(CanRefundTicket));
                OnPropertyChanged(nameof(CanReprintTicket));
                VoidSelectedCommand.NotifyCanExecuteChanged();
                RefundSelectedCommand.NotifyCanExecuteChanged();
                ReprintReceiptCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string SelectedSummaryText => SelectedTicket == null
        ? "No ticket selected"
        : $"Ticket #{SelectedTicket.TicketNumber} ({SelectedTicket.Status})\n" +
          $"Total: {SelectedTicket.TotalAmount}  Paid: {SelectedTicket.PaidAmount}  Due: {SelectedTicket.DueAmount}\n" +
          $"Lines: {SelectedTicket.OrderLines.Count}  Payments: {SelectedTicket.Payments.Count}";




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

    public string? LastResult
    {
        get => _lastResult;
        private set => SetProperty(ref _lastResult, value);
    }

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand VoidSelectedCommand { get; }
    public AsyncRelayCommand RefundSelectedCommand { get; }

    public AsyncRelayCommand ReprintReceiptCommand { get; }

    // State-aware visibility properties
    public bool CanVoidTicket => SelectedTicket?.Status == Domain.Enumerations.TicketStatus.Open;
    public bool CanRefundTicket => SelectedTicket?.Status == Domain.Enumerations.TicketStatus.Closed;
    public bool CanReprintTicket => SelectedTicket?.Status == Domain.Enumerations.TicketStatus.Closed 
                                     || SelectedTicket?.Status == Domain.Enumerations.TicketStatus.Refunded;

    private bool CanVoidSelected() => CanVoidTicket;
    private bool CanRefundSelected() => CanRefundTicket;
    private bool CanReprintSelected() => CanReprintTicket;

    public async Task RefreshAsync()
    {
        Error = null;
        LastResult = null;
        await LoadTicketsAsync();
    }

    private async Task LoadTicketsAsync()
    {
        Console.WriteLine("[TicketManagement] LoadTicketsAsync STARTED");
        IsBusy = true;
        try
        {
            Console.WriteLine("[TicketManagement] Calling _getOpenTickets.HandleAsync...");
            var tickets = await _getOpenTickets.HandleAsync(new GetOpenTicketsQuery());
            Console.WriteLine($"[TicketManagement] Received {tickets?.Count() ?? 0} tickets");
            OpenTickets = tickets.ToList();
            Console.WriteLine($"[TicketManagement] OpenTickets set. Count: {OpenTickets.Count}");

            if (SelectedTicket != null)
            {
                Console.WriteLine($"[TicketManagement] Reselecting ticket: {SelectedTicket.Id}");
                SelectedTicket = OpenTickets.FirstOrDefault(t => t.Id == SelectedTicket.Id);
            }
            Console.WriteLine("[TicketManagement] LoadTicketsAsync COMPLETED successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TicketManagement] LoadTicketsAsync EXCEPTION: {ex.GetType().Name}");
            Console.WriteLine($"[TicketManagement] Message: {ex.Message}");
            Console.WriteLine($"[TicketManagement] StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[TicketManagement] InnerException: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
            }
            Error = ex.Message;
        }
        finally
        {
            Console.WriteLine("[TicketManagement] LoadTicketsAsync FINALLY block");
            IsBusy = false;
        }
    }

    private async Task VoidSelectedAsync()
    {
        Error = null;
        LastResult = null;
        
        try
        {
            if (SelectedTicket == null) { Error = "Select a ticket first."; return; }

            var dialog = new VoidTicketDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Initialize ViewModel
            dialog.ViewModel.Initialize(SelectedTicket);
            
            await dialog.ShowAsync();
            
            // Refresh logic - always refresh to show updated status
            await RefreshAsync();
            
            // Check if void was successful by checking status (simple check)
            var refreshedTicket = OpenTickets.FirstOrDefault(t => t.Id == SelectedTicket.Id);
            if (refreshedTicket != null && refreshedTicket.Status == Domain.Enumerations.TicketStatus.Voided)
            {
                LastResult = $"Voided ticket #{SelectedTicket.TicketNumber}.";
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    private async Task RefundSelectedAsync()
    {
        Error = null;
        LastResult = null;
        
        try
        {
            if (SelectedTicket == null) { Error = "Select a ticket first."; return; }

            var dialog = new RefundWizardDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Re-fetch ticket to get latest payments for the wizard
            // Or pass current SelectedTicket if up-to-date.
            
            // Mapping TicketDto to payment list for ViewModel
            // Our RefundWizardViewModel expects (TicketDto, QueryHandler, CommandHandler, List<PaymentDto>, CloseAction)
            
            dialog.DataContext = new RefundWizardViewModel(
                SelectedTicket,
                _refundPreviewQuery,
                _refundTicket,
                _authHandler,
                SelectedTicket.Payments, // List<PaymentDto>
                () => dialog.Hide() // Close Action
            );
            
            await dialog.ShowAsync();
            
            await RefreshAsync();
            LastResult = "Refund wizard completed."; 
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    private async Task ReprintSelectedAsync()
    {
        Error = null;
        LastResult = null;
        
        try
        {
            if (SelectedTicket == null) { Error = "Select a ticket first."; return; }

            var result = await _printReceipt.HandleAsync(new PrintReceiptCommand
            {
                TicketId = SelectedTicket.Id,
                ReceiptType = ReceiptType.Ticket
            });

            if (result.Success)
            {
                LastResult = $"Reprint initiated for ticket #{SelectedTicket.TicketNumber}.";
            }
            else
            {
                Error = "Reprint failed.";
            }
        }
        catch (Exception ex)
        {
             Error = ex.Message;
        }
    }


}
