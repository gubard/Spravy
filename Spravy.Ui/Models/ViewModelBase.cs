using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Grpc.Core;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Client.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Ui.Features.ErrorHandling.ViewModels;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ViewModelBase : NotifyBase
{
    [Inject]
    public required INavigator Navigator { get; init; }

    [Inject]
    public required IDialogViewer DialogViewer { get; set; }

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
        if (exception is TaskCanceledException)
        {
            return;
        }

        if (exception is RpcException rpc)
        {
            switch (rpc.StatusCode)
            {
                case StatusCode.Cancelled:
                    return;
            }
        }

        if (exception is GrpcException { InnerException: RpcException rpc2 })
        {
            switch (rpc2.StatusCode)
            {
                case StatusCode.Cancelled:
                    return;
            }
        }

        Log.Logger.Error(exception, "UI error");

        await DialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
            _ => DialogViewer.CloseErrorDialogAsync(CancellationToken.None)
                .IfSuccessAsync(
                    () => DialogViewer.CloseProgressDialogAsync(CancellationToken.None)
                ),
            viewModel => viewModel.Exception = exception,
            CancellationToken.None
        );
    }
}