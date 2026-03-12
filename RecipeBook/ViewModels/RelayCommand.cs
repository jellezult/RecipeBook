using System.Windows.Input;

namespace RecipeBook.ViewModels;

public class RelayCommand : ICommand
{
    private readonly Action execute;
    private readonly Func<bool>? canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => System.Windows.Input.CommandManager.RequerySuggested += value;
        remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => this.canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => this.execute();
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> execute;
    private readonly Predicate<T>? canExecute;

    public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => System.Windows.Input.CommandManager.RequerySuggested += value;
        remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        if (this.canExecute == null) return true;
        return parameter is T t && this.canExecute(t);
    }

    public void Execute(object? parameter)
    {
        if (parameter is T t)
            this.execute(t);
    }
}
