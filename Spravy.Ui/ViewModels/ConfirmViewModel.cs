using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ConfirmViewModel : ViewModelBase
{
    private object? content;

    public ConfirmViewModel()
    {
        CancelCommand = CreateInitializedCommand(CancelAsync);
        ConfirmCommand = CreateInitializedCommand(ConfirmAsync);
    }

    public object? Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public Func<object, Task>? ConfirmTask { get; set; }
    public Func<object, Task>? CancelTask { get; set; }
    public ICommand CancelCommand { get; }
    public ICommand ConfirmCommand { get; }

    private async Task CancelAsync()
    {
        if (CancelTask is null)
        {
            return;
        }

        var con = Content.ThrowIfNull();
        await CancelTask.Invoke(con);
    }

    private async Task ConfirmAsync()
    {
        if (ConfirmTask is null)
        {
            return;
        }

        var con = Content.ThrowIfNull();
        await ConfirmTask.Invoke(con);
    }
}