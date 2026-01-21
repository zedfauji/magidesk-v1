using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.ViewModels;

public partial class MiscItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private string _priceString = string.Empty;

    [ObservableProperty]
    private bool _isConfirmed;

    public Action? CloseAction { get; set; }
    public Action? CancelAction { get; set; }

    // TODO: Add Tax Group support when TaxService is fully implemented
    // For now, allow simple Price/Description entry.

    public MiscItemViewModel()
    {
    }

    [RelayCommand]
    private void AppendNumber(string number)
    {
        if (PriceString.Contains(".") && number == ".") return; // Prevent double decimal
        
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

    [RelayCommand]
    private void Confirm()
    {
        IsConfirmed = true;
        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        CancelAction?.Invoke();
    }
}
