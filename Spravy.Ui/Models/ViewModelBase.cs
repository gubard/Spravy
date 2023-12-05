using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Models;

public class ViewModelBase : NotifyBase
{
    private static readonly TimeSpan TaskTimeout = TimeSpan.FromSeconds(1);

    static ViewModelBase()
    {
    }

    [Inject]
    public required INavigator Navigator { get; init; }

    [Inject]
    public required IDialogViewer DialogViewer { get; set; }

    protected ICommand CreateInitializedCommand(Action action)
    {
        var command = ReactiveCommand.Create(action);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateInitializedCommand(Func<ConfiguredTaskAwaitable> action)
    {
        var command = ReactiveCommand.Create(action);
        SetupCommand(command);

        return command;
    }

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

    protected ICommand CreateCommand(Action action)
    {
        var command = ReactiveCommand.Create(action);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommand(Func<DispatcherOperation> action)
    {
        var command = ReactiveCommand.Create(action);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommand<TArgs>(Action<TArgs> action)
    {
        var command = ReactiveCommand.Create(action);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(execute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask(Func<ConfiguredTaskAwaitable> execute)
    {
        var command = ReactiveCommand.CreateFromTask(async () => await execute.Invoke());
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask<TParam>(Func<TParam, Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(execute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask<TParam>(Func<TParam, ConfiguredTaskAwaitable> execute)
    {
        var command = ReactiveCommand.CreateFromTask(async (TParam param) => await execute.Invoke(param));
        SetupCommand(command);

        return command;
    }

    private async Task<bool> IsTakeMoreThen(Task task, TimeSpan timeout)
    {
        var delay = Task.Delay(timeout);

        var tasks = new[]
        {
            task, delay,
        };

        var resultTask = await Task.WhenAny(tasks);

        return resultTask != task;
    }

    private void SetupCommand<TParam, TResult>(ReactiveCommand<TParam, TResult> command)
    {
        command.ThrownExceptions.Subscribe(OnNextError);
    }

    private async void OnNextError(Exception exception)
    {
        if (exception is TaskCanceledException)
        {
            return;
        }

        Log.Logger.Error(exception, "UI error");

        await DialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
            async _ =>
            {
                await DialogViewer.CloseErrorDialogAsync(CancellationToken.None);
                await DialogViewer.CloseProgressDialogAsync(CancellationToken.None);
            },
            viewModel => viewModel.Exception = exception,
            CancellationToken.None
        );
    }
}