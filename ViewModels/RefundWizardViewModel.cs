using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Magidesk.Application.Commands.Security; // For AuthorizeManagerCommand
using AuthorizationResult = Magidesk.Application.DTOs.Security.AuthorizationResult;

namespace Magidesk.Presentation.ViewModels
{
    public partial class RefundWizardViewModel : ObservableObject
    {
        private readonly IQueryHandler<CalculateRefundPreviewQuery, RefundPreviewDto> _previewQuery;
        private readonly ICommandHandler<RefundTicketCommand, RefundTicketResult> _refundCommand;
        private readonly ICommandHandler<AuthorizeManagerCommand, AuthorizationResult> _authHandler;
        private readonly TicketDto _ticket;
        private readonly Action _closeAction;

        [ObservableProperty]
        private int _currentStepIndex = 0;

        partial void OnCurrentStepIndexChanged(int value)
        {
            OnPropertyChanged(nameof(IsStep1Visible));
            OnPropertyChanged(nameof(IsStep2Visible));
            OnPropertyChanged(nameof(IsStep3Visible));
            OnPropertyChanged(nameof(IsStep4Visible));
            UpdateTitle();
            
            // Force UI refresh
            Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()?.TryEnqueue(() =>
            {
            });
        }

        [ObservableProperty]
        private string _stepTitle = "Refund Mode";

        [ObservableProperty]
        private string _primaryButtonText = "Next";

        [ObservableProperty]
        private RefundMode _selectedMode = RefundMode.Full;

        public bool IsFullMode
        {
            get => SelectedMode == RefundMode.Full;
            set { if (value) SelectedMode = RefundMode.Full; }
        }

        public bool IsPartialMode
        {
            get => SelectedMode == RefundMode.Partial;
            set { if (value) SelectedMode = RefundMode.Partial; }
        }

        public bool IsSpecificMode
        {
            get => SelectedMode == RefundMode.SpecificPayments;
            set { if (value) SelectedMode = RefundMode.SpecificPayments; }
        }

        partial void OnSelectedModeChanged(RefundMode value)
        {
            OnPropertyChanged(nameof(IsFullMode));
            OnPropertyChanged(nameof(IsPartialMode));
            OnPropertyChanged(nameof(IsSpecificMode));
        }

        [ObservableProperty]
        private double _partialAmountInput;

        [ObservableProperty]
        private ObservableCollection<SelectablePaymentDto> _specificPayments = new();

        [ObservableProperty]
        private RefundPreviewDto? _preview;

        [ObservableProperty]
        private string _refundReason = string.Empty;

        [ObservableProperty]
        private string _managerPin = string.Empty;

        [ObservableProperty]
        private string? _errorMessage;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        partial void OnErrorMessageChanged(string? value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        [ObservableProperty]
        private bool _isBusy;

        public bool IsNotBusy => !IsBusy;

        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        // Steps Visibility
        public bool IsStep1Visible => CurrentStepIndex == 0;
        public bool IsStep2Visible => CurrentStepIndex == 1; // Scope (Partial input or Grid)
        public bool IsStep3Visible => CurrentStepIndex == 2; // Preview
        public bool IsStep4Visible => CurrentStepIndex == 3; // Auth

        public AsyncRelayCommand NextCommand { get; }
        public RelayCommand BackCommand { get; }
        public AsyncRelayCommand ConfirmRefundCommand { get; }

        public RefundWizardViewModel(
            TicketDto ticket,
            IQueryHandler<CalculateRefundPreviewQuery, RefundPreviewDto> previewQuery,
            ICommandHandler<RefundTicketCommand, RefundTicketResult> refundCommand,
            ICommandHandler<AuthorizeManagerCommand, AuthorizationResult> authHandler,
            List<PaymentDto> payments,
            Action closeAction)
        {
            _ticket = ticket;
            _previewQuery = previewQuery;
            _refundCommand = refundCommand;
            _authHandler = authHandler;
            _closeAction = closeAction;

            foreach (var p in payments.Where(p => p.TransactionType == TransactionType.Credit && !p.IsVoided))
            {
                SpecificPayments.Add(new SelectablePaymentDto(p));
            }

            NextCommand = new AsyncRelayCommand(OnNextAsync);
            BackCommand = new RelayCommand(OnBack);
            ConfirmRefundCommand = new AsyncRelayCommand(OnConfirmAsync);

            UpdateTitle();
        }

        private async Task OnNextAsync()
        {
            ErrorMessage = null;

            try
            {
                if (CurrentStepIndex == 0) // Mode Select
                {
                    if (SelectedMode == RefundMode.Full)
                    {
                        // Skip scope step, go to Preview
                        await LoadPreviewAsync();
                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            return;
                        }
                        CurrentStepIndex = 2;
                    }
                    else
                    {
                        CurrentStepIndex = 1;
                    }
                    return;
                }

            if (CurrentStepIndex == 1) // Scope
            {
                if (SelectedMode == RefundMode.Partial && PartialAmountInput <= 0)
                {
                    ErrorMessage = "Enter a valid amount.";
                    return;
                }
                if (SelectedMode == RefundMode.SpecificPayments && !SpecificPayments.Any(p => p.IsSelected))
                {
                    ErrorMessage = "Select at least one payment.";
                    return;
                }
                
                await LoadPreviewAsync();
                
                if (!string.IsNullOrEmpty(Preview?.ValidationWarning))
                {
                    ErrorMessage = Preview.ValidationWarning;
                    return;
                }

                CurrentStepIndex = 2;
                UpdateTitle();
                OnPropertyChanged(nameof(IsStep2Visible));
                OnPropertyChanged(nameof(IsStep3Visible));
                return;
            }

            if (CurrentStepIndex == 2) // Preview
            {
                // Go to Auth
                CurrentStepIndex = 3;
                UpdateTitle();
                OnPropertyChanged(nameof(IsStep3Visible));
                OnPropertyChanged(nameof(IsStep4Visible));
                return;
            }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading refund preview: {ex.Message}";
            }
        }

        private void OnBack()
        {
            ErrorMessage = null;
            if (CurrentStepIndex == 3) CurrentStepIndex = 2;
            else if (CurrentStepIndex == 2)
            {
                 if (SelectedMode == RefundMode.Full) CurrentStepIndex = 0;
                 else CurrentStepIndex = 1;
            }
            else if (CurrentStepIndex == 1) CurrentStepIndex = 0;

            UpdateTitle();
            OnPropertyChanged(nameof(IsStep1Visible));
            OnPropertyChanged(nameof(IsStep2Visible));
            OnPropertyChanged(nameof(IsStep3Visible));
            OnPropertyChanged(nameof(IsStep4Visible));
        }

        private async Task LoadPreviewAsync()
        {
            IsBusy = true;
            try
            {
                var query = new CalculateRefundPreviewQuery
                {
                    TicketId = _ticket.Id,
                    Mode = SelectedMode,
                    PartialAmount = SelectedMode == RefundMode.Partial ? new Money((decimal)PartialAmountInput) : null,
                    SpecificPaymentIds = SpecificPayments.Where(p => p.IsSelected).Select(p => p.Id).ToList()
                };

                Preview = await _previewQuery.HandleAsync(query);
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnConfirmAsync()
        {
            if (string.IsNullOrWhiteSpace(RefundReason))
            {
                ErrorMessage = "Reason is required.";
                return;
            }

            // Manager PIN Authorization (Inline)
            if (string.IsNullOrWhiteSpace(ManagerPin))
            {
                 ErrorMessage = "Manager PIN is required.";
                 return;
            }

            IsBusy = true;
            AuthorizationResult authorizationResult = null;

            try
            {
                var authCommand = new AuthorizeManagerCommand(ManagerPin, "Refund Ticket");
                authorizationResult = await _authHandler.HandleAsync(authCommand);
            }
            catch (Exception ex)
            {
                 ErrorMessage = $"Authorization error: {ex.Message}";
                 IsBusy = false;
                 return;
            }

            if (authorizationResult == null || !authorizationResult.Authorized || authorizationResult.AuthorizingUserId == null)
            {
                ErrorMessage = authorizationResult?.FailureReason ?? "Authorization failed.";
                IsBusy = false;
                return;
            }
            try
            {
                 var command = new RefundTicketCommand
                 {
                     TicketId = _ticket.Id,
                     ProcessedBy = new UserId(authorizationResult.AuthorizingUserId.Value),
                     TerminalId = Guid.NewGuid(), // TODO: Inject ITerminalContext
                     Reason = RefundReason,
                     Mode = SelectedMode,
                     PartialAmount = SelectedMode == RefundMode.Partial ? new Money((decimal)PartialAmountInput) : null,
                     SpecificPaymentIds = SpecificPayments.Where(p => p.IsSelected).Select(p => p.Id).ToList()
                 };

                 var result = await _refundCommand.HandleAsync(command);
                 if (result.Success)
                 {
                     _closeAction.Invoke();
                 }
                 else
                 {
                     ErrorMessage = result.ErrorMessage;
                 }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateTitle()
        {
            StepTitle = CurrentStepIndex switch
            {
                0 => "Refund Mode",
                1 => "Scope Selection",
                2 => "Review Impact",
                3 => "Authorization",
                _ => "Refund"
            };

            // Hide "Next" button on the final authorization/confirmation step
            PrimaryButtonText = CurrentStepIndex == 3 ? string.Empty : "Next";
        }
    }

    public partial class SelectablePaymentDto : ObservableObject
    {
        public Guid Id { get; }
        public string Description { get; }
        public decimal Amount { get; }
        
        [ObservableProperty]
        private bool _isSelected;

        public SelectablePaymentDto(PaymentDto p)
        {
            Id = p.Id;
            Amount = p.Amount; // PaymentDto.Amount is decimal
            Description = $"{p.PaymentType} - {p.Amount:C}";
        }
    }
}
