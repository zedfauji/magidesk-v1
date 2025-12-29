using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class QuantityViewModel : ViewModelBase
{
    private readonly int _maxQuantity = 999;
    
    private string _displayText = "1";
    public string DisplayText
    {
        get => _displayText;
        set => SetProperty(ref _displayText, value);
    }
    
    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    
    private decimal _currentValue = 1;
    
    public decimal ResultQuantity => _currentValue;

    public QuantityViewModel()
    {
        Title = "Enter Quantity";
    }

    [RelayCommand]
    private void AppendNumber(string number)
    {
        ErrorMessage = string.Empty;
        
        if (DisplayText == "0" || DisplayText == "1") // Reset initial default if user starts typing
        {
            // If it's the initial state, replace unless user types '0' repeatedly
            if (_currentValue == 1 && DisplayText == "1")
            {
                 DisplayText = number;
            }
            else
            {
                DisplayText += number;
            }
        }
        else
        {
            DisplayText += number;
        }

        if (decimal.TryParse(DisplayText, out var result))
        {
            if (result > _maxQuantity)
            {
                DisplayText = _maxQuantity.ToString();
                ErrorMessage = $"Max quantity is {_maxQuantity}";
            }
            _currentValue = decimal.Parse(DisplayText);
        }
    }

    [RelayCommand]
    private void Backspace()
    {
        ErrorMessage = string.Empty;
        if (DisplayText.Length > 1)
        {
            DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
        }
        else
        {
            DisplayText = "0";
        }
        
        if (decimal.TryParse(DisplayText, out var result))
        {
            _currentValue = result;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        ErrorMessage = string.Empty;
        DisplayText = "0";
        _currentValue = 0;
    }
}
