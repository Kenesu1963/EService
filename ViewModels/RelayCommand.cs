using System;
using System.Windows.Input;

namespace EService.ViewModels;

/// <summary>
/// This is the C# equivalent of Java's ActionListener. Instead of writing:
///
///     button.addActionListener(new ActionListener() {
///         public void actionPerformed(ActionEvent evt) { ... }
///     });
///
/// ...a Button in XAML just does:  Command="{Binding SaveCommand}"
/// and the ViewModel exposes:      public ICommand SaveCommand { get; }
///
/// RelayCommand is a small, dependency-free way to turn a plain method into
/// something a XAML "Command" binding understands. (Frameworks like
/// CommunityToolkit.Mvvm generate this for you with an attribute, but we're
/// keeping this project dependency-light and easy to read, so it's written
/// out by hand here, once, and reused everywhere.)
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();

    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Call this if a button's enabled/disabled state depends on something
    /// that just changed (e.g. a text field became non-empty).
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Same idea as RelayCommand, but for commands that need a parameter -
/// e.g. "add THIS dish to the cart" where the dish is passed in from XAML
/// via CommandParameter="{Binding}".
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

    public void Execute(object? parameter) => _execute((T?)parameter);

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
