using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public partial class GuestCountViewModel : ObservableObject
{
    private int _guestCount = 0;

    public int GuestCount
    {
        get => _guestCount;
        set
        {
            if (SetProperty(ref _guestCount, value))
            {
                OnPropertyChanged(nameof(GuestCountText));
            }
        }
    }

    public string GuestCountText => GuestCount == 0 ? "" : GuestCount.ToString();

    public GuestCountViewModel()
    {
    }

    [RelayCommand]
    private void AppendNumber(string number)
    {
        if (int.TryParse(number, out int digit))
        {
            // Limit to reasonable number of guests (e.g., 999) to prevent overflow/nonsense
            if (GuestCount < 100) 
            {
                GuestCount = (GuestCount * 10) + digit;
            }
        }
    }

    [RelayCommand]
    private void Clear()
    {
        GuestCount = 0;
    }

    [RelayCommand]
    private void Backspace()
    {
        GuestCount = GuestCount / 10;
    }

    [RelayCommand]
    private void Confirm()
    {
        // ContentDialog Confirm logic is mostly handled by the button, 
        // but we might want validation here (e.g. Ensure > 0)
        // For now, allow 0 or generic confirmation.
    }
}
