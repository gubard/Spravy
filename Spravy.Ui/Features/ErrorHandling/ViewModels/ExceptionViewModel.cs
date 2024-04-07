using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Features.ToDo.ViewModels;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ExceptionViewModel : ViewModelBase
{
    public ExceptionViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Reactive]
    public Exception? Exception { get; set; }

    [Reactive]
    public string? FullExceptionText { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        Disposables.Add(this.WhenAnyValue(x => x.Exception).Subscribe(x => FullExceptionText = x?.ToString()));

        return Result.AwaitableFalse;
    }
}