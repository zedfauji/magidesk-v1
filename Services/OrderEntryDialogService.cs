using System;
using System.Threading.Tasks;
using Magidesk.ViewModels;
using Magidesk.ViewModels.Dialogs;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Views.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Services
{
    public class OrderEntryDialogService : IOrderEntryDialogService
    {
        public async Task ShowModifierSelectionAsync(ModifierSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.ModifierSelectionDialog(viewModel);
            
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        public async Task ShowCookingInstructionAsync(CookingInstructionViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.CookingInstructionDialog { ViewModel = viewModel };
            
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                viewModel.CloseAction = () => dialog.Hide();
                viewModel.CancelAction = () => dialog.Hide();

                await dialog.ShowAsync();
            }
        }

        public async Task ShowAddOnSelectionAsync(AddOnSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.AddOnSelectionDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowComboSelectionAsync(ComboSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.ComboSelectionDialog(viewModel);
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowPizzaModifierAsync(PizzaModifierViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.PizzaModifierDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowPriceEntryAsync(PriceEntryViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.PriceEntryDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                viewModel.CancelAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowSizeSelectionAsync(SizeSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.SizeSelectionDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowItemSearchAsync(ItemSearchViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.ItemSearchDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowTicketFeeAsync(TicketFeeViewModel viewModel)
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

        public async Task ShowSeatSelectionAsync(SeatSelectionViewModel viewModel)
        {
            var dialog = new Magidesk.Views.Dialogs.SeatSelectionDialog { ViewModel = viewModel };
            if (App.MainWindowInstance?.Content?.XamlRoot != null)
            {
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                viewModel.CloseAction = () => dialog.Hide();
                await dialog.ShowAsync();
            }
        }

        public async Task ShowMiscItemAsync(MiscItemViewModel viewModel)
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
