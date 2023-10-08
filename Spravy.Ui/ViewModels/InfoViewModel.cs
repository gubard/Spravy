using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class InfoViewModel : ViewModelBase
{
    private object? content;

    public InfoViewModel()
    {
        OkCommand = CreateCommandFromTask(OkAsync);
    }

    public object? Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public Func<object, Task>? OkTask { get; set; }
    public ICommand OkCommand { get; }

    private async Task OkAsync()
    {
        if (OkTask is null)
        {
            return;
        }

        var con = Content.ThrowIfNull();
        await OkTask.Invoke(con);
    }
}