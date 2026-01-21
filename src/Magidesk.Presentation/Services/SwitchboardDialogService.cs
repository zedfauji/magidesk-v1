using System.Threading.Tasks;
using Magidesk.Presentation;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;

namespace Magidesk.Presentation.Services
{
    public class SwitchboardDialogService : ISwitchboardDialogService
    {
        private readonly NavigationService _navigationService;

        public SwitchboardDialogService(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public async Task ShowOrderTypeSelectionAsync(OrderTypeSelectionViewModel viewModel)
        {
            var dialog = new OrderTypeSelectionDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                await _navigationService.ShowDialogAsync(dialog);
            }
        }

        public async Task ShowGuestCountAsync(GuestCountViewModel viewModel)
        {
            var dialog = new GuestCountDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                await _navigationService.ShowDialogAsync(dialog);
            }
        }

        public async Task ShowShiftStartAsync(ShiftStartViewModel viewModel)
        {
            var dialog = new ShiftStartDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                await _navigationService.ShowDialogAsync(dialog);
            }
        }
    }
}
