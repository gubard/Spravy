using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    static CommandStorage()
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        navigator = kernel.Get<INavigator>();
        openerLink = kernel.Get<IOpenerLink>();
        dialogViewer = kernel.Get<IDialogViewer>();
        mainSplitViewModel = kernel.Get<MainSplitViewModel>();
        toDoService = kernel.Get<IToDoService>();
        SwitchPane = CreateCommand(SwitchPaneAsync, MaterialIconKind.Menu, "Open pane");
        NavigateToToDoItem = CreateCommand<Guid>(NavigateToToDoItemAsync, MaterialIconKind.ListBox, "Open");
        CompleteToDoItem = CreateCommand<ICanComplete>(CompleteToDoItemAsync, MaterialIconKind.Check, "Complete");
        SelectAll = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            SelectAllAsync,
            MaterialIconKind.CheckAll,
            "Select all"
        );
        DeleteToDoItem = CreateCommand<IDeletable>(DeleteSubToDoItemAsync, MaterialIconKind.Delete, "Delete");
        ChangeToActiveDoItem = CreateCommand(ChangeToActiveDoItemAsync, MaterialIconKind.ArrowRight, "Go to active");
        ChangeOrderIndex = CreateCommand<ToDoItemNotify>(
            ChangeOrderIndexAsync,
            MaterialIconKind.ReorderHorizontal,
            "Reorder"
        );
        OpenLink = CreateCommand<ToDoItemNotify>(
            OpenLinkAsync,
            MaterialIconKind.Link,
            "Reorder"
        );
        RemoveToDoItemFromFavorite = CreateCommand<Guid>(
            RemoveFavoriteToDoItemAsync,
            MaterialIconKind.Link,
            "Remove from favorite"
        );
        AddToDoItemToFavorite = CreateCommand<Guid>(
            AddFavoriteToDoItemAsync,
            MaterialIconKind.Link,
            "Add to favorite"
        );
        SetToDoShortItem = CreateCommand<IToDoShortItemProperty>(
            SetToDoShortItemAsync,
            MaterialIconKind.Pencil,
            "Set to do item"
        );
        SetDueDateTime = CreateCommand<IDueDateTimeProperty>(
            SetDueDateTimeAsync,
            MaterialIconKind.Pencil,
            "Set due date time"
        );
    }

    private static readonly INavigator navigator;
    private static readonly IDialogViewer dialogViewer;
    private static readonly MainSplitViewModel mainSplitViewModel;
    private static readonly IToDoService toDoService;
    private static readonly IOpenerLink openerLink;

    public static CommandParameters SwitchPane { get; }
    public static ICommand SwitchPaneCommand => SwitchPane.Value.Command;
    public static CommandItem SwitchPaneItem => SwitchPane.Value;

    public static CommandParameters<ICanComplete> CompleteToDoItem { get; }
    public static ICommand CompleteToDoItemCommand => CompleteToDoItem.Value.Command;
    public static CommandItem CompleteToDoItemItem => CompleteToDoItem.Value;

    public static CommandParameters ChangeToActiveDoItem { get; }
    public static ICommand ChangeToActiveDoItemCommand => ChangeToActiveDoItem.Value.Command;
    public static CommandItem ChangeToActiveDoItemItem => ChangeToActiveDoItem.Value;

    public static CommandParameters<ToDoItemNotify> ChangeOrderIndex { get; }
    public static ICommand ChangeOrderIndexCommand => ChangeOrderIndex.Value.Command;
    public static CommandItem ChangeOrderIndexItem => ChangeOrderIndex.Value;

    public static CommandParameters<IDeletable> DeleteToDoItem { get; }
    public static ICommand DeleteToDoItemCommand => DeleteToDoItem.Value.Command;
    public static CommandItem DeleteToDoItemItem => DeleteToDoItem.Value;

    public static CommandParameters<Guid> NavigateToToDoItem { get; }
    public static ICommand NavigateToToDoItemCommand => NavigateToToDoItem.Value.Command;
    public static CommandItem NavigateToToDoItemItem => NavigateToToDoItem.Value;

    public static CommandParameters<ToDoItemNotify> OpenLink { get; }
    public static ICommand OpenLinkCommand => OpenLink.Value.Command;
    public static CommandItem OpenLinkItem => OpenLink.Value;

    public static CommandParameters<Guid> RemoveToDoItemFromFavorite { get; }
    public static ICommand RemoveToDoItemFromFavoriteCommand => RemoveToDoItemFromFavorite.Value.Command;
    public static CommandItem RemoveToDoItemFromFavoriteItem => RemoveToDoItemFromFavorite.Value;

    public static CommandParameters<Guid> AddToDoItemToFavorite { get; }
    public static ICommand AddToDoItemToFavoriteCommand => AddToDoItemToFavorite.Value.Command;
    public static CommandItem AddToDoItemToFavoriteItem => AddToDoItemToFavorite.Value;

    public static CommandParameters<IToDoShortItemProperty> SetToDoShortItem { get; }
    public static ICommand SetToDoShortItemCommand => SetToDoShortItem.Value.Command;
    public static CommandItem SetToDoShortItemItem => SetToDoShortItem.Value;

    public static CommandParameters<IDueDateTimeProperty> SetDueDateTime { get; }
    public static ICommand SetDueDateTimeCommand => SetDueDateTime.Value.Command;
    public static CommandItem SetDueDateTimeItem => SetDueDateTime.Value;

    public static CommandParameters<AvaloniaList<Selected<ToDoItemNotify>>> SelectAll { get; }

    private static async Task SetDueDateTimeAsync(IDueDateTimeProperty property, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowDateTimeConfirmDialogAsync(
                async value =>
                {
                    await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await cancellationToken.InvokeUIBackgroundAsync(() => property.DueDateTime = value);
                },
                calendar =>
                {
                    calendar.SelectedDate = DateTimeOffset.Now.ToCurrentDay().DateTime;
                    calendar.SelectedTime = TimeSpan.Zero;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private static Task SetToDoShortItemAsync(IToDoShortItemProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async itemNotify =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                await cancellationToken.InvokeUIBackgroundAsync(
                    () => property.ShortItem = new()
                    {
                        Id = itemNotify.Id,
                        Name = itemNotify.Name
                    }
                );
            },
            view =>
            {
                if (property.ShortItem is null)
                {
                    return;
                }

                view.DefaultSelectedItemId = property.ShortItem.Id;
            },
            cancellationToken
        );
    }

    private static async Task RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await toDoService.RemoveFavoriteToDoItemAsync(id, cancellationToken).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken);
    }

    private static async Task AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await toDoService.AddFavoriteToDoItemAsync(id, cancellationToken).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken);
    }

    private static async Task OpenLinkAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        var link = item.Link.ThrowIfNull().ToUri();
        cancellationToken.ThrowIfCancellationRequested();
        await openerLink.OpenLinkAsync(link, cancellationToken).ConfigureAwait(false);
    }

    private static async Task ChangeOrderIndexAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                async viewModel =>
                {
                    await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                    var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);
                    cancellationToken.ThrowIfCancellationRequested();
                    await toDoService.UpdateToDoItemOrderIndexAsync(options, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    await RefreshCurrentViewAsync(cancellationToken);
                },
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                viewModel => viewModel.Id = item.Id,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private static async Task ChangeToActiveDoItemAsync(CancellationToken cancellationToken)
    {
        var item = await toDoService.GetCurrentActiveToDoItemAsync(cancellationToken).ConfigureAwait(false);

        if (item is null)
        {
            await navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await navigator.NavigateToAsync<ToDoItemViewModel>(view => view.Id = item.Value.Id, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static async Task DeleteSubToDoItemAsync(IDeletable deletable, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                async view =>
                {
                    await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                    await toDoService.DeleteToDoItemAsync(deletable.Id, cancellationToken)
                        .ConfigureAwait(false);

                    await RefreshCurrentViewAsync(cancellationToken);
                },
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.Header = deletable.Header,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private static async Task SelectAllAsync(
        AvaloniaList<Selected<ToDoItemNotify>> items,
        CancellationToken cancellationToken
    )
    {
        await cancellationToken.InvokeUIAsync(
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

    private static Task CompleteToDoItemAsync(ICanComplete canComplete, CancellationToken cancellationToken)
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

                    await RefreshCurrentViewAsync(cancellationToken);
                };
            },
            cancellationToken
        );
    }

    private static async Task RefreshCurrentViewAsync(CancellationToken cancellationToken)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return;
        }

        await refresh.RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Task NavigateToToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = id, cancellationToken);
    }

    private static async Task SwitchPaneAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await cancellationToken.InvokeUIAsync(() => mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen);
    }

    public static CommandParameters CreateCommand(
        Func<CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask(work.RunAsync);
        SetupCommand(command);

        return new CommandParameters(new CommandItem(icon, command, name, null), work);
    }

    public static CommandParameters<TParam> CreateCommand<TParam>(
        Func<TParam, CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync);
        SetupCommand(command);

        return new CommandParameters<TParam>(new CommandItem(icon, command, name, null), work);
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