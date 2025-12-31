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
        private readonly ITerminalContext _terminalContext;
        private readonly ICommandHandler<ClockInCommand> _clockInHandler;
        private readonly ICommandHandler<ClockOutCommand> _clockOutHandler;
        private readonly ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> _closeSessionHandler;

        public ManagerFunctionsViewModel(
            NavigationService navigationService,
            ICashSessionRepository cashSessionRepository,
            IUserService userService,
            ITerminalContext terminalContext,
            ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> closeSessionHandler,
            ICommandHandler<ClockInCommand> clockInHandler,
            ICommandHandler<ClockOutCommand> clockOutHandler)
        {
            _navigationService = navigationService;
            _cashSessionRepository = cashSessionRepository;
            _userService = userService;
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

            _navigationService.Navigate(typeof(Views.BackOfficePage));
        }

        [RelayCommand]
        private async Task ClockInOutAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            if (_userService.CurrentUser?.Id == null) return;

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
                    await _clockInHandler.HandleAsync(new ClockInCommand { UserId = _userService.CurrentUser.Id });
                    
                    var success = new Microsoft.UI.Xaml.Controls.ContentDialog
                    {
                         Title = "Success",
                         Content = "Clocked In Successfully",
                         CloseButtonText = "OK",
                         XamlRoot = App.MainWindowInstance.Content.XamlRoot
                    };
                    await _navigationService.ShowDialogAsync(success);
                }
                else if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Secondary)
                {
                    // Clock Out
                    await _clockOutHandler.HandleAsync(new ClockOutCommand { UserId = _userService.CurrentUser.Id });

                    var success = new Microsoft.UI.Xaml.Controls.ContentDialog
                    {
                         Title = "Success",
                         Content = "Clocked Out Successfully",
                         CloseButtonText = "OK",
                         XamlRoot = App.MainWindowInstance.Content.XamlRoot
                    };
                    await _navigationService.ShowDialogAsync(success);
                }
            }
            catch (Exception ex)
            {
                var error = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                        Title = "Error",
                        Content = $"Clock Action Failed: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await _navigationService.ShowDialogAsync(error);
            }
        }

        [RelayCommand]
        private async Task SettingsAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0111 / F-0128: System Config
            // Navigate to SystemConfigPage
            _navigationService.Navigate(typeof(Views.SystemConfigPage)); 
            await Task.CompletedTask;
        }
        
        [RelayCommand]
        private async Task EndShiftAsync()
        {
            CloseAction?.Invoke();
            await Task.Delay(100);

            // F-0061: End Shift
            if (_userService.CurrentUser?.Id == null || _terminalContext.TerminalId == null)
            {
                return;
            }

            var terminalId = _terminalContext.TerminalId.Value;
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);

            if (session == null)
            {
                var noSessionDialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = "No Active Session",
                    Content = "There is no active cash session to close.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await _navigationService.ShowDialogAsync(noSessionDialog);
                return;
            }

            // Create ViewModel manually to pass session
            var vm = new Dialogs.ShiftEndViewModel(session, _userService.CurrentUser.Id, _closeSessionHandler);
            var dialog = new Views.Dialogs.ShiftEndDialog(vm);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            await _navigationService.ShowDialogAsync(dialog);
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
