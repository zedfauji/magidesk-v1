using System.Threading.Tasks;

namespace Magidesk.Presentation.Services
{
    public interface ISwitchboardDialogService
    {
        Task ShowOrderTypeSelectionAsync(Magidesk.Presentation.ViewModels.OrderTypeSelectionViewModel viewModel);
        Task ShowGuestCountAsync(Magidesk.Presentation.ViewModels.GuestCountViewModel viewModel);
        Task ShowShiftStartAsync(Magidesk.Presentation.ViewModels.ShiftStartViewModel viewModel);
    }
}
