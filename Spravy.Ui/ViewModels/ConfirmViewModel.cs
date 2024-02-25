using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : ViewModelBase, ISaveState
{
    public ConfirmViewModel()
    {
        CancelCommand = CreateInitializedCommand(CancelAsync);
        ConfirmCommand = CreateInitializedCommand(ConfirmAsync);
    }

    [Reactive]
    public object? Content { get; set; }

    public Func<object, Task>? ConfirmTask { get; set; }
    public Func<object, Task>? CancelTask { get; set; }
    public ICommand CancelCommand { get; }
    public ICommand ConfirmCommand { get; }

    private async Task CancelAsync()
    {
        var con = Content.ThrowIfNull();
        await CancelTask.ThrowIfNull().Invoke(con).ConfigureAwait(false);
    }

    private async Task ConfirmAsync()
    {
        var con = Content.ThrowIfNull();
        await ConfirmTask.ThrowIfNull().Invoke(con).ConfigureAwait(false);
    }

    public Task SaveStateAsync()
    {
        if (Content is ISaveState saveState)
        {
            return saveState.SaveStateAsync();
        }

        return Task.CompletedTask;
    }
}