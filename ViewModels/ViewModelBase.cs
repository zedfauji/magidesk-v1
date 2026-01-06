using CommunityToolkit.Mvvm.ComponentModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Base view model for MVVM.
/// </summary>
public abstract class ViewModelBase : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private bool _isBusy;
    private string? _title;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string? _statusMessage;
    public string? StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
}
