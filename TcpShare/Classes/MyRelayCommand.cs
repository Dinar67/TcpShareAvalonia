using System;
using System.Windows.Input;

namespace TcpShare.Classes;

public class MyRelayCommand : ICommand
{
    private Action<object?> _execute;
    private Func<object?, bool> _canExecute;
    public MyRelayCommand(Action<object?> execute, Func<object?, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter)
        => _canExecute == null ? true : _canExecute!.Invoke(parameter);
    public void Execute(object? parameter)
        => _execute.Invoke(parameter);

    public event EventHandler? CanExecuteChanged;
}