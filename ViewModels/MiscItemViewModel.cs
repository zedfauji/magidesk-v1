using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.ViewModels;

public partial class MiscItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private string _priceString = string.Empty;

    // TODO: Add Tax Group support when TaxService is fully implemented
    // For now, allow simple Price/Description entry.

    public MiscItemViewModel()
    {
    }

    [RelayCommand]
    private void AppendNumber(string number)
    {
        if (PriceString.Contains(".") && number == ".") return; // Prevent double decimal
        
        // Basic numpad logic (simulating string entry for now, or could use math)
        // Similar to QuantityViewModel logic if we want consistency
        PriceString += number;
        if (decimal.TryParse(PriceString, out var result))
        {
            Price = result;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        PriceString = string.Empty;
        Price = 0;
    }
}
