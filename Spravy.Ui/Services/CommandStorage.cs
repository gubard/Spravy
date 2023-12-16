using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Material.Icons;
using Material.Icons.Avalonia;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Services;

public class CommandStorage
{
    public static readonly CommandStorage Default = DiHelper.Kernel.ThrowIfNull().Get<CommandStorage>();

    private readonly INavigator navigator;
    private readonly IDialogViewer dialogViewer;
    private readonly MainSplitViewModel mainSplitViewModel;
    private readonly IToDoService toDoService;

    public CommandStorage(
        INavigator navigator,
        IDialogViewer dialogViewer,
        MainSplitViewModel mainSplitViewModel,
        IToDoService toDoService
    )
    {
        this.navigator = navigator;
        this.dialogViewer = dialogViewer;
        this.mainSplitViewModel = mainSplitViewModel;
        this.toDoService = toDoService;
        SwitchPane = CreateCommand(SwitchPaneAsync, MaterialIconKind.SwapHorizontal, "Open pane");
        NavigateToToDoItem = CreateCommand<Guid>(NavigateToToDoItemAsync, MaterialIconKind.ListBox, "Open");
        CompleteToDoItem = CreateCommand<ICanComplete>(CompleteToDoItemAsync, MaterialIconKind.Check, "Complete");
        SelectAll = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            SelectAllAsync,
            MaterialIconKind.CheckAll,
            "Select all"
        );
        DeleteToDoItem = CreateCommand<IDeletable>(DeleteSubToDoItemAsync, MaterialIconKind.Delete, "Delete");
    }

    public CommandParameters SwitchPane { get; }
    public CommandParameters<Guid> NavigateToToDoItem { get; }
    public CommandParameters<ICanComplete> CompleteToDoItem { get; }
    public CommandParameters<AvaloniaList<Selected<ToDoItemNotify>>> SelectAll { get; }
    public CommandParameters<IDeletable> DeleteToDoItem { get; }

    private async Task DeleteSubToDoItemAsync(IDeletable deletable, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                async view =>
                {
                    await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    await toDoService.DeleteToDoItemAsync(deletable.Id, cancellationToken)
                        .ConfigureAwait(false);

                    if (mainSplitViewModel.Content is IRefresh refresh)
                    {
                        await refresh.RefreshAsync(cancellationToken).ConfigureAwait(false);
                    }
                },
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.Header = deletable.Header,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task SelectAllAsync(AvaloniaList<Selected<ToDoItemNotify>> items, CancellationToken arg)
    {
        await this.InvokeUIAsync(
            () =>
            {
                if (items.All(x => x.IsSelect))
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = false;
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = true;
                    }
                }
            }
        );
    }

    private Task CompleteToDoItemAsync(ICanComplete canComplete, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetCompleteStatus(canComplete.IsCan);

                viewModel.Complete = async status =>
                {
                    await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    switch (status)
                    {
                        case CompleteStatus.Complete:
                            await toDoService.UpdateToDoItemCompleteStatusAsync(
                                    canComplete.Id,
                                    true,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Incomplete:
                            await toDoService.UpdateToDoItemCompleteStatusAsync(
                                    canComplete.Id,
                                    false,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Skip:
                            await toDoService.SkipToDoItemAsync(canComplete.Id, cancellationToken)
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Fail:
                            await toDoService.FailToDoItemAsync(canComplete.Id, cancellationToken)
                                .ConfigureAwait(false);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
                    }

                    if (mainSplitViewModel.Content is IRefresh refresh)
                    {
                        await refresh.RefreshAsync(cancellationToken).ConfigureAwait(false);
                    }
                };
            },
            cancellationToken
        );
    }

    private Task NavigateToToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = id, cancellationToken);
    }

    private async Task SwitchPaneAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await this.InvokeUIAsync(() => mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen);
    }

    public CommandParameters CreateCommand(Func<CancellationToken, Task> func, MaterialIconKind icon, string name)
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask(work.RunAsync);
        SetupCommand(command);

        return new CommandParameters(new ToDoItemCommand(icon, command, name, null), work);
    }

    public CommandParameters<TParam> CreateCommand<TParam>(
        Func<TParam, CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync);
        SetupCommand(command);

        return new CommandParameters<TParam>(new ToDoItemCommand(icon, command, name, null), work);
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

        Log.Logger.Error(exception, "UI error");

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