using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Presentation.Services;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.ViewModels
{
    public partial class ManagerFunctionsViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly ICashSessionRepository _cashSessionRepository;
        private readonly IUserService _userService;
        private readonly IUserContextService _userContextService;
        private readonly ITerminalContext _terminalContext;
        private readonly ICommandHandler<ClockInCommand> _clockInHandler;
        private readonly ICommandHandler<ClockOutCommand> _clockOutHandler;
        private readonly ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> _closeSessionHandler;

        public ManagerFunctionsViewModel(
            NavigationService navigationService,
            ICashSessionRepository cashSessionRepository,
            IUserService userService,
            IUserContextService userContextService,
            ITerminalContext terminalContext,
            ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> closeSessionHandler,
            ICommandHandler<ClockInCommand> clockInHandler,
            ICommandHandler<ClockOutCommand> clockOutHandler)
        {
            _navigationService = navigationService;
            _cashSessionRepository = cashSessionRepository;
            _userService = userService;
            _userContextService = userContextService;
            _terminalContext = terminalContext;
            _closeSessionHandler = closeSessionHandler;
            _clockInHandler = clockInHandler;
            _clockOutHandler = clockOutHandler;
            
            Title = "Manager Functions";
        }

        public Action? CloseAction { get; set; }

        [RelayCommand]
        private async Task DrawerPullAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0012: Drawer Pull Report Dialog (Existing View)
            var dialog = new Views.DrawerPullReportDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            await _navigationService.ShowDialogAsync(dialog);
        }

        [RelayCommand]
        private async Task CashDropAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0010: Cash Drop Dialog (Existing View)
            var dialog = new Views.CashDropManagementDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            await _navigationService.ShowDialogAsync(dialog);
        }

        [RelayCommand]
        private async Task OpenTicketsAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0011: Open Tickets Dialog (Existing View)
            var dialog = new Views.OpenTicketsListDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            await _navigationService.ShowDialogAsync(dialog);
        }

        [RelayCommand]
        private async Task ReportsAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // Navigate to BackOffice with Reports selected
            _navigationService.Navigate(typeof(Views.BackOfficePage), typeof(Views.SalesReportsPage));
        }

        [RelayCommand]
        private async Task ClockInOutAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            if (_userContextService.GetCurrentUserId() == Guid.Empty) 
            {
                var errorDialog = new Views.Dialogs.ConfirmationDialog();
                errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                await errorDialog.ShowConfirmationAsync(
                    "Error",
                    "No user is currently logged in.",
                    "OK",
                    "",
                    "❌",
                    "Error",
                    "Please log in before using the time clock.");
                return;
            }

            // Simple dialog to choose action
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Time Clock",
                Content = "Select action:",
                PrimaryButtonText = "Clock In",
                SecondaryButtonText = "Clock Out",
                CloseButtonText = "Cancel",
                XamlRoot = App.MainWindowInstance.Content.XamlRoot
            };

            var result = await _navigationService.ShowDialogAsync(dialog);

            try
            {
                if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    // Clock In
                    await _clockInHandler.HandleAsync(new ClockInCommand { UserId = _userContextService.GetCurrentUserId() });
                    
                    var successDialog = new Views.Dialogs.ConfirmationDialog();
                    successDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                    
                    await successDialog.ShowConfirmationAsync(
                        "Clock In Successful",
                        "You have been clocked in successfully.",
                        "OK",
                        "",
                        "✅",
                        "Success",
                        $"Time: {DateTime.Now:g}\nUser: {_userService.CurrentUser?.FirstName} {_userService.CurrentUser?.LastName}");
                }
                else if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Secondary)
                {
                    // Clock Out
                    await _clockOutHandler.HandleAsync(new ClockOutCommand { UserId = _userContextService.GetCurrentUserId() });

                    var successDialog = new Views.Dialogs.ConfirmationDialog();
                    successDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                    
                    await successDialog.ShowConfirmationAsync(
                        "Clock Out Successful",
                        "You have been clocked out successfully.",
                        "OK",
                        "",
                        "✅",
                        "Success",
                        $"Time: {DateTime.Now:g}\nUser: {_userService.CurrentUser?.FirstName} {_userService.CurrentUser?.LastName}");
                }
            }
            catch (Exception ex)
            {
                var errorDialog = new Views.Dialogs.ConfirmationDialog();
                errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                await errorDialog.ShowConfirmationAsync(
                    "Clock Action Failed",
                    "The time clock operation could not be completed.",
                    "OK",
                    "",
                    "❌",
                    "Error",
                    $"Error details: {ex.Message}\n\nPlease try again or contact your manager if the problem persists.");
            }
        }

        [RelayCommand]
        private async Task SettingsAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0111 / F-0128: System Config
            // Navigate to BackOffice with SystemConfig selected
            _navigationService.Navigate(typeof(Views.BackOfficePage), typeof(Views.SystemConfigPage)); 
            await Task.CompletedTask;
        }
        
        [RelayCommand]
        private async Task EndShiftAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0061: End Shift
            if (_userContextService.GetCurrentUserId() == Guid.Empty || _terminalContext.TerminalId == null)
            {
                var errorDialog = new Views.Dialogs.ConfirmationDialog();
                errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                await errorDialog.ShowConfirmationAsync(
                    "Error",
                    "Unable to end shift: missing user or terminal context.",
                    "OK",
                    "",
                    "❌",
                    "Error",
                    "Please ensure you are logged in and the terminal is properly configured.");
                return;
            }

            var terminalId = _terminalContext.TerminalId.Value;
            
            try
            {
                var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);

                if (session == null)
                {
                    var noSessionDialog = new Views.Dialogs.ConfirmationDialog();
                    noSessionDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                    
                    await noSessionDialog.ShowConfirmationAsync(
                        "No Active Session",
                        "There is no active cash session to close.",
                        "OK",
                        "",
                        "ℹ️",
                        "Info",
                        "A cash session must be active to end a shift. Please start a cash session first.");
                    return;
                }

                // Show confirmation dialog
                var confirmationDialog = new Views.Dialogs.ConfirmationDialog();
                confirmationDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                var confirmed = await confirmationDialog.ShowConfirmationAsync(
                    "End Shift",
                    "Are you sure you want to end the current shift?",
                    "End Shift",
                    "Cancel",
                    "🕐",
                    "Warning",
                    "This will close the active cash session and require reconciliation.");

                if (!confirmed)
                {
                    return; // User cancelled
                }

                // Create ViewModel manually to pass session
                var vm = new Dialogs.ShiftEndViewModel(session, _userContextService.GetCurrentUserId(), _closeSessionHandler);
                var dialog = new Views.Dialogs.ShiftEndDialog(vm);
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                await _navigationService.ShowDialogAsync(dialog);
            }
            catch (Exception ex)
            {
                var errorDialog = new Views.Dialogs.ConfirmationDialog();
                errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                await errorDialog.ShowConfirmationAsync(
                    "Error",
                    "An error occurred while ending the shift.",
                    "OK",
                    "",
                    "❌",
                    "Error",
                    $"Error details: {ex.Message}\n\nPlease try again or contact your system administrator.");
            }
        }

        [RelayCommand]
        private async Task RefundTicketAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0051: Refund Button (Entry to Refund/Void Screen)
            // Navigate to Ticket Management (Explorer) which has Refund logic
            _navigationService.Navigate(typeof(Views.TicketManagementPage));
        }

        [RelayCommand]
        private async Task GroupSettleAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0046: Group Settle Ticket Dialog
            var selectionWindow = new Views.GroupSettleTicketSelectionWindow();
            selectionWindow.XamlRoot = App.MainWindowInstance.Content.XamlRoot; // Ensure Root
            var selectionResult = await _navigationService.ShowDialogAsync(selectionWindow);

            if (selectionResult == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                var selectedTickets = selectionWindow.ViewModel.AvailableTickets
                    .Where(t => t.IsSelected)
                    .ToList();

                if (selectedTickets.Count > 0)
                {
                    var groupSettleDialog = new Views.GroupSettleTicketDialog();
                    groupSettleDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot; // Ensure Root
                    var selectedTicketsCollection = new System.Collections.ObjectModel.ObservableCollection<GroupSettleTicketDto>(selectedTickets);
                    groupSettleDialog.ViewModel.SetSelectedTickets(selectedTicketsCollection);
                    
                    await _navigationService.ShowDialogAsync(groupSettleDialog);
                }
            }
        }
    }
}
