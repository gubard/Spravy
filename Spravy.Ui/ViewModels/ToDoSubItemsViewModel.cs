using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemOrderChanger
{
    private IRefresh? refreshToDoItem;

    public ToDoSubItemsViewModel()
    {
        CompleteSubToDoItemCommand =
            CreateCommandFromTask<ToDoItemNotify>(TaskWork.Create<ToDoItemNotify>(CompleteSubToDoItemAsync).RunAsync);
        DeleteSubToDoItemCommand =
            CreateCommandFromTask<ToDoItemNotify>(TaskWork.Create<ToDoItemNotify>(DeleteSubToDoItemAsync).RunAsync);
        ChangeToDoItemCommand =
            CreateCommandFromTask<ToDoItemNotify>(TaskWork.Create<ToDoItemNotify>(ChangeToDoItemAsync).RunAsync);
        AddSubToDoItemToFavoriteCommand =
            CreateCommandFromTask<ToDoItemNotify>(TaskWork.Create<ToDoItemNotify>(AddFavoriteToDoItemAsync).RunAsync);
        RemoveSubToDoItemFromFavoriteCommand = CreateCommandFromTask<ToDoItemNotify>(
            TaskWork.Create<ToDoItemNotify>(RemoveFavoriteToDoItemAsync).RunAsync
        );
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(
            TaskWork.Create<ActiveToDoItemNotify>(ChangeToActiveDoItemAsync).RunAsync
        );
        CompleteSelectedToDoItemsCommand =
            CreateCommandFromTask(TaskWork.Create(CompleteSelectedToDoItemsAsync).RunAsync);
        ChangeOrderIndexCommand =
            CreateCommandFromTask<ToDoItemNotify>(TaskWork.Create<ToDoItemNotify>(ChangeOrderIndexAsync).RunAsync);
        OpenLinkCommand =
            CreateCommandFromTask<ToDoItemNotify>(TaskWork.Create<ToDoItemNotify>(OpenLinkAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemNotify> Missed { get; } = new();
    public AvaloniaList<ToDoItemNotify> Planned { get; } = new();
    public AvaloniaList<ToDoItemNotify> ReadyForCompleted { get; } = new();
    public AvaloniaList<ToDoItemNotify> Completed { get; } = new();
    public AvaloniaList<ToDoItemNotify> FavoriteToDoItems { get; } = new();
    public AvaloniaList<ToDoItemNotify> SelectedMissed { get; } = new();
    public AvaloniaList<ToDoItemNotify> SelectedPlanned { get; } = new();
    public AvaloniaList<ToDoItemNotify> SelectedReadyForCompleted { get; } = new();
    public AvaloniaList<ToDoItemNotify> SelectedCompleted { get; } = new();
    public AvaloniaList<ToDoItemNotify> SelectedFavoriteToDoItems { get; } = new();
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddSubToDoItemToFavoriteCommand { get; }
    public ICommand RemoveSubToDoItemFromFavoriteCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public ICommand CompleteSelectedToDoItemsCommand { get; }
    public ICommand ChangeOrderIndexCommand { get; }
    public ICommand OpenLinkCommand { get; }

    [Inject]
    public required IOpenerLink OpenerLink { get; set; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    private Task OpenLinkAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        var link = item.Link.ThrowIfNull().ToUri();
        cancellationToken.ThrowIfCancellationRequested();

        return OpenerLink.OpenLinkAsync(link, cancellationToken);
    }

    private Task ChangeOrderIndexAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
            async viewModel =>
            {
                await DialogViewer.CloseContentDialogAsync(cancellationToken);
                var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);
                cancellationToken.ThrowIfCancellationRequested();
                await ToDoService.UpdateToDoItemOrderIndexAsync(options, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                await RefreshAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => DialogViewer.CloseContentDialogAsync(cancellationToken),
            viewModel => viewModel.Id = item.Id,
            cancellationToken
        );
    }

    private Task CompleteSelectedToDoItemsAsync(CancellationToken cancellationToken)
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => DialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetAllStatus();

                viewModel.Complete = async status =>
                {
                    await CompleteAsync(SelectedCompleted, status, cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(SelectedMissed, status, cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(SelectedFavoriteToDoItems, status, cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(SelectedReadyForCompleted, status, cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(SelectedPlanned, status, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                };
            },
            cancellationToken
        );
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task CompleteSubToDoItemAsync(ToDoItemNotify subItemValue, CancellationToken cancellationToken)
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => DialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetCompleteStatus(subItemValue.IsCan);

                viewModel.Complete = async status =>
                {
                    switch (status)
                    {
                        case CompleteStatus.Complete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(
                                    subItemValue.Id,
                                    true,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Incomplete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(
                                    subItemValue.Id,
                                    false,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Skip:
                            await ToDoService.SkipToDoItemAsync(subItemValue.Id, cancellationToken)
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Fail:
                            await ToDoService.FailToDoItemAsync(subItemValue.Id, cancellationToken)
                                .ConfigureAwait(false);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
                    }

                    await RefreshAsync(cancellationToken);
                    await DialogViewer.CloseInputDialogAsync(cancellationToken);
                };
            },
            cancellationToken
        );
    }

    private async Task CompleteAsync(
        IEnumerable<ToDoItemNotify> items,
        CompleteStatus status,
        CancellationToken cancellationToken
    )
    {
        switch (status)
        {
            case CompleteStatus.Complete:
                foreach (var item in items)
                {
                    await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Skip:
                foreach (var item in items)
                {
                    await ToDoService.SkipToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Fail:
                foreach (var item in items)
                {
                    await ToDoService.FailToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Incomplete:
                foreach (var item in items)
                {
                    await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken);
                }

                break;
            default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    private async Task RemoveFavoriteToDoItemAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await ToDoService.RemoveFavoriteToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
        await RefreshAsync(cancellationToken);
    }

    private async Task RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken).ConfigureAwait(false);
        await RefreshToDoItemListAsync(FavoriteToDoItems, ids, cancellationToken);
    }

    private async Task RefreshToDoItemListAsync(
        AvaloniaList<ToDoItemNotify> items,
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken
    )
    {
        await Dispatcher.UIThread.InvokeAsync(() => items.Clear());

        await foreach (var item in LoadToDoItemsAsync(ids, cancellationToken).ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<ToDoItemNotify>(item);
            await Dispatcher.UIThread.InvokeAsync(() => items.Add(itemNotify));
        }
    }

    private async Task RefreshToDoItemListsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                Missed.Clear();
                ReadyForCompleted.Clear();
                Completed.Clear();
                Planned.Clear();
            }
        );

        await foreach (var item in LoadToDoItemsAsync(ids, cancellationToken).ConfigureAwait(false))
        {
            await AddToDoItemAsync(item);
        }
    }

    private DispatcherOperation AddToDoItemAsync(ToDoItem item)
    {
        var itemNotify = Mapper.Map<ToDoItemNotify>(item);

        return itemNotify.Status switch
        {
            ToDoItemStatus.Miss => Dispatcher.UIThread.InvokeAsync(() => Missed.Add(itemNotify)),
            ToDoItemStatus.ReadyForComplete => Dispatcher.UIThread.InvokeAsync(() => ReadyForCompleted.Add(itemNotify)),
            ToDoItemStatus.Planned => Dispatcher.UIThread.InvokeAsync(() => Planned.Add(itemNotify)),
            ToDoItemStatus.Completed => Dispatcher.UIThread.InvokeAsync(() => Completed.Add(itemNotify)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async IAsyncEnumerable<ToDoItem> LoadToDoItemsAsync(
        IEnumerable<Guid> ids,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        foreach (var id in ids)
        {
            yield return await ToDoService.GetToDoItemAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }

    private Task DeleteSubToDoItemAsync(ToDoItemNotify subItem, CancellationToken cancellationToken)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
            async view =>
            {
                await ToDoService.DeleteToDoItemAsync(view.Item.ThrowIfNull().Id, cancellationToken)
                    .ConfigureAwait(false);
                await DialogViewer.CloseContentDialogAsync(cancellationToken);
                await RefreshAsync(cancellationToken);
            },
            _ => DialogViewer.CloseContentDialogAsync(cancellationToken),
            view => view.Item = subItem,
            cancellationToken
        );
    }

    private Task ChangeToDoItemAsync(ToDoItemNotify subItemValue, CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync<ToDoItemViewModel>(v => v.Id = subItemValue.Id, cancellationToken);
    }

    private async Task AddFavoriteToDoItemAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await ToDoService.AddFavoriteToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
        await RefreshAsync(cancellationToken);
    }

    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item, CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync<ToDoItemViewModel>(v => v.Id = item.Id, cancellationToken);
    }

    public Task UpdateItemsAsync(IEnumerable<Guid> ids, IRefresh refresh, CancellationToken cancellationToken)
    {
        refreshToDoItem = refresh;

        return Task.WhenAll(
                RefreshToDoItemListsAsync(ids, cancellationToken),
                RefreshFavoriteToDoItemsAsync(cancellationToken)
            )
            .WaitAsync(cancellationToken);
    }
}