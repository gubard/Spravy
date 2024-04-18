using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class TaskWork
{
    private readonly Delegate del;
    private readonly IErrorHandler errorHandler;
    private CancellationTokenSource cancellationTokenSource = new();
    private ConfiguredValueTaskAwaitable<Result>? current;

    private TaskWork(Delegate del, IErrorHandler errorHandler)
    {
        this.del = del;
        this.errorHandler = errorHandler;
    }

    public ConfiguredValueTaskAwaitable<Result> Current => current.ThrowIfNullStruct();

    public async Task RunAsync()
    {
        Cancel();
        current = (ConfiguredValueTaskAwaitable<Result>)del.DynamicInvoke(cancellationTokenSource.Token).ThrowIfNull();
        var result = await Current;
        await errorHandler.ErrorsHandleAsync(result.Errors, CancellationToken.None);
    }

    public async Task RunAsync<T>(T value)
    {
        Cancel();

        current = (ConfiguredValueTaskAwaitable<Result>)del.DynamicInvoke(value, cancellationTokenSource.Token)
            .ThrowIfNull();

        var result = await Current;
        await errorHandler.ErrorsHandleAsync(result.Errors, CancellationToken.None);
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    public static TaskWork Create(Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> task)
    {
        return new(task, DiHelper.Kernel.ThrowIfNull().Get<IErrorHandler>());
    }

    public static TaskWork Create<T>(Func<T, CancellationToken, ConfiguredValueTaskAwaitable<Result>> task)
    {
        return new(task, DiHelper.Kernel.ThrowIfNull().Get<IErrorHandler>());
    }
}