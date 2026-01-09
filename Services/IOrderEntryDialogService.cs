using Magidesk.ViewModels;
using Magidesk.ViewModels.Dialogs;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Services
{
    public interface IOrderEntryDialogService
    {
        Task ShowModifierSelectionAsync(ModifierSelectionViewModel viewModel);
        Task ShowCookingInstructionAsync(CookingInstructionViewModel viewModel);
        Task ShowAddOnSelectionAsync(AddOnSelectionViewModel viewModel);
        Task ShowComboSelectionAsync(ComboSelectionViewModel viewModel);
        Task ShowPizzaModifierAsync(PizzaModifierViewModel viewModel);
        Task ShowPriceEntryAsync(PriceEntryViewModel viewModel);
        Task ShowSizeSelectionAsync(SizeSelectionViewModel viewModel);
        Task ShowItemSearchAsync(ItemSearchViewModel viewModel);
        Task ShowTicketFeeAsync(TicketFeeViewModel viewModel);
        Task ShowSeatSelectionAsync(SeatSelectionViewModel viewModel);
        Task ShowMiscItemAsync(MiscItemViewModel viewModel);
        Task ShowErrorAsync(string title, string message);
    }
}
