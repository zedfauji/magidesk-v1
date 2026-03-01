using System;
using System.Threading.Tasks;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Services
{
    public class OrderEntryDialogService : IOrderEntryDialogService
    {
        public async Task ShowModifierSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.ModifierSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.ModifierSelectionDialog(viewModel);
            
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        public async Task ShowCookingInstructionAsync(Magidesk.Presentation.ViewModels.Dialogs.CookingInstructionViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.CookingInstructionDialog { ViewModel = viewModel };
            
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                viewModel.CloseAction = () => dialog.Hide();
                viewModel.CancelAction = () => dialog.Hide();

                await dialog.ShowAsync();
            }
        }

        public async Task ShowAddOnSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.AddOnSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.AddOnSelectionDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowComboSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.ComboSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.ComboSelectionDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowPizzaModifierAsync(Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.PizzaModifierDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowPriceEntryAsync(Magidesk.Presentation.ViewModels.Dialogs.PriceEntryViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.PriceEntryDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                viewModel.CancelAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowSizeSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.SizeSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.SizeSelectionDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowItemSearchAsync(Magidesk.Presentation.ViewModels.Dialogs.ItemSearchViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.ItemSearchDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowTicketFeeAsync(Magidesk.Presentation.ViewModels.TicketFeeViewModel viewModel)
        {
            var dialog = new TicketFeeDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                viewModel.CancelAction = () => dialog.Hide();
                var result = await dialog.ShowAsync();
                viewModel.IsConfirmed = result == ContentDialogResult.Primary;
            }
        }

        public async Task ShowSeatSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.SeatSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Presentation.Views.Dialogs.SeatSelectionDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowMiscItemAsync(Magidesk.Presentation.ViewModels.MiscItemViewModel viewModel)
        {
            var dialog = new MiscItemDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                viewModel.CancelAction = () => dialog.Hide();
                var result = await dialog.ShowAsync();
                viewModel.IsConfirmed = result == ContentDialogResult.Primary;
            }
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                var dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
    }
}
