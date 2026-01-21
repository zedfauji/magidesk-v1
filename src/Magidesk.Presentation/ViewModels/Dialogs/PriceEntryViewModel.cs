using System;
using System.Windows.Input;
using Magidesk.ViewModels;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class PriceEntryViewModel : ViewModelBase
{
    private decimal _price;
    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public string ItemName { get; }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    
    // Number Pad Commands
    public ICommand AppendDigitCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand BackspaceCommand { get; }

    public System.Action? CloseAction { get; set; }
    public System.Action? CancelAction { get; set; }

    public bool IsConfirmed { get; private set; }

    public PriceEntryViewModel(string itemName, decimal initialPrice = 0)
    {
        Title = "Price Entry";
        ItemName = itemName;
        Price = initialPrice;

        ConfirmCommand = new RelayCommand(() => { IsConfirmed = true; CloseAction?.Invoke(); });
        CancelCommand = new RelayCommand(() => { IsConfirmed = false; CancelAction?.Invoke(); });
        
        AppendDigitCommand = new RelayCommand<string>(AppendDigit);
        ClearCommand = new RelayCommand(() => Price = 0);
        BackspaceCommand = new RelayCommand(Backspace);
    }

    private void AppendDigit(string digit)
    {
        // Simple logic: Shift current value and add digit? Or string manipulation?
        // Money usually implies 2 decimal places. 
        // Logic: Treat current Price as integer (cents) -> shift -> / 100?
        // Or standard Calculator style. 
        // Let's use standard: Price * 10 + digit (if treating as whole number) isn't right for currency unless we track cents.
        
        // Let's stick to string parsing for safety or simple accumulation based on needs.
        // F-0024 used a Numpad for Quantity (Integer).
        // For Price, we likely want "125" -> 1.25 or 125.00?
        // Let's assume user enters DOLLARS.CENTS or raw numbers.
        // Simpler implementation: String-based entry then parse.
        
        // Actually, let's keep it simple: string representation handling
        // But property is decimal. 
        // Let's defer strict keypad logic and rely on TextBox binding for now? 
        // F-0035 says "Numeric keypad".
        // Let's treat Price as the value.
        // If user types '5', Price = 5. '0' -> 50.
        // Wait, typical POS: 1 -> $0.01; 12 -> $0.12; 125 -> $1.25. (Accumulator)
        
        // Impl: Accumulator Logic
        long cents = (long)(Price * 100);
        if (long.TryParse(digit, out var d))
        {
             cents = (cents * 10) + d;
             Price = cents / 100m;
        }
    }

    private void Backspace()
    {
        long cents = (long)(Price * 100);
        cents = cents / 10;
        Price = cents / 100m;
    }
}
