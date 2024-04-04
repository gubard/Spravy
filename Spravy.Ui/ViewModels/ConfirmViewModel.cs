using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : ViewModelBase, ISaveState
{
    public ConfirmViewModel()
    {
        CancelCommand = CreateInitializedCommand(async () => await CancelAsync());
        ConfirmCommand = CreateInitializedCommand(async () => await ConfirmAsync());
    }

    [Reactive]
    public object? Content { get; set; }

    public Func<object, ConfiguredValueTaskAwaitable<Result>>? ConfirmTask { get; set; }
    public Func<object, ConfiguredValueTaskAwaitable<Result>>? CancelTask { get; set; }
    public ICommand CancelCommand { get; }
    public ICommand ConfirmCommand { get; }

    private async ValueTask<Result> CancelAsync()
    {
        var con = Content.ThrowIfNull();

        return await CancelTask.ThrowIfNull().Invoke(con);
    }

    private async ValueTask<Result> ConfirmAsync()
    {
        var con = Content.ThrowIfNull();

        return await ConfirmTask.ThrowIfNull().Invoke(con);
    }

    public ValueTask<Result> SaveStateAsync()
    {
        if (Content is ISaveState saveState)
        {
            return saveState.SaveStateAsync();
        }

        return Result.SuccessValueTask;
    }
}