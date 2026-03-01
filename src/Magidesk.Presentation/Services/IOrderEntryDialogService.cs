using System.Threading.Tasks;

// No usings for ViewModels to avoid ambiguity

namespace Magidesk.Presentation.Services
{
    public interface IOrderEntryDialogService
    {
        Task ShowModifierSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.ModifierSelectionViewModel viewModel);
        Task ShowCookingInstructionAsync(Magidesk.Presentation.ViewModels.Dialogs.CookingInstructionViewModel viewModel);
        Task ShowAddOnSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.AddOnSelectionViewModel viewModel);
        Task ShowComboSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.ComboSelectionViewModel viewModel);
        Task ShowPizzaModifierAsync(Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel viewModel);
        Task ShowPriceEntryAsync(Magidesk.Presentation.ViewModels.Dialogs.PriceEntryViewModel viewModel);
        Task ShowSizeSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.SizeSelectionViewModel viewModel);
        Task ShowItemSearchAsync(Magidesk.Presentation.ViewModels.Dialogs.ItemSearchViewModel viewModel);
        Task ShowTicketFeeAsync(Magidesk.Presentation.ViewModels.TicketFeeViewModel viewModel);
        Task ShowSeatSelectionAsync(Magidesk.Presentation.ViewModels.Dialogs.SeatSelectionViewModel viewModel);
        Task ShowMiscItemAsync(Magidesk.Presentation.ViewModels.MiscItemViewModel viewModel);
        Task ShowErrorAsync(string title, string message);
    }
}
