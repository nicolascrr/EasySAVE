using System;
using System.Windows.Input;
using Client.Model;

namespace ProjetG2AdminDev.Command;

public class ActionCommand : ICommand
{
    Predicate<object> _canExecute;
    Action<object> _execute;
    private Action<object?, BackupJob?> pauseBackupJob;

    public event EventHandler? CanExecuteChanged;

    public ActionCommand(Action<object> execute) : this(null, execute)
    {
    }

    public ActionCommand(Predicate<object> canExecute, Action<object> execute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        if (_canExecute == null)
        {
            return true;
        }

        return _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}