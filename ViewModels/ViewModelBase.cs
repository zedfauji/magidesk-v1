namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Base view model for MVVM.
/// </summary>
public abstract class ViewModelBase : ObservableObject
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
}
