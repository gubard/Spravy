using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ButtonViewModel : ViewModelBase
{
    private MaterialIconKind icon;
    private ICommand? command;
    private string name = string.Empty;
    private object? parameter;

    public TaskWork? Work { get; set; }

    public MaterialIconKind Icon
    {
        get => icon;
        set => this.RaiseAndSetIfChanged(ref icon, value);
    }

    public ICommand? Command
    {
        get => command;
        set => this.RaiseAndSetIfChanged(ref command, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public object? Parameter
    {
        get => parameter;
        set => this.RaiseAndSetIfChanged(ref parameter, value);
    }

    public static ButtonViewModel Create<TParam>(MaterialIconKind icon,  Func<TParam, CancellationToken, Task> func, string name)
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        var result = kernel.Get<ButtonViewModel>();
        var work = TaskWork.Create(func); 
        //result.Work = work;
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync);
        SetupCommand(command);
        result.Command = command;
        result.Name = name;
        result.Icon = icon;

        return result;
    }
    
    public static ButtonViewModel Create(MaterialIconKind icon, Func<CancellationToken, Task> func, string name)
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        var result = kernel.Get<ButtonViewModel>();
        var work = TaskWork.Create(func); 
        result.Work = work;
        var command = ReactiveCommand.CreateFromTask(work.RunAsync);
        SetupCommand(command);
        result.Command = command;
        result.Name = name;
        result.Icon = icon;

        return result;
    }

    private static void SetupCommand<TParam, TResult>(ReactiveCommand<TParam, TResult> command)
    {
        command.ThrownExceptions.Subscribe(OnNextError);
    }

    private static async void OnNextError(Exception exception)
    {
        if (exception is TaskCanceledException)
        {
            return;
        }

        Log.Logger.Error(exception, "UI error");
        var kernel = DiHelper.Kernel.ThrowIfNull();
        var dialogViewer = kernel.Get<IDialogViewer>();

        await dialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
            async _ =>
            {
                await dialogViewer.CloseErrorDialogAsync(CancellationToken.None);
                await dialogViewer.CloseProgressDialogAsync(CancellationToken.None);
            },
            viewModel => viewModel.Exception = exception,
            CancellationToken.None
        );
    }
}