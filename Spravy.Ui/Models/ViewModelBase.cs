using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ViewModelBase : NotifyBase
{
    [Inject]
    public required INavigator Navigator { get; init; }

    [Inject]
    public required IErrorHandler ErrorHandler { get; set; }

    protected ICommand CreateInitializedCommand(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(execute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateInitializedCommand<TArgs>(Action<TArgs> action)
    {
        var command = ReactiveCommand.Create(action);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(execute);
        SetupCommand(command);
        Disposables.Add(command);

        return command;
    }

    protected ICommand CreateCommandFromTask(Func<Task> execute, IObservable<bool> canExecute)
    {
        var command = ReactiveCommand.CreateFromTask(execute, canExecute);
        SetupCommand(command);
        Disposables.Add(command);

        return command;
    }

    protected ICommand CreateCommandFromTask<TParam>(Func<TParam, Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(execute);
        SetupCommand(command);
        Disposables.Add(command);

        return command;
    }

    private void SetupCommand<TParam, TResult>(ReactiveCommand<TParam, TResult> command)
    {
        command.ThrownExceptions.Subscribe(OnNextError);
    }

    private async void OnNextError(Exception exception)
    {
        await ErrorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
    }
}