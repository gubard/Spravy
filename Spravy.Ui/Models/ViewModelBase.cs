using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ViewModelBase : NotifyBase
{
    private static readonly TimeSpan TaskTimeout = TimeSpan.FromMilliseconds(120);

    [Inject]
    public required INavigator Navigator { get; init; }

    [Inject]
    public required IDialogViewer DialogViewer { get; set; }

    protected ICommand CreateCommandFromObservable<TResult>(Func<IObservable<TResult>> execute)
    {
        var command = ReactiveCommand.CreateFromObservable(execute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommand(Action action)
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

    protected ICommand CreateCommandFromTaskWithDialogProgressIndicator(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(CreateWithDialogProgressIndicatorAsync(execute));
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTaskWithDialogProgressIndicator<TParam>(Func<TParam, Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(CreateWithDialogProgressIndicatorAsync(execute));
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask<TParam>(Func<TParam, Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(execute);
        SetupCommand(command);

        return command;
    }

    protected async Task SafeExecuteAsync(Func<Task> func)
    {
        try
        {
            await func.Invoke();
        }
        catch (Exception e)
        {
            Navigator.NavigateTo<IExceptionViewModel>(viewModel => viewModel.Exception = e);
        }
    }

    protected Func<Task> CreateWithDialogProgressIndicatorAsync(Func<Task> execute)
    {
        return async () =>
        {
            var task = execute.Invoke();

            if (await IsTakeMoreThen(task, TaskTimeout))
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                DialogViewer.ShowProgressDialogAsync<IDialogProgressIndicator>();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await task;
                await DialogViewer.CloseProgressDialogAsync();
            }

            await task;
        };
    }

    protected Func<TParam, Task> CreateWithDialogProgressIndicatorAsync<TParam>(Func<TParam, Task> execute)
    {
        return async param =>
        {
            var task = execute.Invoke(param);

            if (await IsTakeMoreThen(task, TaskTimeout))
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                DialogViewer.ShowProgressDialogAsync<IDialogProgressIndicator>();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await task;
                await DialogViewer.CloseProgressDialogAsync();
            }

            await task;
        };
    }

    private async Task<bool> IsTakeMoreThen(Task task, TimeSpan timeout)
    {
        var tasks = new[]
        {
            task, Task.Delay(timeout),
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
        Console.WriteLine(exception.ToString());
        await DialogViewer.ShowInfoErrorDialogAsync<IExceptionViewModel>(
            async _ =>
            {
                await DialogViewer.CloseErrorDialogAsync();
                await DialogViewer.CloseProgressDialogAsync();
            },
            viewModel => viewModel.Exception = exception
        );
    }
}