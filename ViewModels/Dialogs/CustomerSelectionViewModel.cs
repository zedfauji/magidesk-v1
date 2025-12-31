using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class CustomerSelectionViewModel : ObservableObject
{
    private string _guestName = string.Empty;
    public string GuestName
    {
        get => _guestName;
        set
        {
            SetProperty(ref _guestName, value);
            // Notify command change if needed
            ((IRelayCommand)ConfirmCommand).NotifyCanExecuteChanged();
        }
    }

    private string _phoneNumber = string.Empty;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    public Action? CloseAction { get; set; }
    public bool IsConfirmed { get; private set; }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public CustomerSelectionViewModel()
    {
        ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    private bool CanConfirm()
    {
        return !string.IsNullOrWhiteSpace(GuestName);
    }

    private void Confirm()
    {
        if (CanConfirm())
        {
            IsConfirmed = true;
            CloseAction?.Invoke();
        }
    }

    private void Cancel()
    {
        IsConfirmed = false;
        CloseAction?.Invoke();
    }
}
