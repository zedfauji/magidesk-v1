using System;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public sealed class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (_canExecute == null) return true;

        if (parameter == null && default(T) == null) return _canExecute(default);

        // Attempt to convert parameter to T
        if (parameter is T t) return _canExecute(t);
        
        // Special case for strings from CommandParameter if T is int, etc. (Not needed for string->string)
        try 
        {
             return _canExecute((T)Convert.ChangeType(parameter, typeof(T)));
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
             _execute((T)Convert.ChangeType(parameter, typeof(T)));
        }
        catch
        {
             // Ignore or log
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
