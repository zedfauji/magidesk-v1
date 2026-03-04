using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for tender entry operations.
/// Handles keypad input, clear, and quick cash entry.
/// </summary>
public partial class SettlePageViewModel
{
    private void OnKeypadDigit(string? digit)
    {
        // Debounce to prevent double-triggering from WinUI button template
        var now = DateTime.Now;
        if ((now - _lastKeypadPress).TotalMilliseconds < KEYPAD_DEBOUNCE_MS)
        {
            System.Diagnostics.Debug.WriteLine($"OnKeypadDigit: Debounced duplicate call for digit {digit}");
            return;
        }
        _lastKeypadPress = now;

        System.Diagnostics.Debug.WriteLine($"OnKeypadDigit called with digit: {digit}");
        _logger.LogInformation("OnKeypadDigit called with digit: {Digit}", digit);
        
        if (string.IsNullOrEmpty(digit))
        {
            System.Diagnostics.Debug.WriteLine("OnKeypadDigit: digit is null or empty, returning");
            return;
        }

        // Handle decimal point
        if (digit == ".")
        {
            // Only allow one decimal point
            if (!_tenderAmountInput.Contains("."))
            {
                // If input is empty, start with "0."
                if (string.IsNullOrEmpty(_tenderAmountInput))
                {
                    _tenderAmountInput = "0.";
                }
                else
                {
                    _tenderAmountInput += ".";
                }
                TenderAmountDisplay = "$" + _tenderAmountInput;
                System.Diagnostics.Debug.WriteLine($"OnKeypadDigit: Added decimal point. Input='{_tenderAmountInput}', Display='{TenderAmountDisplay}'");
            }
            return;
        }

        // Handle digits 0-9
        if (digit.Length == 1 && char.IsDigit(digit[0]))
        {
            // Append digit to raw input
            _tenderAmountInput += digit;

            // Try to parse and format
            if (decimal.TryParse(_tenderAmountInput, out var amount))
            {
                _tenderAmount = amount;
                
                // If there's a decimal point in the input, show it as-is with $ prefix
                if (_tenderAmountInput.Contains("."))
                {
                    TenderAmountDisplay = "$" + _tenderAmountInput;
                }
                else
                {
                    // No decimal point yet, format as currency
                    TenderAmountDisplay = FormatCurrency(amount);
                }
            }
            else
            {
                // Keep building the string
                TenderAmountDisplay = "$" + _tenderAmountInput;
            }
            
            System.Diagnostics.Debug.WriteLine($"OnKeypadDigit: Input='{_tenderAmountInput}', Display='{TenderAmountDisplay}', Amount={_tenderAmount}");
        }
    }

    private string FormatCurrency(decimal amount)
    {
        return amount.ToString("C2");
    }

    private void OnClearTender()
    {
        _tenderAmount = 0m;
        _tenderAmountInput = "";
        TenderAmountDisplay = "$0.00";
        
        _logger.LogDebug("Tender amount cleared");
    }

    private void OnQuickCash(decimal amount)
    {
        System.Diagnostics.Debug.WriteLine($"OnQuickCash called with amount: {amount}");
        _logger.LogInformation("OnQuickCash called with amount: {Amount}", amount);
        
        if (amount <= 0)
        {
            System.Diagnostics.Debug.WriteLine("OnQuickCash: amount is zero or negative, returning");
            return;
        }

        _tenderAmount = amount;
        _tenderAmountInput = amount.ToString("F2"); // Store as "20.00" format
        TenderAmountDisplay = FormatCurrency(amount);
        
        _logger.LogDebug("Quick cash amount set to {Amount}", amount);
    }

}
