using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class InfoViewModel : ViewModelBase
{
    public InfoViewModel()
    {
        OkCommand = CreateCommandFromTask(OkAsync);
    }

    [Reactive]
    public object? Content { get; set; }

    public Func<object, Task>? OkTask { get; set; }
    public ICommand OkCommand { get; }

    private async Task OkAsync()
    {
        var con = Content.ThrowIfNull();
        await OkTask.ThrowIfNull().Invoke(con).ConfigureAwait(false);
    }
}