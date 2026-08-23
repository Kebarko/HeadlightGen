using System.Windows.Input;

namespace KE.MSTS.HeadlightGen.ViewModels.Common;

/// <summary>
/// A generic implementation of <see cref="ICommand"/> that relays command execution and can-execute logic to delegates.
/// </summary>
/// <remarks>
/// This class provides a simple way to create commands in MVVM scenarios by delegating the execute and can-execute 
/// logic to Action and Func delegates respectively. It automatically handles command re-query notifications through 
/// the <see cref="CommandManager.RequerySuggested"/> event.
/// </remarks>
public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
{
    /// <summary>
    /// The action to execute when the command is invoked.
    /// </summary>
    private readonly Action<object?> execute = execute;

    /// <summary>
    /// The function to determine whether the command can be executed. If null, the command is always executable.
    /// </summary>
    private readonly Func<object?, bool>? canExecute = canExecute;

    /// <summary>
    /// Occurs when the ability to execute the command changes.
    /// </summary>
    /// <remarks>
    /// This event is automatically triggered by <see cref="CommandManager.RequerySuggested"/>, which WPF uses to 
    /// notify all commands when their can-execute status may have changed.
    /// </remarks>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Determines whether the command can be executed with the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter passed to the command.</param>
    /// <returns>true if the command can be executed; otherwise, false. Returns true if no can-execute function was provided.</returns>
    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

    /// <summary>
    /// Executes the command with the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter passed to the command.</param>
    public void Execute(object? parameter) => execute(parameter);
}