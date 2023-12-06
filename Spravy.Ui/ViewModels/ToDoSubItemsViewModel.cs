using System;
using System.Collections.Generic;
using System.Linq;
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
using Spravy.Ui.Extensions;
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
        ToMultiEditingToDoItemsCommand = CreateCommandFromTask(TaskWork.Create(ToMultiEditingToDoItemsAsync).RunAsync);
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
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddSubToDoItemToFavoriteCommand { get; }
    public ICommand RemoveSubToDoItemFromFavoriteCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public ICommand ToMultiEditingToDoItemsCommand { get; }
    public ICommand ChangeOrderIndexCommand { get; }
    public ICommand OpenLinkCommand { get; }

    [Inject]
    public required IOpenerLink OpenerLink { get; set; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    private async Task OpenLinkAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        var link = item.Link.ThrowIfNull().ToUri();
        cancellationToken.ThrowIfCancellationRequested();
        await OpenerLink.OpenLinkAsync(link, cancellationToken).ConfigureAwait(false);
    }

    private async Task ChangeOrderIndexAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await DialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                async viewModel =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
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
            )
            .ConfigureAwait(false);
    }

    private async Task ToMultiEditingToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = Missed.Concat(Planned)
            .Concat(ReadyForCompleted)
            .Concat(Completed)
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id);

        await Navigator.NavigateToAsync<MultiEditingToDoSubItemsViewModel>(
                viewModel => viewModel.Ids.AddRange(ids),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task CompleteSubToDoItemAsync(ToDoItemNotify subItemValue, CancellationToken cancellationToken)
    {
        await DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
                _ => DialogViewer.CloseInputDialogAsync(cancellationToken),
                viewModel =>
                {
                    viewModel.SetCompleteStatus(subItemValue.IsCan);

                    viewModel.Complete = async status =>
                    {
                        await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

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

                        await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    };
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task RemoveFavoriteToDoItemAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await ToDoService.RemoveFavoriteToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken).ConfigureAwait(false);
        await RefreshToDoItemListAsync(FavoriteToDoItems, ids.ToArray(), cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshToDoItemListAsync(
        AvaloniaList<ToDoItemNotify> items,
        Guid[] ids,
        CancellationToken cancellationToken
    )
    {
        await this.InvokeUIBackgroundAsync(() => items.Clear());
        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item);
            await this.InvokeUIBackgroundAsync(() => items.AddRange(itemNotify));
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private async Task RefreshToDoItemListsAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        await this.InvokeUIBackgroundAsync(
            () =>
            {
                Missed.Clear();
                ReadyForCompleted.Clear();
                Completed.Clear();
                Planned.Clear();
            }
        );

        cancellationToken.ThrowIfCancellationRequested();
        await RefreshFavoriteToDoItemsAsync(cancellationToken).ConfigureAwait(false);

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            await AddToDoItemAsync(item);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private DispatcherOperation AddToDoItemAsync(IEnumerable<ToDoItem> items)
    {
        return this.InvokeUIBackgroundAsync(
            () =>
            {
                var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(items);


                foreach (var item in itemNotify)
                {
                    switch (item.Status)
                    {
                        case ToDoItemStatus.Miss:
                            Missed.Add(item);
                            break;
                        case ToDoItemStatus.ReadyForComplete:
                            ReadyForCompleted.Add(item);
                            break;
                        case ToDoItemStatus.Planned:
                            Planned.Add(item);
                            break;
                        case ToDoItemStatus.Completed:
                            Completed.Add(item);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        );
    }

    private async Task DeleteSubToDoItemAsync(ToDoItemNotify subItem, CancellationToken cancellationToken)
    {
        await DialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                async view =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.DeleteToDoItemAsync(view.Item.ThrowIfNull().Id, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                _ => DialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.Item = subItem,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeToDoItemAsync(ToDoItemNotify subItemValue, CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync<ToDoItemViewModel>(v => v.Id = subItemValue.Id, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task AddFavoriteToDoItemAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await ToDoService.AddFavoriteToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item, CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync<ToDoItemViewModel>(v => v.Id = item.Id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpdateItemsAsync(Guid[] ids, IRefresh refresh, CancellationToken cancellationToken)
    {
        refreshToDoItem = refresh;
        await RefreshToDoItemListsAsync(ids, cancellationToken).ConfigureAwait(false);
    }
}