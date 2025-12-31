using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.DTOs;
using Magidesk.Presentation.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels
{
    public partial class GroupSettleTicketViewModel : ViewModelBase
    {
        private readonly ICommandHandler<GroupSettleCommand, GroupSettleResult> _groupSettleHandler;
        private readonly IUserService _userService;
        private readonly ITerminalContext _terminalContext;

        private ObservableCollection<GroupSettleTicketDto> _selectedTickets = new();
        private decimal _combinedTotal = 0;
        private string _selectedPaymentMethod = "Cash";
        private string _tenderAmount = "";
        private bool _isProcessing = false;
        private string _processingMessage = "Processing payment...";
        private string? _error;

        public ObservableCollection<GroupSettleTicketDto> SelectedTickets
        {
            get => _selectedTickets;
            set => SetProperty(ref _selectedTickets, value);
        }

        public decimal CombinedTotal
        {
            get => _combinedTotal;
            set => SetProperty(ref _combinedTotal, value);
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => SetProperty(ref _selectedPaymentMethod, value);
        }

        public string TenderAmount
        {
            get => _tenderAmount;
            set => SetProperty(ref _tenderAmount, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public string ProcessingMessage
        {
            get => _processingMessage;
            set => SetProperty(ref _processingMessage, value);
        }

        public string? Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public GroupSettleTicketViewModel(
            ICommandHandler<GroupSettleCommand, GroupSettleResult> groupSettleHandler,
            IUserService userService,
            ITerminalContext terminalContext)
        {
            Title = "Group Settlement";
            _groupSettleHandler = groupSettleHandler;
            _userService = userService;
            _terminalContext = terminalContext;
        }

        public Action? CloseDialog { get; set; }

        public void SetSelectedTickets(ObservableCollection<GroupSettleTicketDto> tickets)
        {
            SelectedTickets.Clear();
            foreach (var ticket in tickets)
            {
                SelectedTickets.Add(ticket);
            }
            UpdateCombinedTotal();
        }

        private void UpdateCombinedTotal()
        {
            CombinedTotal = SelectedTickets.Sum(t => t.TotalAmount);
        }

        [RelayCommand]
        private async Task ProcessPaymentAsync()
        {
            if (SelectedTickets.Count == 0)
            {
                Error = "No tickets selected";
                return;
            }

            if (string.IsNullOrEmpty(TenderAmount) || !decimal.TryParse(TenderAmount, out var tenderAmount))
            {
                Error = "Please enter a valid tender amount";
                return;
            }

            if (tenderAmount < CombinedTotal)
            {
                Error = "Tender amount is less than combined total";
                return;
            }

            IsProcessing = true;
            Error = null;
            ProcessingMessage = $"Processing {SelectedPaymentMethod} payment of {CombinedTotal:C}...";

            try
            {
                // Parse payment type
                if (!Enum.TryParse<PaymentType>(SelectedPaymentMethod, out var paymentType))
                {
                    Error = "Invalid payment type";
                    return;
                }

                // Create GroupSettleCommand
                if (_userService.CurrentUser?.Id == null)
                {
                    Error = "No current user is set. Please login again.";
                    return;
                }

                if (_terminalContext.TerminalId == null)
                {
                    Error = "Terminal identity is not initialized.";
                    return;
                }

                var command = new GroupSettleCommand
                {
                    TicketIds = SelectedTickets.Select(t => t.Id).ToList(),
                    Amount = new Money(CombinedTotal, "USD"),
                    TenderAmount = new Money(tenderAmount, "USD"),
                    PaymentType = paymentType,
                    ProcessedBy = _userService.CurrentUser.Id,
                    TerminalId = _terminalContext.TerminalId.Value,
                    GlobalId = Guid.NewGuid().ToString()
                };

                // Process the group settlement
                var result = await _groupSettleHandler.HandleAsync(command);

                if (result.Success)
                {
                    ProcessingMessage = "Payment successful! Closing dialog...";
                    await Task.Delay(1000);
                    CloseDialog?.Invoke();
                }
                else
                {
                    Error = $"Payment failed: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                Error = $"Payment error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseDialog?.Invoke();
        }
    }
}
