using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class InfoViewModel : ViewModelBase
{
    public InfoViewModel()
    {
        OkCommand = CreateCommandFromTask(async () => await OkAsync());
    }

    [Reactive]
    public object? Content { get; set; }

    public Func<object, ConfiguredValueTaskAwaitable<Result>>? OkTask { get; set; }
    public ICommand OkCommand { get; }

    private async ValueTask<Result> OkAsync()
    {
        var con = Content.ThrowIfNull();

        return await OkTask.ThrowIfNull().Invoke(con);
    }
}