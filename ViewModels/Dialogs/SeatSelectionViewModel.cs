using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class SeatSelectionViewModel : ViewModelBase
{
    private string _input = "";
    public string Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }

    public Action? CloseAction { get; set; }
    public bool IsConfirmed { get; private set; }

    public int? ResultSeatNumber => int.TryParse(Input, out int val) ? val : null;

    [RelayCommand]
    private void Append(string value)
    {
        if (Input.Length < 2) // Max 99
        {
            Input += value;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Input = "";
    }

    [RelayCommand]
    private void Confirm()
    {
        if (!string.IsNullOrEmpty(Input))
        {
            IsConfirmed = true;
            CloseAction?.Invoke();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        CloseAction?.Invoke();
    }
}
