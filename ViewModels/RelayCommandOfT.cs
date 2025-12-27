using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter == null && default(T) == null)
        {
             return _canExecute?.Invoke(default) ?? true;
        }

        if (parameter is T t)
        {
            return _canExecute?.Invoke(t) ?? true;
        }
        
        // Try simple conversion if possible (e.g. string to int) or just return false
        try
        {
             return _canExecute?.Invoke((T?)parameter) ?? true;
        } 
        catch 
        {
             return false;
        }
    }

    public void Execute(object? parameter)
    {
        if (parameter == null && default(T) == null)
        {
             _execute(default);
             return;
        }

        if (parameter is T t)
        {
            _execute(t);
            return;
        }
        
        try 
        {
            _execute((T?)parameter);
        }
        catch 
        {
            // Ignore execution with invalid parameter
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
