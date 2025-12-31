using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class PaymentProcessWaitViewModel : ViewModelBase
{
    private string _message = "Please wait...";

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
    public bool CanClose { get; set; } = false;

    public PaymentProcessWaitViewModel()
    {
        Title = "Processing Payment";
    }

    public void SetMessage(string message)
    {
        Message = message;
    }

    public void AllowClose()
    {
        CanClose = true;
    }
}
