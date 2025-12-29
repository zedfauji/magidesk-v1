using CommunityToolkit.Mvvm.ComponentModel;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class PaymentProcessWaitViewModel : ObservableObject
{
    private string _message = "Please wait...";

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
    public bool CanClose { get; set; } = false;

    public void SetMessage(string message)
    {
        Message = message;
    }

    public void AllowClose()
    {
        CanClose = true;
    }
}
