using System;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ViewModelBase : NotifyBase
{
    private static readonly BehaviorSubject<bool> CanExecute;
    private static readonly TimeSpan TaskTimeout = TimeSpan.FromSeconds(1);

    static ViewModelBase()
    {
        CanExecute = new(true);
    }

    [Inject]
    public required INavigator Navigator { get; init; }

    [Inject]
    public required IDialogViewer DialogViewer { get; set; }

    public void ReleaseCommands()
    {
        CanExecute.OnNext(true);
    }

    protected ICommand CreateInitializedCommand(Action action)
    {
        var command = ReactiveCommand.Create(WrapCommand(action));
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateInitializedCommand(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(WrapCommand(CreateWithDialogProgressIndicatorAsync(execute)));
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateInitializedCommand<TArgs>(Action<TArgs> action)
    {
        var command = ReactiveCommand.Create(WrapCommand(action));
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommand(Action action)
    {
        var command = ReactiveCommand.Create(WrapCommand(action), CanExecute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommand<TArgs>(Action<TArgs> action)
    {
        var command = ReactiveCommand.Create(WrapCommand(action), CanExecute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(WrapCommand(execute), CanExecute);
        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTaskWithDialogProgressIndicator(Func<Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(
            WrapCommand(CreateWithDialogProgressIndicatorAsync(execute)),
            CanExecute
        );

        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTaskWithDialogProgressIndicator<TParam>(Func<TParam, Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(
            WrapCommand(CreateWithDialogProgressIndicatorAsync(execute)),
            CanExecute
        );

        SetupCommand(command);

        return command;
    }

    protected ICommand CreateCommandFromTask<TParam>(Func<TParam, Task> execute)
    {
        var command = ReactiveCommand.CreateFromTask(WrapCommand(execute), CanExecute);
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
            await Navigator.NavigateToAsync<IExceptionViewModel>(viewModel => viewModel.Exception = e);
        }
    }

    protected async Task SafeExecuteAsync(Func<ConfiguredTaskAwaitable> func)
    {
        try
        {
            await func.Invoke();
        }
        catch (Exception e)
        {
            await Navigator.NavigateToAsync<IExceptionViewModel>(viewModel => viewModel.Exception = e);
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
        Log.Logger.Error(exception, "UI error");

        await DialogViewer.ShowInfoErrorDialogAsync<IExceptionViewModel>(
            async _ =>
            {
                await DialogViewer.CloseErrorDialogAsync();
                await DialogViewer.CloseProgressDialogAsync();
            },
            viewModel => viewModel.Exception = exception
        );
    }

    private Action WrapCommand(Action action)
    {
        return () =>
        {
            CanExecute.OnNext(false);

            try
            {
                action.Invoke();
            }
            finally
            {
                CanExecute.OnNext(true);
            }
        };
    }

    private Action<TArgs> WrapCommand<TArgs>(Action<TArgs> action)
    {
        return args =>
        {
            CanExecute.OnNext(false);

            try
            {
                action.Invoke(args);
            }
            finally
            {
                CanExecute.OnNext(true);
            }
        };
    }

    private Func<Task> WrapCommand(Func<Task> func)
    {
        return async () =>
        {
            CanExecute.OnNext(false);

            try
            {
                await func.Invoke();
            }
            finally
            {
                CanExecute.OnNext(true);
            }
        };
    }

    private Func<TParam, Task> WrapCommand<TParam>(Func<TParam, Task> func)
    {
        return async param =>
        {
            CanExecute.OnNext(false);

            try
            {
                await func.Invoke(param);
            }
            finally
            {
                CanExecute.OnNext(true);
            }
        };
    }
}