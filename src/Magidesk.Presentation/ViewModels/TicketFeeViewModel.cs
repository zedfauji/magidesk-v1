using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace Magidesk.ViewModels;

public enum TicketFeeType
{
    ServiceCharge,
    DeliveryCharge,
    Adjustment
}

public partial class TicketFeeViewModel : ObservableObject
{
    private TicketFeeType _selectedFeeType;
    public TicketFeeType SelectedFeeType
    {
        get => _selectedFeeType;
        set
        {
            if (SetProperty(ref _selectedFeeType, value))
            {
                OnPropertyChanged(nameof(IsReasonVisible));
            }
        }
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string _reason = string.Empty;
    public string Reason
    {
        get => _reason;
        set => SetProperty(ref _reason, value);
    }

    public ObservableCollection<TicketFeeType> FeeTypes { get; } = new(Enum.GetValues<TicketFeeType>());

    public bool IsReasonVisible => SelectedFeeType == TicketFeeType.Adjustment;

    [ObservableProperty]
    private bool _isConfirmed;

    public Action? CloseAction { get; set; }
    public Action? CancelAction { get; set; }

    public TicketFeeViewModel()
    {
        SelectedFeeType = TicketFeeType.ServiceCharge;
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
