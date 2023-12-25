using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Features.ToDo.ViewModels;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemOrderChanger
{
    private IRefresh? refreshToDoItem;

    public ToDoSubItemsViewModel()
    {
        //MultiCompleteCommand = CreateInitializedCommand(TaskWork.Create(MultiCompleteAsync).RunAsync);
        MultiChangeTypeCommand = CreateInitializedCommand(TaskWork.Create(MultiChangeTypeAsync).RunAsync);
        MultiSetParentItemCommand = CreateInitializedCommand(TaskWork.Create(MultiChangeRootItemAsync).RunAsync);
    }

   // public ICommand MultiCompleteCommand { get; }
    public ICommand MultiChangeTypeCommand { get; }
    public ICommand MultiSetParentItemCommand { get; }

    [Inject]
    public required MultiToDoItemsViewModel List { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids.ToArray(), 5, cancellationToken)
                           .ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item);
            List.AddFavoritesAsync(itemNotify);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private async Task RefreshToDoItemListsAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        await List.ClearAsync();
        cancellationToken.ThrowIfCancellationRequested();
        await RefreshFavoriteToDoItemsAsync(cancellationToken).ConfigureAwait(false);

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item);
            await List.AddItemsAsync(itemNotify);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task UpdateItemsAsync(Guid[] ids, IRefresh refresh, CancellationToken cancellationToken)
    {
        refreshToDoItem = refresh;
        await RefreshToDoItemListsAsync(ids, cancellationToken).ConfigureAwait(false);
    }

    private async Task MultiChangeRootItemAsync(CancellationToken cancellationToken)
    {
        var ids = List.MultiToDoItems.GroupByNone.Items.Items.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();

        await DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                async item =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var id in ids)
                    {
                        await ToDoService.UpdateToDoItemParentAsync(id, item.Id, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                viewModel => viewModel.IgnoreIds.AddRange(ids),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task MultiCompleteAsync(CancellationToken cancellationToken)
    {
        var items = List.MultiToDoItems.GroupByNone.Items.Items.Where(x => x.IsSelect).Select(x => x.Value).ToArray();

        await DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => DialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetAllStatus();

                viewModel.Complete = async status =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(items, status, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                };
            },
            cancellationToken
        );
    }

    private async Task MultiChangeTypeAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(
                async type =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var item in List.MultiToDoItems.GroupByNone.Items.Items)
                    {
                        if (!item.IsSelect)
                        {
                            continue;
                        }

                        await ToDoService.UpdateToDoItemTypeAsync(item.Value.Id, type, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                viewModel =>
                {
                    viewModel.Items.AddRange(Enum.GetValues<ToDoItemType>().OfType<object>());
                    viewModel.SelectedItem = viewModel.Items.First();
                },
                cancellationToken
            )
            .ConfigureAwait(false);
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
                    await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                break;
            default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }
}